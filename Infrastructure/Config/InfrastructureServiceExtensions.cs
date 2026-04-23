using Aplication.Ports.In;
using Aplication.Ports.Out;
using Aplication.UseCases;
using Domain.Services;
using Infrastructure.Adapters.Persistence;
using Infrastructure.Mappers;
using Infrastructure.Mappers.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Config
{
    /// <summary>
    /// Métodos de extensión para inyectar todas las dependencias del microservicio en el contenedor de IoC.
    /// </summary>
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                sqlServerOptionsAction: sqlOptions =>
                {
                    // Política de reintentos habilitada para entornos contenerizados (Docker)
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                }));

            services.AddScoped<ISupplierMapper, SupplierMapper>();
            services.AddScoped<ISupplierRepositoryPort, SupplierAdapter>();
            services.AddScoped<ISupplierUseCasePort, SupplierUseCase>();
            services.AddScoped<SupplierService>();

            return services;
        }
    }
}
