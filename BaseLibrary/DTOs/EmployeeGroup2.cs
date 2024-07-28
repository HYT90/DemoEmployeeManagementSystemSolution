using System.ComponentModel.DataAnnotations;


namespace BaseLibrary.DTOs
{
    public class EmployeeGroup2
    {
        [Required]
        public string JobName { get; set; } = string.Empty;
        [Required, Range(1, 99999, ErrorMessage = "Branch is invalid")]
        public int BranchId { get; set; }
        [Required, Range(1, 99999, ErrorMessage = "Town is invalid")]
        public int TownId { get; set; }
        [Required]
        public string? Other {  get; set; }
    }
}
