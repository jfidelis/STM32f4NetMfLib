using System;
using Microsoft.SPOT;
using STM32f4NetMfLib;
using static Common.Stm32F4Discovery;
using System.Threading;
using Microsoft.SPOT.Hardware;
using MicroLiquidCrystal;

namespace Esp8266WifiTest
{
    public class Program
    {
        private static Esp8266Wifi esp8266;
        private static string lcdLine1, lcdLine2;
        private static bool refleshLCD = false;

        public static void Main()
        {
            //Debug.Print(Resources.GetString(Resources.StringResources.String1));

            /* Pins Configu 
            *  ESP_RX  => PD5
            *  ESP_TX  => PD6
            */

            esp8266 = new Esp8266Wifi(SerialPorts.COM4, BaudRates.Baud115200, 0, 0);
            esp8266.CommandResult += Esp8266_CommandResult;
            //esp8266.SendAtData("AT+RST\r\n");

            Thread.Sleep(5000);

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


            using (I2CDevice device = new I2CDevice(null))
            {
                Pcf8574 provider = new Pcf8574(device);
                Lcd lcd = new Lcd(provider);

                lcdLine1 = "STM32F4NetMFLib";
                lcdLine2 = "ESP8266Demo";
                refleshLCD = true;

                while (true)
                {
                    if (refleshLCD)
                    {
                        refleshLCD = false;
                        lcd.Begin(16, 2);

                        lcd.Clear();

                        lcd.Write(lcdLine1);
                        lcd.SetCursorPosition(0, 1);
                        lcd.Write(lcdLine2);
                    }
                }
            }

        }

        private static void Esp8266_CommandResult(Esp8266WifiResponse response)
        {
            if(response.status == EspCommandStatus.Error)
            {
                Debug.Print("Erro no comando: " + response.command);
            }
            else
            {
                switch (response.command)
                {
                    case EspCommandType.SetReset:
                        break;
                    case EspCommandType.SetEchoType:
                        break;
                    case EspCommandType.SetWifiMode:
                        break;
                    case EspCommandType.GetVersion:
                        lcdLine2 = response.result[0].ToString();
                        refleshLCD = true;
                        break;
                    case EspCommandType.Ping:
                        break;
                    default:
                        break;
                }

                Debug.Print("Comando Processado: " + response.command);
            }
        }

        private static void kb_OnKeyUp(uint KeyCode, uint Unused, DateTime time)
        {
            Debug.Print("Key released: " + KeyCode.ToString());

            if(KeyCode == 1)
            {
                esp8266.SendPing();
            }

            if(KeyCode == 3)
            {
                esp8266.SendReset();
            }

            if(KeyCode == 4)
            {
                esp8266.SetMode(Esp8266Wifi.WifiModeType.StationAndSoftAP);
            }

            if(KeyCode == 5)
            {
                esp8266.SetEchoMode(Esp8266Wifi.EchoType.On);
            }

            if(KeyCode == 6)
            {
                esp8266.SetEchoMode(Esp8266Wifi.EchoType.Off);
            }

            if(KeyCode == 7)
            {
                esp8266.GetVersion();
            }
        }

        private static void kb_OnKeyDown(uint KeyCode, uint Unused, DateTime time)
        {
            Debug.Print("Key pressed: " + KeyCode.ToString());
        }
    }
}
