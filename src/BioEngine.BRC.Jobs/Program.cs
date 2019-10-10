using System;
using System.Threading.Tasks;
using BioEngine.BRC.Common;
using BioEngine.Extra.IPB;
using BioEngine.Extra.IPB.Auth;

namespace BioEngine.BRC.Jobs
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var bioEngine = new Core.BioEngine(args)
                .AddPostgresDb()
                .AddBrcDomain()
                .AddLogging()
                .AddS3Storage()
                .AddModule<IPBSiteModule, IPBSiteModuleConfig>((configuration, env) =>
                {
                    if (!Uri.TryCreate(configuration["BE_IPB_URL"], UriKind.Absolute, out var ipbUrl))
                    {
                        throw new ArgumentException($"Can't parse IPB url; {configuration["BE_IPB_URL"]}");
                    }

                    return new IPBSiteModuleConfig(ipbUrl) {ApiReadonlyKey = configuration["BE_IPB_API_READONLY_KEY"]};
                })
                .AddIpbUsers<IPBSiteUsersModule, IPBSiteUsersModuleConfig, IPBSiteCurrentUserProvider>()
                .AddModule<JobsModule>();

            await bioEngine.RunAsync<Startup>();
        }
    }
}
