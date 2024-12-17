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

            public GraphController()
            {
                // Initialize GraphData
                _graphData = new GraphData();
            }

            /// <summary>
            /// Fetch graph data for a given location.
            /// </summary>
            /// <param name="location">The location identifier.</param>
            /// <returns>Graph data as a dictionary.</returns>
            [HttpGet("{location}")]
            public IActionResult GetGraphData(string location)
            {
                try
                {
                    var graphData = _graphData.FetchGraphData(location);
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
