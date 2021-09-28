using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using ProgrammingForTheCloudPT2021.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProgrammingForTheCloudPT2021.DataAccess.Interfaces;
using ProgrammingForTheCloudPT2021.DataAccess.Repositories;
using Google.Cloud.Diagnostics.AspNetCore;
using Microsoft.AspNetCore.Http;
using Google.Cloud.SecretManager.V1;
using Newtonsoft.Json;

namespace ProgrammingForTheCloudPT2021
{
    public class Startup
    {
        private IWebHostEnvironment _host;
        string projectId;
        public Startup(IConfiguration configuration, IWebHostEnvironment host)
        {
            Configuration = configuration;
            _host = host;

            projectId = configuration.GetSection("ProjectId").Value;


            string prefixAbsolutePath = _host.ContentRootPath;
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
                prefixAbsolutePath + "/pfcpt2021-1b9c895bcae7.json"
                );
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //removing identity and leaving only external authentication:
            //https://stackoverflow.com/questions/48120508/net-core-external-authentication-without-asp-net-identity

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });


            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("DefaultConnection")));

            //994079389678-hq78hmr1sqrmmmjm6qvcoikoqr6h0ku6.apps.googleusercontent.com
            //Ob0Gv8xXMMYTHH9YpMmLkIRW

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            //services.AddControllersWithViews();
            //services.AddRazorPages();

            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
           .AddDefaultUI()
           .AddEntityFrameworkStores<ApplicationDbContext>()
           .AddDefaultTokenProviders();


            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            SecretVersionName secretVersionName = new SecretVersionName(projectId, "pfcdemo1", "1");

            AccessSecretVersionResponse result = client.AccessSecretVersion(secretVersionName);

            string str = result.Payload.Data.ToStringUtf8();
            dynamic myPass = JsonConvert.DeserializeObject(str);
            string clientSecret = myPass.google_secret;


            services.AddAuthentication().AddGoogle(options =>
                   {
                       options.ClientId = "994079389678-hq78hmr1sqrmmmjm6qvcoikoqr6h0ku6.apps.googleusercontent.com";
                       options.ClientSecret = clientSecret;
                   });


            services.AddAuthentication().AddMicrosoftAccount(options =>
            {
                options.ClientId = "6d13ed3d-a32d-415a-8aa6-c863a3ae6e30";
                options.ClientSecret = "oHM~N3zF0BKd~zY_~1KWp-q1gsS7HSPxR~";
            });

            services.AddScoped<IFirestoreAccess, FireStoreAccess>();
            services.AddScoped<ICacheAccess, CacheAccess>();
            services.AddScoped<IPubSubAccess, PubSubAccess>();
            services.AddScoped<ILogAccess, LogsAccess>();

            //6d13ed3d-a32d-415a-8aa6-c863a3ae6e30
            //724e2ea2-e79c-4101-8f28-839f46920c8e

            services.AddGoogleExceptionLogging(options =>
            {
                options.ProjectId = projectId;
                options.ServiceName = "PFCService";
                options.Version = "0.01";
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //} 
            app.UseHttpsRedirection();
            app.UseGoogleExceptionLogging();
      
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }


        private void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                // TODO: Use your User Agent library of choice here.

                // For .NET Core < 3.1 set SameSite = (SameSiteMode)(-1)
                options.SameSite = SameSiteMode.Unspecified;

            }
        }



    }
}
