using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WebRequestsAutomater.Common;
using WebRequestsAutomater.Services.Interfaces;

namespace WebRequestsAutomater.Services.Implementations
{
    public class VoterHttpService : IVoterHttpService
    {
        private string CookieToken = string.Empty;

        private void GetIndexPage()
        {
            var preLoginFunctionContent = GetLoginAsync("TODO enter url");
            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: getting index page ended with result code: {preLoginFunctionContent.Result.Item2.StatusCode}");
        }

        private void RunInitialLogin()
        {
            var loginFunctionContent = GetLoginAsync("TODO enter url login");
            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: initializing login action ended with result code: {loginFunctionContent.Result.Item2.StatusCode}");

            foreach (var item in loginFunctionContent.Result.Item2.Headers)
            {
                foreach (var value in item.Value)
                {
                    if (item.Key == "Set-Cookie")
                    {
                        if (value.StartsWith("ci_session"))
                        {
                            CookieToken = value;
                        }
                    }
                }
            }

            var cookieValue = CookieToken.Split("=")[1].Split(";")[0];
            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: cookie for this session: {cookieValue}");
        }

        private void RunRealLogin(string username, string password)
        {
            var loginActionContent = PostLoginAsync("TODO enter url ajax/login", false, username, password, CookieToken);
            // TODO write this better
            var isLogged = loginActionContent.Result.Item1.Contains("alogowan");
            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: initializing login action ended with result: {isLogged}");
        }

        public void RunLogin(string username, string password, out string cookieToken)
        {
            GetIndexPage();
            RunInitialLogin();
            RunRealLogin(username, password);

            cookieToken = CookieToken;
        }

        public void RunLogout(string cookieToken)
        {
            var logoutFunctionContent = GetLogoutAsync("TODO enter url logout", CookieToken);
            var isLoggedOut = logoutFunctionContent.Result.Item1.Length > 76;
            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: logging out ended with result: {isLoggedOut}");
        }

        public void VoteForProject(int uniqueValue, string cookieToken, out bool isSuccessful)
        {
            var loginVoteActionContent = PostLoginAsync($"TODO enter url api/competitions/{uniqueValue}/likes/add", true, null, null, CookieToken);
            // TODO write this better
            var hasVoted = loginVoteActionContent.Result.Item1.Contains("odano prawid");
            Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: voting for project with ID {uniqueValue} ended with result: {hasVoted}");
            isSuccessful = hasVoted;
        }

        public void LikeArticles(int[] uniqueValues, string cookieToken, out bool isSuccessful)
        {
            var allOk = true;
            foreach (var uniqueValue in uniqueValues)
            {
                var loginVoteArt1ActionContent = PostLoginAsync($"TODO enter urlapi/news/{uniqueValue}/likes/add", true, null, null, CookieToken);
                // TODO write this better
                var hasLiked = loginVoteArt1ActionContent.Result.Item1.Contains("odano prawid");
                Console.WriteLine($"> {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}: liking article with ID {uniqueValue} ended with result: {hasLiked}");
                allOk = allOk && hasLiked;
            }

            isSuccessful = allOk;
        }

        public static async Task<Tuple<string, HttpResponseMessage>> GetLogoutAsync(string url, string cookieValue)
        {
            var httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(cookieValue))
            {
                httpClient.DefaultRequestHeaders.Add("Accept", "text/html, application/xhtml+xml, application/xml;q =0.9, */*; q=0.8");
                httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                httpClient.DefaultRequestHeaders.Add("Accept-Language", "pl-PL");
                httpClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                httpClient.DefaultRequestHeaders.Add("Cookie", cookieValue);
                httpClient.DefaultRequestHeaders.Add("DNT", "1");
                httpClient.DefaultRequestHeaders.Add("Host", "TODO enter host");
                httpClient.DefaultRequestHeaders.Add("Referer", Constants.LoginUrl);
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.18362");
            }
            var response = await httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();

            return new Tuple<string, HttpResponseMessage>(responseContent, response);
        }

        public static async Task<Tuple<string, HttpResponseMessage>> PostLoginAsync(string url, bool isVote, string username, string password, string cookieValue = null)
        {
            var httpClient = new HttpClient();
            var values = new Dictionary<string, string>
            {
            { Constants.EmailField, username },
            { Constants.PasswordField, password },
            { Constants.TokenField, "" }
            };

            if (!string.IsNullOrEmpty(cookieValue))
            {
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
                httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                httpClient.DefaultRequestHeaders.Add("Accept-Language", "pl-PL");
                if (!isVote)
                {
                    httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                    httpClient.DefaultRequestHeaders.Add("Origin", Constants.BaseUrl);
                }

                httpClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                httpClient.DefaultRequestHeaders.Add("Cookie", cookieValue);
                httpClient.DefaultRequestHeaders.Add("DNT", "1");
                httpClient.DefaultRequestHeaders.Add("Host", "TODO enter host");
                httpClient.DefaultRequestHeaders.Add("Referer", Constants.LoginUrl);
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.18362");
                httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

                //httpClient.DefaultRequestHeaders.Add("Content-Length", "388");
                //httpClient.DefaultRequestHeaders.Add("Content-Type", "multipart/form-data; boundary=---------------------------7e31362540a74");
            }

            HttpResponseMessage response;
            if (!isVote)
            {
                var content = new FormUrlEncodedContent(values);
                response = await httpClient.PostAsync(url, content);
            }
            else
            {
                response = await httpClient.GetAsync(url);
            }


            var responseContent = await response.Content.ReadAsStringAsync();

            return new Tuple<string, HttpResponseMessage>(responseContent, response);
        }

        public static async Task<Tuple<string, HttpResponseMessage>> GetLoginAsync(string url)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();

            return new Tuple<string, HttpResponseMessage>(responseContent, response);
        }
    }
}
