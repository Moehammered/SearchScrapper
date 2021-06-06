using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SearchScraping.Models.Configuration;
using SearchScraping.Services;
using SearchScraping.Templates;

namespace SearchScrapperWebService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
            => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SearchResultCaptureTemplate>(Configuration.GetSection("google-au"));
            services.Configure<SearchEngineConfiguration>(Configuration.GetSection("google"));
            services.AddHttpClient<ISearchService, SearchService>(x =>
            {
                //with these headers, the retrieved html matches a regular google search...
                //var defaultHeaders = Configuration.GetSection("clientRequestHeaders")
                //    .GetChildren()
                //    .ToDictionary(x => x.Key, y => y.Value);

                //var headers = x.DefaultRequestHeaders;
                //headers.UserAgent.Clear();
                //foreach (var header in defaultHeaders)
                //    headers.Add(header.Key, header.Value);

                //var colonHeaders = Configuration.GetSection("colonEncasedHeaders")
                //    .GetChildren()
                //    .ToDictionary(x => $":{x.Key}:", y => y.Value);
                //foreach (var header in colonHeaders)
                //    headers.TryAddWithoutValidation(header.Key, header.Value);
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SearchScrapperWebService", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SearchScrapperWebService v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
