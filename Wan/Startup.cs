using Microsoft.Owin;
using Owin;

[assembly :OwinStartup(typeof(Wan.Startup))]
namespace Wan
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}