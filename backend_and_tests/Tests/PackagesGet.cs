using System.Linq;
using System.Threading.Tasks;
using backend.Controllers;
using backend.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace Tests;

public class PackagesGet : TestBase
{
    [Fact]
    public async Task All()
    {
        using var context = CreateContext();
        var controller = new PackagesController(context);

        var result = await controller.GetPackage();

        var packages = Assert.IsType<IEnumerable<PackageDTO>>(result.Value, exactMatch: false);

        var list = packages.ToList();

        Assert.Equal(3, list.Count);
        Assert.Equal("Created", packages.First().Name);
        Assert.Equal("Sent", packages.Last().Name);
    }
}