using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace application.DTOs.Chat
{
    public class ChatMessageDto
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Message { get; set; }
        public string Reciever { get; set; }
        public DateTime DateSended { get; set; }
        public string GroupName { get; set; }

        private string persianDateForFlutter;

        public string DaySended
        {
            get
            {
                return persianDateForFlutter;
            }
            set
            {
                TimeSpan oSpan = DateTime.UtcNow.Subtract(DateSended);
                double TotalMinutes = oSpan.TotalMinutes;

                if (TotalMinutes < 0.0)
                {
                    TotalMinutes = Math.Abs(TotalMinutes);
                }

                var aValue = new SortedList<double, Func<string>>();
                aValue.Add(0.75, () => "لحظاتی پیش ");
                aValue.Add(1.5, () => "دقایقی پیش");
                aValue.Add(45, () => string.Format(" {0} دقیقه " + "پیش", Math.Round(TotalMinutes)));
                aValue.Add(90, () => "1 ساعت پیش");
                aValue.Add(1440, () => string.Format(" {0} ساعت " + "پیش ", Math.Round(Math.Abs(oSpan.TotalHours))));
                aValue.Add(2880, () => "1 روز پیش");
                aValue.Add(43200, () => string.Format(" {0} روز " + "پیش ", Math.Floor(Math.Abs(oSpan.TotalDays))));
                aValue.Add(86400, () => "1 ماه پیش");
                aValue.Add(525600, () => string.Format(" {0} ماه " + "پیش ", Math.Floor(Math.Abs(oSpan.TotalDays / 30))));
                aValue.Add(1051200, () => "1 سال پیش");
                aValue.Add(double.MaxValue, () => string.Format(" {0} سال " + "پیش ", Math.Floor(Math.Abs(oSpan.TotalDays / 365))));

                persianDateForFlutter = aValue.First(n => TotalMinutes < n.Key).Value.Invoke();

            }
        }
    }
}
