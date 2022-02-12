

using application.Interfaces;
using SmsIrRestfulNetCore;
using System.Collections.Generic;

namespace application.Services
{
    public class SmsIrOTPService : IOTPSevice
    {

        public async void SendMessage(string phonenumber, int code)
        {
            Token tk = new();
            string token = tk.GetToken("8449d52edcd7eab89db7d531", "it66)%#teBC!@*&");



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
