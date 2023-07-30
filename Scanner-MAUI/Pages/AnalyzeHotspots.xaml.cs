using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Map = Esri.ArcGISRuntime.Mapping.Map;

namespace Scanner_MAUI.Pages;

public partial class AnalyzeHotspots : ContentPage
    {
    private Viewpoint _initialViewpoint = new Viewpoint(new MapPoint(-13631205.660131, 4546829.846004, SpatialReferences.WebMercator), 25000);

    public AnalyzeHotspots()
    {
        InitializeComponent();

        Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.ApiKey = "AAPK251cd3ef2a7e41c6afbf1de480e30b68JgLIdErsDZRkzGXWeygr-WA_-d8iMLflw3m3fsI26ikJuPq8JkxpOtM9BYuSQW8X";
        
    }
   
}
