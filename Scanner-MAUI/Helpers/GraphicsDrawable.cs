using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Text;
using Scanner_MAUI.Pages;
using System.ComponentModel;
using Font = Microsoft.Maui.Graphics.Font;
using System.Timers;

namespace Scanner_MAUI.Helpers
{
    public class GraphicsDrawable : IDrawable/*, INotifyPropertyChanged*/
    {
        private const int MaxSignalStrength = 100;
        private const int MinSignalStrength = 0;


        //store the signal strength in its own property
        public int SignalStrength { get; set; }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            //LinearGradientPaint linearGradientPaint = new LinearGradientPaint
            //{
            //    StartColor = Colors.Red,
            //    EndColor = Colors.Green,
            //    StartPoint = new Point(0, 1.3),
            //    EndPoint = new Point(1, 0)
            //};

            LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush();

            myLinearGradientBrush.StartPoint = new Point(0, 0);
            myLinearGradientBrush.EndPoint = new Point(1, 1);
            //myLinearGradientBrush.GradientStops.Add(
            //    new GradientStop(Colors.Yellow, (float)0.0));

            myLinearGradientBrush.GradientStops.Add(
                new GradientStop(Colors.Red, (float)0.1));

            myLinearGradientBrush.GradientStops.Add(
                new GradientStop(Colors.Orange, (float)0.235));

            myLinearGradientBrush.GradientStops.Add(
                new GradientStop(Colors.Green, (float)1.0));

            RectF linearRectangle = new RectF(10, 10, 400, 50);
            canvas.SetFillPaint(myLinearGradientBrush, linearRectangle);
            canvas.SetShadow(new SizeF(4, 4), 4, Colors.LightGray);
            canvas.FillRoundedRectangle(linearRectangle, 12);

            // Calculate the position and dimensions of the vertical line based on signal strength
            int signalStrength = SignalStrength;
            //int signalStrength = 3;
            float lineX = linearRectangle.Left + (linearRectangle.Width / MaxSignalStrength) * signalStrength;
            float lineY1 = linearRectangle.Top;
            float lineY2 = linearRectangle.Bottom;

            // Draw the vertical line
            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 4;
            canvas.DrawLine(lineX, lineY1, lineX, lineY2);
            

            // Draw the linear axis with number labels
            int numLabels = 11; // Number of labels to display
            float labelSpacing = linearRectangle.Width / (numLabels - 1);
            float tickHeight = 10; // Height of the tick marks
            //float lineY = linearRectangle.Bottom + tickHeight; // Y-coordinate of the line
            
            for (int i = 0; i < numLabels; i++)
            {
                float labelX = linearRectangle.Left + (labelSpacing * i);
                float labelY = linearRectangle.Bottom + 10;
                float width = 100;
                float height = 100;
                string labelText = (i * 10).ToString()+"%";

                canvas.StrokeColor = Colors.Black;
                canvas.StrokeSize = (float)1.5;
                canvas.DrawLine(labelX, linearRectangle.Bottom, labelX, linearRectangle.Bottom + tickHeight); // Draw tick marks

                // Draw a line connecting the tick marks
                if (i < numLabels -1)
                {
                   
                    float nextLabelX = linearRectangle.Left + (labelSpacing * (i + 1));
                    canvas.DrawLine(labelX, labelY, nextLabelX, labelY);

                    canvas.FontColor = Colors.Black;
                    canvas.FontSize = 12;
                    canvas.Font = Font.Default;
                  
                    canvas.DrawString(labelText, labelX, labelY, width, height, 0, 0);
                }
            }
        }
    }
}
