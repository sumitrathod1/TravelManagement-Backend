using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelManagement.AppDBContext;
using TravelManagement.Models;
using TravelManagement.Models.DTO;

namespace TravelManagement.Repository
{
    public class EmailBookingBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly EmailSettings _settings;
        public EmailBookingBackgroundService(IServiceProvider serviceProvider, IOptions<EmailSettings> options)
        {
            _serviceProvider = serviceProvider;
            _settings = options.Value;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var bookings = await ReadBookingsFromEmailAsync();
                        foreach (var bookingDto in bookings)
                        {
                            if (bookingDto != null)
                            {
                                var inquiry = new EmailInquiry
                                {
                                    CustomerName = bookingDto.CustomerName ?? "",
                                    CustomerNumber = bookingDto.CustomerNumber,
                                    From = bookingDto.From,
                                    To = bookingDto.To,
                                    TravelDate = !string.IsNullOrWhiteSpace(bookingDto.TravelDate) ? DateOnly.FromDateTime(DateTime.Parse(bookingDto.TravelDate)) : null,
                                    Pax = bookingDto.Pax ?? 1,
                                    VehicleName = bookingDto.VehicleName
                                };
                                await dbContext.EmailInquiries.AddAsync(inquiry);
                            }
                        }
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[EmailBookingBackgroundService] Error: {ex}");
                }
                await Task.Delay(_settings.PollIntervalSeconds * 1000, stoppingToken);
            }
        }
        private async Task<List<BookingEmailDto>> ReadBookingsFromEmailAsync()
        {
            var bookings = new List<BookingEmailDto>();
            using
            var client = new ImapClient();
            await client.ConnectAsync(_settings.Host, _settings.Port, _settings.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);
            var inbox = client.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadWrite);

            var uids = await inbox.SearchAsync(SearchQuery.NotSeen);
            var summaries = await inbox.FetchAsync(uids, MessageSummaryItems.Envelope);
            var latestSenderUids = summaries
            .Where(s =>
                s.Envelope?.From?.Mailboxes.Any(mb =>
                    string.Equals(mb.Address, "ezygoataxiservices@gmail.com", StringComparison.OrdinalIgnoreCase)) == true)
            .OrderByDescending(s => s.Envelope.Date?.DateTime ?? DateTime.MinValue)
            .Take(10)
            .Select(s => s.UniqueId)
            .ToList();
            foreach (var uid in latestSenderUids)
            {
                var message = await inbox.GetMessageAsync(uid);
                var booking = ParseBookingEmail(message.TextBody);
                if (booking != null)
                {

                    bookings.Add(booking);

                    await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true);
                    if (!string.IsNullOrEmpty(_settings.ProcessedFolder))
                    {
                        var processed = await GetOrCreateFolder(client, _settings.ProcessedFolder);
                    }
                }
            }
            await client.DisconnectAsync(true);
            return bookings;
        }

        private async Task<IMailFolder> GetOrCreateFolder(ImapClient client, string folderName)
        {
            var personal = client.GetFolder(client.PersonalNamespaces[0]);
            foreach (var f in await personal.GetSubfoldersAsync(false))
            {
                if (f.FullName.Equals(folderName, StringComparison.OrdinalIgnoreCase)) return f;
            }
            return await personal.CreateAsync(folderName, true);
        }

        private BookingEmailDto? ParseBookingEmail(string body)
        {
            if (string.IsNullOrWhiteSpace(body)) return null;
            var dto = new BookingEmailDto
            {
                CustomerName = GetValue(body, "Name:"),
                CustomerNumber = GetValue(body, "Phone Number:"),
                TravelDate = GetValue(body, "Pickup Date:"),
                Pax = int.TryParse(GetValue(body, "No. of Passenger:"), out
              var pax) ? pax : null,
                VehicleName = GetValue(body, "Select your Car*:"),
                From = GetValue(body, "Select Pick Up Spot:"),
                To = GetValue(body, "Select Drop Off Spot:")
            };
            return dto;
        }
        private string? GetValue(string body, string key)
        {
            var lines = body.Split('\n');
            foreach (var line in lines)
            {
                if (line.Trim().StartsWith(key)) return line.Replace(key, "").Trim();
            }
            return null;
        }
    }
}