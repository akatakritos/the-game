using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LeaderboardSite.Startup))]
namespace LeaderboardSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
