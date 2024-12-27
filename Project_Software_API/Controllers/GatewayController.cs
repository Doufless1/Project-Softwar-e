namespace Project_Software_API.Controllers
{

    using Microsoft.AspNetCore.Mvc;
    using Project_Software_API.Properties.Backend.Services; // Namespace for GraphData
    using System;

    namespace WeatherAPI.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class GatewayController : ControllerBase
        {
            private readonly GraphData _graphData;
            private readonly ILogger<GatewayController> _logger;

            public GatewayController(GraphData graphData, ILogger<GatewayController> logger)
            {
                _graphData = graphData ?? throw new ArgumentNullException(nameof(graphData));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }
            
            /// <summary>
            /// Fetch gateway  data for a given location.
            /// </summary>
            /// <param name="location">The location identifier.</param>
            /// <returns>Graph data as a dictionary.</returns>
            [HttpGet("{location}")]
            public async Task<IActionResult> GetGatewayData(string location)
            {
                try
                {
                    _logger.LogInformation("Fetching gateway data for location: {Location}", location);
                    var gatewayData  = await _graphData.FetchGatewayData(location);
                    _logger.LogInformation("Fetched gateway data: {Data}", gatewayData);
                    return Ok(gatewayData); // Returns HTTP 200 with data
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        message = "Error fetching gateway  data.",
                        details = ex.Message
                    });
                }
            }
        }
    }
}
