﻿using EntityFramework.Exceptions.SqlServer;
using VSADemo.Common.Interceptors;

namespace VSADemo.Common.Persistence;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddScoped<EntitySaveChangesInterceptor>();
        services.AddScoped<DispatchDomainEventsInterceptor>();
        services.AddSingleton(TimeProvider.System);

        builder.AddSqlServerDbContext<ApplicationDbContext>("app-db",
            null,
            options =>
            {
                var serviceProvider = builder.Services.BuildServiceProvider();

                options.LogTo(Console.WriteLine, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();

                options.AddInterceptors(
                    serviceProvider.GetRequiredService<EntitySaveChangesInterceptor>(),
                    serviceProvider.GetRequiredService<DispatchDomainEventsInterceptor>());

                // Return strongly typed useful exceptions
                options.UseExceptionProcessor();
            });
    }
}