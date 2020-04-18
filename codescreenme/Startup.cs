using codescreenme.HostedServices;
using codescreenme.Hubs;
using codescreenme.Processing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace codescreenme
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddSignalR();
      services.AddControllersWithViews();

      services.AddHttpContextAccessor();
      services.AddSingleton<IUserRepository, UserRepository>();
      services.AddSingleton<ICodeSessionsRepository, CodeSessionsRepository>();

      // In production, the React files will be served from this directory
      services.AddSpaStaticFiles(configuration =>
      {
        configuration.RootPath = "ClientApp/build";
      });

      services.AddDistributedMemoryCache();

      services.AddSession(options =>
      {
        options.IdleTimeout = TimeSpan.FromMinutes(10);
        options.Cookie.Name = "codescreen_session";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
      });

      services.AddHostedService<RegularCleanUpHostedService>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseSpaStaticFiles();

      app.UseHttpContextItemsMiddleware();

      app.UseRouting();

      app.UseSession();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller}/{action=Index}/{id?}");

        endpoints.MapHub<CodeHub>("/codehub");
      });

      app.UseSpa(spa =>
      {
        spa.Options.SourcePath = "ClientApp";

        if (env.IsDevelopment())
        {
          spa.UseReactDevelopmentServer(npmScript: "start");
        }
      });
    }
  }
}
