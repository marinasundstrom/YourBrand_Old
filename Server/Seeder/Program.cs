﻿using Microsoft.Extensions.DependencyInjection;
using YourCompany.IdentityService.Client;

const string ApiKey = "foobar";

var services = BuildServiceProvider();

var usersClient = services.GetRequiredService<IUsersClient>();

await usersClient.CreateUserAsync(new CreateUserDto
{
    FirstName = "Administrator",
    LastName = "Administrator",
    DisplayName = "Administrator",
    Role = "Administrator",
    Ssn = "",
    Email = "admin@email.com",
    Password = "Abc123!?"
});

Console.WriteLine("Created user");

static IServiceProvider BuildServiceProvider()
{
    ServiceCollection services = new();

    services.AddHttpClient(nameof(IUsersClient), (sp, http) =>
    {
        http.BaseAddress = new Uri($"https://identity.local/");
        http.DefaultRequestHeaders.Add("X-API-KEY", ApiKey);
    })
    .AddTypedClient<IUsersClient>((http, sp) => new UsersClient(http));

    /*
    services.AddHttpClient(nameof(YourCompany.TimeReport.Client.IProjectsClient), (sp, http) =>
    {
        http.BaseAddress = new Uri($"https://localhost:5050/");
        http.DefaultRequestHeaders.Add("X-API-KEY", ApiKey);
    })
    .AddTypedClient<YourCompany.TimeReport.Client.IProjectsClient>((http, sp) => new YourCompany.TimeReport.Client.ProjectsClient(http));
    */

    return services.BuildServiceProvider();
}