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
using UtahCrashesCracked.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UtahCrashesCracked.Models;
using Microsoft.ML.OnnxRuntime;


namespace UtahCrashesCracked
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
            services.AddDbContext<CrashDbContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<AuthDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddSingleton<InferenceSession>(
                new InferenceSession("wwwroot/crashdata4.onnx")
                );

            //code for user database connection
            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseMySql(Configuration["ConnectionStrings:AuthDbContextConnection"]);
            });

            services.Configure<IdentityOptions>(options =>
            {
                //Default password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
            });
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
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "Filter",
                    pattern: "/Crashes/{county}/Page{pageNum}",
                    defaults: new { Controller = "Home", action = "Crashes" }
                    );

                endpoints.MapControllerRoute(
                    name: "Paging",
                    pattern: "/Crashes/Page{pageNum}",
                    defaults: new { Controller = "Home", action = "Crashes", pageNum = 1 });

                endpoints.MapControllerRoute("county",
                    "/Crashes/{county}",
                    new { Controller = "Home", action = "Crashes", pageNum = 1 });


                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
