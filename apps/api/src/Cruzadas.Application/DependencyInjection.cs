using Cruzadas.Application.Interfaces;
using Cruzadas.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cruzadas.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IQuizService, QuizService>();
        return services;
    }
}
