using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Todo.Infrastructure.Data;
using Todo.Domain.Models;

namespace Todo.IntegrationTests;

public class CustomWebApplicationFactory 
    : WebApplicationFactory<Program>
{
    private SqliteConnection _connection;
    

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.Database.EnsureCreated();

            SeedDatabase(db);
        });
    }

    private static void SeedDatabase(AppDbContext db)
    {
        db.Todos.RemoveRange(db.Todos); 

        var todo1 = new TodoItem("Todo 1");
        var todo2 = new TodoItem("Todo 2");
        todo2.MarkAsCompleted();

        db.Todos.AddRange(todo1, todo2);
        db.SaveChanges();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        _connection?.Close();
    }
}