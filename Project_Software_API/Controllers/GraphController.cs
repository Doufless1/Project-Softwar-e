namespace Project_Software_API.Controllers
{

    using Microsoft.AspNetCore.Mvc;
    using Project_Software_API.Properties.Backend.Services; // Namespace for GraphData
    using System;

    namespace WeatherAPI.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class GraphController : ControllerBase
        {
            private readonly GraphData _graphData;
            private readonly ILogger<GraphController> _logger;

            public GraphController(GraphData graphData, ILogger<GraphController> logger)
            {
                _graphData = graphData ?? throw new ArgumentNullException(nameof(graphData));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            /// <summary>
            /// Fetch graph data for a given location.
            /// </summary>
            /// <param name="location">The location identifier.</param>
            /// <returns>Graph data as a dictionary.</returns>
            [HttpGet("{location}")]
            public async Task<IActionResult>GetGraphData(string location)
            {
                try
                {
                    _logger.LogInformation("Fetching graph data for location: {Location}", location);
                    var graphData = await _graphData.FetchGraphData(location);
                    return Ok(graphData); // Returns HTTP 200 with data
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        message = "Error fetching graph data.",
                        details = ex.Message
                    });
                }
            }
        }
    }
}
