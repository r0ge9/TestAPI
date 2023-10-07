namespace TestAPI.Auth
{
    public interface IAPIUserRepo
    {
        public APIUserDto GetAPIUser(APIUser user);
    }
}
