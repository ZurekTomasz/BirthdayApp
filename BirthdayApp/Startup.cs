using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BirthdayApp.Startup))]
namespace BirthdayApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
