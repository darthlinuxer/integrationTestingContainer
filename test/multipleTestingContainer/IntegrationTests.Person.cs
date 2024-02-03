namespace multipleContainers;

[TestClass]
public class PersonTests
{
    private CustomWebApplicationFactory? _factory;
    private IContainer _container;
    private HttpClient? _client;
    public PersonTests()
    {
        _container = new ContainerBuilder()
             .WithImage("mysql:latest")
             .WithExposedPort(3306)
             .WithPortBinding(3306, true)
             .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(_ => _.ForPort(3306)))
             .WithCleanUp(true)
             .WithAutoRemove(true)
             .WithHostname("mysqlcontainer")
             .WithName("mysqlcontainer")
             .WithEnvironment("MYSQL_ROOT_PASSWORD", "123456")
             .WithEnvironment("MYSQL_DATABASE", "test")
             .Build();
    }

    [TestInitialize]
    public async Task TestInitialize()
    {
        // Start the container.
        await _container.StartAsync().ConfigureAwait(false);

        _factory = new CustomWebApplicationFactory(_container);
        _client = _factory.CreateClient();
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(121)]
    public async Task CreatePerson_ReturnsBadRequest_WhenAgeIsInvalid(int age)
    {
        // Construct the request URI by specifying the scheme, hostname, assigned random host port, and the endpoint "uuid".
        var requestUri = new UriBuilder(Uri.UriSchemeHttp, _container.Hostname, _container.GetMappedPublicPort(3306), "/api/person").Uri;

        var result = await _client!.PostAsJsonAsync<Person>(requestUri,
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
        // Construct the request URI by specifying the scheme, hostname, assigned random host port, and the endpoint "uuid".
        var requestUri = new UriBuilder(Uri.UriSchemeHttp, _container.Hostname, _container.GetMappedPublicPort(3306), "/api/person").Uri;
        var result = await _client!.PostAsJsonAsync<Person>(requestUri,
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
    [DataRow("camilo.chaves@gmail")]
    public async Task CreatePerson_ReturnsBadRequest_WhenEmailIsInvalid(string email)
    {
        // Construct the request URI by specifying the scheme, hostname, assigned random host port, and the endpoint "uuid".
        var requestUri = new UriBuilder(Uri.UriSchemeHttp, _container.Hostname, _container.GetMappedPublicPort(3306), "/api/person").Uri;
        var result = await _client!.PostAsJsonAsync<Person>(requestUri,
        new Person
        {
            FirstName = "Camilo",
            SurName = "Chaves",
            Email = email,
            Age = 47
        });
        Assert.IsFalse(result.IsSuccessStatusCode);
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        _client!.Dispose();
        _factory!.Dispose();
        await _container!.DisposeAsync();
    }
}