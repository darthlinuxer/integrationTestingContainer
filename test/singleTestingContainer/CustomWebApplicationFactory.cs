namespace singleContainer;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncDisposable
{
    private IContainer container;
    private string baseAddr = "http://localhost:5001";
    public CustomWebApplicationFactory(IContainer container)
    {
        this.container = container;
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);
        // Set the desired headers (including application/json)
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri(baseAddr);

        // You can also add other headers if needed
        // client.DefaultRequestHeaders.Add("Authorization", "Bearer YourAccessToken");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("urls", baseAddr);

        builder.ConfigureTestServices(services =>
        {
            // Remove the existing AppDbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = $"server={container.Hostname};port={container.GetMappedPublicPort(3306)};user=root;password=123456;database=test;";
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