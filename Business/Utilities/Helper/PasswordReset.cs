
using System.Net;
using System.Net.Mail;

namespace Business.Utilities.Helper
{
    public static class PasswordReset
    {
        public static void PasswordResetSendEmail(string link)
        {
            MailMessage mail = new();
            SmtpClient smtp = new ("mail.teknohub.net"); 

            mail.From = new MailAddress("fcakiroglu@teknohub.net"); 
            mail.To.Add("f-cakiroglu@outlook.com");
            mail.Subject = $"www.bıdıbıdı.com::Password Reset";
            mail.Body = "<h2>Click the link below to reset password</h2><hr />";
            mail.Body = $"<a href='{link}'>Password Reset Link</a>";
            mail.IsBodyHtml = true;
            smtp.Port = 587;
            smtp.Credentials = new NetworkCredential("fcakiroglu@teknohub.net", "aşlsdkgjkf");
            smtp.Send(mail);
        }
    }
}
