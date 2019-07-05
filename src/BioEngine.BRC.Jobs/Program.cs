using System;
using System.Threading.Tasks;
using BioEngine.BRC.Common;
using BioEngine.Extra.IPB;

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
                .AddModule<IPBSiteModule, IPBModuleConfig>((configuration, env) =>
                {
                    if (!Uri.TryCreate(configuration["BE_IPB_URL"], UriKind.Absolute, out var ipbUrl))
                    {
                        throw new ArgumentException($"Can't parse IPB url; {configuration["BE_IPB_URL"]}");
                    }

                    int.TryParse(configuration["BE_IPB_API_ADMIN_GROUP_ID"], out var adminGroupId);
                    int.TryParse(configuration["BE_IPB_API_PUBLISHER_GROUP_ID"], out var publisherGroupId);
                    int.TryParse(configuration["BE_IPB_API_EDITOR_GROUP_ID"], out var editorGroupId);
                    
                    return new IPBModuleConfig(ipbUrl)
                    {
                        AdminGroupId = adminGroupId,
                        PublisherGroupId = publisherGroupId,
                        EditorGroupId = editorGroupId,
                        ApiClientId = configuration["BE_IPB_OAUTH_CLIENT_ID"],
                        ApiClientSecret = configuration["BE_IPB_OAUTH_CLIENT_SECRET"],
                        CallbackPath = "/login/ipb",
                        AuthorizationEndpoint = configuration["BE_IPB_AUTHORIZATION_ENDPOINT"],
                        TokenEndpoint = configuration["BE_IPB_TOKEN_ENDPOINT"],
                        ApiReadonlyKey = configuration["BE_IPB_API_READONLY_KEY"]
                    };
                }).AddModule<JobsModule>();

            await bioEngine.RunAsync<Startup>();
        }
    }
}
