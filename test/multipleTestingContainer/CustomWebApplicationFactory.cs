namespace multipleContainers;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncDisposable
{
    private IContainer container;
    public CustomWebApplicationFactory(IContainer container)
    {
        this.container = container;
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(c => c.ServiceType == typeof(DbContextPool<AppDbContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }
            services.AddPooledDbContextFactory<AppDbContext>(options =>
            {
                var connectionString = $"server={container.Hostname};port={container.GetMappedPublicPort(3306)};user=root;password=123456;database=test";
                options.UseMySql(
                    connectionString, // Connection string
                    new MySqlServerVersion(new Version(8, 0, 21)), // MySQL server version
                    mySqlOptions => mySqlOptions.MigrationsAssembly("example.api"));
            });

            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        });
    }
}