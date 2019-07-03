using BioEngine.Core.DB;
using BioEngine.Core.Modules;
using BioEngine.Core.Posts.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BioEngine.BRC.Jobs
{
    public class JobsModule : BaseBioEngineModule
    {
        public override void ConfigureEntities(IServiceCollection serviceCollection, BioEntitiesManager entitiesManager)
        {
            base.ConfigureEntities(serviceCollection, entitiesManager);
            entitiesManager.RegisterEntity<Post>();
        }

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            base.ConfigureServices(services, configuration, environment);
            services.AddSingleton<IHostedService, JobsService>();
            services.AddScoped<IPBJobsRunner>();
        }
    }
}
