using Microsoft.SPOT.Hardware;
using STM32f4NetMfLib;
using STM32f4NetMfLib.LCDili9341.HelpersFonts;
using System;
using System.Threading;
using static Common.Stm32F4Discovery;

namespace BasicDeviceOne
{
    public class Program
    {
        static private STM32f4NetMfLib.LCDili9341.Driver tftLcd;
        private static bool flag;
        private static Font font = new StandardFixedWidthFont();
        private const ushort COST_COLOR_RED = 0xF800;
        private const ushort COST_COLOR_BLUE = 0x0000FF;
        private static int Alt = 0;
        private static bool isCapsLock = false;

        static private char[] keyMapAlt = new char[16 * 4]
        {
           // Alt0 
            '1','2','3','A',
            '4','5','6','B',
            '7','8','9','C',
            '*','0','#','D',
           
           // Alt1
            'a','d','g',' ',
            'j','m','p',' ',
            's','v','y',' ',
            '*',' ','#',' ',
           
           // Alt2
            'b','e','h',' ',
            'k','n','q',' ',
            't','w','z',' ',
            '*',' ','#',' ',

           // Alt3
            'c','f','i',' ',
            'l','o','r',' ',
            'u','x',' ',' ',
            '*',' ','#',' '
        };

        static private string teclado;

        private static char ReturnKey(int pos)
        {
            char c = keyMapAlt[pos + (Alt * 16)];

            if ((isCapsLock) && (c > 0x60 && c < 0x7b))
            {
                c =  c.ToUpper();
            }

            return c;
        }

        private static void PrintKeyBoard()
        {
            int size_button = 30;

            tftLcd.DrawRectangle(100, 100, 110, 140, COST_COLOR_RED);

            // Coluna 1
            tftLcd.DrawRectangle(110, 110 + (size_button * 0), size_button, size_button, COST_COLOR_BLUE);
            tftLcd.DrawString(115, 115 + (size_button * 0), new string(new char[] { ReturnKey(0) }), 0x0000FF, font);

            tftLcd.DrawRectangle(110, 110 + (size_button * 1), size_button, size_button, COST_COLOR_BLUE);
            tftLcd.DrawString(115, 115 + (size_button * 1), new string(new char[] { ReturnKey(4) }), 0x0000FF, font);

            tftLcd.DrawRectangle(110, 110 + (size_button * 2), size_button, size_button, COST_COLOR_BLUE);
            tftLcd.DrawString(115, 115 + (size_button * 2), new string(new char[] { ReturnKey(8) }), 0x0000FF, font);

            tftLcd.DrawRectangle(110, 110 + (size_button * 3), size_button, size_button, COST_COLOR_BLUE);
            tftLcd.DrawString(115, 115 + (size_button * 3), new string(new char[] { ReturnKey(12) }), 0x0000FF, font);


            // Coluna 2
            tftLcd.DrawRectangle(110 + (size_button * 1), 110 + (size_button * 0), size_button, size_button, COST_COLOR_BLUE);
            tftLcd.DrawString(115 + (size_button * 1), 115 + (size_button * 0), new string(new char[] { ReturnKey(1) }), 0x0000FF, font);

            tftLcd.DrawRectangle(110 + (size_button * 1), 110 + (size_button * 1), size_button, size_button, COST_COLOR_BLUE);
            tftLcd.DrawString(115 + (size_button * 1), 115 + (size_button * 1), new string(new char[] { ReturnKey(5) }), 0x0000FF, font);

            tftLcd.DrawRectangle(110 + (size_button * 1), 110 + (size_button * 2), size_button, size_button, COST_COLOR_BLUE);
            tftLcd.DrawString(115 + (size_button * 1), 115 + (size_button * 2), new string(new char[] { ReturnKey(9) }), 0x0000FF, font);

            tftLcd.DrawRectangle(110 + (size_button * 1), 110 + (size_button * 3), size_button, size_button, COST_COLOR_BLUE);
            tftLcd.DrawString(115 + (size_button * 1), 115 + (size_button * 3), new string(new char[] { ReturnKey(13) }), 0x0000FF, font);


            // Coluna 3
            tftLcd.DrawRectangle(110 + (size_button * 2), 110 + (size_button * 0), size_button, size_button, COST_COLOR_BLUE);
            tftLcd.DrawString(115 + (size_button * 2), 115 + (size_button * 0), new string(new char[] { ReturnKey(2) }), 0x0000FF, font);

            tftLcd.DrawRectangle(110 + (size_button * 2), 110 + (size_button * 1), size_button, size_button, COST_COLOR_BLUE);
            tftLcd.DrawString(115 + (size_button * 2), 115 + (size_button * 1), new string(new char[] { ReturnKey(6) }), 0x0000FF, font);

            tftLcd.DrawRectangle(110 + (size_button * 2), 110 + (size_button * 2), size_button, size_button, COST_COLOR_BLUE);
            tftLcd.DrawString(115 + (size_button * 2), 115 + (size_button * 2), new string(new char[] { ReturnKey(10) }), 0x0000FF, font);

            tftLcd.DrawRectangle(110 + (size_button * 2), 110 + (size_button * 3), size_button, size_button, COST_COLOR_BLUE);
            tftLcd.DrawString(115 + (size_button * 2), 115 + (size_button * 3), new string(new char[] { ReturnKey(14) }), 0x0000FF, font);

        }

        public static void Main()
        {
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

            tftLcd.DrawRectangle(10, 40, 320, 40, 0xF800);
            PrintKeyBoard();

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

            System.Threading.Timer tmrClock = new System.Threading.Timer(new TimerCallback(OnClock), null, 500, 500);

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
                    tftLcd.DrawString(50, 90, keystr, 0x00ff00, font);
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
                teclado = "";
                tftLcd.DrawString(50, 90, "                         ", 0x00ff00, font);
                return;
            }

            // # = ENTER
            if (KeyCode == 14)
            {
                return;
            }

            if (KeyCode == 3) // A
            {
                Alt++;
            }
            if (KeyCode == 7) // B
            {
                Alt--;
            }
            if(KeyCode == 11) // C
            {
                isCapsLock = !isCapsLock;
            }

            if (Alt > 3)
                Alt = 0;

            if (Alt < 0)
                Alt = 3;

            if(KeyCode == 3 || KeyCode == 7 || KeyCode == 11)
            {
                PrintKeyBoard();
                return;
            }

            teclado = teclado + ReturnKey((int)KeyCode);

        }

        private static void OnClock(object state)
        {
            flag = true;
        }
    }
}
