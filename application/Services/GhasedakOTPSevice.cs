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
            request.AddHeader("apikey", "47541ce5eed0c53032beda134b59199b7d7da859ef6c9c25de5b06f7a9de0798");
            request.AddParameter("receptor", $"{phonenumber}");
            request.AddParameter("type", 1);
            request.AddParameter("template", "alamut2");
            request.AddParameter("param1", $"{code}");
            await client.PostAsync(request);
        }
    }
}
