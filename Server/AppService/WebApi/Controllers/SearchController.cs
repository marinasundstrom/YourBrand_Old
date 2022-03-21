﻿
using YourCompany.Application.Common.Models;
using YourCompany.Application.Search;
using YourCompany.Application.Search.Commands;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourCompany.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
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