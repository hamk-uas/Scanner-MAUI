using Scanner_MAUI.Pages;

namespace Scanner_MAUI.Helpers
{
    public class GraphicsDrawable : IDrawable
    {
        private const int MaxSignalStrength = 10;
        private const int MinSignalStrength = 1;

        //store the signal strength in its own property
        public int SignalStrength { get; set; }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            LinearGradientPaint linearGradientPaint = new LinearGradientPaint
            {
                StartColor = Colors.Red,
                EndColor = Colors.Green,
                EndPoint = new Point(1, 0)
            };

            RectF linearRectangle = new RectF(10, 10, 500, 60);
            canvas.SetFillPaint(linearGradientPaint, linearRectangle);
            canvas.SetShadow(new SizeF(4, 4), 4, Colors.Grey);
            canvas.FillRoundedRectangle(linearRectangle, 12);

            // Calculate the position and dimensions of the vertical line based on signal strength
            int signalStrength = SignalStrength;
            //int signalStrength = 3;
            float lineX = linearRectangle.Left + (linearRectangle.Width / MaxSignalStrength-1) * signalStrength;
            float lineY1 = linearRectangle.Top;
            float lineY2 = linearRectangle.Bottom;

            // Draw the vertical line
            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 2;
            canvas.DrawLine(lineX, lineY1, lineX, lineY2);
        }
    }
}
