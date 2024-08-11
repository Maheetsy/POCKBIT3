using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(POCKBIT_v2.Startup))]
namespace POCKBIT_v2
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
