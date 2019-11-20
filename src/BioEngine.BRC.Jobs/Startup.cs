using System;
using System.Collections.Generic;
using BioEngine.BRC.Common;
using BioEngine.Core.Web;
using BioEngine.Extra.IPB.Controllers;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BioEngine.BRC.Jobs
{
    public class Startup : BioEngineWebStartup
    {
        public Startup(IConfiguration configuration, IHostEnvironment environment) : base(configuration,
            environment)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddAuthorization();

            services.AddHangfire(config =>
            {
                config.UsePostgreSqlStorage(Configuration.GetPostgresConnectionString(),
                    new PostgreSqlStorageOptions
                    {
                        InvisibilityTimeout = TimeSpan.FromHours(5), DistributedLockTimeout = TimeSpan.FromHours(5)
                    });
            });
        }

        protected override IMvcBuilder ConfigureMvc(IMvcBuilder mvcBuilder)
        {
            return base.ConfigureMvc(mvcBuilder)
                .AddApplicationPart(typeof(UserController).Assembly);
        }

        protected override void ConfigureAfterRouting(IApplicationBuilder app, IHostEnvironment env)
        {
            base.ConfigureAfterRouting(app, env);
            GlobalConfiguration.Configuration.UseActivator(new HangfireActivator(app.ApplicationServices));

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireServer();
            app.UseHangfireDashboard(options: new DashboardOptions
            {
                Authorization = new List<IDashboardAuthorizationFilter> {new HangfireDashboardAuthorizationFilter()}
            });
        }
    }
}
