using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Movies.API.Handler;
using Movies.API.Handler.Middleware;
using Movies.Data.Repositories.ActorRepository;
using Movies.Data.Repositories.MovieRepository;
using Movies.Driver.SqlServer;
using System.Reflection;

namespace Movies.API
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
            // Add Application Insights telemetry
            services.AddApplicationInsightsTelemetry();

            services.AddControllers();

            services.AddMemoryCache();

            services.AddDbContextPool<MovieDbContext>(o => o.UseSqlServer(Configuration.GetConnectionString("MoviesDb"), b => b.MigrationsAssembly("Movies.API")));

            services.AddLogging();

            services.AddSingleton(typeof(ResilientOperationHandler));

            services.AddSingleton<IActorRepository, ActorRepository>();

            services.AddSingleton<IMovieRepository, MovieRepository>();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Movies API",
                    Version = "v1",
                    Description = "Movies API with SQL Server database",
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // Add the custom exception handling middleware
            app.UseExceptionHandlerMiddleware();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Movies APIs"));
        }
    }
}
