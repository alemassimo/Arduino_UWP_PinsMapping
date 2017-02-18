# Arduino UWP PinsMapping     -    Proj under development
Arduino Uwp library for pins mapping, based on Windows Remote Arduino Library (https://github.com/ms-iot/remote-wiring).

This library simplifies the use of the Windows Remote Arduino Library .

The application domain is based on two classes : AModule and APin


- AModule represents an Arduino module as a sensor or an actuator. 
- APin is a single pin of arduino board.

# Usage
```c#
public MainPage()
        {
          AModule SensorTemperature = new AModule("Temperature Sensor", "External temperature sensor");
          APin TempPin = new APin("Temperature", "5", PinMode.SERIAL);
          SensorTemperature.pins.Add(TempPin);

          //Connection to Arduino over bluetooth 
          uint baudRate = 115200;
          ArduinoController arduino = ArduinoController.BluetoothConnection("DeviceID", baudRate, SerialConfig.SERIAL_8N1);
          
          arduino.ConnectionLostEvent += Arduino_ConnectionLostEvent;
          arduino.ConnectionFailEvent += Arduino_ConnectionFailEvent;
        }
        
private void Arduino_onPinUpdateEvent(AModule module, APin pin, ushort value)
        {
           // you code
        }
```
