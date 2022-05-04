using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using UnityPerformancePortal.Driver;
using UnityPerformancePortal.Driver.BigQuery;
using UnityPerformancePortal.Driver.LiteDB;

namespace UnityPerformancePortal.Server
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "UnityPerformancePortal.Server", Version = "v1" });
			});

			if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("UPP_BIG_QUERY_PROJECT_ID")))
			{
				services.AddSingleton<IReportDriver, LiteDBReportDriver>();
			}
			else
			{
				services.AddSingleton(x =>
				{
					GoogleCredential credential = null;
					var json = Environment.GetEnvironmentVariable("UPP_BIG_QUERY_CREDENTIAL");
					if (!string.IsNullOrEmpty(json))
					{
						credential = GoogleCredential.FromJson(json);
					}
					return BigQueryClient.Create(Environment.GetEnvironmentVariable("UPP_BIG_QUERY_PROJECT_ID"), credential);
				});
				services.AddSingleton<IReportDriver, BigQueryReportDriver>();
			}
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UnityPerformancePortal.Server v1"));
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			DefaultFilesOptions defFileOptions = new DefaultFilesOptions();
			defFileOptions.DefaultFileNames.Clear();
			defFileOptions.DefaultFileNames.Add("index.html");
			app.UseDefaultFiles(defFileOptions);

			app.UseStaticFiles(new StaticFileOptions
			{
				ServeUnknownFileTypes = true,
			});

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapFallbackToFile("index.html");
			});
		}
	}
}