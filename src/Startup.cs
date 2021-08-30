using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CoreCodeCamp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CampContext>();
            services.AddScoped<ICampRepository, CampRepository>();
            services.AddControllers();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            //services.AddApiVersioning();
            services.AddApiVersioning(option =>
           {
               option.AssumeDefaultVersionWhenUnspecified = true; //assume default version if it's not specified, in this case 1.1 is default
               option.DefaultApiVersion = new ApiVersion(1, 1);   //to set defualt version to api-version=1.1
               option.ReportApiVersions = true;  //report the valid api versions in the response header
               option.ApiVersionReader = new QueryStringApiVersionReader("ver"); //we change the name of the query string value, from "api-version" to just "ver" --> http://....?ver=versionNumber
               //option.ApiVersionReader = new HeaderApiVersionReader("X-Version"); //we specify the name of the header parameter to add with the version we watn. Header -> X-Vesion=1.0
               //option.ApiVersionReader = UrlSegmentApiVersionReader(); //this is going to allow us versioning by URL  api/v1/camps or api/v2/camps
           });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(cfg =>
            {
                cfg.MapControllers();
            });
        }
    }
}
