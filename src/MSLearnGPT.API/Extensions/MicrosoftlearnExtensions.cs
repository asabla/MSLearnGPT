using System.Net.Http.Headers;

using MSLearnGPT.API.Services;

namespace MSLearnGPT.API.Extensions;

public static class MicrosoftlearnExtensions
{
    public static WebApplicationBuilder ConfigureMicrosoftLearnSearchService(
        this WebApplicationBuilder builder)
    {
        builder.Services
            .AddSingleton<IMicrosoftLearnSearchService, MicrosoftLearnSearchService>();

        return builder;
    }

    public static WebApplicationBuilder ConfigureMicrosoftLearnHttpClients(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient(
            name: nameof(MicrosoftLearnSearchService),
            configureClient: opt =>
            {
                opt.BaseAddress = new Uri("https://learn.microsoft.com");

                // Clear out default junk headers
                opt.DefaultRequestHeaders.Clear();

                opt.DefaultRequestHeaders.Accept.Add(
                    item: new MediaTypeWithQualityHeaderValue("application/json"));
            });

        return builder;
    }
}