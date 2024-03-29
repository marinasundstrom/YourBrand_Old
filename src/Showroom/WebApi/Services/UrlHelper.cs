﻿using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Infrastructure;
namespace YourBrand.Showroom.WebApi.Services;

public class UrlHelper : IUrlHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UrlHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetHostUrl()
    {
        var request = _httpContextAccessor.HttpContext!.Request;

        string host = $"{request.Scheme}://{request.Host}";

        return $"{host}";
    }

    public string? CreateImageUrl(string? id)
    {
        if (id is null) return null;

        var host = GetHostUrl();

        //return $"{host}/content/images/{id}";

        return $"https://localhost:5174/content/images/{id}";
    }
}