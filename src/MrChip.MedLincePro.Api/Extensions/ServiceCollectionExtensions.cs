using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using MrChip.MedLincePro.Api.Configuration;
using MrChip.MedLincePro.Api.Security;
using MrChip.MedLincePro.Business.Configuration;
using MrChip.MedLincePro.Business.Interfaces;
using MrChip.MedLincePro.Business.Interfaces.Adm;
using MrChip.MedLincePro.Business.Interfaces.Auth;
using MrChip.MedLincePro.Business.Services.Adm;
using MrChip.MedLincePro.Business.Services.Auth;
using MrChip.MedLincePro.Business.Services.Security;

namespace MrChip.MedLincePro.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMedLinceProApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<PasswordCompatibilityOptions>(configuration.GetSection(PasswordCompatibilityOptions.SectionName));

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, HttpUserContext>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IUsuarioPasswordService, LegacyPasswordService>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmpresaService, EmpresaService>();
        services.AddScoped<IBureauService, BureauService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IProfissionalService, ProfissionalService>();
        services.AddScoped<IUnidadeHospitalarService, UnidadeHospitalarService>();
        services.AddScoped<IOperadoraService, OperadoraService>();

        var jwt = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();
        var key = Encoding.UTF8.GetBytes(jwt.Key);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = true;
            options.SaveToken = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = !string.IsNullOrWhiteSpace(jwt.Issuer),
                ValidIssuer = jwt.Issuer,
                ValidateAudience = !string.IsNullOrWhiteSpace(jwt.Audience),
                ValidAudience = jwt.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2)
            };
        });

        services.AddSingleton<IAuthorizationPolicyProvider, ClaimAuthorizationPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, ClaimAuthorizationHandler>();
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }
}
