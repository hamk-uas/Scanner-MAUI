using Esri.ArcGISRuntime.UI;

namespace Scanner_MAUI.Pages;

public partial class StartPage : ContentPage
{
	public StartPage()
	{
		InitializeComponent();
	}

    private async void StartButton_Clicked(object sender, EventArgs e)
    {
        //await Navigation.PushAsync(new AppShell());
        await Navigation.PushAsync(new RealTimeData());
    }

    private async void HistoricalData_Clicked(object sender, EventArgs e)
    {
        //await Navigation.PushAsync(new AppShell());
        await Navigation.PushAsync(new HistoricalData());
    }
}