﻿namespace TravelManagement.Models.DTO
{
    public class CancelBookingDTO
    {
        public int BookingId { get; set; }
        public required DateOnly selectedDate { get; set; }
        public required string Type { get; set; }
    }
}
