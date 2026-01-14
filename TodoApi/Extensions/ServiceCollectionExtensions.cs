using FluentValidation;
using TodoApi.Interfaces;
using TodoApi.Repository;
using TodoApi.Services;
using TodoApi.Validators;

namespace TodoApi.Extensions
{
    /// <summary>
    /// Extension methods for configuring application services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds application services to the dependency injection container
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register repositories
            services.AddScoped<ITodoRepository, TodoRepository>();
            
            // Register services
            services.AddScoped<ITodoService, TodoService>();
            
            // Register validators
            services.AddValidatorsFromAssemblyContaining<CreateTodoDtoValidator>();
            
            return services;
        }
    }
}