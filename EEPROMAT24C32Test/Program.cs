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

            //eeprom.I2CClean();

            eeprom.I2CWrite(0, (byte)'T');
            eeprom.I2CWrite(1, (byte)'e');
            eeprom.I2CWrite(2, (byte)'s');
            eeprom.I2CWrite(3, (byte)'T');
            eeprom.I2CWrite(4, (byte)'e');

            for(int j = 0; j < 255; j++)
            {
                eeprom.I2CWrite(5 + j, (byte)j);
            }

            for(int i = 0; i < 5; i++)
            {
                byte r = eeprom.I2CRead(i);
                Debug.Print(((char)r).ToString());
            }

            byte[] sendBuff = new byte[100];
            byte[] readBuff = new byte[100];

            string data = "QWERTYUIOPASDFGHJKLÇ^ZXCVBNM<>1234567890";
            int p = 0;
            foreach (var c in data.ToCharArray())
            {
                sendBuff[p++] = (byte)c;
            }

            eeprom.I2CWriteArray(0, sendBuff);

            readBuff = eeprom.I2CReadArray(0, 100);

            ConfigHelpers config = new ConfigHelpers();

            byte[] ip = new byte[] { 192, 168, 0, 100 };
            bool isServer = true;
            string senha = "12345";

            config.SetConfig(new object[] { ip, isServer, senha });

        }
    }
}
