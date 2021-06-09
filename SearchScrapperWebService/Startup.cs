using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SearchScraping.Interfaces;
using SearchScraping.Models.Configuration;
using SearchScraping.Services;
using SearchScraping.Templates;
using SearchScraping.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace SearchScraperWebService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly HttpHeaderUtility HeaderUtility;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            HeaderUtility = new HttpHeaderUtility(CreateRandomHeaderProvider(configuration), true);
        }

        private IHttpHeaderProvider CreateRandomHeaderProvider(IConfiguration config)
        {
            var colonHeaders = BuildColonHeaders(Configuration);
            var accepts = Configuration.GetSection("accepts").Get<IEnumerable<string>>();
            var languages = Configuration.GetSection("acceptLanguages").Get<IEnumerable<string>>();
            var userAgents = Configuration.GetSection("userAgents").Get<IEnumerable<string>>();

            return new RandomHttpHeaders(accepts, userAgents, languages, colonHeaders);
        }

        private IEnumerable<KeyValuePair<string, string>> BuildColonHeaders(IConfiguration config)
        {
            var specialHeaders = Configuration.GetSection("colonEncasedHeaders").Get<IDictionary<string, string>>();
            return specialHeaders.Select(x => new KeyValuePair<string, string>($":{x.Key}:", x.Value));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SearchResultCaptureTemplate>(Configuration.GetSection("google-au"));
            services.Configure<SearchEngineConfiguration>(Configuration.GetSection("google"));
            services.AddHttpClient<ISearchService, CachedSearchService>(client =>
            {
                HeaderUtility.ConfigureHttpClient(client);
            }).ConfigureHttpMessageHandlerBuilder(handler =>
            {
                if (handler.PrimaryHandler is HttpClientHandler clientHandler)
                    clientHandler.UseCookies = true;
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
