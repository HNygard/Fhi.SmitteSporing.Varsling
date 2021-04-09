using Fhi.Smittesporing.Helsenorge.Api.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;

namespace Fhi.Smittesporing.Helsenorge.Api
{
    public class HelsenorgeSecurityTokenFilter : IOperationFilter
    {
        private static readonly Type _appliesTo = typeof(InnsynController);

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.MethodInfo.DeclaringType == _appliesTo)
            {
                var scheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme }
                };

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [ scheme ] = new List<string>()
                    }
                };
            }
        }
    }
}
