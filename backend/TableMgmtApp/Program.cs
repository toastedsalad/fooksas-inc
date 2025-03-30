using Microsoft.EntityFrameworkCore;
using TableMgmtApp.Persistence;
// using Microsoft.Data.Sqlite;

namespace TableMgmtApp;

public class Program {
    public async static Task Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();

        // Add database.
        builder.Services.AddDbContext<TableMgmtAppDbContext>(options =>
                options.UseSqlite("DataSource=mgmtapp.db"));  // SQLite connection string

        // Add App services
        // TODO move timer settings to config.
        builder.Services.AddScoped<ITableRepository, TableSQLRepository>();
        builder.Services.AddScoped<IPlaySessionRepository, PlaySessionSQLRepository>();
        builder.Services.AddScoped<IPlayerRepository, PlayerSQLRepository>();
        builder.Services.AddSingleton<ITimeProvider, SystemTimeProvider>();
        builder.Services.AddSingleton<TableManagerService>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Ensure database is created at app startup
        // using (var scope = app.Services.CreateScope()) {
        //     var dbContext = scope.ServiceProvider.GetRequiredService<TableMgmtAppDbContext>();
        //     dbContext.Database.EnsureCreated();  // Ensure the schema is created
        // }
        // Ensure database is up to date with the latest migrations at app startup
        //

        using (var scope = app.Services.CreateScope()) {
            var dbContext = scope.ServiceProvider.GetRequiredService<TableMgmtAppDbContext>();
            dbContext.Database.Migrate();  // Apply any pending migrations
        }

        // Get a list of tables from DB.
        List<PoolTable> tables = default!;
        using (var scope = app.Services.CreateScope()) {
            var repo = scope.ServiceProvider.GetRequiredService<ITableRepository>();
            tables = await repo.GetAllAsync();
        }

        // Create table service with a dict of running tables.
        using (var scope = app.Services.CreateScope()) {
            var tableManagerService = scope.ServiceProvider.GetRequiredService<TableManagerService>();
            tableManagerService.CreateAllTableManagersAsync(tables);
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
