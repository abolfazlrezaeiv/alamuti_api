namespace API
{
    public class JwtConfig
    {

        public string Secret { get; set; } = "default secret jwtconfig";


        public TimeSpan ExpiryTimeFrame { get; internal set; }
    }
}
