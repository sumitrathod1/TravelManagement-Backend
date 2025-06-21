using System.ComponentModel.DataAnnotations;

namespace TravelManagement.Models.DTO
{
    public class ChangePasswordDTO
    {
        public required int UserId{ get; set; }
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
