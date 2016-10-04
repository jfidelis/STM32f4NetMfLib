using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using STM32f4NetMfLib;
using MicroLiquidCrystal;
using System.Threading;

namespace Pcf8574LCD1602Test
{
    public class Program
    {
        private static bool flag;
        //private static readonly DS1307 Ds1307 = new DS1307();

        public static void Main()
        {

            //DateTime dt = Ds1307.GetDateTime();
            //Utility.SetLocalTime(dt);

            System.Threading.Timer tmrClock = new System.Threading.Timer(new TimerCallback(OnClock), null, 5000, 1000);

            using (I2CDevice device = new I2CDevice(null))
            {
                Pcf8574 provider = new Pcf8574(device);
                Lcd lcd = new Lcd(provider);

                lcd.Begin(16, 2);

                lcd.Clear();

                Thread.Sleep(2000);

                lcd.Write("STM32F4NetMFLib");
                lcd.SetCursorPosition(0, 1);
                lcd.Write("Bom Dia...");

                while (true)
                {

                    if (flag == true)
                    {
                        flag = false;

                        DateTime now = DateTime.Now;
                        string line1 = "Data: " + now.ToString("dd-MM-yyyy");
                        string line2 = "Hora: " + now.ToString("HH:mm:ss");
                        lcd.Clear();
                        lcd.Write(line1);
                        lcd.SetCursorPosition(0, 1);
                        lcd.Write(line2);
                    }
                }
            }
        }

        private static void OnClock(object state)
        {
            flag = true;
        }
    }
}
