using System.Collections.Generic;

namespace application
{
    public class AuthResult
    {
        public string Token { get; set; }
        public bool Seccess { get; set; }
        public List<string> Errors { get; set; }
    }
}
