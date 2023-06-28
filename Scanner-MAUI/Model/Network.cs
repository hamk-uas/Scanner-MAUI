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
    public class Network /*: INotifyPropertyChanged*/
    {
        private string _name;

        public Network() { }

        public string Message { get; set; }
        public string Name { get; set; }
        //public string Name
        //{
        //    get => _name;
        //    set
        //    {
        //        if (_name != value)
        //        {
        //            _name = value;
        //            OnPropertyChanged();
        //        }

        //    }
        //}
        public string Type { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int RSSI { get; set; }
        public double SNR { get; set; }

        //public event PropertyChangedEventHandler PropertyChanged;

        //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
}
