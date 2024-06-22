using System.IO.Ports;

namespace CSSerialProber
{
    /// <summary>
    /// Utility class for probing serial ports and searching for a port that responds with a specified message.
    /// </summary>
    public class SerialProber
    {
        // Default settings for serial communication
        public int BaudRate { get; set; } = 9600;
        public int DataBits { get; set; } = 8;
        public bool DiscardNull { get; set; } = false;
        public bool DtrEnable { get; set; } = false;
        public Handshake Handshake { get; set; } = Handshake.None;
        public Parity Parity { get; set; } = Parity.None;
        public byte ParityReplace { get; set; } = 63;
        public int ReadBufferSize { get; set; } = 1000;
        public int ReadTimeout { get; set; } = -1;
        public int ReceivedBytesThreshold { get; set; } = 1;
        public bool RtsEnable { get; set; } = false;
        public StopBits StopBits { get; set; } = StopBits.One;
        public int WriteBufferSize { get; set; } = 1000;
        public int WriteTimeout { get; set; } = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialProber"/> class with specified serial port settings.
        /// </summary>
        /// <param name="baundRate">The baud rate for the serial communication.</param>
        public SerialProber(int baundRate)
        {
            BaudRate = baundRate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialProber"/> class with specified serial port settings.
        /// </summary>
        /// <param name="baundRate">The baud rate for the serial communication.</param>
        /// <param name="dtrEnable">A value indicating whether the Data Terminal Ready (DTR) signal is enabled.</param>
        /// <param name="rtsEnable">A value indicating whether the Request to Send (RTS) signal is enabled.</param>
        public SerialProber(int baundRate, bool dtrEnable, bool rtsEnable)
        {
            BaudRate = baundRate;
            DtrEnable = dtrEnable;
            RtsEnable = rtsEnable;
        }

        /// <summary>
        /// Probes for a serial port that responds with the specified pong message to the specified ping message within a given timeout.
        /// </summary>
        /// <param name="pingMsg">The message sent to each serial port as a ping.</param>
        /// <param name="pongMsg">The expected response message (pong) from the serial port.</param>
        /// <param name="timeout">The maximum time, in milliseconds, to wait for a response from each serial port.</param>
        /// <returns>The name of the first serial port that responds with the expected pong message within the timeout period. Returns null if no such port is found.</returns>
        public string? Probe(string pingMsg, string pongMsg, int timeout = 300)
        {
            // Iterate through all available serial port names.
            foreach (string portName in SerialPort.GetPortNames())
            {
                using (SerialPort port = new SerialPort(portName))
                {
                    ConfigureSerialPort(port);

                    var responseReceivedEvent = new ManualResetEventSlim(false);
                    string? receivedPortName = null;

                    // Event handler for receiving data
                    void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
                    {
                        try
                        {
                            string response = port.ReadLine().Trim();
                            if (response.Equals(pongMsg, StringComparison.Ordinal))
                            {
                                receivedPortName = port.PortName;
                                // Signal that the response has been received.
                                responseReceivedEvent.Set();
                            }
                        }
                        catch (Exception) { }
                    }

                    try
                    {
                        port.WriteTimeout = timeout;
                        port.DataReceived += DataReceivedHandler;
                        port.Open();
                        port.WriteLine(pingMsg);

                        // Wait for the pong response within the specified timeout.
                        if (responseReceivedEvent.Wait(timeout))
                        {
                            return receivedPortName; // Return the name of the port that responded.
                        }
                    }
                    catch (Exception)
                    {
                        // Continue probing the next port in case of an exception.
                        continue;
                    }
                    finally
                    {
                        // Clean up resources.
                        responseReceivedEvent.Dispose();
                        port.DataReceived -= DataReceivedHandler;
                        if (port.IsOpen)
                            port.Close();
                    }
                }
            }
            // Return null if no matching port is found.
            return null;
        }

        private void ConfigureSerialPort(SerialPort port)
        {
            port.BaudRate = BaudRate;
            port.DataBits = DataBits;
            port.DiscardNull = DiscardNull;
            port.DtrEnable = DtrEnable;
            port.Handshake = Handshake;
            port.Parity = Parity;
            port.ParityReplace = ParityReplace;
            port.ReadBufferSize = ReadBufferSize;
            port.ReadTimeout = ReadTimeout;
            port.ReceivedBytesThreshold = ReceivedBytesThreshold;
            port.RtsEnable = RtsEnable;
            port.StopBits = StopBits;
            port.WriteBufferSize = WriteBufferSize;
            port.WriteTimeout = WriteTimeout;
        }
    }
}
