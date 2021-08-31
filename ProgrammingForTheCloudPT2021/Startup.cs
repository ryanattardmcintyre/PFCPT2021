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

namespace ProgrammingForTheCloudPT2021
{
    public class Startup
    {
        private IWebHostEnvironment _host;
        public Startup(IConfiguration configuration, IWebHostEnvironment host)
        {
            Configuration = configuration;
            _host = host;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //removing identity and leaving only external authentication:
            //https://stackoverflow.com/questions/48120508/net-core-external-authentication-without-asp-net-identity


            /*   string prefixAbsolutePath = _host.ContentRootPath;
               System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
                   prefixAbsolutePath + "/pfcpt2021-1b9c895bcae7.json"
                   );
            */


            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("DefaultConnection")));

            //994079389678-hq78hmr1sqrmmmjm6qvcoikoqr6h0ku6.apps.googleusercontent.com
            //Ob0Gv8xXMMYTHH9YpMmLkIRW

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddAuthentication().AddGoogle(options =>
                   {
                       options.ClientId = "994079389678-hq78hmr1sqrmmmjm6qvcoikoqr6h0ku6.apps.googleusercontent.com";
                       options.ClientSecret = "Ob0Gv8xXMMYTHH9YpMmLkIRW";
                   });


            services.AddAuthentication().AddMicrosoftAccount(options =>
            {
                options.ClientId = "6d13ed3d-a32d-415a-8aa6-c863a3ae6e30";
                options.ClientSecret = "oHM~N3zF0BKd~zY_~1KWp-q1gsS7HSPxR~";
            });

            services.AddScoped<IFirestoreAccess, FireStoreAccess>();

            //6d13ed3d-a32d-415a-8aa6-c863a3ae6e30
            //724e2ea2-e79c-4101-8f28-839f46920c8e


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

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
    }
}
