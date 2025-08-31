using System.Linq;
using System.Threading.Tasks;
using backend.Controllers;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;
using backend.Models.DTOs;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Tests
{
    public class PackagesPost : TestBase
    {
        //my app handles input validation with variable annotations, however, they're only enforced when handling actual HTTP requests,
        //not when calling controller methods in unit tests,
        //therefore, I emulate the same kind of validation with this method
        private List<ValidationResult> ValidateDTO(object dto)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(dto);

            Validator.TryValidateObject(dto, context, results, validateAllProperties: true);

            foreach (var property in dto.GetType().GetProperties())
            {
                var value = property.GetValue(dto);
                if (value != null && !property.PropertyType.IsPrimitive && property.PropertyType != typeof(string))
                {
                    results.AddRange(ValidateDTO(value));
                }
            }

            return results;
        }

        [Fact]
        public void IncorrectInput_MissingPackageName()
        {
            var dto = new CreatePackageDTO
            {
                Name = "",
                Sender = new CreateContactDTO { Name = "Apple", Address = "Cupertino, USA", Phone = "+1 123456" },
                Recipient = new CreateContactDTO { Name = "Tester", Address = "Vilnius, LT", Phone = "+370 654321" }
            };

            var validationErrorResults = ValidateDTO(dto);

            Assert.NotEmpty(validationErrorResults);
            Assert.Contains(validationErrorResults, r => r.MemberNames.Contains("Name"));
        }

        [Fact]
        public void IncorrectInput_IncorrectPhone()
        {
            var dto = new CreateContactDTO
            {
                Name = "Apple",
                Address = "Cupertino",
                Phone = "abc"
            };

            var validationErrorResults = ValidateDTO(dto);

            Assert.NotEmpty(validationErrorResults);
            Assert.Contains(validationErrorResults, r => r.MemberNames.Contains("Phone"));
        }

        [Fact]
        public void IncorrectInput_MissingSender()
        {
            var dto = new CreatePackageDTO
            {
                Name = "MacBook Test",
                Recipient = new CreateContactDTO { Name = "Tester", Address = "Vilnius, LT", Phone = "+370 654321" }
            };

            var validationErrorResults = ValidateDTO(dto);

            Assert.NotEmpty(validationErrorResults);
            Assert.Contains(validationErrorResults, r => r.MemberNames.Contains("Sender"));
        }

        [Fact]
        public async Task CorrectInput()
        {
            using var context = CreateContext();
            var controller = new PackagesController(context);

            var dto = new CreatePackageDTO
            {
                Name = "MacBook Test",
                Sender = new CreateContactDTO { Name = "Apple", Address = "Cupertino, USA", Phone = "+1 123456" },
                Recipient = new CreateContactDTO { Name = "Tester", Address = "Vilnius, LT", Phone = "+370 654321" }
            };

            var validationErrorResults = ValidateDTO(dto);
            Assert.Empty(validationErrorResults);

            await controller.PostPackage(dto);

            var dbPackage = await context.Package.SingleAsync(p => p.Name == "MacBook Test",
                cancellationToken: TestContext.Current.CancellationToken);
            Assert.Equal("Apple", dbPackage.Sender.Name);
        }


    }
}
