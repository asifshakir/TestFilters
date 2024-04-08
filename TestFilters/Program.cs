using TestFilters.Models;
using TestFilters.Repositories;
using TestFilters.Providers;

IEnumerable<RequestContext> requests = [
    new() { Domain = "ALL", Client = "ALL", Environment = "ALL" },
    new() { Domain = "domain1", Client = "ALL", Environment = "ALL" },
    new() { Domain = "domain2", Client = "ALL", Environment = "ALL" },
    new() { Domain = "ALL", Client = "client1", Environment = "ALL" },
    new() { Domain = "ALL", Client = "client2", Environment = "ALL" },
    new() { Domain = "ALL", Client = "ALL", Environment = "env1" },
    new() { Domain = "ALL", Client = "ALL", Environment = "env2" },
    new() { Domain = "ALL", Client = "client1", Environment = "env1" },
    new() { Domain = "ALL", Client = "client2", Environment = "env2" },
    new() { Domain = "domain1", Client = "client1", Environment = "ALL" },
    new() { Domain = "domain2", Client = "client2", Environment = "ALL" },
    new() { Domain = "domain1", Client = "ALL", Environment = "env1" },
    new() { Domain = "domain2", Client = "ALL", Environment = "env2" },
    new() { Domain = "domain1", Client = "client1", Environment = "env1" },
    new() { Domain = "domain1", Client = "client1", Environment = "env2"}
    ];

// If a user has a claim of "*:*:*" then they have access to all data
string[] claims = ["*:*:*"];
TestFilters(claims, requests);

// If a user has a claim of "domain1:*:*" then they have access to all data for domain1
claims = ["domain1:*:*"];
TestFilters(claims, requests);

// If a user has a claim of "domain1:client1:*" then they have access to all data for domain1 and client1
claims = ["domain1:client1:*"];
TestFilters(claims, requests);

// If a user has a claim of "domain1:client1:env1" then they have access to all data for domain1, client1, and env1
claims = ["domain1:client1:env1"];
TestFilters(claims, requests);

static void TestFilters(string[] claims, IEnumerable<RequestContext> requests)
{
    foreach (var request in requests)
    {
        Console.WriteLine("================================================");
        foreach (var claim in claims)
        {
            Console.WriteLine($"Claim: {claim}");
        }
        Console.WriteLine("-------------------------------------------------");
        Console.WriteLine($"Request: {request.Domain}:{request.Client}:{request.Environment}");

        var requestContextProvider = new RequestContextProvider(claims);
        var isValidRequest = requestContextProvider.IsValidRequest(request);
        if (isValidRequest)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Request is valid");
            Console.ResetColor();
            var dataRepository = new DataRepository();
            var data = dataRepository.GetData();
            var filteredData = requestContextProvider.FilterData(data, request);
            foreach (var item in filteredData)
            {
                Console.WriteLine($"Data: {item.Domain}:{item.Client}:{item.Environment}");
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Request is invalid");
            Console.ResetColor();
        }
        Console.WriteLine("================================================");
    }
}

namespace TestFilters.Repositories
{
    public sealed class DataRepository
    {
        public IEnumerable<Data> GetData()
        {
            return [
                new() { Domain = "domain1", Client = "client1", Environment = "env1" },
                new() { Domain = "domain1", Client = "client1", Environment = "env2" },
                new() { Domain = "domain1", Client = "client2", Environment = "env1" },
                new() { Domain = "domain1", Client = "client2", Environment = "env2" },
                new() { Domain = "domain2", Client = "client1", Environment = "env1" },
                new() { Domain = "domain2", Client = "client1", Environment = "env2" },
                new() { Domain = "domain2", Client = "client2", Environment = "env1" },
                new() { Domain = "domain2", Client = "client2", Environment = "env2" }
            ];
        }
    }
}

namespace TestFilters.Models
{
    public interface IRequestContext
    {
        string Domain { get; }
        string Client { get; }
        string Environment { get; }
    }

