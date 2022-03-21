﻿using System;
using System.Net.Http.Headers;

using YourCompany.Showroom.Application.Common.Interfaces;
using YourCompany.Showroom.Application.ConsultantProfiles.Queries;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace YourCompany.Showroom.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(typeof(GetConsultantProfilesQuery));
        
        services.AddScoped<Handler>();

        services.AddHttpClient(nameof(Worker.Client.INotificationsClient), (sp, http) =>
        {
            var conf = sp.GetRequiredService<IConfiguration>();

            http.BaseAddress = conf.GetServiceUri("worker");
        })
        .AddTypedClient<Worker.Client.INotificationsClient>((http, sp) => new Worker.Client.NotificationsClient(http))
        .AddHttpMessageHandler<Handler>();

        return services;
    }
}

public class Handler : DelegatingHandler
{
    private readonly ICurrentUserService _currentUserService;

    public Handler(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("bearer", _currentUserService.GetAccessToken());

        return base.SendAsync(request, cancellationToken);
    }
}