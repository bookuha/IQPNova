using Xunit;
using Exercism.Tests;

public class HelloWorldTests
{
    [Fact]
    public void HelloWorld()
    {
        Assert.Equal("Hello, World!", Solution.HelloWorld());
    }
}