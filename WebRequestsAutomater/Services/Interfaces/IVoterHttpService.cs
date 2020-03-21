namespace WebRequestsAutomater.Services.Interfaces
{
    public interface IVoterHttpService
    {
        void RunLogin(string username, string password, out string cookieToken);
        void RunLogout(string cookieToken);
        void VoteForProject(int uniqueValue, string cookieToken, out bool isSuccessful);
        void LikeArticles(int[] uniqueValues, string cookieToken, out bool isSuccesful);
    }
}
