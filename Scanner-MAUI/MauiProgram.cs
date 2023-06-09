using Microsoft.Extensions.Logging;
using Esri.ArcGISRuntime.Maui;
using Esri.ArcGISRuntime.Toolkit.Maui;
using Esri.ArcGISRuntime;
using Scanner_MAUI.Helpers;
using Esri.ArcGISRuntime.Security;

namespace Scanner_MAUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		var apiKey = Keys.Instance["Settings:API-key"];
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.UseArcGISRuntime()
			//.UseArcGISRuntime(config => config.UseApiKey(apiKey))
			//.UseArcGISRuntime(config => config
			//	.ConfigureAuthentication(auth => auth
			//		.UseDefaultChallengeHandler()
			//	)
			//)
			.UseArcGISToolkit();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
