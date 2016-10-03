using System;
using MicroLiquidCrystal;
using Microsoft.SPOT.Hardware;

namespace STM32f4NetMfLib
{
    public class Pcf8574 : ILcdTransferProvider
    {
        private readonly I2CDevice _device;
        private readonly I2CDevice.Configuration _config;
        private readonly I2CDevice.I2CTransaction[] _transactions;

        public Pcf8574(I2CDevice dev, ushort address = 0x27)
        {
            _device = dev;
            _config = new I2CDevice.Configuration(address, 100);

            I2CDevice.I2CTransaction transaction = I2CDevice.CreateWriteTransaction(new byte[1]);
            _transactions = new[] { transaction };
        }

        public bool FourBitMode
        {
            get
            {
                return true;
            }
        }

        public void Send(byte data, bool mode, bool backlight)
        {
            int send = data & 0xf0;

            if (backlight)
            {
                // P3
                send |= 0x08;
            }

            if (mode)
            {
                // P0
                send |= 0x01; 
            }

            // E = 1
            Send((byte)(send | 0x04));
            // E = 0
            Send((byte)send);

            // Lo 4 bits + backlight + mode
            send = (data << 4) | (send & 0x0f);
            // E = 1
            Send((byte)(send | 0x04));
            // E = 0
            Send((byte)send);
        }

        private void Send(byte data)
        {
            lock (_device)
            {
                _device.Config = _config;
                _transactions[0].Buffer[0] = data;
                _device.Execute(_transactions, 100);
            }
        }
    }
}
