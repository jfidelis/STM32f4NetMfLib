using STM32f4NetMfLib.LCDili9341.HelpersFonts;
using System;
using System.Threading;
using static Common.Stm32F4Discovery;

namespace LCDili9341Test
{
    public class Program
    {
        static private STM32f4NetMfLib.LCDili9341.Driver tftLcd;
        private static bool flag;

        public static void Main()
        {
            // STM32F4_SPI_SCLK PA5
            // STM32F4_SPI_MISO PA6
            // STM32F4_SPI_MOSI PA7
            tftLcd = new STM32f4NetMfLib.LCDili9341.Driver(
                                 isLandscape: true,
                                 lcdChipSelectPin: Pins.PD5,
                                 dataCommandPin: Pins.PD6,
                                 backlightPin: Pins.PD7);

            var font = new StandardFixedWidthFont();

            tftLcd.ClearScreen();
            tftLcd.DrawString(10, 10, "LCD ILI9341 WITH STM32F4_DISCOVERY", 0xF800, font);
            tftLcd.DrawString(10, 20, "DEMO USING .NET MF 4.2", 0xF800, font);
            tftLcd.DrawString(10, 30, "DATE AND TIME", 0x0000FF, font);
            tftLcd.BacklightOn = true;

            System.Threading.Timer tmrClock = new System.Threading.Timer(new TimerCallback(OnClock), null, 5000, 1000);

            while (true)
            {

                if (flag == true)
                {
                    flag = false;

                    DateTime now = DateTime.Now;
                    string line1 = "DATE: " + now.ToString("dd-MM-yyyy");
                    string line2 = "TIME: " + now.ToString("HH:mm:ss");

                    tftLcd.DrawString(50, 50, line1, 0x0000FF, font);
                    tftLcd.DrawString(50, 60, line2, 0x0000FF, font);
                }
            }
        }

        private static void OnClock(object state)
        {
            flag = true;
        }
    }
}
