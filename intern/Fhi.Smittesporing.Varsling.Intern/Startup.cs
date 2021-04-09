using System;
using System.Reflection;
using AutoMapper;
using Fhi.Smittesporing.Varsling.Datalag;
using Fhi.Smittesporing.Varsling.Domene;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Indekspasienter;
using Fhi.Smittesporing.Varsling.Domene.InnsynsLogg;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Domene.Utils;
using Fhi.Smittesporing.Varsling.Eksternetjenester;
using Fhi.Smittesporing.Varsling.Intern.Autorisering;
using Fhi.Smittesporing.Varsling.Intern.MockControllers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Optional;

namespace Fhi.Smittesporing.Varsling.Intern
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            InkluderMockApis = Configuration["Funksjonsbrytere:InkluderMockApis"] == "True";
        }

        public IConfiguration Configuration { get; }
        public bool InkluderMockApis { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddControllers().ConfigureApplicationPartManager(o =>
            {
                o.ApplicationParts.Clear();
                o.ApplicationParts.Add(new AssemblyPart(typeof(Startup).Assembly));
                if (InkluderMockApis)
                {
                    o.ApplicationParts.Add(new AssemblyPart(typeof(KontaktInfoMockController).Assembly));
                }
            });
            services.AddAuthorization(options =>
            {
                var authKonfig = new Autoriseringskonfig();
                Configuration.GetSection("Authorization").Bind(authKonfig);
                options.AddPolicy(
                    Tilgangsregler.Basic, 
                    policy => policy.RequireRole(authKonfig.Basic));
                options.AddPolicy(
                    Tilgangsregler.Smittesporer, 
                    policy => policy.RequireRole(authKonfig.Smittesporer));
                options.AddPolicy(
                    Tilgangsregler.Innsyn,
                    policy => policy.RequireRole(authKonfig.Innsyn));
            });

            var connection = Configuration.GetConnectionString("SmitteVarslingContext");
            services.LeggTilDatalag(connection);

            services.Configure<CryptoManagerFacade.Konfig>(Configuration.GetSection("Kryptering"));
            services.AddScoped<ICryptoManagerFacade, CryptoManagerFacade>();
            services.AddScoped<ITelefonNormalFacade, TelefonNormalFacade>();

            services.LeggTilEksternetjeneste(Configuration.GetSection("Eksternetjenester"));
            services.LeggTilVarslingsregler(Configuration.GetSection("Varslingsregler"));

            services.Configure<FunksjonsbrytereKonfig>(Configuration.GetSection("Funksjonsbrytere"));

            services.AddScoped<IArbeidskontekst, Arbeidskontekst>();
            services.AddScoped<IEksporterInnsynService, EksporterInnsynService>();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddAutoMapper(typeof(DomeneMapperprofil).GetTypeInfo().Assembly);


            // Registrer Swagger generator, definere 1 eller flere Swagger dokumenter
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Fhi.Smittesporing.Varsling.Intern API",
                    Version = "v1",
                    Description = "Fhi.Smittesporing.Varsling.Intern API"
                });
                if (InkluderMockApis)
                {
                    c.SwaggerDoc("mocks", new OpenApiInfo
                    {
                        Title = "Fhi.Smittesporing.Varsling.Intern MOCK",
                        Version = "v1",
                        Description = "Mocks av eksterne API-er for uavhengig testing / utvikling"
                    });
                }
            });

            services.AddMediatR(typeof(HentListe).GetTypeInfo().Assembly);

            // Registrer hostedservice utenfor Startup for å sørge for at DB-migrations er kjørt før de starter
            // -> ConfigureServices i Program.cs
            //services.LeggTilPeriodiskJobb<HenteNyeIndekspasienterJobb, HenteNyeIndekspasienterJobb.Konfig>(cfgs.GetSection("Bakgrunnsjobber:HenteNyeSmittetilfeller"))
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SmitteVarslingContext smitteVarslingContext)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fhi.Smittesporing.Varsling");
                if (InkluderMockApis)
                {
                    c.SwaggerEndpoint("/swagger/mocks/swagger.json", "Mock APIS eksterne tjenester");
                }
                c.RoutePrefix = "swagger";
            });

            if (env.IsDevelopment() || env.IsEnvironment("Test"))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.Use(async (context, next) =>
            {
                var arbeidskontekst = context.RequestServices.GetService<IArbeidskontekst>();
                var aktivBruker = (context.User?.Identity?.Name).SomeNotNull();
                aktivBruker.Match(
                    none: () => arbeidskontekst.SettAnonymkontekst(),
                    some: x => arbeidskontekst.SettBrukerkontekst(x));
                await next.Invoke();
            });
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }

                spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
                {
                    OnPrepareResponse = context =>
                    {
                        // never cache index.html
                        if (context.File.Name == "index.html")
                        {
                            context.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                            context.Context.Response.Headers.Add("Expires", "-1");
                        }
                    }
                };
            });

            using (var scope = app.ApplicationServices.CreateScope())
            {
                try
                {
                    //må kjøres før HostedService starter
                    var dbContext = scope.ServiceProvider.GetRequiredService<SmitteVarslingContext>();
                    dbContext.Database.Migrate();
                    //dbContext.Database.EnsureDeleted();
                    //dbContext.Database.EnsureCreated();
                }
                catch (Exception)
                {
                    // TODO: logges / føre til at service ikke starter?
                }
            }
        }
    }
}
