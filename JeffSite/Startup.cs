using System;
using JeffSite.Data;
using JeffSite.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JeffSite
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
            services.AddControllersWithViews();
            services.AddSession();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDbContext<JeffContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("JeffSite"),
                    //new MySqlServerVersion(new Version(5, 5, 51)),
                    builder => builder.MigrationsAssembly("JeffSite")
                ));

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(60*5);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddScoped<UserService>();
            services.AddScoped<SocialMidiaService>();
            services.AddScoped<ConfiguracaoService>();
            services.AddScoped<SeedingService>();
            services.AddScoped<CarouselService>();
            services.AddScoped<LeitorService>();
            services.AddScoped<LivroService>();
            services.AddScoped<LojaService>();
            services.AddScoped<MallingService>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SeedingService seedingService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                seedingService.Seed();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                seedingService.Seed(); //aplica seeding para producao
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
