using System;
using Microsoft.SPOT;

namespace STM32f4NetMfLib.LCDili9341.HelpersFonts
{
    public abstract class Font
    {
        public abstract byte SpaceWidth { get; }
        public abstract FontCharacter GetFontData(char character);
    }
}
