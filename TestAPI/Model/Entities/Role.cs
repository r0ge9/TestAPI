using TestAPI.Model.Enums;

namespace TestAPI.Model.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public RoleName Name { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
