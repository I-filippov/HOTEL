using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HOTEL.Startup))]
namespace HOTEL
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
