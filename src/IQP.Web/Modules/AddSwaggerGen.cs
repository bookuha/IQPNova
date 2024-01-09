using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace IQP.Web.Modules;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwaggerGen(this IServiceCollection services, IConfiguration config)
    {
        services.AddSwaggerGen(o =>
        {
            o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description =
                    "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\""
            });
            o.MapType<ProblemDetails>(() => new OpenApiSchema
            {
                Type = "object",
                Properties =
                {
                    {"type", new OpenApiSchema {Type = "string"}},
                    {"title", new OpenApiSchema {Type = "string"}},
                    {"status", new OpenApiSchema {Type = "integer", Format = "int32"}},
                    {"detail", new OpenApiSchema {Type = "string"}},
                    {"traceId", new OpenApiSchema {Type = "string"}},
                },
            });
            o.MapType<ValidationProblemDetails>(() => new OpenApiSchema
            {
                Type = "object",
                Properties =
                {
                    {"type", new OpenApiSchema {Type = "string"}},
                    {"title", new OpenApiSchema {Type = "string"}},
                    {"status", new OpenApiSchema {Type = "integer", Format = "int32"}},
                    {"detail", new OpenApiSchema {Type = "string"}},
                    {"traceId", new OpenApiSchema {Type = "string"}},
                    {
                        "errors", new OpenApiSchema
                        {
                            Type = "object",
                            AdditionalPropertiesAllowed = true,
                            AdditionalProperties = new OpenApiSchema
                            {
                                Type = "array",
                                Items = new OpenApiSchema {Type = "string"}
                            }
                        }
                    }
                },
            });
            o.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });
        return services;
    }
}
