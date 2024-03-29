﻿using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Newtonsoft.Json.Linq;
using Scanner_MAUI.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace Scanner_MAUI.Helpers
{
    public class PopulateTable
    {

        public Grid CreateHeaderGrid()
        {
            var grid = new Grid
            {
                ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(18, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(14, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(14, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(14, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(14, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(13, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(14, GridUnitType.Star) }
            }
            };

            var labelName = new Label { Text = "Name", Margin = new Thickness(8), FontAttributes = FontAttributes.Bold };
            var labelType = new Label { Text = "Type", Margin = new Thickness(8), FontAttributes = FontAttributes.Bold };
            var labelLat = new Label { Text = "Latitude", Margin = new Thickness(8), FontAttributes = FontAttributes.Bold };
            var labelLon = new Label { Text = "Longitude", Margin = new Thickness(8), FontAttributes = FontAttributes.Bold };
            var labelRSSI = new Label { Text = "RSSI", Margin = new Thickness(8), FontAttributes = FontAttributes.Bold };
            var labelSNR = new Label { Text = "SNR", Margin = new Thickness(8), FontAttributes = FontAttributes.Bold };
            var labelDateTime = new Label { Text = "Date and Time", Margin = new Thickness(8), FontAttributes = FontAttributes.Bold };

            grid.Children.Add(labelName);
            grid.Children.Add(labelType);
            grid.Children.Add(labelLat);
            grid.Children.Add(labelLon);
            grid.Children.Add(labelRSSI);
            grid.Children.Add(labelSNR);
            grid.Children.Add(labelDateTime);

            Grid.SetColumn(labelName, 0);
            Grid.SetColumn(labelType, 1);
            Grid.SetColumn(labelLat, 2);
            Grid.SetColumn(labelLon, 3);
            Grid.SetColumn(labelRSSI, 4);
            Grid.SetColumn(labelSNR, 5);
            Grid.SetColumn(labelDateTime, 6);

            return grid;
        }

        public void showHeader(TableSection tableSection)
        {
            // Add the header Grid to a new ViewCell
            var headerGrid = CreateHeaderGrid();
            var headerViewCell = new ViewCell { View = headerGrid };
            tableSection.Add(headerViewCell);
        }

        public void populateTable(System.Collections.ObjectModel.ObservableCollection<Network> sDContent, TableSection tableSection)
        {
            // Clear the existing rows in the TableSection
            tableSection.Clear();
            tableSection.Title = "Scanner Historical Data By Network Name";

            // Add the header Grid to a new ViewCell
            showHeader(tableSection);

            int rowIndex = 1; // Start at row index 1 for the data rows
            foreach (Network row in sDContent)
            {
                var rowGrid = new Grid
                {
                    ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(18, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(14, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(14, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(14, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(14, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(13, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(14, GridUnitType.Star) }
                }
                };

                // Add the Labels for each network item
                var nameLabel = new Label { Text = row.Name, Margin = new Thickness(3) };
                var typeLabel = new Label { Text = row.Type, Margin = new Thickness(5) };
                var latLabel = new Label { Text = row.Lat.ToString(), Margin = new Thickness(5) };
                var lonLabel = new Label { Text = row.Lon.ToString(), Margin = new Thickness(5) };
                var rssiLabel = new Label { Text = row.RSSI.ToString(), Margin = new Thickness(5) };
                var snrLabel = new Label { Text = row.SNR.ToString(), Margin = new Thickness(5) };
                var dateTimeLabel = new Label { Text = row.Timestamp.ToString(), Margin = new Thickness(5) };


                Grid.SetColumn(nameLabel, 0);
                Grid.SetColumn(typeLabel, 1);
                Grid.SetColumn(latLabel, 2);
                Grid.SetColumn(lonLabel, 3);
                Grid.SetColumn(rssiLabel, 4);
                Grid.SetColumn(snrLabel, 5);
                Grid.SetColumn(dateTimeLabel, 6);

                rowGrid.Children.Add(nameLabel);
                rowGrid.Children.Add(typeLabel);
                rowGrid.Children.Add(latLabel);
                rowGrid.Children.Add(lonLabel);
                rowGrid.Children.Add(rssiLabel);
                rowGrid.Children.Add(snrLabel);
                rowGrid.Children.Add(dateTimeLabel);

                Grid.SetRow(rowGrid, rowIndex);
                var viewCell = new ViewCell { View = rowGrid };
                tableSection.Add(viewCell);

                rowIndex++;
            }
        }
        public void WriteNetworkDataToCSV(System.Collections.ObjectModel.ObservableCollection<Network> sDContent)
        {
            string targetFileName = "networkData.csv";
            //string path = @"C:\..\..\..\..\..\..\Scanner-MAUI\networkData.csv";
            string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, targetFileName);
            using FileStream outputStream = System.IO.File.OpenWrite(targetFile);
            // Delete the file if it exists.
            //if (File.Exists(path))
            //{
            //    File.Delete(path);
            //}

            using (StreamWriter writer = new StreamWriter(outputStream))
            {
                // Write the CSV header
                writer.WriteLine("Name;Type;Lat;Lon;RSSI;SNR;Timestamp");
                //writer.WriteLine("name,type,latitude,longitude,rssi,snr");
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";
                // Write the network data
                foreach (Network row in sDContent)
                {
                    double rssi = Convert.ToDouble(row.RSSI);
                    string csvLine = $"{row.Name};{row.Type};{row.Lat.ToString(nfi)};{row.Lon.ToString(nfi)};{rssi.ToString(nfi)/*.Replace("-", "")*/};{row.SNR};{row.Timestamp}";
                    writer.WriteLine(csvLine);
                }
            }
            outputStream.Close();

            // copy the csv to a more easier to find dir
            string targetPath = @"C:\Users\Public\CSVs";
            //string sourceFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, targetFileName);
            string destFile = System.IO.Path.Combine(targetPath, targetFileName);
            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }

            File.Copy(targetFile, destFile, true);
        }
        public void ClearCSV(System.Collections.ObjectModel.ObservableCollection<Network> sDContent)
        {

            string targetFileName = "networkData.csv";
            //string path = @"C:\..\..\..\..\..\..\Scanner-MAUI\networkData.csv";
            string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, targetFileName);
            using FileStream outputStream = System.IO.File.OpenWrite(targetFile);


            using (StreamWriter writer = new StreamWriter(outputStream))
            {
                // Write the CSV header
                writer.WriteLine("Name;Type;Lat;Lon;RSSI;SNR;Timestamp");
                //writer.WriteLine("name,type,latitude,longitude,rssi,snr");

                // Write the network data
                foreach (Network row in sDContent)
                {
                    string csvLine = string.Empty;
                    writer.WriteLine(csvLine);
                }
            }
            outputStream.Close();
        }

        internal void PopulateTableCSV(ObservableCollection<Network> cSVContent, TableSection tableSection)
        {
            // Clear the existing rows in the TableSection
            tableSection.Clear();
            tableSection.Title = "Scanner Historical Data By Network Name";

            // Add the header Grid to a new ViewCell
            showHeader(tableSection);

            int rowIndex = 1; // Start at row index 1 for the data rows
            foreach (Network row in cSVContent)
            {
                var rowGrid = new Grid
                {
                    ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(18, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(14, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(14, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(14, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(14, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(13, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(14, GridUnitType.Star) }
                }
                };

                // Add the Labels for each network item
                var nameLabel = new Label { Text = row.Name, Margin = new Thickness(3) };
                var typeLabel = new Label { Text = row.Type, Margin = new Thickness(5) };
                var latLabel = new Label { Text = row.Lat.ToString(), Margin = new Thickness(5) };
                var lonLabel = new Label { Text = row.Lon.ToString(), Margin = new Thickness(5) };
                var rssiLabel = new Label { Text = row.RSSI.ToString(), Margin = new Thickness(5) };
                var snrLabel = new Label { Text = row.SNR.ToString(), Margin = new Thickness(5) };
                var dateTimeLabel = new Label { Text = row.Timestamp.ToString(), Margin = new Thickness(5) };


                Grid.SetColumn(nameLabel, 0);
                Grid.SetColumn(typeLabel, 1);
                Grid.SetColumn(latLabel, 2);
                Grid.SetColumn(lonLabel, 3);
                Grid.SetColumn(rssiLabel, 4);
                Grid.SetColumn(snrLabel, 5);
                Grid.SetColumn(dateTimeLabel, 6);

                rowGrid.Children.Add(nameLabel);
                rowGrid.Children.Add(typeLabel);
                rowGrid.Children.Add(latLabel);
                rowGrid.Children.Add(lonLabel);
                rowGrid.Children.Add(rssiLabel);
                rowGrid.Children.Add(snrLabel);
                rowGrid.Children.Add(dateTimeLabel);

                Grid.SetRow(rowGrid, rowIndex);
                var viewCell = new ViewCell { View = rowGrid };
                tableSection.Add(viewCell);

                rowIndex++;
            }
        }
    }
}
