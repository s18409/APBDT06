using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using tutorial6.Middlewares;
using tutorial6.Services;

namespace tutorial6
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        
        public void ConfigureServices(IServiceCollection services)  
        {
            services.AddTransient<IStudentsDbService, SqlServerStudentDbService>();
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Student API ", Version = "v1" });

            });      
        }

        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env ,IStudentsDbService service)  
        {                                                                                                          
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSwagger();
            app.UseSwaggerUI(c=> 
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student API v1");
            
            }); 

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<LoggingMiddleware>(); 

            
            app.Use(async (context ,next) => { 

                if (!context.Request.Headers.ContainsKey("Index"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await  context.Response.WriteAsync("index is required"); 

                    return; 
                }

                string index = context.Request.Headers["Index"].ToString();
                var student = service.GetStudentbyIndex(index);
                if (student == null)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Incorrect Index!!"); 
                    return;
                }
                await next(); 

            });



            
            app.UseRouting();  
             
           

            app.UseEndpoints(endpoints => 
            {
                endpoints.MapControllers();
            });
        }
    }
}
