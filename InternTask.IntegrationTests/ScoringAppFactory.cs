// // ScoringAppFactory.cs in your InternTask.IntegrationTests project
//
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Mvc.Testing;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Logging;
// using InternTask.DB; // Adjust namespace if needed
// using System.Linq; // For LINQ operations like .SingleOrDefault() and .Where()
//
// public class ScoringAppFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
// {
//     protected override IHostBuilder CreateHostBuilder()
//     {
//         // 1. Call your application's CreateHostBuilder to get the base configuration.
//         // This will include the Npgsql DbContext registration.
//         var hostBuilder = Program.CreateHostBuilder(Array.Empty<string>());
//
//         // 2. Configure the WebHost to modify services.
//         hostBuilder.ConfigureWebHost(webBuilder =>
//         {
//             webBuilder.UseEnvironment("Test"); // Still good practice to set test environment
//
//             webBuilder.ConfigureServices(services =>
//             {
//                 // --- CRITICAL: REMOVE EXISTING DB CONTEXT SERVICES ---
//                 // Find and remove ALL DbContext-related descriptors for ScoringDbContext.
//                 // This is more robust than just removing DbContextOptions.
//                 var dbContextDescriptors = services.Where(
//                     d => d.ServiceType == typeof(ScoringDbContext) ||
//                          d.ServiceType == typeof(DbContextOptions<ScoringDbContext>) ||
//                          d.ServiceType == typeof(DbContextOptions)
//                 ).ToList(); // .ToList() to avoid modifying collection while enumerating
//
//                 foreach (var descriptor in dbContextDescriptors)
//                 {
//                     services.Remove(descriptor);
//                 }
//
//                 // --- CRITICAL: ADD IN-MEMORY DB CONTEXT ---
//                 // Add ScoringDbContext using an in-memory database for testing.
//                 services.AddDbContext<ScoringDbContext>(options =>
//                 {
//                     options.UseInMemoryDatabase("InMemoryScoringDbForTesting");
//                 });
//
//                 // You might also need to remove other Npgsql-specific services if they cause conflicts.
//                 // For example, if you explicitly added Npgsql services like NpgsqlConnection or NpgsqlDataSource.
//                 // However, AddDbContext for Npgsql usually handles its own dependencies which are then removed above.
//             });
//         });
//
//         // Return the configured IHostBuilder. WebApplicationFactory will then build it into a TestHost.
//         return hostBuilder;
//     }
//
//     // This method is called by WebApplicationFactory to configure the HttpClient
//     // It's a great place to ensure a clean database state for each test.
//     protected override void ConfigureClient(HttpClient client)
//     {
//         // Get a service scope to access the DbContext
//         using (var scope = Services.CreateScope())
//         {
//             var dbContext = scope.ServiceProvider.GetRequiredService<ScoringDbContext>();
//             // Ensure the database is deleted and recreated for a fresh start for each test
//             // This is safe for in-memory databases and ensures test isolation.
//             dbContext.Database.EnsureDeleted();
//             dbContext.Database.EnsureCreated();
//         }
//
//         base.ConfigureClient(client);
//     }
// }