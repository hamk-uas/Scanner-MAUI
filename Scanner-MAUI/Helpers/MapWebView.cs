using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner_MAUI.Helpers
{
    public class MapWebView
    {
        public void GetWebMap(Grid MapView, WebView webView)
        {
            webView = new WebView
            {
                Source = new HtmlWebViewSource
                {
                    Html = @" 
                        <!DOCTYPE html>
                        <html>
                          <head>
                            <meta charset=""utf-8"" />
                            <meta
                              name=""viewport""
                              content=""initial-scale=1,maximum-scale=1,user-scalable=no""
                            />
                            <title>Intro to CSVLayer - 4.15</title>

                            <style>
                              html,
                              body,
                              #viewDiv {
                                padding: 0;
                                margin: 0;
                                height: 100%;
                                width: 100%;
                                background-color: aliceblue;
                              }
                            </style>

                            <link
                              rel=""stylesheet""
                              href=""https://js.arcgis.com/4.15/esri/themes/light/main.css""
                            />
                            <script src=""https://js.arcgis.com/4.15/""></script>

                            <script>
                              require([
                                ""esri/Map"",
                                ""esri/views/MapView"",
                                ""esri/layers/CSVLayer"",
                                ""esri/widgets/Legend""
                              ], (Map, MapView, CSVLayer, Legend) => {
        
                                let fileInputField;
                                let fileInput = document.getElementById(""file-input"");
                                fileInput.addEventListener('change', () => {          
                                  const url = URL.createObjectURL(fileInput.files[0]);
                                 
                                const template = {
                                  title: ""{Name}"",
                                  content: ""RSSI: {RSSI}""
                                };
                                    
                                const renderer = {
                                  type: ""heatmap"",
                                  colorStops: [
                                    { color: ""rgba(63, 40, 102, 0)"", ratio: 0 },
                                    { color: ""#472b77"", ratio: 0.083 },
                                    { color: ""#4e2d87"", ratio: 0.166 },
                                    { color: ""#563098"", ratio: 0.249 },
                                    { color: ""#5d32a8"", ratio: 0.332 },
                                    { color: ""#6735be"", ratio: 0.415 },
                                    { color: ""#7139d4"", ratio: 0.498 },
                                    { color: ""#7b3ce9"", ratio: 0.581 },
                                    { color: ""#853fff"", ratio: 0.664 },
                                    { color: ""#a46fbf"", ratio: 0.747 },
                                    { color: ""#c29f80"", ratio: 0.83 },
                                    { color: ""#e0cf40"", ratio: 0.913 },
                                    { color: ""#ffff00"", ratio: 1 }
                                  ],
                                  maxDensity: 0.01,
                                  minDensity: 0
                                };
                                

                                  const csvLayer = new CSVLayer({
                                    url: url, 
                                    field: ""RSSI"",    
                                    title: ""Network Locations"",
                                    popupTemplate: template,
                                    renderer: renderer,
                                    labelsVisible: true,
                                    labelingInfo: [
                                    {
                                      symbol: {
                                        type: ""text"", // autocasts as new TextSymbol()
                                        color: ""white"",
                                        font: {
                                          family: ""Noto Sans"",
                                          size: 4
                                        },
                                        haloColor: ""#472b77"",
                                        haloSize: 0.75
                                      },
                                      labelPlacement: ""center-center"",
                                      labelExpressionInfo: {
                                        expression: ""Text($feature.RSSI, '#.0')""
                                      },
                                      where: ""RSSI < 5""
                                    }
                                  ]
                                  });
          
                                  map.add(csvLayer);          
                                },false);        

                                const map = new Map({
                                  basemap: ""osm"",
                                  // layers: [csvLayer]
                                });
        
                                const view = new MapView({
                                  container: ""viewDiv"",
                                  map: map,
                                  center: [24.4590, 60.9929],
                                  zoom: 3,
                                  scale: 62223.819286
                                });

                                 view.ui.add(
                                  new Legend({
                                    view: view
                                  }),
                                  ""bottom-left""
                                );

                              });
                            </script>
                          </head>

                          <body>
                            <span>
                              Browse... <input type=""file"" id=""file-input"" name=""files[]"" accept=""text/csv"">
                            </span>    
                            <div id=""viewDiv""></div>
                          </body>
                        </html> "
                }
            };
            MapView.Add(webView);
        }
    }
}
