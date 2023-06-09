using Scanner_MAUI.Helpers;

namespace Scanner_MAUI.Pages;

public partial class RealTimeData : ContentPage
{

    public RealTimeData()
    {
        InitializeComponent();
        _ = ViewMap.LoadWMTSLayer(MyMapView);
    }
}