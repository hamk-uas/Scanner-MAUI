using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Scanner_MAUI.Helpers;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using Map = Esri.ArcGISRuntime.Mapping.Map;

namespace Scanner_MAUI.Pages;

public partial class MainWindow : ContentPage
{
    // URL to a sample layer with earthquake points.
    private string _earthquakesUrl = "https://sampleserver6.arcgisonline.com/arcgis/rest/services/Earthquakes_Since1970/FeatureServer/0";

    // Store a reference to the quakes layer and its default renderer.
    private FeatureLayer _quakesLayer;
    private Renderer _defaultRenderer;

    private List<Field> numericFields = new List<Field>();

    public MainWindow()
	{
        InitializeComponent();
        Init();
        Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.ApiKey = "AAPK251cd3ef2a7e41c6afbf1de480e30b68JgLIdErsDZRkzGXWeygr-WA_-d8iMLflw3m3fsI26ikJuPq8JkxpOtM9BYuSQW8X";
    }
    private async void Init()
    {
        // Create the OpenStreetMap basemap.
        Basemap osmBasemap = new Basemap(BasemapStyle.OSMStandard);

        // Create the map with the OpenStreetMap basemap.
        Map map = new Map(osmBasemap);

        // Create the earthquake layer, load it, and get its default renderer.
        _quakesLayer = new FeatureLayer(new Uri(_earthquakesUrl));
        await _quakesLayer.LoadAsync();
        _defaultRenderer = _quakesLayer.Renderer;

        // Add the quakes layer to the map, add the map to the map view.
        map.OperationalLayers.Add(_quakesLayer);
        MyMapView.Map = map;

        //// Set the color lists with some standard colors.
        //Color[] colors = new Color[] { Colors.Red, Colors.Yellow, Colors.Orange, Colors.Blue, Colors.Green };
        //StartColorComboBox.ItemsSource = colors;
        //EndColorComboBox.ItemsSource = colors;

        // Fill the field list with the layer's numeric fields.
        
        int x =1;
        foreach (var fld in _quakesLayer.FeatureTable.Fields)
        {
            //if (fld.FieldType.HasFlag(FieldType.Float32 | FieldType.Float64 | FieldType.Int16 | FieldType.Int32))
            //{
            //    numericFields.Add(fld);
            //}
            if (fld.FieldType == FieldType.Float32 || fld.FieldType == FieldType.Float64 || fld.FieldType == FieldType.Int16 || fld.FieldType == FieldType.Int32)
            {
                numericFields.Add(fld);
            }
            Debug.WriteLine(x + "-fields: " + fld);
            x++;
        }
        Debug.WriteLine("numericFields: " + numericFields[11]);

        //FieldComboBox.ItemsSource = numericFields;

        // Create the blur radius combo box.
        for (int i = 5; i < 30; i++)
        {
            //BlurRadiusComboBox.Items.Add(i.ToString());
        }
    }

    private void UpdateRendererButton_Clicked(object sender, EventArgs e)
    {
        // Parse some of the user inputs.
        //double.TryParse(MinIntensityTextBox.Text, out double minIntensity);
        //double.TryParse(MaxIntensityTextBox.Text, out double maxIntensity);
        //int.TryParse(BlurRadiusComboBox.SelectedItem.ToString(), out int blurRadius);

        // Create a new HeatMapRenderer with info provided by the user.
        HeatMapRenderer heatMapRendererInfo = new HeatMapRenderer
        {
            BlurRadius = 20,
            MinPixelIntensity = 0.0,
            MaxPixelIntensity = 1000.0,
            Field = _quakesLayer.FeatureTable.Fields[12].Name
        };

        //heatMapRendererInfo.Field = numericFields[11].Name;

        // Use a selected field to weight the point density if the user chooses to do so.
        //if (UseFieldCheckBox.IsChecked == true)
        //{
        //    heatMapRendererInfo.Field = (FieldComboBox.SelectedItem as Field).Name;
        //}

        // Add color stops to the HeatMapRenderer
        heatMapRendererInfo.AddColorStop(0.0, Colors.Transparent);
        heatMapRendererInfo.AddColorStop(0.10, Colors.Red);    // Red color at position 0
        heatMapRendererInfo.AddColorStop(1.0, Colors.Yellow);  // Yellow color at position 0.5
        //heatMapRendererInfo.AddColorStop(1.0, new Color(0, 1, 0, (float)0.6));    // Green color at position 1

        // Add the chosen color stops (plus transparent for empty areas).
        //heatMapRendererInfo.AddColorStop(0.0, Colors.Transparent);
        //heatMapRendererInfo.AddColorStop(0.10, (Color)StartColorComboBox.SelectedValue);
        //heatMapRendererInfo.AddColorStop(1.0, (Color)EndColorComboBox.SelectedValue);

        // Get the JSON representation of the renderer class.
        string heatMapJson = heatMapRendererInfo.ToJson();

        // Use the static Renderer.FromJson method to create a new renderer from the JSON string.
        var heatMapRenderer = Renderer.FromJson(heatMapJson);

        // Apply the renderer to a point layer in the map.
        _quakesLayer.Renderer = heatMapRenderer;
    }

    private void ResetRenderer_Clicked(object sender, EventArgs e)
    {
        // Reapply the default renderer for the layer.
        _quakesLayer.Renderer = _defaultRenderer;
    }
}