    public sealed class RequestContext : IRequestContext
    {
        public string Domain { get; set; } = "ALL";
        public string Client { get; set; } = "ALL";
        public string Environment { get; set; } = "ALL";
    }

    public sealed class UserPermissions : IRequestContext
    {
        public string Domain { get; set; }
        public string Client { get; set; }
        public string Environment { get; set; }

        public UserPermissions(string domain, string client, string environment)
        {
            Domain = domain;
            Client = client;
            Environment = environment;
        }

        // permission is a string in the format of "domain:client:environment"
        public static UserPermissions Create(string permission)
        {
            var parts = permission.Split(':');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Permission must be in the format of 'domain:client:environment'");
            }
            return new UserPermissions(parts[0], parts[1], parts[2]);
        }
    }

    public sealed class Data : IRequestContext
    {
        public required string Domain { get; set; }
        public required string Client { get; set; }
        public required string Environment { get; set; }
    }
}

namespace TestFilters.Providers
{
    public sealed class RequestContextProvider
    {
        public RequestContextProvider(string[] userClaims)
        {
            List<IRequestContext> userPermissions = [];
            foreach (var claim in userClaims)
            {
                userPermissions.Add(Models.UserPermissions.Create(claim));
            }
            this.UserPermissions = userPermissions;
        }
        public IEnumerable<IRequestContext> UserPermissions { get; init; }

        public bool HasAllAccess => this.UserPermissions.Any(x => x.Domain == "*" && x.Client == "*" && x.Environment == "*");

        public bool IsValidRequest(IRequestContext requestContext)
        {
            // Check for universal request or universal access
            bool isUniversalRequest = requestContext.Domain == "ALL" && requestContext.Client == "ALL" && requestContext.Environment == "ALL";
            bool hasUniversalAccess = this.UserPermissions.Any(x => x.Domain == "*" && x.Client == "*" && x.Environment == "*");

            if (isUniversalRequest || hasUniversalAccess) return true;

            // Simplify the checks by using Any to look for a match in permissions that meets any of the request's conditions,
            // including wildcard permissions.
            return this.UserPermissions.Any(permission =>
                (permission.Domain == requestContext.Domain || permission.Domain == "*" || requestContext.Domain == "ALL") &&
                (permission.Client == requestContext.Client || permission.Client == "*" || requestContext.Client == "ALL") &&
                (permission.Environment == requestContext.Environment || permission.Environment == "*" || requestContext.Environment == "ALL"));
        }


        public bool IsValidData(IRequestContext dataContext, IRequestContext requestContext)
        {
            // Check if the user has requested to access the data and if the request itself is valid.
            // This ensures the method doesn't proceed if the request context is not permitted.
            if (!IsValidRequest(requestContext)) return false;

            // Ensure the data context matches the request context for domain, client, and environment.
            // This step specifically addresses the need to filter out unwanted access when the user
            // has broader permissions but the request is for a specific domain, client, or environment.
            bool matchesRequestContext = true;
            matchesRequestContext &= requestContext.Domain == "ALL" || dataContext.Domain == requestContext.Domain;
            matchesRequestContext &= requestContext.Client == "ALL" || dataContext.Client == requestContext.Client;
            matchesRequestContext &= requestContext.Environment == "ALL" || dataContext.Environment == requestContext.Environment;

            // If the data context doesn't match the request context, deny access.
            if (!matchesRequestContext) return false;

            // Check if the user has permission to access the specified data.
            // This permission check now only needs to confirm that the user has access to the requested
            // data context, given the request context has already been matched to the data context.
            bool hasPermission = this.UserPermissions.Any(permission =>
                (permission.Domain == dataContext.Domain || permission.Domain == "*") &&
                (permission.Client == dataContext.Client || permission.Client == "*") &&
                (permission.Environment == dataContext.Environment || permission.Environment == "*"));

            return hasPermission;
        }



        public IEnumerable<T> FilterData<T>(IEnumerable<T> data, IRequestContext requestContext) where T : IRequestContext
        {
            return data.Where(x => IsValidData(x, requestContext));
        }
    }
}