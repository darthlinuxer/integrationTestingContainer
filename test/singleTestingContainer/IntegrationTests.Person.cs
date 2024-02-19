using MySqlConnector;

namespace singleContainer;

[TestClass]
public class PersonTests
{
    private static CustomWebApplicationFactory? _factory;
    private static IContainer _container = new ContainerBuilder()
             .WithImage("mysql:latest")
             .WithPortBinding(3306, true)
             .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(3306))
             .WithCleanUp(true)
             .WithAutoRemove(true)
             .WithEnvironment("MYSQL_ROOT_PASSWORD", "123456")
             .WithEnvironment("MYSQL_DATABASE", "test")
             .Build();
    private static HttpClient? _client;

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext context)
    {
        // Start the container.
        await _container.StartAsync().ConfigureAwait(false);

        _factory = new CustomWebApplicationFactory(_container);
        _client = _factory.CreateClient();
    }

    [TestInitialize]
    public async Task TestInitialization()
    {
        var connectionString = $"server={_container.Hostname};port={_container.GetMappedPublicPort(3306)};user=root;password=123456;database=test;";
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        using var command = new MySqlCommand($"TRUNCATE TABLE Person;", connection);
        await command.ExecuteNonQueryAsync();
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(121)]
    public async Task CreatePerson_ReturnsBadRequest_WhenAgeIsInvalid(int age)
    {
        var result = await _client!.PostAsJsonAsync<Person>("/api/person",
        new Person
        {
            FirstName = "Camilo",
            SurName = "Chaves",
            Email = "hello@test.com",
            Age = age
        });
        Assert.IsFalse(result.IsSuccessStatusCode);
    }

    [TestMethod]
    [DataRow("camilo", "chaves", "camilo.chaves@gmail.com", 47)]
    [DataRow("luke", "skywalker", "rebelalliance@gmail.com", 22)]
    public async Task CreatePerson_ReturnsCreated_WhenDataIsInvalid(
        string firstName,
        string surName,
        string email,
        int age)
    {
        var result = await _client!.PostAsJsonAsync<Person>("/api/person",
        new Person
        {
            FirstName = firstName,
            SurName = surName,
            Email = email,
            Age = age
        });
        Assert.IsTrue(result.IsSuccessStatusCode);
    }

    [TestMethod]
    [DataRow("camilochaves@")]
    [DataRow("")]
    public async Task CreatePerson_ReturnsBadRequest_WhenEmailIsInvalid(string email)
    {
        var result = await _client!.PostAsJsonAsync<Person>("/api/person",
        new Person
        {
            FirstName = "Camilo",
            SurName = "Chaves",
            Email = email,
            Age = 47
        });
        Assert.IsFalse(result.IsSuccessStatusCode);
    }

    [ClassCleanup]
    public static async Task TestCleanup()
    {
        _client!.Dispose();
        _factory!.Dispose();
        await _container!.DisposeAsync();
    }
}