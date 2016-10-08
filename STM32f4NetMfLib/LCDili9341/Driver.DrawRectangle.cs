namespace STM32f4NetMfLib.LCDili9341
{
    public partial class Driver
    {
        public void DrawRectangle(int x, int y, int width, int height, ushort color)
        {
            var x1 = (x + width);
            var y1 = (y + height);

            DrawVerticalLine(x, y, height, color); //left
            DrawVerticalLine(x1, y, height + 1, color); //right
            DrawHorizontalLine(x, y, width, color); //top
            DrawHorizontalLine(x, y1, width + 1, color); //bottom
        }
    }
}
