using CryptoPriceAggregator.Application.Features.Prices.Queries.GetAggregatedPrice;
using Microsoft.AspNetCore.Mvc;
using CryptoPriceAggregator.Application.Features.Prices.Queries.GetPriceHistory;

namespace CryptoPriceAggregator.Api.Controllers;

[ApiController]
[Route("api/prices")]
public class PricesController : ControllerBase
{
    private readonly GetAggregatedPriceQueryHandler _getAggregatedPriceHandler;
    private readonly GetPriceHistoryQueryHandler _getPriceHistoryQueryHandler;

    public PricesController(
        GetAggregatedPriceQueryHandler getAggregatedPriceQueryHandler,
        GetPriceHistoryQueryHandler getPriceHistoryQueryHandler)
    {
        _getAggregatedPriceHandler = getAggregatedPriceQueryHandler;
        _getPriceHistoryQueryHandler = getPriceHistoryQueryHandler;
    }

    [HttpGet("aggregated")]
    [ProducesResponseType(typeof(GetAggregatedPriceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<GetAggregatedPriceResult>> GetAggregatedPrice(
        [FromQuery] string instrument,
        [FromQuery] DateTime timestampUtc,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _getAggregatedPriceHandler.HandleAsync(
                new GetAggregatedPriceQuery(instrument, timestampUtc),
                cancellationToken);

            return Ok(result);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
        catch (HttpRequestException exception)
        {
            return StatusCode(
                StatusCodes.Status502BadGateway,
                new { error = $"Price provider error: {exception.Message}" });
        }
        catch (InvalidOperationException exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { error = exception.Message });
        }
    }

    [HttpGet("history")]
    [ProducesResponseType(typeof(GetPriceHistoryResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GetPriceHistoryResult>> GetPriceHistory(
    [FromQuery] string instrument,
    [FromQuery] DateTime fromUtc,
    [FromQuery] DateTime toUtc,
    CancellationToken cancellationToken)
    {
        try
        {
            var result = await _getPriceHistoryQueryHandler.HandleAsync(
                new GetPriceHistoryQuery(instrument, fromUtc, toUtc),
                cancellationToken);

            return Ok(result);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }
}