using System;
using System.Collections.Generic;
using System.Linq;
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
                .AddModule<IPBSiteModule, IPBSiteModuleConfig>((configuration, env) =>
                {
                    if (!Uri.TryCreate(configuration["BE_IPB_URL"], UriKind.Absolute, out var ipbUrl))
                    {
                        throw new ArgumentException($"Can't parse IPB url; {configuration["BE_IPB_URL"]}");
                    }

                    int.TryParse(configuration["BE_IPB_API_ADMIN_GROUP_ID"], out var adminGroupId);
                    int.TryParse(configuration["BE_IPB_API_SITE_TEAM_GROUP_ID"], out var siteTeamGroupId);
                    var additionalGroupIds = new List<int> {siteTeamGroupId};
                    if (!string.IsNullOrEmpty(configuration["BE_IPB_API_ADDITIONAL_GROUP_IDS"]))
                    {
                        var ids = configuration["BE_IPB_API_ADDITIONAL_GROUP_IDS"].Split(',');
                        foreach (var id in ids)
                        {
                            if (int.TryParse(id, out var parsedId))
                            {
                                additionalGroupIds.Add(parsedId);
                            }
                        }
                    }
                    
                    return new IPBSiteModuleConfig(ipbUrl)
                    {
                        AdminGroupId = adminGroupId,
                        AdditionalGroupIds = additionalGroupIds.Distinct().ToArray(),
                        ApiClientId = configuration["BE_IPB_OAUTH_CLIENT_ID"],
                        ApiClientSecret = configuration["BE_IPB_OAUTH_CLIENT_SECRET"],
                        CallbackPath = "/login/ipb",
                        AuthorizationEndpoint = configuration["BE_IPB_AUTHORIZATION_ENDPOINT"],
                        TokenEndpoint = configuration["BE_IPB_TOKEN_ENDPOINT"],
                        ApiReadonlyKey = configuration["BE_IPB_API_READONLY_KEY"],
                        DataProtectionPath = configuration["BE_IPB_DATA_PROTECTION_PATH"]
                    };
                }).AddModule<JobsModule>();

            await bioEngine.RunAsync<Startup>();
        }
    }
}
