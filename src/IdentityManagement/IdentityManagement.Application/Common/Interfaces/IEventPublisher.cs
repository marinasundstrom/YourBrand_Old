﻿namespace YourBrand.IdentityManagement.Application.Common.Interfaces;

public interface IEventPublisher
{
    Task PublishEvent<T>(T ev)
        where T : class;
}