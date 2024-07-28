
using System.Text.Json.Serialization;

namespace BaseLibrary.Entities
{
    public class Branch : BaseEntity
    {
        // Many Branch to one Department
        public Department? Department { get; set; }
        public int DepartmentId { get; set; }
        [JsonIgnore]
        public List<Employee>? Employees { get; set; }
    }
}
