using System;
using Microsoft.SPOT;
using STM32f4NetMfLib;

namespace EEPROMAT24C32Test
{
    public class Program
    {
        public static void Main()
        {
            EepromAT24C32 eeprom = new EepromAT24C32(EepromAT24C32.CONST_DEVICE_ADDRESS1);

            eeprom.I2CWrite(0, (byte)'P');
            eeprom.I2CWrite(1, (byte)'e');
            eeprom.I2CWrite(2, (byte)'d');
            eeprom.I2CWrite(3, (byte)'r');
            eeprom.I2CWrite(4, (byte)'o');

            for(int i = 0; i < 5; i++)
            {
                byte r = eeprom.I2CRead(i);
                Debug.Print(((char)r).ToString());
            }

        }
    }
}
