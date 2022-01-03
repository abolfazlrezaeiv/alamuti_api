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
        public byte[] Photo1 { get; set; }
        public bool Published { get; set; }
        public byte[] Photo2 { get; set; }
        public DateTime DatePosted { get ;  set; }
        public string UserId { get; set; }
        public string AdsType { get; set; }
        public string PhoneNumber { get; set; }
        public int? Area { get; set; }
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
                aValue.Add(0.75, () => "لحظاتی پیش ");
                aValue.Add(1.5, () => "دقایقی پیش");
                aValue.Add(45, () => string.Format( " {0} دقیقه " + "پیش", Math.Round(TotalMinutes)));
                aValue.Add(90, () => "1 ساعت پیش");
                aValue.Add(1440, () => string.Format( " {0} ساعت " + "پیش ", Math.Round(Math.Abs(oSpan.TotalHours))));
                aValue.Add(2880, () => "1 روز پیش"); 
                aValue.Add(43200, () => string.Format( " {0} روز " + "پیش ", Math.Floor(Math.Abs(oSpan.TotalDays)))); 
                aValue.Add(86400, () => "1 ماه پیش"); 
                aValue.Add(525600, () => string.Format( " {0} ماه " + "پیش ", Math.Floor(Math.Abs(oSpan.TotalDays / 30)))); 
                aValue.Add(1051200, () => "1 سال پیش"); 
                aValue.Add(double.MaxValue, () => string.Format( " {0} سال " + "پیش ", Math.Floor(Math.Abs(oSpan.TotalDays / 365))));

                persianDateForFlutter = aValue.First(n => TotalMinutes < n.Key).Value.Invoke() ;
             
            }
        }



    }
}
