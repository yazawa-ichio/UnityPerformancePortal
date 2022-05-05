using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace UnityPerformancePortal.Front
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("#app");

			builder.Services.AddBlazoredLocalStorage();

			//TODO:BaseAddressをconfig等で変更出来るように。
			if (builder.HostEnvironment.IsProduction())
			{
				builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress + "api/") });
			}
			else
			{
				builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5325/api/") });
			}
			builder.Services.AddScoped<PortalClient>();
			builder.Services.AddMudServices(configuration: config =>
			{
				config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
			});

			await builder.Build().RunAsync();
		}
	}
}