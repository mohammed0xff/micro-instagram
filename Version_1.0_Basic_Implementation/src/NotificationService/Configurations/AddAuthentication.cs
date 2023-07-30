using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserService.Common;

namespace NotificationService.Configurations
{
    public static class AddAuthentication
    {
        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var tokenConfigurationSection = configuration.GetSection("TokenConfiguration");

            var appSettings = tokenConfigurationSection.Get<TokenConfiguration>();
            var key = Encoding.ASCII.GetBytes(appSettings!.Secret);
            
            services.AddAuthentication(configureOpts =>
            {
                configureOpts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                configureOpts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(configureOpts =>
            {
                configureOpts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = appSettings.Issuer,
                    ValidAudience = appSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
             });

            return services;
        }
    }
}
