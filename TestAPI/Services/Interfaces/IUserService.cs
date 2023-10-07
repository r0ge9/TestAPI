using TestAPI.Model.Entities;
using TestAPI.Model.Enums;
using TestAPI.Model.Parameters;

namespace TestAPI.Services.Interfaces
{
    public interface IUserService
    {
        public Task<List<User>> GetUsers(UserParameter userParameter);

        public Task<User> GetUserById(int id);

        public Task<User> CreateUser(User user);

        public Task<User> UpdateUser(int id,User user);

        public Task<User> DeleteUser(int id);
        public Task<User> AddRoleForUser(int id, RoleName name);
    }
}
