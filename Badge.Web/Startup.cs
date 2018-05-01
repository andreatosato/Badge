using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Badge.EF;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Badge.EF.Entity;
using Badge.Web.Models.People;
using Badge.Web.Models.Machines;
using Badge.Web.Models.Swipes;
using Badge.Web.Models.Badges;
using Badge.Web.Services;

namespace Badge.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<BadgeContext>(options => options.UseSqlServer(Configuration.GetConnectionString("BadgeContext")));
            services.AddScoped<IUploadBlob, UploadBlob>(c => new UploadBlob(Configuration["AzureStorage:ConnectionString"]));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            this.ConfigureAutoMapper();
        }

        private void ConfigureAutoMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Person, PeopleViewModel>()
                .ForMember(dest => dest.AvatarImage, opt => opt.Ignore())
                .ReverseMap(); 

                cfg.CreateMap<PopulateBadge, BadgesViewModel>().ReverseMap();

                cfg.CreateMap<Machine, MachinesViewModel>()
                .ForMember(dest => dest.Nome, opt => opt.ResolveUsing(origin => origin.Name))
                .ReverseMap()
                .ForMember(dest => dest.Name, opt => opt.ResolveUsing(origin => origin.Nome));

                cfg.CreateMap<Swipe, SwipesViewModel>()
                   .ForMember(dest => dest.IdPerson, opt => opt.Ignore())
                   .ReverseMap();
            });

            Mapper.AssertConfigurationIsValid();
        }
    }
}
