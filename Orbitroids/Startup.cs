using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Orbitroids.Startup))]
namespace Orbitroids
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
