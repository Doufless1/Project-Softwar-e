using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Project_Software_API.Properties.Backend.Services;
using Project_Software_API.Properties.Backend.SQL.sql_fetcher;

// this create a webppapplicaiton which configure the applicaiton based on the arguments provided
var builder = WebApplication.CreateBuilder(args);
// this is for Dependency Injection container 


builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(50);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(50);
});


builder.Services.AddControllers();

// Register your custom services
builder.Services.AddSingleton<DataAccess>();
builder.Services.AddSingleton<GraphData>();

// Add Authorization Services
builder.Services.AddAuthorization();
// this is the DI where all reigsters required services operates
// like mine GraphController and GatewayController

// This is for CORS policy
// this allows like http://localhost:3000 to access the API
// the allowanymehtod is for get post put delete
// the allowanyheader is for the http headers
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", buildera =>
    {
        buildera.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// this build the applcaitons isntance
var app = builder.Build();

// This is to check for develport or production
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Detailed error pages during development
}


app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
// the diffrence between http and https is that it secures the data 
// if u remember we had to do somehting like wss for our websocket to workac

app.UseRouting();
// routing middleware to the pipe;ine 
// this mean like lets say we have a http request
// it will go through the pipeline and the routing middleware will route the request to the correct controller
// for example  /api/gateway will go to the GatewayController
app.UseCors("AllowAll"); // Apply CORS policy (if enabled)
// this si the CORS policy we created above so it works correctly
app.UseAuthorization();
// am not sure If I need this but this is for authorization
app.MapControllers(); // Map controller routes automatically
// for exmaple GET /api/graph/{location} will go to the GraphController
// and GET /api/gateway/{location} will go to the GatewayController

// Start the application
app.Run();