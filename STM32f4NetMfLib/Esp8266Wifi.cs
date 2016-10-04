using System;
using Microsoft.SPOT;
using Common;
using Microsoft.SPOT.Hardware;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Collections;

namespace STM32f4NetMfLib
{

    public enum EspCommandType
    {
        SetReset,
        SetEchoType,
        SetWifiMode,
        GetVersion,
        Ping,
    }

    public enum EspCommandStatus
    {
        Error,
        OK,
        ResultList,
        NoChange
    }

    public class Esp8266WifiResponse
    {
        public EspCommandType command { get; set; }
        public EspCommandStatus status { get; set; }
        //public string[] result { get; set; }
        public ArrayList result { get; set; }
    }

    public class Esp8266Wifi : IDisposable
    {
        private string _com;
        private BaudRate _baudrate;
        private SerialPort _uart;
        private System.Threading.Timer tmrTimeout;

        private const string AT = "AT";
        private const string ATPLUS = "AT+";
        private const string END = "\r\n";

        private const string CMD_RST = "RST";       // Restart module
        private const string CMD_GMR = "GMR";       // View version info
        private const string CMD_GSLP = "GSLP";     // Enter deep-sleep mode
        private const string CMD_E = "E";           // AT commands echo or not
        private const string CMD_SLEEP = "SLEEP";   // Sleep mode
        private const string CMD_CWMODE = "CWMODE"; // Wi-Fi mode(sta/AP/sta+AP)
        private const string CMD_CWJAP = "CWJAP";   // Connect to AP
        private const string CMD_CWLAP = "CWLAP";   // Lists available APs         

        private string _rxBuffer = "";
        private Esp8266WifiResponse _commandContext;

        public enum WifiModeType
        {
            Station = 1,
            SoftAP = 2,
            StationAndSoftAP = 3
        }

        public enum EchoType
        {
            Off = 0,
            On = 1
        }

        public delegate void Esp8266WifiResponseEventHandler(Esp8266WifiResponse response);
        public event Esp8266WifiResponseEventHandler CommandResult;

        protected virtual void OnCommandResult(Esp8266WifiResponse response)
        {
            Esp8266WifiResponseEventHandler handler = CommandResult;
            if (handler != null)
            {
                handler(response);
            }
        }

        public Esp8266Wifi(string com, BaudRate baudrate, int readTimeout, int writeTimeout)
        {
            _uart = new SerialPort(com, (int)baudrate);
            _uart.ReadTimeout = readTimeout;
            _uart.WriteTimeout = writeTimeout;
            _uart.Parity = Parity.None;
            _uart.StopBits = StopBits.None;
            _commandContext = new Esp8266WifiResponse();
            _uart.DataReceived += _uart_DataReceived;
            tmrTimeout  = new System.Threading.Timer(new TimerCallback(OnTimeOut), null, Timeout.Infinite, Timeout.Infinite);
        }

        private void DisassemblyCommand()
        {
            bool isValid = false;
            int pos = 0;
            string[] data;

            if (_rxBuffer.LastIndexOf("OK") != -1)
            {

                if(_commandContext.command == EspCommandType.GetVersion)
                {
                    _commandContext.result = new ArrayList();
                    data = _rxBuffer.Split(new char[] { '\r', '\n' });

                    _commandContext.result.Add(data[0]);
                }

                _commandContext.status = EspCommandStatus.OK;



                OnCommandResult(_commandContext);

                isValid = true;

            }
            else if (_rxBuffer.LastIndexOf("ERROR") != -1)
            {
                _commandContext.status = EspCommandStatus.Error;

                OnCommandResult(_commandContext);

                isValid = true;
            }
            else if (_rxBuffer.LastIndexOf("no change") != 1)
            {
                _commandContext.status = EspCommandStatus.NoChange;

                OnCommandResult(_commandContext);

                isValid = true;
            }



            if (isValid)
            {
                tmrTimeout.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private void _uart_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int bytesRead = _uart.BytesToRead;
            byte[] data = new byte[bytesRead];
            if (bytesRead > 0)
            {
                _uart.Read(data, 0, bytesRead);
                _rxBuffer = _rxBuffer + BytesToString(data);

                DisassemblyCommand();
            }
        }

        private void OnTimeOut(object state)
        {
            Debug.Print("Timeout RX");
            Debug.Print("Receiver data: " + _rxBuffer);

            tmrTimeout.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private string BytesToString(byte[] bytes)
        {
            string response = string.Empty;

            foreach (byte b in bytes)
                response += (char)b;

            return response;
        }

        private void Open()
        {
            if(_uart == null)
            {
                _uart = new SerialPort(_com, (int)_baudrate);
            }

            if (!_uart.IsOpen)
            {
                _uart.Open();
            }
        }

        private void Close()
        {
            if (_uart.IsOpen)
                _uart.Close();
        }

        public void SendAtData(string data, int timeout = 10000)
        {
            Open();
            _uart.Flush();
            _rxBuffer = "";

            tmrTimeout.Change(timeout, timeout);

            var bytes = Encoding.UTF8.GetBytes(data);
            _uart.Write(bytes, 0, bytes.Length);

            while(_uart.BytesToWrite > 0)
            {
                Thread.Sleep(100);
            }

        }

        public bool SendReset()
        {
            _commandContext.command = EspCommandType.SetReset;
            string cmd = ATPLUS + CMD_RST + END;
            SendAtData(cmd);

            return true;
        }

        public void SendPing()
        {
            _commandContext.command = EspCommandType.Ping;
            string cmd = AT + END;
            SendAtData(cmd);
        }

        public void GetListAP()
        {
            string cmd = ATPLUS + END;
        }

        public void SetMode(WifiModeType mode)
        {
            _commandContext.command = EspCommandType.SetWifiMode;
            string cmd = ATPLUS + CMD_CWMODE + "=" + mode + END;
            SendAtData(cmd);
        }

        public void SetEchoMode(EchoType mode)
        {
            _commandContext.command = EspCommandType.SetEchoType;
            string cmd = AT + CMD_E + mode + END;
            SendAtData(cmd);
        }

        public void GetVersion()
        {
            _commandContext.command = EspCommandType.GetVersion;
            string cmd = ATPLUS + CMD_GMR + END;
            SendAtData(cmd);
        }

        public void Dispose()
        {
            if(_uart != null)
                _uart.Dispose();
        }
    }
}
