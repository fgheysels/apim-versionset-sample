using System.Globalization;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;

namespace Fg.Samples.MultipleApiVersions.Swagger
{
    public class VersionedApiDescriptionProvider : IApiDescriptionProvider
    {
        private readonly ApiExplorerOptions _options;

        public VersionedApiDescriptionProvider(IOptions<ApiExplorerOptions> options)
        {
            _options = options.Value;
        }

        public int Order => -1; // Execute after DefaultApiVersionDescriptionProvider.OnProvidersExecuted

        public void OnProvidersExecuting(ApiDescriptionProviderContext context) { }

        public void OnProvidersExecuted(ApiDescriptionProviderContext context)
        {
            var newResults = new List<ApiDescription>();

            var existingDescriptions = context.Results.ToArray();

            foreach (var existing in existingDescriptions)
            {
                var apiVersion = existing.GetApiVersion();
                var versionGroupName = apiVersion.ToString(_options.GroupNameFormat, CultureInfo.CurrentCulture);

                var modifiedDescription = existing.Clone();
                modifiedDescription.GroupName = $"{existing.GroupName}_{versionGroupName}";

                newResults.Add(modifiedDescription);
            }

            foreach (var original in existingDescriptions)
            {
                context.Results.Remove(original);
            }

            foreach (var newDescription in newResults)
            {
                context.Results.Add(newDescription);
            }
        }
    }
}
