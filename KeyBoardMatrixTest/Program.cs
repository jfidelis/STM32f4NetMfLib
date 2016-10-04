using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using static Common.Stm32F4Discovery;
using STM32f4NetMfLib;
using System.Threading;

namespace KeyBoardMatrixTest
{
    public class Program
    {
        public static void Main()
        {

            // Row pins. The keypad exists out of 4 rows.
            // D0, D1, D2 e D3
            Cpu.Pin[] RowPins = { Pins.PD0, Pins.PD1, Pins.PD2, Pins.PD3 };

            // Col pins. The keypad exists out of 4 columns.
            // C1, C2, C4 e C5
            Cpu.Pin[] ColPins = { Pins.PC1, Pins.PC2, Pins.PC4, Pins.PC5 };

            // Initializes the new keypad
            KeyBoard kb = new KeyBoard(RowPins, ColPins);

            // Bind both events
            kb.OnKeyDown += new NativeEventHandler(kb_OnKeyDown);
            kb.OnKeyUp += new NativeEventHandler(kb_OnKeyUp);

            // Lets wait forever for events to occure
            Thread.Sleep(Timeout.Infinite);

        }

        private static void kb_OnKeyUp(uint KeyCode, uint Unused, DateTime time)
        {
            Debug.Print("Key released: " + KeyCode.ToString());
        }

        private static void kb_OnKeyDown(uint KeyCode, uint Unused, DateTime time)
        {
            Debug.Print("Key pressed: " + KeyCode.ToString());
        }
    }
}
