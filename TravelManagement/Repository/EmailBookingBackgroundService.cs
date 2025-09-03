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

        public EmailBookingBackgroundService(
            IServiceProvider serviceProvider,
            IOptions<EmailSettings> options)
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
                        var bookingRepo = scope.ServiceProvider.GetRequiredService<BookingRepository>();
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        var bookings = await ReadBookingsFromEmailAsync();
                        foreach (var bookingDto in bookings)
                        {
                            var newBooking = await MapToNewBookingDTOAsync(bookingDto, bookingRepo, dbContext);
                            if (newBooking != null)
                            {
                                await bookingRepo.CreateBooking(newBooking);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception (use your logger or Console)
                    Console.WriteLine($"[EmailBookingBackgroundService] Error: {ex}");
                }

                await Task.Delay(_settings.PollIntervalSeconds * 1000, stoppingToken);
            }
        }

        private async Task<List<BookingEmailDto>> ReadBookingsFromEmailAsync()
        {
            var bookings = new List<BookingEmailDto>();
            using var client = new ImapClient();
            await client.ConnectAsync(_settings.Host, _settings.Port, _settings.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);

            var inbox = client.Inbox;
            await inbox.OpenAsync(MailKit.FolderAccess.ReadWrite);

            var query = SearchQuery.NotSeen;
            var uids = await inbox.SearchAsync(query);
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
                //var fromAddress = message.From.Mailboxes.FirstOrDefault()?.Address;
                //if (!string.Equals(fromAddress, "ezygoataxiservices@gmail.com", StringComparison.OrdinalIgnoreCase))
                //{
                //    continue;
                //}
                var booking = ParseBookingEmail(message.TextBody);
                if (booking != null)
                {
                    bookings.Add(booking);
                    if (!string.IsNullOrEmpty(_settings.ProcessedFolder))
                    {
                       //v    ed = client.GetFolder(_settings.ProcessedFolder);
                        var processed = await GetOrCreateFolder(client, _settings.ProcessedFolder);
                        //if (!processed.IsOpen || processed.Access != FolderAccess.ReadWrite)
                        //{
                        //    await processed.OpenAsync(FolderAccess.ReadWrite);
                        //}
                        try
                        {
                            await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error moving message UID {uid}: {ex.Message}");
                        }
                    }
                    else
                    {
                        await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true);
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
                if (f.FullName.Equals(folderName, StringComparison.OrdinalIgnoreCase))
                {
                    return f; // Don't open
                }
            }

            // Agar exist nahi karta toh create
            return await personal.CreateAsync(folderName, true);
        }

        private BookingEmailDto? ParseBookingEmail(string body)
        {
            if (string.IsNullOrWhiteSpace(body)) return null;
            var dto = new BookingEmailDto();
            dto.CustomerName = GetValue(body, "Name:");
            dto.CustomerNumber = GetValue(body, "Phone Number:");
            dto.TravelDate = GetValue(body, "Pickup Date:");
            dto.Pax = int.TryParse(GetValue(body, "No. of Passenger:"), out var pax) ? pax : null;
            dto.VehicleName = GetValue(body, "Select your Car*:");
            dto.From = GetValue(body, "Select Pick Up Spot:");
            dto.To = GetValue(body, "Select Drop Off Spot:");
            return dto;
        }

        private string? GetValue(string body, string key)
        {
            var lines = body.Split('\n');
            foreach (var line in lines)
            {
                if (line.Trim().StartsWith(key))
                    return line.Replace(key, "").Trim();
            }
            return null;
        }

        // Map BookingEmailDto to NewBookiingDTO for repository
        private async Task<NewBookiingDTO?> MapToNewBookingDTOAsync(BookingEmailDto dto, BookingRepository repo, AppDbContext dbContext)
        {
            if (dto == null) return null;

            // Lookup VehicleId by VehicleName (implement this as per your DB)
            int vehicleId = await GetVehicleIdByNameAsync(dto.VehicleName, dbContext);


            // Parse TravelDate
            DateOnly bookingDate = DateOnly.FromDateTime(DateTime.Today);
            if (!string.IsNullOrWhiteSpace(dto.TravelDate))
            {
                DateTime dt;
                if (DateTime.TryParse(dto.TravelDate, out dt))
                    bookingDate = DateOnly.FromDateTime(dt);
            }
            // You may need to fetch VehicleId, UserId, etc. from DB based on names/numbers
            // For demo, set dummy values or implement lookup as needed
            return new NewBookiingDTO
            {
                CustomerName = dto.CustomerName,
                CustomerNumber = dto.CustomerNumber,
                From = dto.From,
                To = dto.To,
                Pax = dto.Pax ?? 1,
                BookingDate = DateOnly.Parse(dto.TravelDate ?? DateTime.Today.ToString("yyyy-MM-dd")),
                BookingTime = TimeOnly.FromDateTime(DateTime.Now),
                VehicleId = vehicleId,
                BookingType = "Notspecified",
                BookingStatus = "Pending",
                Amount = 0, // Set as per your logic
                Payment = "Admin",
                UserId = 2 // Set as per your logic
            };
        }

        private async Task<int> GetVehicleIdByNameAsync(string? vehicleName, AppDbContext dbContext)
        {
            if (string.IsNullOrWhiteSpace(vehicleName))
                return 1;

            var vehicle = await dbContext.Vehicles.FirstOrDefaultAsync(v => v.VehicleName == vehicleName);
            return vehicle?.VehicleId ?? 1;
        }
    }
}