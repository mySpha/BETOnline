using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace bet_online.API.services
{
    public class AuthorizeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authAttributes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Distinct();

            if (authAttributes.Any())
            {
                operation.Security = new List<OpenApiSecurityRequirement>();
                var secReq = new List<OpenApiSecurityRequirement>();
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    {
                    new OpenApiSecurityScheme
                    {
                        Description = "Add token to header",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        In = ParameterLocation.Header,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }

                    },new List<string>()
                    }
                });

            }
        }
    }
}
