using System;
using Microsoft.SPOT;

namespace STM32f4NetMfLib.LCDili9341.HelpersFonts
{
    public struct FontCharacter
    {
        public byte[] Data { get; set; }
        public byte Width { get; set; }
        public byte Height { get; set; }
        public byte Space { get; set; }
    }
}
