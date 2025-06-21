﻿namespace TravelManagement.Models.DTO
{
    public class AddVehicleExpenceDTO
    {
        public DateTime ExpenseDate { get; set; }
        public decimal Amount { get; set; }
        public string CategoryType { get; set; } = "Repair";
        public int VehicleID { get; set; }
    }
}