using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner_MAUI.Functions
{
    class TimeStamp
    {
        public static void TimeStampViewr(Label dateTimeLabel)
        {
            DateTime localDate = DateTime.Now;
            String[] cultureNames = { "fi-FI", "en-US" };

            //foreach (var cultureName in cultureNames)
            //{
            var culture = new CultureInfo(cultureNames[0]);
            string formattedDate = localDate.ToString(culture);
            dateTimeLabel.Text = $"Date & Time: {formattedDate}";
            //}
        }
    }
}
