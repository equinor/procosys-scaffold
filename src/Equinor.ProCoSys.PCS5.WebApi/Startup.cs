﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Equinor.ProCoSys.PCS5.Command;
using Equinor.ProCoSys.PCS5.Query;
using Equinor.ProCoSys.PCS5.WebApi.DIModules;
using Equinor.ProCoSys.PCS5.WebApi.Middleware;
using Equinor.ProCoSys.PCS5.WebApi.Seeding;
using Equinor.ProCoSys.PCS5.WebApi.Synchronization;
using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using Equinor.ProCoSys.Auth;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Common.Swagger;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Sender.Interfaces;

namespace Equinor.ProCoSys.PCS5.WebApi;

public class Startup
{
    private readonly string AllowAllOriginsCorsPolicy = "AllowAllOrigins";
    private readonly IWebHostEnvironment _environment;

    public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        Configuration = configuration;
        _environment = webHostEnvironment;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        if (_environment.IsDevelopment() || _environment.IsTest())
        {
            var migrateDatabase = Configuration.GetValue<bool>("MigrateDatabase");
            if (migrateDatabase)
            {
                services.AddHostedService<DatabaseMigrator>();
            }
        }
        if (_environment.IsDevelopment())
        {
            DebugOptions.DebugEntityFrameworkInDevelopment = Configuration.GetValue<bool>("DebugEntityFrameworkInDevelopment");

            if (Configuration.GetValue<bool>("SeedDummyData"))
            {
                services.AddHostedService<Seeder>();
            }
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                Configuration.Bind("API", options);
            });

        services.AddCors(options =>
        {
            options.AddPolicy(AllowAllOriginsCorsPolicy,
                builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        services.AddMvc(config =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            config.Filters.Add(new AuthorizeFilter(policy));
        }).AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        if (Configuration.GetValue<bool>("UseAzureAppConfiguration"))
        {
            services.AddAzureAppConfiguration();
        }

        services.AddFluentValidationAutoValidation(fv =>
        {
            fv.DisableDataAnnotationsValidation = true;
        });
        services.AddValidatorsFromAssemblies(new List<Assembly>
        {
            typeof(IQueryMarker).GetTypeInfo().Assembly,
            typeof(ICommandMarker).GetTypeInfo().Assembly,
            typeof(Startup).Assembly
        });

        var scopes = Configuration.GetSection("Swagger:Scopes").Get<Dictionary<string, string>>() ?? new Dictionary<string, string>();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProCoSys PCS5 API", Version = "v1" });
            var authorizationUrl = Configuration.GetRequiredConfiguration("Swagger:AuthorizationUrl");

            //Define the OAuth2.0 scheme that's in use (i.e. Implicit Flow)
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(authorizationUrl),
                        Scopes = scopes
                    }
                }
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                    },
                    scopes.Keys.ToArray()
                }
            });

            c.OperationFilter<AddRoleDocumentation>();

        });

        services.ConfigureSwaggerGen(options =>
        {
            options.CustomSchemaIds(x => x.FullName);
        });

        services.AddFluentValidationRulesToSwagger();

        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
        });

        services.AddPcsAuthIntegration();

        services.AddApplicationInsightsTelemetry(options =>
        {
            options.ConnectionString = Configuration.GetRequiredConfiguration("ApplicationInsights:ConnectionString");
        });
        services.AddMediatrModules();
        services.AddApplicationModules(Configuration);

        var serviceBusEnabled = Configuration.GetValue<bool>("ServiceBus:Enable") &&
                                (!_environment.IsDevelopment() || Configuration.GetValue<bool>("ServiceBus:EnableInDevelopment"));
        if (serviceBusEnabled)
        {
            // Env variable used in kubernetes. Configuration is added for easier use locally
            var leaderElectorUrlPart =
                Environment.GetEnvironmentVariable("LEADERELECTOR_SERVICE")
                ?? Configuration.GetRequiredConfiguration("ServiceBus:LeaderElectorUrl");

            var leaderElectorRenewLeaseInterval = Configuration.GetRequiredIntConfiguration("ServiceBus:LeaderElectorRenewLeaseInterval");

            services.AddPcsServiceBusIntegration(options => options
                .UseBusConnection(Configuration.GetRequiredConnectionString("ServiceBus"))
                .WithLeaderElector("http://" + leaderElectorUrlPart + ":3003")
                .WithRenewLeaseInterval(leaderElectorRenewLeaseInterval)
                .WithSubscription("Project", "pcs5_project")
                //THIS METHOD SHOULD BE FALSE IN NORMAL OPERATION.
                //ONLY SET TO TRUE WHEN A LARGE NUMBER OF MESSAGES HAVE FAILED AND ARE COPIED TO DEAD LETTER.
                //WHEN SET TO TRUE, MESSAGES ARE READ FROM DEAD LETTER QUEUE INSTEAD OF NORMAL QUEUE
                .WithReadFromDeadLetterQueue(Configuration.GetValue("ServiceBus:ReadFromDeadLetterQueue", defaultValue: false)));

            var topics = Configuration["ServiceBus:TopicNames"];
            if (topics != null)
            {
                services.AddTopicClients(Configuration.GetRequiredConnectionString("ServiceBus"), topics);
            }
        }
        else
        {
            services.AddSingleton<IPcsBusSender>(new DisabledServiceBusSender());
        }
        services.AddHostedService<VerifyApplicationExistsAsPerson>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (Configuration.GetValue<bool>("UseAzureAppConfiguration"))
        {
            app.UseAzureAppConfiguration();
        }

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseGlobalExceptionHandling();

        app.UseCors(AllowAllOriginsCorsPolicy);

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProCoSys PCS5 API V1");
            c.DocExpansion(DocExpansion.List);
            c.DisplayRequestDuration();

            c.OAuthClientId(Configuration["Swagger:ClientId"]);
            c.OAuthAppName("ProCoSys PCS5 API V1");
            c.OAuthScopeSeparator(" ");
            var audience = Configuration.GetRequiredConfiguration("API:Audience");
            c.OAuthAdditionalQueryStringParams(new Dictionary<string, string> { { "resource", audience } });
        });

        app.UseHttpsRedirection();

        app.UseRouting();

        // order of adding middelwares are crucial. Some depend that other has been run in advance
        app.UseCurrentPlant();
        app.UseCurrentBearerToken();
        app.UseAuthentication();
        app.UseCurrentUser();
        app.UsePersonValidator();
        app.UsePlantValidator();
        app.UseVerifyOidInDb();
        app.UseAuthorization();

        app.UseResponseCompression();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
