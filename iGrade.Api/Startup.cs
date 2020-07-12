using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace iGrade.Api
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            // Add Cors

            //services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            //{
            //    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            //}));
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
           // app.UseHttpsRedirection();
           app.UseFileServer();

            // Enable Cors
            //app.UseCors("AllowAll");

            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\schoolfiles")),
                RequestPath = "/File"
            });

        //    app.UseStaticFiles(new StaticFileOptions()
        //{
        //    FileProvider = new PhysicalFileProvider(
        //    Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
        //    RequestPath = new Microsoft.AspNetCore.Http.PathString("/File")
        //});  
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseMvc();


}
}
}
