
namespace Scanner_MAUI.Functions
{
    public class GraphicsDrawable : IDrawable
    {
        private const int MaxSignalStrength = 10;
        private const int MinSignalStrength = 1;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            LinearGradientPaint linearGradientPaint = new LinearGradientPaint
            {
                StartColor = Colors.Red,
                EndColor = Colors.Green,
                // StartPoint is already (0,0)
                EndPoint = new Point(1, 0)
            };

            RectF linearRectangle = new RectF(10, 10, 500, 60);
            canvas.SetFillPaint(linearGradientPaint, linearRectangle);
            canvas.SetShadow(new SizeF(4, 4), 4, Colors.Grey);
            canvas.FillRoundedRectangle(linearRectangle, 12);

            // Calculate the position and dimensions of the vertical line based on signal strength
            int signalStrength = GetSignalStrength();
            float lineX = linearRectangle.Left + (linearRectangle.Width / MaxSignalStrength) * signalStrength;
            float lineY1 = linearRectangle.Top;
            float lineY2 = linearRectangle.Bottom;

            // Draw the vertical line
            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 2;
            canvas.DrawLine(lineX, lineY1, lineX, lineY2);
        }

        private int GetSignalStrength()
        {
            // Determine the signal strength based on specific conditions

            int connectionStrength = 2; 
            if (connectionStrength >= 7 && connectionStrength <= 10)
            {
                return MaxSignalStrength-1;
            }
            else if (connectionStrength >= 4 && connectionStrength <= 6)
            {
                return (MaxSignalStrength + MinSignalStrength) / 2;
            }
            else
            {
                return MinSignalStrength;
            }
        }

        //private int GetConnectionStrength()
        //{
        //    // Replace this with your logic to retrieve the actual connection strength
        //    // Here, I'm just returning a random value between the minimum and maximum connection strength
        //    Random random = new Random();
        //    return random.Next(MinSignalStrength, MaxSignalStrength + 1);
        //}
    }
}
