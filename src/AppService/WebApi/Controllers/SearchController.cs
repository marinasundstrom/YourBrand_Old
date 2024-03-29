﻿
using YourBrand.Application.Common.Models;
using YourBrand.Application.Search;
using YourBrand.Application.Search.Commands;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace YourBrand.WebApi.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize]
public class SearchController : Controller
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Results<SearchResultItem>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Results<SearchResultItem>>> Search(string searchText,
        int page = 1, int pageSize = 5, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new SearchCommand(searchText, page - 1, pageSize, sortBy, sortDirection), cancellationToken));
    }
}
