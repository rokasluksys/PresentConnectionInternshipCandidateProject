using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend;
using backend.Models;
using Humanizer;
using System.ComponentModel.DataAnnotations;
using backend.Models.DTOs;

namespace backend.Controllers
{
    [Route("api/packages")]
    [ApiController]
    public class PackagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PackagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/packages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PackageDTO>>> GetPackage()
        {
            return await _context.Package
                .Include(p => p.Sender)
                .Include(p => p.Recipient)
                .Include(p => p.Statuses)
                .Select(p => PackageToDTO(p))
                .ToListAsync();
        }

        // GET: api/packages/5
        [HttpGet("{trackingNumber}")]
        public async Task<ActionResult<PackageDTO>> GetPackage(int trackingNumber)
        {
            var package = await _context.Package
                .Include(p => p.Sender)
                .Include(p => p.Recipient)
                .Include(p => p.Statuses)
                .FirstOrDefaultAsync(p => p.TrackingNumber == trackingNumber);

            if (package == null)
            {
                return NotFound();
            }

            return PackageToDTO(package);
        }

        // PATCH: api/packages/5
        [HttpPatch("{trackingNumber}")]
        public async Task<IActionResult> UpdateStatus(int trackingNumber, [FromBody] UpdateStatusDTO body)
        {
            if (!Enum.TryParse<Status.StatusTypes>(body.NewStatus, out var newStatus))
            {
                return BadRequest($"Invalid status: {body.NewStatus}");
            }

            var package = await _context.Package
                .Include(p => p.Statuses)
                .Include(p => p.Sender)
                .Include(p => p.Recipient)
                .FirstOrDefaultAsync(p => p.TrackingNumber == trackingNumber);


            if (package == null)
            {
                return NotFound();
            }

            var previousStatus = package.Statuses.Last().StatusName;

            if (previousStatus == Status.StatusTypes.Accepted || previousStatus == Status.StatusTypes.Canceled)
            {
                return BadRequest($"Cannot change the state of {previousStatus}");
            }

            var transitions = new Dictionary<Status.StatusTypes, Status.StatusTypes[]>
            {
                { Status.StatusTypes.Created,  new[] { Status.StatusTypes.Sent, Status.StatusTypes.Canceled } },
                { Status.StatusTypes.Sent,     new[] { Status.StatusTypes.Accepted, Status.StatusTypes.Returned, Status.StatusTypes.Canceled } },
                { Status.StatusTypes.Returned, new[] { Status.StatusTypes.Sent, Status.StatusTypes.Canceled } },
            };

            if (transitions.TryGetValue(previousStatus, out var allowedTransitions) && allowedTransitions.Contains(newStatus))
            {
                package.Statuses.Add(new Status
                {
                    StatusName = newStatus,
                    Timestamp = DateTime.UtcNow
                });
            }
            else
            {
                return BadRequest($"{previousStatus} can only transition to {string.Join(", ", allowedTransitions!)}, not {newStatus}");
            }

            await _context.SaveChangesAsync();

            return Ok(PackageToDTO(package));
        }

        // POST: api/packages
        [HttpPost]
        public async Task<ActionResult<Package>> PostPackage([FromBody] CreatePackageDTO body)
        {
            var sender = await _context.Contact
                .FirstOrDefaultAsync(c => c.Name == body.Sender.Name && c.Address == body.Sender.Address && c.Phone == body.Sender.Phone)
                ?? new Contact
                {
                    Name = body.Sender.Name,
                    Address = body.Sender.Address,
                    Phone = body.Sender.Phone
                };

            var recipient = await _context.Contact
                .FirstOrDefaultAsync(c => c.Name == body.Recipient.Name && c.Address == body.Recipient.Address && c.Phone == body.Recipient.Phone) 
                ?? new Contact
                {
                    Name = body.Recipient.Name,
                    Address = body.Recipient.Address,
                    Phone = body.Recipient.Phone
                };


            var package = new Package
            {
                Name = body.Name,
                Sender = sender,
                Recipient = recipient,
                Statuses = new List<Status>
                {
                    new Status
                    {
                        StatusName = Status.StatusTypes.Created,
                        Timestamp = DateTime.UtcNow
                    }
                }
            };

            _context.Package.Add(package);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPackage", new { package.TrackingNumber }, PackageToDTO(package));
        }

        // DELETE: api/Packages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePackage(int id)
        {
            var package = await _context.Package.FindAsync(id);
            if (package == null)
            {
                return NotFound();
            }

            _context.Package.Remove(package);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PackageExists(int id)
        {
            return _context.Package.Any(e => e.TrackingNumber == id);
        }

        public static PackageDTO PackageToDTO(Package package)
        {
            List<StatusDTO> statusesDTO = new List<StatusDTO>();

            foreach (Status status in package.Statuses)
            {
                StatusDTO statusDTO = new StatusDTO
                {
                    StatusName = status.StatusName.ToString(),
                    Timestamp = status.Timestamp
                };

                statusesDTO.Add(statusDTO);
            }

            return new PackageDTO
            {
                TrackingNumber = package.TrackingNumber,
                Name = package.Name,
                Sender = new ContactDTO
                {
                    ContactID = package.SenderID,
                    Name = package.Sender.Name,
                    Address = package.Sender.Address,
                    Phone = package.Sender.Phone
                },
                Recipient = new ContactDTO
                {
                    ContactID = package.RecipientID,
                    Name = package.Recipient.Name,
                    Address = package.Recipient.Address,
                    Phone = package.Recipient.Phone
                },
                Statuses = statusesDTO
            };
        }
    }
}
