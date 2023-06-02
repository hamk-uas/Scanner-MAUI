using Microsoft.Extensions.Logging;
using Esri.ArcGISRuntime.Maui;
using Esri.ArcGISRuntime.Toolkit.Maui;
using Esri.ArcGISRuntime;

namespace Scanner_MAUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.UseArcGISRuntime(/*config => config.UseApiKey("")*/)
            .UseArcGISToolkit();


#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
