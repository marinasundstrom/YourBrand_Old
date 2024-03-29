﻿using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Infrastructure.Persistence;
using YourBrand.TimeReport.Infrastructure.Persistence.Interceptors;
using YourBrand.TimeReport.Infrastructure.Services;
using Quartz;
using YourBrand.TimeReport.Infrastructure.BackgroundJobs;

namespace YourBrand.TimeReport.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        services.AddQuartz(configure =>
            {
                var jobKey = new JobKey(nameof(ProcessOutboxMessagesJob));

                configure
                    .AddJob<ProcessOutboxMessagesJob>(jobKey)
                    .AddTrigger(trigger => trigger.ForJob(jobKey)
                        .WithSimpleSchedule(schedule => schedule
                            .WithIntervalInSeconds(10)
                            .RepeatForever()));

                configure.UseMicrosoftDependencyInjectionJobFactory();
            });

            services.AddQuartzHostedService();

        return services;
    }
}