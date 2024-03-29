using System.Diagnostics;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace example;

[TestClass]
public class ExampleUnitTest
{
    private IContainer container;
    private HttpClient httpClient;

  public ExampleUnitTest()
  {
    container = new ContainerBuilder()
      // Set the image for the container to "testcontainers/helloworld:1.1.0".
      .WithImage("testcontainers/helloworld:1.1.0")
      // Bind port 8080 of the container to a random port on the host.
      .WithPortBinding(8080, true)
      // Wait until the HTTP endpoint of the container is available.
      .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(8080)))
      // Build the container configuration.
      .WithAutoRemove(true)
      .WithCleanUp(true)
      .Build();


    // Create a new instance of HttpClient to send HTTP requests.
    httpClient = new HttpClient();
  }

  [TestInitialize]
  public async Task Init()
  {
    // Start the container.
    await container.StartAsync()
      .ConfigureAwait(false);
  }

  [TestMethod]
  public async Task SingleTest()
  {
    // Construct the request URI by specifying the scheme, hostname, assigned random host port, and the endpoint "uuid".
    var requestUri = new UriBuilder(Uri.UriSchemeHttp, container.Hostname, container.GetMappedPublicPort(8080), "uuid").Uri;

    // Send an HTTP GET request to the specified URI and retrieve the response as a string.
    var guid = await httpClient.GetStringAsync(requestUri)
      .ConfigureAwait(false);

    // Ensure that the retrieved UUID is a valid GUID.
    Debug.Assert(Guid.TryParse(guid, out _));

  }
}