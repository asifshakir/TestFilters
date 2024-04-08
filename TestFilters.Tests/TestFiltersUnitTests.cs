using TestFilters.Models;
using TestFilters.Providers;

namespace TestFilters.Tests;

[TestClass]
public class TestFiltersUnitTests
{

    [TestInitialize]
    public void Setup()
    {
    }

    private IEnumerable<Data> datas = [
        new() { Domain = "domain1", Client = "client1", Environment = "env1"},
        new() { Domain = "domain1", Client = "client1", Environment = "env2"},
        new() { Domain = "domain1", Client = "client2", Environment = "env1"},
        new() { Domain = "domain1", Client = "client2", Environment = "env2"},
        new() { Domain = "domain2", Client = "client1", Environment = "env1"},
        new() { Domain = "domain2", Client = "client1", Environment = "env2"},
        new() { Domain = "domain2", Client = "client2", Environment = "env1"},
        new() { Domain = "domain2", Client = "client2", Environment = "env2"},
    ];

    [TestMethod]
    [DataRow(new string[] { "*:*:*" }, "ALL:ALL:ALL", true, new string[] { "domain1:client1:env1", "domain1:client1:env2", "domain1:client2:env1", "domain1:client2:env2", "domain2:client1:env1", "domain2:client1:env2", "domain2:client2:env1", "domain2:client2:env2" })]
    [DataRow(new string[] { "*:*:*" }, "domain1:ALL:ALL", true, new string[] { "domain1:client1:env1", "domain1:client1:env2", "domain1:client2:env1", "domain1:client2:env2" })]
    [DataRow(new string[] { "*:*:*" }, "domain2:ALL:ALL", true, new string[] { "domain2:client1:env1", "domain2:client1:env2", "domain2:client2:env1", "domain2:client2:env2" })]
    [DataRow(new string[] { "*:*:*" }, "ALL:client1:ALL", true, new string[] { "domain1:client1:env1", "domain1:client1:env2", "domain2:client1:env1", "domain2:client1:env2" })]
    [DataRow(new string[] { "*:*:*" }, "ALL:client2:ALL", true, new string[] { "domain1:client2:env1", "domain1:client2:env2", "domain2:client2:env1", "domain2:client2:env2" })]
    [DataRow(new string[] { "*:*:*" }, "ALL:ALL:env1", true, new string[] { "domain1:client1:env1", "domain1:client2:env1", "domain2:client1:env1", "domain2:client2:env1" })]
    [DataRow(new string[] { "*:*:*" }, "ALL:ALL:env2", true, new string[] { "domain1:client1:env2", "domain1:client2:env2", "domain2:client1:env2", "domain2:client2:env2" })]
    [DataRow(new string[] { "*:*:*" }, "domain1:client1:ALL", true, new string[] { "domain1:client1:env1", "domain1:client1:env2" })]
    [DataRow(new string[] { "*:*:*" }, "domain2:client2:ALL", true, new string[] { "domain2:client2:env1", "domain2:client2:env2" })]
    [DataRow(new string[] { "*:*:*" }, "ALL:client1:env1", true, new string[] { "domain1:client1:env1", "domain2:client1:env1" })]
    [DataRow(new string[] { "*:*:*" }, "ALL:client2:env2", true, new string[] { "domain1:client2:env2", "domain2:client2:env2" })]
    [DataRow(new string[] { "*:*:*" }, "domain1:ALL:env1", true, new string[] { "domain1:client1:env1", "domain1:client2:env1" })]
    [DataRow(new string[] { "*:*:*" }, "domain2:ALL:env2", true, new string[] { "domain2:client1:env2", "domain2:client2:env2" })]
    [DataRow(new string[] { "*:*:*" }, "domain1:client1:env1", true, new string[] { "domain1:client1:env1" })]
    [DataRow(new string[] { "*:*:*" }, "domain2:client2:env2", true, new string[] { "domain2:client2:env2" })]
    [DataRow(new string[] { "domain1:*:*" }, "ALL:ALL:ALL", true, new string[] { "domain1:client1:env1", "domain1:client1:env2", "domain1:client2:env1", "domain1:client2:env2" })]
    [DataRow(new string[] { "domain1:*:*" }, "domain1:ALL:ALL", true, new string[] { "domain1:client1:env1", "domain1:client1:env2", "domain1:client2:env1", "domain1:client2:env2" })]
    [DataRow(new string[] { "domain1:*:*" }, "domain2:ALL:ALL", false, new string[] { })]
    [DataRow(new string[] { "domain1:*:*" }, "ALL:client1:ALL", true, new string[] { "domain1:client1:env1", "domain1:client1:env2" })]
    [DataRow(new string[] { "domain1:*:*" }, "ALL:client2:ALL", true, new string[] { "domain1:client2:env1", "domain1:client2:env2" })]
    [DataRow(new string[] { "domain1:*:*" }, "ALL:ALL:env1", true, new string[] { "domain1:client1:env1", "domain1:client2:env1" })]
    [DataRow(new string[] { "domain1:*:*" }, "ALL:ALL:env2", true, new string[] { "domain1:client1:env2", "domain1:client2:env2" })]
    [DataRow(new string[] { "domain1:*:*" }, "domain1:client1:ALL", true, new string[] { "domain1:client1:env1", "domain1:client1:env2" })]
    [DataRow(new string[] { "domain1:*:*" }, "domain2:client2:ALL", false, new string[] { })]
    [DataRow(new string[] { "domain1:*:*" }, "ALL:client1:env1", true, new string[] { "domain1:client1:env1" })]
    [DataRow(new string[] { "domain1:*:*" }, "ALL:client2:env2", true, new string[] { "domain1:client2:env2" })]
    [DataRow(new string[] { "domain1:*:*" }, "domain1:ALL:env1", true, new string[] { "domain1:client1:env1", "domain1:client2:env1" })]
    [DataRow(new string[] { "domain1:*:*" }, "domain2:ALL:env2", false, new string[] { })]
    [DataRow(new string[] { "domain1:*:*" }, "domain1:client1:env1", true, new string[] { "domain1:client1:env1" })]
    [DataRow(new string[] { "domain1:*:*" }, "domain2:client2:env2", false, new string[] { })]
    [DataRow(new string[] { "domain1:client1:*" }, "ALL:ALL:ALL", true, new string[] { "domain1:client1:env1", "domain1:client1:env2" })]
    [DataRow(new string[] { "domain1:client1:*" }, "domain1:ALL:ALL", true, new string[] { "domain1:client1:env1", "domain1:client1:env2" })]
    [DataRow(new string[] { "domain1:client1:*" }, "domain2:ALL:ALL", false, new string[] { })]
    [DataRow(new string[] { "domain1:client1:*" }, "ALL:client1:ALL", true, new string[] { "domain1:client1:env1", "domain1:client1:env2" })]
    [DataRow(new string[] { "domain1:client1:*" }, "ALL:client2:ALL", false, new string[] { })]
    [DataRow(new string[] { "domain1:client1:*" }, "ALL:ALL:env1", true, new string[] { "domain1:client1:env1" })]
    [DataRow(new string[] { "domain1:client1:*" }, "ALL:ALL:env2", true, new string[] { "domain1:client1:env2" })]
    [DataRow(new string[] { "domain1:client1:*" }, "domain1:client1:ALL", true, new string[] { "domain1:client1:env1", "domain1:client1:env2" })]
    [DataRow(new string[] { "domain1:client1:*" }, "domain2:client2:ALL", false, new string[] { })]
    [DataRow(new string[] { "domain1:client1:*" }, "ALL:client1:env1", true, new string[] { "domain1:client1:env1" })]
    [DataRow(new string[] { "domain1:client1:*" }, "ALL:client2:env2", false, new string[] { })]
    [DataRow(new string[] { "domain1:client1:*" }, "domain1:ALL:env1", true, new string[] { "domain1:client1:env1" })]
    [DataRow(new string[] { "domain1:client1:*" }, "domain2:ALL:env2", false, new string[] { })]
    [DataRow(new string[] { "domain1:client1:*" }, "domain1:client1:env1", true, new string[] { "domain1:client1:env1" })]
    [DataRow(new string[] { "domain1:client1:*" }, "domain2:client2:env2", false, new string[] { })]
    [DataRow(new string[] { "domain1:client1:env1" }, "ALL:ALL:ALL", true, new string[] { "domain1:client1:env1" })]
    [DataRow(new string[] { "domain1:client1:env1" }, "domain1:ALL:ALL", true, new string[] { "domain1:client1:env1" })]
    [DataRow(new string[] { "domain1:client1:env1" }, "domain2:ALL:ALL", false, new string[] { })]
    [DataRow(new string[] { "domain1:client1:env1" }, "ALL:client1:ALL", true, new string[] { "domain1:client1:env1" })]
    [DataRow(new string[] { "domain1:client1:env1" }, "ALL:client2:ALL", false, new string[] { })]
    [DataRow(new string[] { "domain1:client1:env1" }, "ALL:ALL:env1", true, new string[] { "domain1:client1:env1" })]
    [DataRow(new string[] { "domain1:client1:env1" }, "ALL:ALL:env2", false, new string[] { })]
    [DataRow(new string[] { "domain1:client1:env1" }, "domain1:client1:ALL", true, new string[] { "domain1:client1:env1" })]
    [DataRow(new string[] { "domain1:client1:env1" }, "domain2:client2:ALL", false, new string[] { })]
    [DataRow(new string[] { "domain1:client1:env1" }, "ALL:client1:env1", true, new string[] { "domain1:client1:env1" })]
    [DataRow(new string[] { "domain1:client1:env1" }, "ALL:client2:env2", false, new string[] { })]
    [DataRow(new string[] { "domain1:client1:env1" }, "domain1:ALL:env1", true, new string[] { "domain1:client1:env1" })]
    [DataRow(new string[] { "domain1:client1:env1" }, "domain2:ALL:env2", false, new string[] { })]
    [DataRow(new string[] { "domain1:client1:env1" }, "domain1:client1:env1", true, new string[] { "domain1:client1:env1" })]
    [DataRow(new string[] { "domain1:client1:env1" }, "domain2:client2:env2", false, new string[] { })]
    public void Test_Request_Filters(string[] claims, string request, bool isValid, string[] expected)
    {
        RequestContextProvider provider = new RequestContextProvider(claims);
        string[] requestParts = request.Split(":");
        var req = new RequestContext() { Domain = requestParts[0], Client = requestParts[1], Environment = requestParts[2] };
        bool testIsValid = provider.IsValidRequest(req);
        Assert.AreEqual(isValid, testIsValid);        
        if (isValid)
        {
            string[] exp = expected.ToArray();
            string[] actual = provider.FilterData(datas, req).Select(d => $"{d.Domain}:{d.Client}:{d.Environment}").ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Cleanup resources if necessary
    }
}

public class RequestTestCase
{
    public string[] Claims { get; set; } = [];
    public string Request { get; set; } = string.Empty;
    public bool IsValid { get; set; } = false;
    public string[] Expected { get; set; } = [];
}