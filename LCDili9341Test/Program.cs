using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using STM32f4NetMfLib;
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
        private static Font font = new StandardFixedWidthFont();

        static private char[] keyMap = new char[16]
        {
            '1','2','3','A','4','5','6','B','7','8','9','C','*','0','#','D'
        };

        static private string teclado;

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

            

            tftLcd.ClearScreen();
            tftLcd.DrawString(10, 10, "LCD ILI9341 WITH STM32F4_DISCOVERY", 0xF800, font);
            tftLcd.DrawString(10, 20, "DEMO USING .NET MF 4.2", 0xF800, font);
            tftLcd.DrawString(10, 30, "DATE AND TIME", 0x0000FF, font);
            tftLcd.BacklightOn = true;

            /* CONFIG PINS FOR KEYPAD MATRIX 4x4 */
            // Row pins. The keypad exists out of 4 rows.
            // D0, D1, D2 e D3
            Cpu.Pin[] RowPins = { Pins.PD0, Pins.PD1, Pins.PD2, Pins.PD3 };

            // Col pins. The keypad exists out of 4 columns.
            // C1, C2, C4 e C0
            Cpu.Pin[] ColPins = { Pins.PC1, Pins.PC2, Pins.PC4, Pins.PC0 };

            // Initializes the new keypad
            KeyBoard kb = new KeyBoard(RowPins, ColPins);

            // Bind both events
            kb.OnKeyDown += new NativeEventHandler(kb_OnKeyDown);
            kb.OnKeyUp += new NativeEventHandler(kb_OnKeyUp);

            System.Threading.Timer tmrClock = new System.Threading.Timer(new TimerCallback(OnClock), null, 1000, 1000);

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


                    string keystr = "Key: " + teclado;
                    //Font font = new HelpersFont(Verdana14.Bitmaps, Verdana14.Descriptors, Verdana14.Height, 5);
                    tftLcd.DrawString(50, 100, keystr, 0x00ff00, font);
                }
            }
        }

        private static void kb_OnKeyUp(uint KeyCode, uint Unused, DateTime time)
        {
            //string keystr = "Key pressed: " + KeyCode.ToString();
            //Font font = new HelpersFont(Verdana14.Bitmaps, Verdana14.Descriptors, Verdana14.Height, 0);
            //tftLcd.DrawString(50, 80, keystr, 0x00ff00, font);
        }

        private static void kb_OnKeyDown(uint KeyCode, uint Unused, DateTime time)
        {
            //tftLcd.ClearScreen();
            //string keystr = "Tecla: " + keyMap[KeyCode];
            //Font font = new HelpersFont(Verdana14.Bitmaps, Verdana14.Descriptors, Verdana14.Height, 5);
            //tftLcd.DrawString(50, 100, keystr, 0x00ff00, font);

            // * = ESC
            if (KeyCode == 12)
            {
                teclado = string.Empty;
                tftLcd.DrawString(50, 100, "                         ", 0x00ff00, font);
                return;
            }

            // # = ENTER
            if (KeyCode == 14)
            {
                return; 
            }

            teclado = teclado + keyMap[KeyCode];



        }

        private static void OnClock(object state)
        {
            flag = true;
        }
    }
}
