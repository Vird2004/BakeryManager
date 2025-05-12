using System.Net.Mail;
using System.Net;

namespace BakeryManager.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587) //cong khac la 465
            {
                EnableSsl = true, //bật bảo mật
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("grumpybakery04@gmail.com", "jgzohwvpvbwpqfmc")
            };

            return client.SendMailAsync(
                new MailMessage(from: "grumpybakery04@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}
