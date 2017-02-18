using Microsoft.Maker.RemoteWiring;
using Microsoft.Maker.Serial;
using Microsoft.Maker.Firmata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Devices.Enumeration;

namespace Arduino_Iot_PinMapping_UWP
{
    public class ArduinoController
    {
        public delegate void ConnectionLostFailHandler(string message);
        public delegate void PinUpdateHandler(AModule module,APin pin,ushort value);

        public event ConnectionLostFailHandler ConnectionLostEvent;
        public event ConnectionLostFailHandler ConnectionFailEvent;
        public event PinUpdateHandler onPinUpdateEvent;

        RemoteDevice arduino;
        BluetoothSerial bluetoothConnection;
        NetworkSerial networkConnection;
        UsbSerial usbConnection;
        Dictionary<string, APin> _pinNum = new Dictionary<string, APin>();
        Dictionary<string, AModule> _moduleNum = new Dictionary<string, AModule>();
        bool deviceReady = false;
        
        private ConnectionType _connType;
        


        public enum ConnectionType { Bluetooth, Network, Usb }
        public ConnectionType connectionType
        {
            get { return _connType; }
        }
        private ArduinoController(IStream connection)
        {
            arduino = new RemoteDevice(connection);
            arduino.DeviceReady += Setup;
        }

        private void Setup()
        {
            lock(_moduleNum)
            {
                foreach(AModule am in _moduleNum.Values)
                {
                    BindPins(am);
                }
                deviceReady = true;
            }
        }

        /// <summary>
        /// Connect to arduino over bloetooth
        /// </summary>
        /// <param name="DeviceID">Device name or ID</param>
        /// <returns></returns>
        public static ArduinoController BluetoothConnection(string DeviceID, uint baudRate = 115200, SerialConfig serialConfig = SerialConfig.SERIAL_8N1)
        {
            BluetoothSerial Connection1 = new BluetoothSerial(DeviceID);
            var res = new ArduinoController(Connection1);
            Connection1.begin(baudRate, serialConfig);
            res.bluetoothConnection = Connection1;
            res._connType = ConnectionType.Bluetooth;
            return res;
        }
        public static ArduinoController NetworkConnection(HostName host,ushort port, uint baudRate = 115200, SerialConfig serialConfig = SerialConfig.SERIAL_8N1)
        {
            NetworkSerial Connection1 = new NetworkSerial(host, port);
            var res = new ArduinoController(Connection1);
            Connection1.begin(baudRate, serialConfig);
            res.networkConnection = Connection1;
            res._connType = ConnectionType.Network;
            return res;
        }
        public static ArduinoController UsbConnection(DeviceInformation device,uint baudRate = 115200,SerialConfig serialConfig = SerialConfig.SERIAL_8N1)
        {
            UsbSerial Connection1 = new UsbSerial(device);
            Connection1.begin(baudRate, serialConfig);
            var res = new ArduinoController(Connection1);
            res.usbConnection = Connection1;
            res._connType = ConnectionType.Usb;
            return res;
        }
        public void AddModule(AModule mod)
        {
            
           lock(_moduleNum)
            {
                foreach(APin apm in mod.pins) if (_pinNum.ContainsKey(apm.pinNumber)) throw new ModuleConflictException("Pin " + apm.pinNumber + " already used in '" + _moduleNum[apm.pinNumber].name + "' module.");
                if(deviceReady)  BindPins(mod);
                foreach (APin apm in mod.pins)
                {
                    _pinNum.Add(apm.pinNumber, apm);
                    _moduleNum.Add(apm.pinNumber, mod);
                }
            }
        }
        private void ArduinoEvents()
        {
            arduino.AnalogPinUpdated += Arduino_AnalogPinUpdated;
            arduino.DigitalPinUpdated += Arduino_DigitalPinUpdated;
            arduino.DeviceConnectionFailed += Arduino_DeviceConnectionFailed;
            arduino.DeviceConnectionLost += Arduino_DeviceConnectionLost;
            arduino.StringMessageReceived += Arduino_StringMessageReceived;
            arduino.SysexMessageReceived += Arduino_SysexMessageReceived;
        }

        private void Arduino_SysexMessageReceived(byte command, Windows.Storage.Streams.DataReader message)
        {
            throw new NotImplementedException();
        }

        private void Arduino_StringMessageReceived(string message)
        {
            throw new NotImplementedException();
        }

        private void Arduino_DeviceConnectionLost(string message)
        {
            if (ConnectionLostEvent != null) ConnectionLostEvent(message);
        }

        private void Arduino_DeviceConnectionFailed(string message)
        {
            if (ConnectionFailEvent != null) ConnectionFailEvent(message);
        }

        private void Arduino_DigitalPinUpdated(byte pin, PinState state)
        {
            string pinNumber = ((short)pin).ToString();
            AModule module = _moduleNum[pinNumber];
            APin p = _pinNum[pinNumber];
            if (onPinUpdateEvent != null) onPinUpdateEvent(module, p, (ushort)state);
        }

        private void Arduino_AnalogPinUpdated(string pin, ushort value)
        {
            AModule module = _moduleNum[pin];
            APin p = _pinNum[pin];
            if (onPinUpdateEvent != null) onPinUpdateEvent(module, p, value);
        }

        private void BindPins(AModule mod)
        {
            foreach (APin pin in mod.pins)
            {
                //Analogic or Digital pin
                
                if (char.IsLetter(pin.pinNumber[0])) arduino.pinMode(pin.pinNumber, pin.pinMode);
                else arduino.pinMode((byte)short.Parse(pin.pinNumber), pin.pinMode);
            }
        }
    }
    public class ModuleConflictException : Exception
    {
        public ModuleConflictException(string message)
        : base(message)
        {
        }


    }
}
