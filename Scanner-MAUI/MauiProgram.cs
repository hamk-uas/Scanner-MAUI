using Microsoft.Extensions.Logging;
using Esri.ArcGISRuntime.Maui;
using Esri.ArcGISRuntime.Toolkit.Maui;
using Esri.ArcGISRuntime;
using Scanner_MAUI.Helpers;
using Esri.ArcGISRuntime.Security;
using Syncfusion.Maui.Core.Hosting;
using SkiaSharp.Views.Maui.Controls.Hosting;
using CommunityToolkit.Maui;

namespace Scanner_MAUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		var apiKey = Keys.Instance["Settings:API-key"];
		builder
			.UseMauiApp<App>()
            .ConfigureSyncfusionCore()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			//.UseArcGISRuntime()
			//.UseArcGISRuntime(config => config.UseApiKey(apiKey))
			.UseArcGISRuntime(config => config
				.ConfigureAuthentication(auth => auth
					.UseDefaultChallengeHandler()
				)
			)
			.UseArcGISToolkit()
            .UseSkiaSharp(true);

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
