using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SUShirts.Configuration;
using SUShirts.Data.Entities;
using SUShirts.Data.Enums;

namespace SUShirts.Business.Services
{
    public class EmailService : IEmailService
    {
        #region Message Bodies

        private const string UserMailBodyHtml = @"
<!DOCTYPE html>
<html lang=""cs"">
<head>
    <title>SU trička – rezervace</title>
    <meta charset=""utf-8""/>
    <style>
        ul {{
            list-style: none;
            padding-left: 1em;
        }}

        ul li::before {{
            content: ""■"";
            color: black;
            font-weight: bold;
            display: inline-block; 
            width: 1.4em;
        }}

        h2    {{ margin-bottom: 1em; }}
        .bold {{ font-weight: bold; }}
        .ctr  {{ text-align: center; }}
        {3}
    </style>
</head>
<body>
<h2>Rezervace triček</h2>
<p>
    Ahoj,<br/><br/>
    zarezervoval/a sis následující trička z&nbsp;nabídky SU FIT: 
</p>
<ul>
    {1}
</ul>
<p>
    Brzy tě bude kontaktovat někdo z SU a domluvíte se spolu na způsobu platby a&nbsp;předání.
    Předběžně počítej s&nbsp;tím, že předání proběhne na nejbližší otvíračce v&nbsp;Kachně.
</p>
<p>
    Připrav si celkem <span class=""bold"">{2} Kč</span>. Pokud jsi členem SU (pokud jsi zaregistrovaný/á v&nbsp;Kachně, jsi sympatizujícím členem SU), po dohodě budeš moci zaplatit i&nbsp;převodem na účet.
</p>
<p>
    Děkujeme a měj se krásně!
</p>
<div class=""ctr"">    
    <img src=""cid:{0}"" alt=""Studentská unie FIT VUT v Brně"" height=""64"" />
</div>
</body>
</html>
";

        #endregion

        private readonly IOptionsMonitor<MessageOptions> _messageOptions;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptionsMonitor<MessageOptions> messageOptions, IWebHostEnvironment environment,
            ILogger<EmailService> logger)
        {
            _messageOptions = messageOptions;
            _environment = environment;
            _logger = logger;
        }

        public async Task SendReservationEmailToUser(Reservation reservation)
        {
            if (!this.IsSmtpEnabled())
                return;

            if (!MailAddress.TryCreate(reservation.Email, out var toAddress))
                return;

            _logger.LogInformation("Sending reservation e-mail to a user.");

            using var client = this.MakeSmtpClient();
            var sender = new MailAddress(_messageOptions.CurrentValue.FromAddress, "SU FIT");

            var msg = new MailMessage(sender, toAddress)
            {
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                Subject = "SU FIT – rezervace triček",
            };

            try
            {
                this.FillUserMessage(msg, reservation);
                await client.SendMailAsync(msg).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot send e-mail to {Address}.", reservation.Email);
            }
        }

        public async Task SendReservationEmailToManager(string reservationUrl)
        {
            if (!this.IsSmtpEnabled())
                return;

            if (!MailAddress.TryCreate(_messageOptions.CurrentValue.ReservationManagerAddress, out var toAddress))
                return;

            _logger.LogInformation("Sending reservation e-mail to the manager.");

            using var client = this.MakeSmtpClient();
            var msg = new MailMessage(new MailAddress(_messageOptions.CurrentValue.FromAddress), toAddress)
            {
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                Subject = "SU FIT – nová rezervace triček",
                Body =
                    $"<h2>Nová rezervace triček</h2><p>Detaily najdeš na <a href=\"{reservationUrl}\">{reservationUrl}</a>.</p>"
            };

            try
            {
                await client.SendMailAsync(msg).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot send e-mail to {Address}.", toAddress.Address);
            }
        }

        private void FillUserMessage(MailMessage message, Reservation reservation)
        {
            var logoPath = Path.Combine(_environment.WebRootPath, "images", "logo_mail.png");

            var attachment = new Attachment(logoPath, "image/png") {ContentId = Guid.NewGuid().ToString()};
            attachment.ContentDisposition.Inline = true;

            var colors = reservation.Items.Select(i => i.ShirtVariant.Shirt.PrimaryColor).Distinct();

            var sb = new StringBuilder();
            foreach (var color in colors)
            {
                if (color is null) continue;
                sb.AppendLine($".col-{color.Id}::before {{ color: #{color.Hex}; }}");
            }

            var styles = sb.ToString();
            sb.Clear();

            var total = 0;
            foreach (var item in reservation.Items)
            {
                total += item.Price * item.Count;
                sb.Append(
                    $"<li class=\"col-{item.ShirtVariant.Shirt.PrimaryColor.Id}\"> {item.ShirtVariant.Shirt.Name}, vel. {item.ShirtVariant.Size.ToString()}, {(item.ShirtVariant.Sex switch {SexVariant.Man => "pánské", SexVariant.Woman => "dámské", SexVariant.Unisex => "unisex"})}, {item.Count} ks – {item.Price * item.Count} Kč</li>");
            }

            var itemsString = sb.ToString();
            var htmlBody = string.Format(UserMailBodyHtml, attachment.ContentId, itemsString, total, styles);
            message.Attachments.Add(attachment);
            message.Body = htmlBody;
        }

        private bool IsSmtpEnabled()
        {
            var options = _messageOptions.CurrentValue;
            return !string.IsNullOrEmpty(options.SmtpServer) &&
                   !string.IsNullOrEmpty(options.SmtpUsername) &&
                   !string.IsNullOrEmpty(options.SmtpPassword) &&
                   !string.IsNullOrEmpty(options.FromAddress) &&
                   MailAddress.TryCreate(options.FromAddress, out _) &&
                   (options.SmtpPort is > 0 and <= 65535);
        }

        private SmtpClient MakeSmtpClient()
        {
            var options = _messageOptions.CurrentValue;
            return new SmtpClient(options.SmtpServer, options.SmtpPort)
            {
                EnableSsl = options.UseSsl,
                Credentials = new NetworkCredential(options.SmtpUsername, options.SmtpPassword),
            };
        }
    }
}
