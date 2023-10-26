namespace DocUploader.Server.EmailServiceManager.Services
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
