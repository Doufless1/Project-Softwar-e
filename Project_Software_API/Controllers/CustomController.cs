using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using Project_Software_API.Properties.Backend.Models;

namespace Project_Software_API.Controllers
{

    using Microsoft.AspNetCore.Mvc;
    using Project_Software_API.Properties.Backend.Services; // Namespace for GraphData
    using System;

    namespace WeatherAPI.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class CustomController : ControllerBase
        {
            private readonly GraphData _graphData;
            private readonly ILogger<GraphController> _logger;

            public CustomController(GraphData graphData, ILogger<GraphController> logger)
            {
                _graphData = graphData ?? throw new ArgumentNullException(nameof(graphData));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            /// <summary>
            /// Fetch graph data for a given location.
            /// </summary>
            /// <param name="location">The location identifier.</param>
            /// <returns>Graph data as a dictionary.</returns>
            [HttpGet("{location}/{beginDate}/{endDate}/{dataType}")]
            public async Task<IActionResult> GetWeatherDataInRange(string location, int beginDate, int end_date, AccesableData dataType)            {
                try
                {
                    _logger.LogInformation("Fetching graph data for location: {Location}", location);
                    var graphData = await _graphData.FetchWeatherDataInRange(beginDate, end_date, dataType, location);
                    var formattedData = graphData.ToDictionary(
                        kvp => JsonSerializer.Serialize(kvp.Key),
                        kvp => kvp.Value
                    );                    
                    return Ok(formattedData); // Returns HTTP 200 with data
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