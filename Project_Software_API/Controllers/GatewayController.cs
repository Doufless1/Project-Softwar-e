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

            public GatewayController()
            {
                // Initialize GraphData
                _graphData = new GraphData();
            }

            /// <summary>
            /// Fetch gateway  data for a given location.
            /// </summary>
            /// <param name="location">The location identifier.</param>
            /// <returns>Graph data as a dictionary.</returns>
            [HttpGet("{location}")]
            public IActionResult GetGatewayData(string location)
            {
                try
                {
                    var gatewayData  = _graphData.FetchGatewayData(location);
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
