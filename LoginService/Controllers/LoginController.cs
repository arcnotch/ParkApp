using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LoginService.Models;
using System.Net.Http;
using Newtonsoft.Json;
using StackExchange.Redis;
using Helpers.Redis;

namespace LoginService.Controllers
{
    [Route("[controller]")]
    public class LoginController : Controller
    {
        IDatabase cachingDB;

        static Dictionary<string,Token> ActiveLogins = new Dictionary<string, Token>();
        static List<User> Users = new List<User>();

        public LoginController(IRedisConnectionFactory caching) {
            cachingDB = caching.Connection().GetDatabase();
        }

        public IEnumerable<String> WriteToCache([FromBody] Token token) {
            cachingDB.StringSet(token._id.ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(token));

            return new List<string>{"ok"};
        }

        public Token ReadFromCache(string id) {
            if (!cachingDB.StringGet(id.ToString()).IsNull){
                Token token = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(cachingDB.StringGet(id.ToString()));
                return token;
            }
            return null;
        }
       
        [HttpGet]
        [Route("/ValidateSession/{tokenId}")]
        public Boolean ValidateSession(string tokenId) {
            Token t= ReadFromCache(tokenId);
            /*/
            var hc = Helpers.CouchDBConnect.GetPark("users");
            var response = await hc.GetAsync("/users/"+tokenId);
            if (!response.IsSuccessStatusCode)
                return false;
            
            var token = (Token) JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(),typeof(Token));
            */
            //if (token.create + token.ttl > now)
            if (t != null)
                if (t.create.AddSeconds(t.ttl).CompareTo(DateTime.Now) > 0)
                    return true;

            return false;
        }



        // POST api/values
        [HttpPost("/UserLogin/")]
        public async Task<dynamic> Post([FromBody]User u)
        {
            var hc = Helpers.CouchDBConnect.GetDB("users");
            var response = await hc.GetAsync("users/"+u._id);
            if (response.IsSuccessStatusCode) {
                User user = (User) JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(),typeof(User));
                if (user.password.Equals(u.password)) {
                    Token t = new Token();
                    t._id = u._id+":token:"+Guid.NewGuid();
                    t.create = DateTime.Now;
                    t.ttl = 600;
                    /* 
                    HttpContent htc = new StringContent(
                        JsonConvert.SerializeObject(t),
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );

                    await hc.PostAsync("users", htc);
                    */
                    WriteToCache(t);
                    return t._id;
                }
            };

            return "-1";

        }

        async  Task<Boolean> DoesUserExist(User u) {
            var hc = Helpers.CouchDBConnect.GetDB("users");
            var response = await hc.GetAsync("users/"+u._id);
            if (response.IsSuccessStatusCode) {
                return true;
            }

            return false;
        }

        [HttpPost("/CreateUser/")]
        public async Task<int> CreateUser([FromBody] User u) {
            var doesExist = await DoesUserExist(u);
            if (doesExist) {
                return -1;
            }

            var hc = Helpers.CouchDBConnect.GetDB("users");
            string json = JsonConvert.SerializeObject(u);
            var jsonObj = Newtonsoft.Json.Linq.JObject.Parse(json);
            jsonObj.Remove("_rev");
            jsonObj.GetValue("_id");
            json = jsonObj.ToString();
            HttpContent htc = new StringContent(json,System.Text.Encoding.UTF8,"application/json");
            var response = await hc.PostAsync("",htc);
            
            Console.WriteLine(response);
            return 1;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
