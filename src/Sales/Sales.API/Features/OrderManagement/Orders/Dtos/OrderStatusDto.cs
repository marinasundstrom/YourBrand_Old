﻿using System.ComponentModel.DataAnnotations;

using YourBrand.Sales.Features;

namespace YourBrand.Sales.API.Features.OrderManagement.Orders.Dtos;

public record OrderStatusDto
(
    int Id,
    string Name,
    string Handle,
    string? Description
);