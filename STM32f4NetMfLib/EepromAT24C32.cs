using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.IO;
using System.Threading;

namespace STM32f4NetMfLib
{
    //#define STM32F4_I2C_SCL_PIN = PB6
    //#define STM32F4_I2C_SDA_PIN = PB9
    public class EepromAT24C32
    {

        private I2CDevice.Configuration _config;
        private const int _transactionTimeout = 3000; // ms
        private const int _clockRateKHz = 100; // de 100 a 400
        public byte Address { get; private set; }
        private readonly I2CDevice _device;

        public const byte CONST_DEVICE_ADDRESS1 = 0X50; // 0b1010xxxx
        public const byte CONST_PAGE_SIZE = 64;
        public const uint CONST_MAX_MEM_SIZE = 4028;


        public EepromAT24C32(byte address)
        {
            Address = address;
            _config = new I2CDevice.Configuration(address, _clockRateKHz);
            _device = new I2CDevice(_config);
        }

        public void I2CWrite(int Address, byte data)
        {
            var xActions = new I2CDevice.I2CTransaction[1];
            xActions[0] = I2CDevice.CreateWriteTransaction(new byte[] { (byte)(Address >> 8), (byte)(Address & 0xFF), data });
            _device.Execute(xActions, _transactionTimeout);
            Thread.Sleep(5); // Mandatory after each Write transaction !!!
        }

        public byte I2CRead(int Address)
        {
            var Data = new byte[1];
            var xActions = new I2CDevice.I2CTransaction[1];
            xActions[0] = I2CDevice.CreateWriteTransaction(new byte[] { (byte)(Address >> 8), (byte)(Address & 0xFF) });
            _device.Execute(xActions, _transactionTimeout);
            Thread.Sleep(5);   // Mandatory after each Write transaction !!!
            xActions[0] = I2CDevice.CreateReadTransaction(Data);
            _device.Execute(xActions, _transactionTimeout);
            return Data[0];
        }
    }
}
