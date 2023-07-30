using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace NotificationService.Configurations
{
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                // add JWT bearer authorization 
                OpenApiSecurityScheme securityDefinition = new OpenApiSecurityScheme()
                {
                    Name = "Bearer",
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                    Description = "Specify the authorization token.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                };
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityDefinition);

                // Make sure swagger UI requires a Bearer token specified
                OpenApiSecurityScheme JwtScheme = new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference()
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                OpenApiSecurityRequirement securityRequirements = 
                    new OpenApiSecurityRequirement() { { JwtScheme, new string[] { } } };
                options.AddSecurityRequirement(securityRequirements);
            });

            return services;
        }
    }
}
