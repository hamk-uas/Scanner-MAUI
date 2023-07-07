using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Scanner_MAUI.Model
{
    public class Network
    {
        private string _message;
        private string _name;
        private string _type;
        private double _latitude;
        private double _longitude;
        private int _rssi;
        private int _snr;

        public Network() { }

        public string Message { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int RSSI { get; set; }
        public double SNR { get; set; }

    }
}
