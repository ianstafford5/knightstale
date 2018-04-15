using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AKnightsTale.Startup))]
namespace AKnightsTale
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
