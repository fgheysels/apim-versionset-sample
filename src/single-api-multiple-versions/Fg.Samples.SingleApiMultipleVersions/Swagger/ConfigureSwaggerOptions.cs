using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fg.Samples.SingleApiMultipleVersions.Swagger
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _versionDescriptionProvider;
        private readonly ApiExplorerOptions _explorerOptions;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider versionDescriptionProvider, ApiExplorerOptions explorerOptions)
        {
            _versionDescriptionProvider = versionDescriptionProvider;
            _explorerOptions = explorerOptions;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _versionDescriptionProvider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description, _explorerOptions));
            }
        }

        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription apiDescription, ApiExplorerOptions options)
        {
            var info = new OpenApiInfo
            {
                Title = apiDescription.GroupName,
                Version = apiDescription.ApiVersion.ToString(options.GroupNameFormat)
            };

            return info;
        }
    }
}
