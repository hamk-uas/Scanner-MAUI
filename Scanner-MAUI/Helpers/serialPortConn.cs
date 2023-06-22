//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO.Ports;
//using System.Threading.Tasks;
//using System.Diagnostics;

//namespace Scanner_MAUI.Helpers
//{
//    class SerialPortConn
//    {
//        private SerialPort _serialPort;
//        private int _inputMode;
//        private bool _continue;

//        public void ScannerData()
//        {
//            _serialPort = new SerialPort("COM6", 115200, Parity.None, 8, StopBits.One);
//            _serialPort.Handshake = Handshake.XOnXOff;
//            if (!_serialPort.IsOpen)
//            {
//                _serialPort.Open();
//                Debug.WriteLine("Serial port connected.");
//                _serialPort.WriteTimeout = 1000;
//                _serialPort.Write("1");
//                _serialPort.WriteLine("");
//                Debug.WriteLine("Serial port connected and input given.");
//                //_serialPort.ReadTimeout = 8000;
                
//                while (true)
//                {
//                    try
//                    {
//                        string receivedData = _serialPort.ReadLine().Trim();
//                        //string packetString = Convert.ToString(packet);
//                        Debug.WriteLine("packet string: " + receivedData);
//                        string decodedPacket = Encoding.UTF8.GetString(Encoding.Default.GetBytes(receivedData)).TrimEnd('\n');

//                        Debug.WriteLine("Decodedpacket string: " + decodedPacket);

//                        //string message = _serialPort.ReadLine();
//                        //string decodedMessage = Encoding.UTF8.GetString(Encoding.Default.GetBytes(message));
//                        Debug.WriteLine("Received: " + decodedPacket);
//                        Debug.WriteLine("Reading....");

//                        string name = ExtractValue(decodedPacket, "name:");
//                        string type = ExtractValue(decodedPacket, "type:");
//                        string rssi = ExtractValue(decodedPacket, "rssi:");
//                        string snr = ExtractValue(decodedPacket, "snr:");
//                        string time = ExtractValue(decodedPacket, "time:");

//                        // Perform any processing or display the extracted information
//                        Debug.WriteLine("Name: " + name);
//                        Debug.WriteLine("Type: " + type);
//                        Debug.WriteLine("RSSI: " + rssi);
//                        Debug.WriteLine("SNR: " + snr);
//                        Debug.WriteLine("Time: " + time);

                           
                    
                        
//                    }
//                    catch (TimeoutException ex)
//                    {
//                        // Handle the timeout exception
//                        Debug.WriteLine("Timeout occurred while reading from the serial port: " + ex.Message);
//                    }
//                }
              
//            }

//            // Close the serial port when done
//            _serialPort.Close();
//            Debug.WriteLine("Serial port connection closed!");
//        }

//        public void SendInput(int input)
//        {
//            // Make sure the serial port is open before sending the input
//            if (_serialPort.IsOpen)
//            {
//                _serialPort.Write(input.ToString());
//                if (input == 1)
//                {
//                    _inputMode = 1;
//                }
//            }
//        }

//        private string ExtractValue(string message, string keyword)
//        {
//            int startIndex = message.IndexOf(keyword) + keyword.Length;
//            int endIndex = message.IndexOf(",", startIndex);
//            if (endIndex == -1)
//                endIndex = message.Length;

//            return message.Substring(startIndex, endIndex - startIndex).Trim();
//        }
//    }
//}
