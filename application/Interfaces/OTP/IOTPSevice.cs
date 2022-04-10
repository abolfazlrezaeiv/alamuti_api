namespace application.Interfaces
{
    public interface IOTPSevice
    {
        void SendMessage(string phonenumber, int code);
    }
}
