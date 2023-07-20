
namespace Scanner_MAUI.Model
{
    public class Network
    {
        private string _message;
        private string _name;
        private string _type;
        private double _lat;
        private double _lon;
        private int _rssi;
        private double _snr;

        public Network() { }

        public string Message { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public int RSSI { get; set; }
        public double SNR { get; set; }

    }
}
