using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Calories_Life_2.Startup))]
namespace Calories_Life_2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
