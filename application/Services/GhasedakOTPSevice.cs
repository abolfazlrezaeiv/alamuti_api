using application.Interfaces;
using RestSharp;

namespace application.Services
{
    public class GhasedakOTPService : IOTPSevice
    {
        public GhasedakOTPService()
        {

        }

        public async void SendMessage(string phonenumber , int code)
        {
            var client = new RestClient("https://api.ghasedak.me/v2/verification/send/simple");
            var request = new RestRequest();
            request.AddHeader("apikey", "");
            request.AddParameter("receptor", $"{phonenumber}");
            request.AddParameter("type", 1);
            request.AddParameter("template", "alamut2");
            request.AddParameter("param1", $"{code}");
            await client.PostAsync(request);
        }
    }
}
