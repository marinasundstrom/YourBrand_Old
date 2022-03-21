﻿using System;

using YourCompany.Application.Common.Interfaces;
using YourCompany.WebApi.Hubs;

using Microsoft.AspNetCore.SignalR;

using Worker.Client;

namespace YourCompany.WebApi.Services;

public class NotificationClient : INotificationClient
{
    private readonly IHubContext<NotificationHub, INotificationClient> _notificationsHubContext;

    public NotificationClient(IHubContext<NotificationHub, INotificationClient> notificationsHubContext)
    {
        _notificationsHubContext = notificationsHubContext;
    }

    public async Task NotificationReceived(NotificationDto notification)
    {
        if (notification.UserId is not null)
        {
            await _notificationsHubContext.Clients.User(notification.UserId).NotificationReceived(notification);
        }
        else
        {
            await _notificationsHubContext.Clients.All.NotificationReceived(notification);
        }
    }
}