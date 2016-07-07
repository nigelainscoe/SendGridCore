using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SendGridCore.Services;

namespace SendGridCore.Controllers
{
    [Route("api/[controller]")]
    public class ListManageController
    {
        private static IAuthService _auth;
        public ListManageController(IAuthService auth)
        {
            _auth = auth;
        }

        /// <summary>
        /// Get does nothing but return a 200. You wouldn't want to try and do any DB work
        /// in a Get. Returning 200 might make some spammers think they are succeeding.
        /// </summary>
        /// <returns>HttpOkResult()</returns>
        [HttpGet]
        public IActionResult Validate()
        {
            return new OkResult();
        }

        /// <summary>
        /// Takes the sign up values. If there is anything in the captchavalue string we presume
        /// that it is a spam/bot entry and ignore it. When you implement your web page you 
        /// should specify your captcha string as <div style="position: absolute; left: -5000px;" 
        /// aria-hidden="true"><input type="text" name="captchavalue" tabindex="-1" value=""/></div>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <param name="captchavalue"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<StatusCodeResult> PostValidate(string email, string firstname,
            string lastname, string captchavalue)
        {
            if (!string.IsNullOrEmpty(captchavalue))
            { return new OkResult(); }

            if (string.IsNullOrEmpty(email))
            { return new BadRequestResult(); }

            var apiKey = _auth.GetAuthString();
            var authorization = _auth.GetAuthMethod();
            var listToManage = _auth.GetListToManage();

            //           var test = TestService.AuthTest();


            // Try adding as a new contact
            var response = await ListServices.AddContact(email, firstname, lastname,
                authorization, apiKey);

            var contactModel = JObject.Parse(response);

            // Assuming the call to the Contact service was successfull
            if ((int)contactModel["error_count"] == 0)
            {

                // todo There is probably a better way of doing this
                var persistedRecipient = (string)contactModel["persisted_recipients"].First;

                // Need the list Id before we can do anything with it.
                response = await ListServices.GetLists(authorization, apiKey);
                var listModel = JObject.Parse(response);
                //                var element = (string)contactModel["persisted_recipients"].Select()

                JToken listToken = listModel.SelectToken("$.lists[?(@.name == '" + listToManage + "')].id");

                int listId;

                if (!int.TryParse(listToken.ToString(), out listId))
                {
                    return new NotFoundResult();
                }

                // Add them to the configured list
                var finalSuccess = await ListServices.AddContactToList(listId, persistedRecipient, authorization, apiKey);

            }


            // todo Add User Secrets for the API Keys

            return new OkResult(); 
        }
    }


}
