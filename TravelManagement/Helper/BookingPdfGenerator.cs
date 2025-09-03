using TravelManagement.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


namespace TravelManagement.Helper
{
    public class BookingPdfGenerator
    {
        public static byte[] Generate(List<Booking> bookings, string agentName)
        {
            var totalAmount = bookings.Sum(b => b.Amount);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text($"Booking Report for Agent: {agentName}")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium).AlignCenter();

                    page.Content().Column(col =>
                    {
                        
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40); 
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.ConstantColumn(40);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                string[] headers = { "NO", "Customer", "From", "To", "Type", "Date", "Vehicle", "Amount" };
                                foreach (var h in headers)
                                {
                                    header.Cell().Background(Colors.Grey.Lighten2).Border(1)
                                        .Padding(5).Text(h).SemiBold().FontSize(10).AlignCenter();
                                }
                            });

                            // ✅ Header row
                            //string[] headers = { "NO", "Customer", "From", "To", "Type", "Date", "Vehicle", "Amount" };
                            //foreach (var h in headers)
                            //{
                            //    table.Cell().Background(Colors.Grey.Lighten2).Border(1)
                            //        .Padding(5).Text(h).SemiBold().FontSize(10).AlignCenter();
                            //}

                            int rowIndex = 0;
                            int count = 1;
                            foreach (var b in bookings)
                            {
                                var bg = rowIndex % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;
                                table.Cell().Background(bg).Border(1).Padding(5).Text(count++.ToString());
                                table.Cell().Background(bg).Border(1).Padding(5).Text(b.Customer?.CustomerName ?? "N/A");
                                table.Cell().Background(bg).Border(1).Padding(5).Text(b.From.ToString());
                                table.Cell().Background(bg).Border(1).Padding(5).Text(b.To);
                                table.Cell().Background(bg).Border(1).Padding(5).Text(b.BookingType.ToString());
                                table.Cell().Background(bg).Border(1).Padding(5).Text(b.travelDate.ToString("dd-MM-yyyy"));
                                table.Cell().Background(bg).Border(1).Padding(5).AlignCenter().Text(b.Vehicle?.VehicleName ?? "N/A");
                                table.Cell().Background(bg).Border(1).Padding(5).AlignRight().Text("₹ " + b.Amount.ToString("N2"));

                                rowIndex++;
                            }

                            table.Cell().ColumnSpan(7).Border(1).Background(Colors.Grey.Lighten2).Padding(5).AlignRight()
                                .Text("Total Amount:").SemiBold();
                            table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).AlignRight()
                                .Text("₹ " + bookings.Sum(x => x.Amount).ToString("N2"))
                                .SemiBold().FontColor(Colors.Green.Darken2);
                        });
                    });
                    
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Generated on: " + DateTime.Now.ToString("dd-MM-yyyy hh:mm tt"));
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
