using Asp.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Fg.Samples.MultipleApiVersions.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Fg.Samples.MultipleApiVersions
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApiVersioning(options =>
                {
                    options.ReportApiVersions = true;

                    options.AssumeDefaultVersionWhenUnspecified = false;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                })
                .AddApiExplorer(options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

            builder.Services.AddRouting(options => options.LowercaseUrls = true);
            builder.Services.AddControllers();

            builder.Services.AddTransient<IApiDescriptionProvider, VersionedApiDescriptionProvider>();
            builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            builder.Services.AddTransient<IConfigureOptions<SwaggerUIOptions>, ConfigureSwaggerUiOptions>();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "api/docs/{documentName}/docs.json";
            });

            app.UseSwaggerUI();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}