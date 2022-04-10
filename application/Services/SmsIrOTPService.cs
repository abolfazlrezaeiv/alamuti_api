using application.Interfaces;
using SmsIrRestfulNetCore;
using System.Collections.Generic;

namespace application.Services
{
    public class SmsIrOTPService : IOTPSevice
    {

        public void SendMessage(string phonenumber, int code)
        {
            Token tk = new();
            string token = tk.GetToken("");

            var ultraFastSend = new UltraFastSend()
            {
                Mobile = long.Parse(phonenumber),
                TemplateId = 62091,
                ParameterArray = new List<UltraFastParameters>()
    {
        new UltraFastParameters()
        {
            Parameter = "VerificationCode" , ParameterValue = code.ToString()
        },
    }.ToArray()

            };
            new UltraFast().Send(token, ultraFastSend);
        }
    }
}
