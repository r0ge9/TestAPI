namespace TestAPI.Auth
{
    public class APIUserRepo : IAPIUserRepo
    {
        private List<APIUserDto> _users=>new()
        {
            new APIUserDto("Admin","123"),
        };

        public APIUserDto GetAPIUser(APIUser user)
        {
           return _users.FirstOrDefault(x =>
            string.Equals(x.username, user.UserName) &&
            string.Equals(x.password, user.Password)) ??
            throw new Exception();
        }
    }
}
