using Microsoft.Extensions.Configuration;

namespace Duende.IdentityServer.Contrib.RedisStore.Tests
{
    public static class ConfigurationUtils
    {
	    public static IConfiguration GetConfiguration()
	    {
		    var config = new ConfigurationBuilder()
			    .AddJsonFile("appsettings.json");

		    return config.Build();
	    }
    }
}
