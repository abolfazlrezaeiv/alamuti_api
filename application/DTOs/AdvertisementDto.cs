using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace application.DTOs
{
    public class AdvertisementDto
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public byte[] Photo { get; set; }

        public DateTime DatePosted { get ;  set; } 
        private string persianDateForFlutter;

        public string DaySended
        {
            get { 

                return persianDateForFlutter;
            }
            set {
                TimeSpan oSpan = DateTime.UtcNow.Subtract(DatePosted);
                double TotalMinutes = oSpan.TotalMinutes;

                if (TotalMinutes < 0.0)
                {
                    TotalMinutes = Math.Abs(TotalMinutes);
                }

                var aValue = new SortedList<double, Func<string>>();
                aValue.Add(0.75, () => "کمتراز یک دقیقه ");
                aValue.Add(1.5, () => " یک دقیقه");
                aValue.Add(45, () => string.Format("در " + "{0} دقیقه " + "پیش", Math.Round(TotalMinutes)));
                aValue.Add(90, () => "یک ساعت پیش");
                aValue.Add(1440, () => string.Format("در" + " {0} ساعت " + "پیش ", Math.Round(Math.Abs(oSpan.TotalHours))));
                aValue.Add(2880, () => "یک روز پیش"); 
                aValue.Add(43200, () => string.Format("در" + " {0} روز " + "پیش ", Math.Floor(Math.Abs(oSpan.TotalDays)))); 
                aValue.Add(86400, () => "یک ماه پیش"); 
                aValue.Add(525600, () => string.Format("در" + " {0} ماه " + "پیش ", Math.Floor(Math.Abs(oSpan.TotalDays / 30)))); 
                aValue.Add(1051200, () => "یک سال پیش"); 
                aValue.Add(double.MaxValue, () => string.Format("ثبت در" + " {0} سال " + "پیش ", Math.Floor(Math.Abs(oSpan.TotalDays / 365))));

                persianDateForFlutter = aValue.First(n => TotalMinutes < n.Key).Value.Invoke() ;
             
            }
        }



    }
}
