using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fg.Samples.MultipleApiVersions.Swagger
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiDescriptionGroupCollectionProvider _descriptionProvider;
        private readonly ApiExplorerOptions _explorerOptions;

        public ConfigureSwaggerOptions(IApiDescriptionGroupCollectionProvider descriptionProvider, IOptions<ApiExplorerOptions> explorerOptions)
        {
            _descriptionProvider = descriptionProvider;
            _explorerOptions = explorerOptions.Value;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var group in _descriptionProvider.ApiDescriptionGroups.Items)
            {
                options.SwaggerDoc($"{group.GroupName}", CreateInfoForApiVersion(group, _explorerOptions));
            }
        }

        private static OpenApiInfo CreateInfoForApiVersion(ApiDescriptionGroup apiDescription, ApiExplorerOptions options)
        {
            var info = new OpenApiInfo
            {
                Title = apiDescription.GroupName,
                Version = apiDescription.Items.First().GetApiVersion()!.ToString(options.GroupNameFormat)
            };

            return info;
        }
    }
}
