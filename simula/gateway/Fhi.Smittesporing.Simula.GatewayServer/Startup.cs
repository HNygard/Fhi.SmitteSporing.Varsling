using System.Collections.Generic;
using System.Reflection;
using Fhi.Smittesporing.Simula.EksternKlient;
using Fhi.Smittesporing.Simula.EksternKlient.Mock;
using Fhi.Smittesporing.Simula.GatewayServer.Handlers;
using Fhi.Smittesporing.Simula.InternApi.Autorisering;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Fhi.Smittesporing.Simula.GatewayServer
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
            services.AddHealthChecks();
            services.AddControllers();

            services.AddApiKeyAuth(Configuration["SimulaInternApi:ApiKey"]);


            if (Configuration["SimulaEksternApiKlient:Mock"] == "True")
            {
                services.AddTransient<ISimulaEksternApiKlient, MockSimulaEksternApiKlient>();
            }
            else if (Configuration.GetSection("SimulaEksternApiKlient:DelvisMock").Exists())
            {
                services.AddDelivMockSimulaApiKlient(Configuration.GetSection("SimulaEksternApiKlient"));
            }
            else
            {
                services.AddSimulaApiKlient(Configuration.GetSection("SimulaEksternApiKlient"));
            }

            services.AddMediatR(typeof(OpprettKontaktrapportHandler).GetTypeInfo().Assembly);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Fhi.Smittesporing.Simula.GatewayServer API",
                    Version = "v1",
                    Description = "Fhi.Smittesporing.Simula.GatewayServer API"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Oppgi API-nøkkel for å kommunisere med dette API-et som \"Bearer <api-key>\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "Authorization",
                            Type = SecuritySchemeType.ApiKey,
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new List<string>()
                    }
                });
                c.CustomSchemaIds(x => x.FullName.Remove(0, x.Namespace.Length + 1));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fhi.Smittesporing.Simula.GatewayServer");
                c.RoutePrefix = "swagger";
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
