using backend.Models;
using backend;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Tests
{
    public abstract class TestBase
    {
        protected DbContextOptions<ApplicationDbContext> ContextOptions { get; }

        protected TestBase()
        {
            ContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"TestDb {Guid.NewGuid().ToString()}")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            Seed();
        }

        private void Seed()
        {
            using var context = new ApplicationDbContext(ContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var senderAmazon = new Contact { Name = "Amazon", Address = "Berlin, DE", Phone = "+49 123456" };
            var senderApple = new Contact { Name = "Apple", Address = "Cupertino, USA", Phone = "+1 123123" };
            var recipientJohn = new Contact { Name = "John Doe", Address = "New York, USA", Phone = "+1 555111" };
            var recipientJane = new Contact { Name = "Jane Smith", Address = "London, UK", Phone = "+44 777888" };

            //package Created
            context.Package.Add(new Package
            {
                Name = "Created",
                Sender = senderAmazon,
                Recipient = recipientJohn,
                Statuses = new List<Status>
                {
                    new Status { StatusName = Status.StatusTypes.Created, Timestamp = DateTime.UtcNow }
                }
            });

            //package Canceled
            context.Package.Add(new Package
            {
                Name = "Canceled",
                Sender = senderApple,
                Recipient = recipientJane,
                Statuses = new List<Status>
                {
                    new Status { StatusName = Status.StatusTypes.Created, Timestamp = DateTime.UtcNow.AddMinutes(-30) },
                    new Status { StatusName = Status.StatusTypes.Canceled, Timestamp = DateTime.UtcNow }
                }
            });

            //package Sent
            context.Package.Add(new Package
            {
                Name = "Sent",
                Sender = senderAmazon,
                Recipient = recipientJane,
                Statuses = new List<Status>
                {
                    new Status { StatusName = Status.StatusTypes.Created, Timestamp = DateTime.UtcNow.AddMinutes(-60) },
                    new Status { StatusName = Status.StatusTypes.Sent, Timestamp = DateTime.UtcNow }
                }
            });

            context.SaveChanges();
        }


        protected ApplicationDbContext CreateContext()
            => new ApplicationDbContext(ContextOptions);
    }
}
