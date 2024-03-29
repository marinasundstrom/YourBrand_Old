﻿using System;

using MassTransit;
using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.Identity;

namespace YourBrand.IdentityManagement.Consumers;

public static class ServiceCollectionBusConfiguratorExtensions
{
    public static void AddAppConsumers(this IBusRegistrationConfigurator conf)
    {
        conf.AddConsumers(typeof(GetUserConsumer).Assembly);
    }
}