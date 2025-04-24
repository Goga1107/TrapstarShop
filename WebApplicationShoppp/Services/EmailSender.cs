//using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using WebApplicationShoppp.Services;
using Microsoft.EntityFrameworkCore;
using WebApplicationShoppp.Models;
using WebApplicationShoppp.Data;

public class EmailSender : IEmailInterface
{
    private readonly IConfiguration _configuration;
    private readonly ProductDbContext _context;
    public EmailSender(IConfiguration configuration, ProductDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    public async Task<string> SendOTPEmailAsync(string email, string otp)
    {
        var username = _configuration["EmailSettings:Username"];
        var password = _configuration["EmailSettings:Password"];
        var fromEmail = _configuration["EmailSettings:From"];
        var host = _configuration["EmailSettings:Host"]; // Ensure this is set correctly

        using (var client = new SmtpClient(host))
        {
            client.Port = int.Parse(_configuration["EmailSettings:Port"]);
            client.Credentials = new NetworkCredential(username, password);
            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = "Your OTP Code",
                Body = $"Your OTP code is {otp}",
                IsBodyHtml = false,
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);

            return email;
        }
    }
    public async Task<string> SendOrderConfirmationEmailAsync(string email, Order order)
    {
        var username = _configuration["EmailSettings:Username"];
        var password = _configuration["EmailSettings:Password"];
        var fromEmail = _configuration["EmailSettings:From"];
        var host = _configuration["EmailSettings:Host"];

        using (var client = new SmtpClient(host))
        {
            client.Port = int.Parse(_configuration["EmailSettings:Port"]);
            client.Credentials = new NetworkCredential(username, password);
            client.EnableSsl = true;

            var product = await _context.Products.FindAsync(order.ProductId);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = "Your Order Confirmation",
                Body = $@"
                <h2>Order Confirmation</h2>
                <p>Thank you for your purchase!</p>
                <p><strong>Order ID:</strong> {order.OrderId}</p>
                <p><strong>Product:</strong> {product.Manufacturer} {product.Model}</p>
                <p><strong>Quantity:</strong> {order.Quantity}</p>
                <p><strong>Size:</strong> {order.Size}</p>
                <p><strong>Color:</strong> {order.Color}</p>
                <p><strong>Total Price:</strong> ${order.TotalPrice}</p>
                <p><strong>Purchase Date:</strong> {order.PurchaseDate.ToString("f")}</p>
                <p>We hope you enjoy your purchase! If you have any questions, feel free to contact us.</p>
            ",
                IsBodyHtml = true,  // Setting this to true allows sending HTML-formatted emails
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);

            return email;
        }
    }
    public async Task<string> SendOrderRefundEmailAsync(string email, Order order)
    {
        var username = _configuration["EmailSettings:Username"];
        var password = _configuration["EmailSettings:Password"];
        var fromEmail = _configuration["EmailSettings:From"];
        var host = _configuration["EmailSettings:Host"];

        using (var client = new SmtpClient(host))
        {
            client.Port = int.Parse(_configuration["EmailSettings:Port"]);
            client.Credentials = new NetworkCredential(username, password);
            client.EnableSsl = true;

            var product = await _context.Products.FindAsync(order.ProductId);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = "Your Order Has Been Refunded",
                Body = $@"
                <h2>Order Refunded</h2>
                <p>Thank you</p>
                <p><strong>Order ID:</strong> {order.OrderId}</p>
                <p><strong>Product:</strong> {product.Manufacturer} {product.Model}</p>
                <p><strong>Quantity:</strong> {order.Quantity}</p>
                <p><strong>Size:</strong> {order.Size}</p>
                <p><strong>Color:</strong> {order.Color}</p>
                <p><strong>Total Price:</strong> ${order.TotalPrice}</p>
                <p><strong>Purchase Date:</strong> {order.PurchaseDate.ToString("f")}</p>
                <p>We hope you enjoy our service! If you have any questions, feel free to contact us.</p>
            ",
                IsBodyHtml = true,  // Setting this to true allows sending HTML-formatted emails
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);

            return email;
        }
    }

}

// private readonly string _clientSecretPath = "client_secret_1071465992210-08fis97kf5jk954999fii77psbb4ui64.apps.googleusercontent.com.json"; // Update with the correct path to your JSON file

/*public async Task SendEmailAsync(string email, string subject, string message)
{
    UserCredential credential;

    using (var stream = new FileStream(_clientSecretPath, FileMode.Open, FileAccess.Read))
    {
        string tokenPath = "token.json";
        credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.Load(stream).Secrets,
            new[] { "https://www.googleapis.com/auth/gmail.send" },
            "user",
            CancellationToken.None,
            new FileDataStore(tokenPath, true));
    }

    var accessToken = await credential.GetAccessTokenForRequestAsync();

    var emailMessage = new MimeMessage();
    emailMessage.From.Add(new MailboxAddress("Your App", "prodgoga499@gmail.com"));
    emailMessage.To.Add(new MailboxAddress("", email));
    emailMessage.Subject = subject;
    emailMessage.Body = new TextPart("html") { Text = message };

    using (var client = new SmtpClient())
    {

        await client.ConnectAsync("smtp.gmail.com", 465, true); 
        await client.AuthenticateAsync("prodgoga499@gmail.com", accessToken); 
        await client.SendAsync(emailMessage);
        await client.DisconnectAsync(true);
    }*/

