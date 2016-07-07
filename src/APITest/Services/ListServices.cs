using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SendGridCore.Services
{
    public class ListServices
    {
        public static IAuthService Auth { get; private set; }

        public ListServices(IAuthService auth)
        {
            Auth = auth;
        }

        /// <summary>
        /// Check the Sendgrid contacts list and add the user if not present
        /// and get the user id. If allready present just return the id
        /// </summary>
        /// <param name="email"></param>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <param name="auth"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public static async Task<string> AddContact(string email, string firstname, string lastname,
            string auth, string apiKey)
        {
            var contact = new SignUp
            {
                email = email,
                first_name = firstname,
                last_name = lastname
            };

            const string requestTarget = "/v3/contactdb/recipients";
            var requestContent = new StringContent("[" + JsonConvert.SerializeObject(contact) + "]",
                Encoding.UTF8,
                "application/json");

            return await PostSendGridApi(requestTarget, requestContent, auth, apiKey);
        }

        public static async Task<string> AddContactToList(int listId, string recipientId, string auth, string apiKey)
        {

            var requestTarget = $"/v3/contactdb/lists/{listId}/recipients/{recipientId}";
            var requestContent = new StringContent("",
                Encoding.UTF8,
                "application/json");

            return await PostSendGridApi(requestTarget, requestContent, auth, apiKey);
        }

        public static async Task<string> GetLists(string auth, string apiKey)
        {
            const string requestTarget = "/v3/contactdb/lists";
           
            return await GetSendGridApi(requestTarget, auth, apiKey);
        }

        private static async Task<string> GetSendGridApi(string requestTarget, string auth, string apiKey)
        {
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.BaseAddress = new Uri("https://api.sendgrid.com");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(auth, apiKey);

                HttpResponseMessage response = await client.GetAsync(requestTarget);
                if (!response.IsSuccessStatusCode) return response.StatusCode.ToString();
                var userData = await response.Content.ReadAsStringAsync();
                return userData;
            }
        }

        private static async Task<string> PostSendGridApi(string requestTarget, HttpContent requestContent,
            string auth, string apiKey)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.sendgrid.com");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(auth, apiKey);

                HttpResponseMessage response = await client.PostAsync(requestTarget, requestContent);
                if (!response.IsSuccessStatusCode) return requestContent.Headers.ToString();
                var userData = await response.Content.ReadAsStringAsync();
                return userData;
            }
        }
    }

    public class SignUp
    {
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        //public string CaptchaValue { get; set; }
    }

}