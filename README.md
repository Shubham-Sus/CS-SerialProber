# CS-SerialProber

[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

CS-SerialProber is a simple C# utility class that helps you find serial COM ports. It probes available serial ports to find the one that responds with a specific message and returns the name of that serial port.


## Installation
Simply copy the [SerialProber.cs](CSSerialProber/SerialProber.cs) file into your project.


## Usage/Examples

> [!NOTE]
> **Ensure that the serial device is configured to respond to the ping message with the expected response.**

Here's how to use the CS-SerialProber utility class to find a serial port that responds with a specific message:
```cs
using CSSerialProber;

string pingMessage = "Arduino";
string expectedResponse = "Uno";
int timeout = 500; // Optional: timeout in milliseconds

SerialProber prober = new SerialProber(115200);
string? portName = prober.Probe(pingMessage, expectedResponse, timeout);

if (portName != null)
{
    Console.WriteLine($"Device found on port: {portName}");
}
else
{
    Console.WriteLine("No device found.");
}

```


### Configuration
The `SerialProber` class comes with several configurable settings that can be adjusted to fit your specific needs:
```cs
// Create an instance of SerialProber
SerialProber prober = new SerialProber();

// Configure the settings as needed
prober.BaudRate = 9600;
prober.DataBits = 8;
prober.DiscardNull = false;
prober.DtrEnable = false;
prober.Handshake = Handshake.None;
prober.Parity = Parity.None;
prober.ParityReplace = 63;
prober.ReadBufferSize = 1000;
prober.ReadTimeout = -1;
prober.ReceivedBytesThreshold = 1;
prober.RtsEnable = false;
prober.StopBits = StopBits.One;
prober.WriteBufferSize = 1000;
prober.WriteTimeout = -1;
```
**You can adjust these properties as needed before calling the `Probe` method.**


### Example
Here's a complete example of how to use the CS-SerialProber in your application:
```cs
using System;
using CSSerialProber;

namespace SerialProberExample
{
    class Program
    {
        static void Main(string[] args)
        {
            string pingMessage = "Arduino";
            string expectedResponse = "Uno";

            // DTR and RTS signals are enabled
            SerialProber prober = new SerialProber(115200, true, true);
            string? portName = prober.Probe(pingMessage, expectedResponse);

            if (portName != null)
            {
                Console.WriteLine($"Device found on port: {portName}");
            }
            else
            {
                Console.WriteLine("No device found.");
            }
        }
    }
}
```
