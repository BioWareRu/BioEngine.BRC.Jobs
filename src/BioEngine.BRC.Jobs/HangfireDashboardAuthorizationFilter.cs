using System.Security.Claims;
using Hangfire.Dashboard;

namespace BioEngine.BRC.Jobs
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            return httpContext.User.Identity.IsAuthenticated && httpContext.User.HasClaim(ClaimTypes.Role, "admin");
        }
    }
}