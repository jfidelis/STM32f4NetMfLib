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
        private const int _clockRateKHz = 400; // de 100 a 400
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

        public void I2CWrite(int address, byte data)
        {
            var xActions = new I2CDevice.I2CTransaction[1];
            xActions[0] = I2CDevice.CreateWriteTransaction(new byte[] { (byte)(address >> 8), (byte)(address & 0xFF), data });
            _device.Execute(xActions, _transactionTimeout);
            Thread.Sleep(5); // Mandatory after each Write transaction !!!
        }

        public byte I2CRead(int address)
        {
            var Data = new byte[1];
            var xActions = new I2CDevice.I2CTransaction[1];
            xActions[0] = I2CDevice.CreateWriteTransaction(new byte[] { (byte)(address >> 8), (byte)(address & 0xFF) });
            _device.Execute(xActions, _transactionTimeout);
            Thread.Sleep(5);   // Mandatory after each Write transaction !!!
            xActions[0] = I2CDevice.CreateReadTransaction(Data);
            _device.Execute(xActions, _transactionTimeout);
            return Data[0];
        }

        public void I2CWriteArray(int address, byte[] data)
        {
            if((address + data.Length) > CONST_MAX_MEM_SIZE)
            {
                throw new ArgumentException("data very large");
            }

            for(int i = 0; i < data.Length; i++)
            {
                I2CWrite(i + address, data[i]);
            }
        }

        public byte[] I2CReadArray(int address, int endAddress)
        {
            if(address > endAddress)
            {
                throw new ArgumentException("address not greater than endAddress");
            }

            int sizeArray = endAddress - address;

            if(sizeArray > CONST_MAX_MEM_SIZE || endAddress > CONST_MAX_MEM_SIZE)
            {
                throw new ArgumentException("endAddress or size very large");
            }

            byte[] res = new byte[sizeArray];
            int pos = address;

            for (int i = 0; i < sizeArray; i++)
            {
                res[i] = I2CRead(address + i);
            }

            return res;
        }

        public void I2CErase()
        {

        }

        public void I2CClean()
        {
            for(int i = 0; i < CONST_MAX_MEM_SIZE; i++)
                I2CWrite(i, 0xff);
        }
    }
}
