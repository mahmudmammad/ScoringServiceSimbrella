using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using InternTask;
using InternTask.DB;
using InternTask.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

public class ScoringControllerIntegrationTests : IAsyncLifetime
{
    private PostgreSqlTestcontainer _pgContainer;
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    public async Task InitializeAsync()
    {
      
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

        _pgContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration
            {
                Database = "testdb",
                Username = "postgres",
                Password = "postgres"
            })
            .WithCleanUp(true)
            .Build();

        await _pgContainer.StartAsync();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
      
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ScoringDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

            
                    services.AddDbContext<ScoringDbContext>(options =>
                        options.UseNpgsql(_pgContainer.ConnectionString));
                });
            });


        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ScoringDbContext>();
        await db.Database.MigrateAsync();

        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await _pgContainer.DisposeAsync();
    }

    [Fact]
    public async Task EvaluateCustomer_ReturnsApproved_WhenConditionsMet()
    {
        var customer = new Customer
        {
            Salary = 5000,
            LoanCount = 1,
            AccountBalance = 5000,
            Age = 30,
            Gender = "Male",
            HasDefaultHistory = false
        };

        var response = await _client.PostAsJsonAsync("/api/scoring/evaluate", customer);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ScoringResult>();
        Assert.NotNull(result);
        Assert.True(result!.IsApproved);
    }
}
