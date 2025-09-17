using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataExporter;
using DataExporter.Controllers;
using DataExporter.Dtos;
using DataExporter.Model;
using DataExporter.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class PoliciesControllerTests
{
    private static ExporterDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ExporterDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var ctx = new ExporterDbContext(options);
        ctx.Database.EnsureDeleted();
        ctx.Database.EnsureCreated();

        ctx.Notes.AddRange(
            new Note { Id = 101, PolicyId = 1, Text = "Some note asdaksd" },
            new Note { Id = 102, PolicyId = 2, Text = "some other note " },
            new Note { Id = 103, PolicyId = 2, Text = "..note" }
        );
        ctx.SaveChanges();

        return ctx;
    }

    private static PoliciesController CreateController(string dbName)
    {
        var ctx = CreateContext(dbName);
        var svc = new PolicyService(ctx);
        return new PoliciesController(svc);
    }

    [Fact]
    public async Task GetPolicies_ReturnsOk_WithSeededItems()
    {
        // arrange
        var controller = CreateController("testdb");

        // act
        var result = await controller.GetPolicies(CancellationToken.None);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IEnumerable<ReadPolicyDto>>(ok.Value);
        Assert.True(list.Any(), "Expected seeded policies.");
    }

   
}
