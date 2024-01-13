using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

using MSLearnGPT.API.Extensions;
using MSLearnGPT.API.Models.Search;
using MSLearnGPT.API.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Setup Micorosoft Learn Search Service
builder
    .ConfigureMicrosoftLearnSearchService()
    .ConfigureMicrosoftLearnHttpClients();

// Setup OpenAPI (Swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddServer(new OpenApiServer
    {
        Url = configuration["OpenApi:ServerUrl"] ?? string.Empty,
        Description = "Unofficial Micosoft Learn OpenAPI server"
    });
});

var app = builder.Build();

app.UseSwagger();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

var msLearn = app.MapGroup("/search");
msLearn.MapGet("/", async(
    [FromServices] IMicrosoftLearnSearchService searchService,
    [FromQuery] string searchQuery = "",
    [FromQuery] int take = 3) =>
        await searchService.SearchAsync(new SearchRequest
        {
            Query = searchQuery,
            Scope = ".Net",
            Locale = "en-us",
            Facets = [ "category", "products", "tags" ],
            Filter = "(scopes/any(s: s eq '.Net'))",
            Take = take,
            ExpandScope = true,
            PartnerId = "LearnSite"
        }) is SearchResponse searchResponse
            ? Results.Ok(searchResponse)
            : Results.NotFound())
    .Produces<SearchResponse>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status500InternalServerError)
    .WithName("Microsoftlearn")
    .WithOpenApi();

app.Run();