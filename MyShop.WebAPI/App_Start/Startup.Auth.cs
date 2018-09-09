using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using MyShop.Data;
using MyShop.Model.Models;
using Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using MyShop.WebAPI.Providers;
using System;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(MyShop.WebAPI.App_Start.Startup))]

namespace MyShop.WebAPI.App_Start
{
	public partial class Startup
	{
		public void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext(MyShopDbContext.Create);

            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
            app.CreatePerOwinContext<UserManager<AppUser>>(CreateManager);

            app.UseCors(CorsOptions.AllowAll);

            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/api/oauth/token"),
                Provider = new AuthorizationServerProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                AllowInsecureHttp = true
            });

            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);

                map.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions()
                {
                    Provider = new QueryStringOAuthBearerProvider()
                });

                var hubConfiguration = new HubConfiguration
                {
                    EnableJSONP = true,
                    EnableDetailedErrors = true
                };
                map.RunSignalR(hubConfiguration);
            });
        }

		private static UserManager<AppUser> CreateManager(IdentityFactoryOptions<UserManager<AppUser>> options, IOwinContext context)
        {
            var userStore = new UserStore<AppUser>(context.Get<MyShopDbContext>());
            var owinManager = new UserManager<AppUser>(userStore);
            return owinManager;
        }
	}
}