using Esri.ArcGISRuntime.Data;
using Scanner_MAUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Scanner_MAUI.Helpers
{
    public class ReadCSV
    {
        public ObservableCollection<Network> NetworkNames { get; set; } = new ObservableCollection<Network>();
        public ObservableCollection<Network> CSVContent { get; set; } = new ObservableCollection<Network>();
        private Network ExistingNetwork { get; set; }
        public void ReadCSVFile()
        {

            string targetFileName = "networkData.csv";
            //string path = @"C:\..\..\..\..\..\..\Scanner-MAUI\networkData.csv";
            string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, targetFileName);
            using FileStream outputStream = System.IO.File.OpenRead(targetFile);

            using (StreamReader reader = new StreamReader(targetFile))
            {
                string readData = reader.ReadToEnd();
                // Process the received data and extract the fields
                Network data = ProcessCSVData(readData);

            }
            outputStream.Close();
        }

        private Network ProcessCSVData(string data)
        {
            Network networkData = new Network();
            string[] lines = data.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            int i = 0;

            foreach (string row in lines)
            {
                if (i > 0)
                {
                    Debug.WriteLine(i + " " + row);
                    string[] fields = row.Split(new string[] { ";" }, StringSplitOptions.None);
                    Debug.WriteLine($"Received data fields count: {fields.Length}");

                    if (fields.Length >= 7)
                    {
                        networkData.Name = fields[0].Trim('\'');
                        Debug.WriteLine("Name " + networkData.Name);
                        networkData.Type = fields[1].Trim();

                        if (double.TryParse(fields[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double latValue))
                        {
                            networkData.Lat = latValue;
                        }


                        if (double.TryParse(fields[3], NumberStyles.Float, CultureInfo.InvariantCulture, out double lonValue))
                        {
                            networkData.Lon = lonValue;
                        }

                        if (int.TryParse(fields[4], out int rssiValue))
                        {
                            networkData.RSSI = rssiValue;
                        }

                        if (double.TryParse(fields[5], NumberStyles.Float, CultureInfo.InvariantCulture, out double snrValue))
                        {
                            networkData.SNR = snrValue;
                        }

                        string timestamp = fields[6].Trim();

                        DateTime dt = DateTime.Parse(timestamp);

                        networkData.Timestamp = dt;

                        // Find the existing network with the same name, if it exists
                        ExistingNetwork = NetworkNames.FirstOrDefault(network => network.Name == networkData.Name);

                        if (ExistingNetwork != null)
                        {
                            if (networkData.Name == ExistingNetwork.Name)
                            {
                                CSVContent.Add(new Network
                                {
                                    Name = networkData.Name,
                                    Type = networkData.Type,
                                    Lat = networkData.Lat,
                                    Lon = networkData.Lon,
                                    RSSI = networkData.RSSI,
                                    SNR = networkData.SNR,
                                    Timestamp = networkData.Timestamp,
                                });
                            }
                        }
                        else
                        {
                            NetworkNames.Add(new Network { Name = networkData.Name });
                        }

                    }

                    else
                    {
                        Debug.WriteLine("Insufficient fields in the data string.");
                    }
                }

                i++;
            }

            return networkData;
        }
    }
}
