using Semifinals.EventService.Repositories;
using Semifinals.EventService.Utils;

[assembly: FunctionsStartup(typeof(Semifinals.EventService.Startup))]

namespace Semifinals.EventService;

class Startup : FunctionsStartup
{
    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        // TODO: Override ConfigureAppConfiguration with Azure Key Vault configuration
        
        base.ConfigureAppConfiguration(builder);
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        // Setup access to configuration
        IConfiguration config = builder.GetContext().Configuration;
        builder.Services.AddSingleton(config);
        
        // Setup logging
        builder.Services.AddLogging();

        // Setup graph database
        builder.Services.AddSingleton<IGraphClient, GraphClient>(provider =>
        {
            GremlinServer server = new(
                hostname: config["GraphDbHostName"],
                port: int.Parse(config["GraphDbPost"]),
                enableSsl: bool.Parse(config["GraphDbEnableSsl"]),
                username: config["GraphDbUsername"],
                password: config["GraphDbPassword"]);

            return new(server);
        });

        builder.Services.AddSingleton(provider =>
        {
            IGraphClient graphClient = provider.GetService<IGraphClient>();
            DriverRemoteConnection driverRemoteConnection = new(graphClient.GremlinClient, "g");
            return AnonymousTraversalSource.Traversal().WithRemote(driverRemoteConnection);
        });

        // Setup document database
        builder.Services.AddSingleton<CosmosClient>(provider =>
        {
            return new(
                accountEndpoint: config["DocumentDbEndpoint"],
                authKeyOrResourceToken: config["DocumentDbAuthKey"]);
        });

        // Setup repositories
        builder.Services.AddTransient<IGameRepository, GameRepository>();
        builder.Services.AddTransient<ISetRepository, SetRepository>();
    }
}
