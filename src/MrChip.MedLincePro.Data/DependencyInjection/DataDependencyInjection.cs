using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MrChip.MedLincePro.Business.Interfaces.Adm;
using MrChip.MedLincePro.Data.Connection;
using MrChip.MedLincePro.Data.Repositories.Adm;

namespace MrChip.MedLincePro.Data.DependencyInjection;

public static class DataDependencyInjection
{
    public static IServiceCollection AddMedLinceProData(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseConnectionOptions>(configuration.GetSection(DatabaseConnectionOptions.SectionName));
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

        services.AddScoped<IEmpresaRepository, EmpresaRepository>();
        services.AddScoped<IBureauRepository, BureauRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IProfissionalRepository, ProfissionalRepository>();
        services.AddScoped<IUnidadeHospitalarRepository, UnidadeHospitalarRepository>();
        services.AddScoped<IOperadoraRepository, OperadoraRepository>();

        return services;
    }
}
