
using System.Text.Json.Serialization;

namespace BaseLibrary.Entities
{
    public class Department : BaseEntity
    {
        // relationship Many Departments to one GeneralDepartment
        public GeneralDepartment? GeneralDepartment {  get; set; } 
        public int GeneralDepartmentId { get; set; }
        [JsonIgnore]
        public List<Branch>? Branches { get; set; }
    }
}
