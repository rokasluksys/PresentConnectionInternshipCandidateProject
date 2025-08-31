using System.Linq;
using System.Threading.Tasks;
using backend.Controllers;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;
using backend.Models.DTOs;

namespace Tests
{
    public class UpdateStatus : TestBase
    {
        [Fact]
        public async Task CorrectInput_CreatedToCanceled()
        {
            using var context = CreateContext();
            var controller = new PackagesController(context);

            int trackingNumber = 1;

            var body = new UpdateStatusDTO
            {
                NewStatus = "Canceled"
            };

            await controller.UpdateStatus(trackingNumber, body);

            var package = await context.Package
                .Include(p => p.Statuses)
                .SingleAsync(p => p.TrackingNumber == trackingNumber, 
                    cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal("Canceled", package.Statuses.Last().StatusName.ToString());
        }

        [Fact]
        public async Task CorrectInput_SentToReturned()
        {
            using var context = CreateContext();
            var controller = new PackagesController(context);

            int trackingNumber = 3;

            var body = new UpdateStatusDTO
            {
                NewStatus = "Returned"
            };

            await controller.UpdateStatus(trackingNumber, body);

            var package = await context.Package
                .Include(p => p.Statuses)
                .SingleAsync(p => p.TrackingNumber == trackingNumber, 
                    cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal("Returned", package.Statuses.Last().StatusName.ToString());
        }

        [Fact]
        public async Task IncorrectInput_SentToCreated()
        {
            using var context = CreateContext();
            var controller = new PackagesController(context);

            int trackingNumber = 3;

            var body = new UpdateStatusDTO
            {
                NewStatus = "Created"
            };

            var result = await controller.UpdateStatus(trackingNumber, body);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var message = Assert.IsType<string>(badRequest.Value);

            Assert.Equal("Sent can only transition to Accepted, Returned, Canceled, not Created", message);
        }

        [Fact]
        public async Task IncorrectInput_CanceledToAccepted()
        {
            using var context = CreateContext();
            var controller = new PackagesController(context);

            int trackingNumber = 2;

            var body = new UpdateStatusDTO
            {
                NewStatus = "Accepted"
            };

            var result = await controller.UpdateStatus(trackingNumber, body);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var message = Assert.IsType<string>(badRequest.Value);

            Assert.Equal("Cannot change the state of Canceled", message);
        }


    }
}
