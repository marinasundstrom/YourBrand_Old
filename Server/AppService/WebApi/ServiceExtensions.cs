﻿using System;

using YourCompany.Application;
using YourCompany.Application.Common.Interfaces;
using YourCompany.Infrastructure;

using YourCompany.WebApi.Hubs;
using YourCompany.WebApi.Services;

namespace YourCompany.WebApi;

public static class ServiceExtensions
{
    public  static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IUrlHelper, UrlHelper>();

        services.AddClients();

        services.AddScoped<IFileUploaderService, FileUploaderService>();

        return services;
    }

    private static IServiceCollection AddClients(this IServiceCollection services)
    {
        services.AddScoped<IItemsClient, ItemsClient>();
        services.AddScoped<IWorkerClient, WorkerClient>();
        services.AddScoped<INotificationClient, NotificationClient>();
        services.AddScoped<ISomethingClient, SomethingClient>();

        services.AddHttpClient(nameof(IdentityService.Client.IUsersClient) + "2", (sp, http) =>
        {
            http.BaseAddress = new Uri($"https://identity.local/");
            http.DefaultRequestHeaders.Add("X-API-KEY", "foobar");
        })
        .AddTypedClient<IdentityService.Client.IUsersClient>((http, sp) => new IdentityService.Client.UsersClient(http));

        return services;
    }
}