using Lab1_6.Data;
using Lab1_6.Kafka;
using Lab1_6.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using System;
using System.Threading.Tasks;

namespace Lab1_6
{
    public class Startup
    {

        public static bool IsReady = false;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            ReadyChecker.Start();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var config = AppConfigs.Init(Configuration);

            services
                .AddSingleton(config)
                .AddTransient(typeof(KafkaProducer<>));

            services
                .AddControllers()
                .AddControllersAsServices();

            services
                .AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = Configuration.GetValue<string>("authUrl");
                    options.Audience = "api1.6.resource";
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false, 
                        NameClaimType = "name"
                    };
                });

            services
                .AddAuthorization(options =>
                {
                    options.AddPolicy("ApiScope", policy =>
                    {
                        policy.RequireAuthenticatedUser();
                        policy.RequireClaim("scope", "ProfileApi");
                    });
                });

            var connStr = UsersDbContextFactory.GetConnStr(Configuration);

            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<UsersDbContext>(options => options.UseNpgsql(connStr));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseHttpMetrics();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapMetrics();
            });
        }
    }

    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public Task Invoke(HttpContext context)
        {
            Console.WriteLine(context.Request.QueryString);
            return _next.Invoke(context);
        }
    }
}
