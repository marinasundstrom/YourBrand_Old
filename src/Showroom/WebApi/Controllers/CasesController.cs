﻿
using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Showroom.Application.Common.Models;
using YourBrand.Showroom.Application.Cases;
using YourBrand.Showroom.Application.Cases.Commands;
using YourBrand.Showroom.Application.Cases.Queries;
using YourBrand.ApiKeys;

namespace YourBrand.Showroom.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AuthSchemes.Default)]
public class CasesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CasesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<Results<CaseDto>> GetCases(int page = 1, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetCasesQuery(page - 1, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<CaseDto?> GetCase(string id, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetCaseQuery(id), cancellationToken);
    }

    [HttpPost]
    [ProducesDefaultResponseType]
    [ProducesResponseType(typeof(CaseDto), StatusCodes.Status201Created)]
    public async Task<ActionResult> CreateCase(CreateCaseDto dto, CancellationToken cancellationToken)
    {
        var dto2 = await _mediator.Send(new CreateCaseCommand(dto.Description, new CasePricing(dto.Pricing.HourlyPrice, dto.Pricing.Hours)), cancellationToken);
        return CreatedAtAction(nameof(GetCase), new { id = dto2.Id }, dto2);
    }

    [HttpPut("{id}")]
    public async Task UpdateCase(string id, UpdateCaseDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateCaseCommand(id, dto.Description), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteCase(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCaseCommand(id), cancellationToken);
    }
}

public record UpdatePricingDto(decimal? HourlyPrice, double? Hours);

public record CreateCaseDto(string? Description, UpdatePricingDto Pricing);

public record UpdateCaseDto(string? Description);