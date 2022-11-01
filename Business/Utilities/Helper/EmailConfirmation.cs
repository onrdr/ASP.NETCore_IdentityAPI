
using System.Net;
using System.Net.Mail;

namespace Business.Utilities.Helper
{
    public static class EmailConfirmation
    {
        public static void SendEmailForConfirmation(string link, string email)
        {
            MailMessage mail = new();
            SmtpClient smtp = new("mail.teknohub.net");

            mail.From = new MailAddress("fcakiroglu@teknohub.net");
            mail.To.Add(email);
            mail.Subject = $"www.bıdıbıdı.com::Email Confirmation";
            mail.Body = "<h2>Click the link below to confirm your email</h2><hr />";
            mail.Body = $"<a href='{link}'>Email Confirmation Link</a>";
            mail.IsBodyHtml = true;
            smtp.Port = 587;
            smtp.Credentials = new NetworkCredential("fcakiroglu@teknohub.net", "aşlsdkgjkf");
            smtp.Send(mail);
        }
    }
}
