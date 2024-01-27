using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Fg.Samples.SingleApiMultipleVersions.Swagger
{
    public class ConfigureSwaggerUiOptions : IConfigureOptions<SwaggerUIOptions>
    {
        private readonly IApiDescriptionGroupCollectionProvider _descriptionProvider;

        public ConfigureSwaggerUiOptions(IApiDescriptionGroupCollectionProvider descriptionProvider)
        {
            _descriptionProvider = descriptionProvider;
        }

        public void Configure(SwaggerUIOptions options)
        {
            options.RoutePrefix = "api/docs";

            foreach (var group in _descriptionProvider.ApiDescriptionGroups.Items)
            {
                options.SwaggerEndpoint($"{group.GroupName}/docs.json", group.GroupName);
            }
        }
    }
}
