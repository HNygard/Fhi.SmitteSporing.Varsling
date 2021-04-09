using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

namespace Fhi.Smittesporing.Helsenorge.Api
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<HelsenorgeKonfigurasjon>(Configuration.GetSection(nameof(HelsenorgeKonfigurasjon)));

            services.AddControllers().AddXmlSerializerFormatters();

            //services
            //    .AddAuthentication(o =>
            //    {
            //        o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            //    })
            //    .AddJwtBearer(o =>
            //    {
            //        o.TokenValidationParameters.RequireSignedTokens = true;
            //        o.TokenValidationParameters.RequireExpirationTime = true;
            //        o.TokenValidationParameters.ValidateActor = true;
            //        o.TokenValidationParameters.ValidateAudience = false;
            //        o.TokenValidationParameters.ValidateIssuer = false;
            //        o.TokenValidationParameters.ValidateIssuerSigningKey = true;
            //        o.TokenValidationParameters.ValidateLifetime = true;
            //        o.TokenValidationParameters.IssuerSigningKey = HelsenorgeToken.GetTokenSigningKey(Configuration["HelsenorgeKonfigurasjon:TokenSigningSertifikatThumbprint"]);
            //        o.IncludeErrorDetails = true;
            //        //o.Events.OnTokenValidated =
            //    });

            services.AddScoped<IInternFacade, InternFacade>();

            services.AddHttpClient<Varslingklient>(c =>
            {
                c.BaseAddress = new Uri(Configuration.GetValue<string>("InternApiUrl"));
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                Credentials = CredentialCache.DefaultNetworkCredentials,
                PreAuthenticate = true
            });

            var assembly = Assembly.GetExecutingAssembly();

            services.AddAutoMapper(assembly);
            services.AddMediatR(assembly);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Fhi.Smittesporing.Helsenorge.Api",
                    Version = "v1",
                    Description = "Fhi.Smittesporing.Helsenorge.Api"
                });

                var bearerScheme = new OpenApiSecurityScheme
                {
                    Description = "Oppgi API-nøkkel for å kommunisere med dette API-et som \"Bearer {token}\"",
                    Name = HeaderNames.Authorization,
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT"
                };

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, bearerScheme);
                c.CustomSchemaIds(x => x.FullName.Remove(0, x.Namespace.Length + 1));
                c.OperationFilter<HelsenorgeSecurityTokenFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint("/swagger/v1/swagger.json", "Fhi.Smittesporing.Helsenorge.Api");
                setupAction.RoutePrefix = "swagger";
            });

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
