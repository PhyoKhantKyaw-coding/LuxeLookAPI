namespace LuxeLookAPI.Share
{
    using System.Net;
    using System.Net.Mail;

    public static class EmailHelper
    {
        private static readonly string FromEmail = "ddd420698@gmail.com"; // your email
        private static readonly string AppPassword = "ilma sfqt uhbm wubu"; // Gmail App Password

        private static bool SendEmail(string toEmail, string subject, string bodyHtml)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(FromEmail);
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = bodyHtml;
                mail.IsBodyHtml = true;

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(FromEmail, AppPassword),
                    EnableSsl = true
                };

                smtpClient.Send(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SendOrderSuccessEmail(string toEmail, string userName, Guid orderId)
        {
            string htmlBody = $@"
        <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; border-radius: 10px; max-width: 500px; margin: auto; background-color: #f9f9f9;'>
            <h2 style='color: #28a745; text-align: center;'>Order Placed Successfully!</h2>
            <p style='font-size: 16px; color: #333;'>Hello <strong>{userName}</strong>,</p>
            <p style='font-size: 16px; color: #333;'>Thank you for your order.</p>
            <p style='font-size: 16px; color: #333;'>Your order ID is <strong>{orderId}</strong>.</p>
            <p style='font-size: 14px; color: #666; text-align: center;'>We will notify you when your delivery is on the way.</p>
        </div>";

            return SendEmail(toEmail, "Order Successful - Retail", htmlBody);
        }

        public static bool SendDeliveryAccessEmail(string toEmail, string userName, Guid orderId)
        {
            string htmlBody = $@"
        <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; border-radius: 10px; max-width: 500px; margin: auto; background-color: #f9f9f9;'>
            <h2 style='color: #007bff; text-align: center;'>Your Order is on the Way</h2>
            <p style='font-size: 16px; color: #333;'>Hello <strong>{userName}</strong>,</p>
            <p style='font-size: 16px; color: #333;'>Good news! Your order <strong>{orderId}</strong> has been picked up by our delivery team.</p>
            <p style='font-size: 16px; color: #333;'>It will arrive within <strong>7 days</strong> at your destination.</p>
            <p style='font-size: 14px; color: #666; text-align: center;'>Thank you for shopping with us!</p>
        </div>";

            return SendEmail(toEmail, "Your Order is on the Way - Retail", htmlBody);
        }
        public static bool SendStockAlertEmail(string toEmail, string productName, int requestedQty, int availableQty)
        {
            string htmlBody = $@"
    <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; border-radius: 10px; max-width: 500px; margin: auto; background-color: #f9f9f9;'>
        <h2 style='color: #dc3545; text-align: center;'>Stock Alert</h2>
        <p style='font-size: 16px; color: #333;'>Dear Admin,</p>
        <p style='font-size: 16px; color: #333;'>There is not enough stock for the following product:</p>
        <ul>
            <li><strong>Product:</strong> {productName}</li>
            <li><strong>Requested Quantity:</strong> {requestedQty}</li>
            <li><strong>Available Quantity:</strong> {availableQty}</li>
        </ul>
        <p style='font-size: 14px; color: #666; text-align: center;'>Please restock this product as soon as possible.</p>
    </div>";

            return SendEmail(toEmail, "⚠️ Stock Alert - Retail", htmlBody);
        }


    }

}
