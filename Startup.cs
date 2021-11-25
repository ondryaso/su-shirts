using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SUShirts.Business.Facades;
using SUShirts.Business.Mapper;
using SUShirts.Business.Services;
using SUShirts.Configuration;
using SUShirts.Data;

namespace SUShirts
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite("Data Source=App.db;Cache=Shared"));

            services.Configure<MessageOptions>(_configuration.GetSection("Messages"));

            services.AddAutoMapper(typeof(MapperProfile));

            services.AddHttpClient();
            services.AddHttpContextAccessor();

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ShopFacade>();
            services.AddScoped<ReservationsFacade>();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                })
                .AddGoogle(options =>
                {
                    var configSection = _configuration.GetSection("Google");

                    options.ClientId = configSection["ClientId"];
                    options.ClientSecret = configSection["ClientSecret"];
                    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                });

            services.AddRazorPages(options => { options.Conventions.AuthorizeFolder("/Admin"); })
                .AddRazorRuntimeCompilation();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseHttpsRedirection();
            }
            else
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions()
                {
                    ForwardedHeaders = ForwardedHeaders.All
                });
                app.UsePathBase("/tricka");
            }

            dbContext.Database.Migrate();
            Bootstrapper.BootstrapDatabase(dbContext);

            app.UseStaticFiles();

            app.UseRequestLocalization();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapRazorPages(); });
        }
    }
}
