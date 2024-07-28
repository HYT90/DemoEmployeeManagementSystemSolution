
using System.Text.Json.Serialization;

namespace BaseLibrary.Entities
{
    public class GeneralDepartment : BaseEntity
    {
        // One to many
        [JsonIgnore]
        public List<Department>? Departments { get; set; }
    }
}
