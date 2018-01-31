using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ParkService.Models;
using ParkService.Helpers;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Net.Http.Headers;
using RawRabbit.Enrichers.MessageContext.Context;
using RawRabbit.Operations.MessageSequence;
using RawRabbit;
using Newtonsoft.Json;

namespace ParkService.Controllers
{
    [Route("[controller]")]
    public class Parking : Controller
    { 
        private static readonly HttpClient client = new HttpClient();
        IBusClient RabbitMq;

        public Parking(IBusClient rmq){
            this.RabbitMq=rmq;
        }
        
        private async  Task<Boolean> DoesParkExist(Park newpark) {
            var hc = Helpers.CouchDBConnect.GetDB("parks");
            var response = await hc.GetAsync("parks/"+(newpark._id));
            if (response.IsSuccessStatusCode) {
                return true;
            }

            return false;
        }

        private async Task<string> GetRev(string _id) {
            var hc = Helpers.CouchDBConnect.GetDB("parks");
            var response = await hc.GetAsync("/parks/"+_id);
            if (!response.IsSuccessStatusCode)
                return "false";
            
            var park = (Park) JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(),typeof(Park));
            if (response.IsSuccessStatusCode) {
                return park._rev.ToString();
            }
            return "false";
        }

        private string getTokenFromRequest(){
            var userId = string.Empty;
            Microsoft.Extensions.Primitives.StringValues headerValues;
            var request = Request.Headers.TryGetValue("Token",out headerValues);
            userId = headerValues.FirstOrDefault();
            Console.WriteLine(userId);
            return userId;
        }

        //GET Token ok
        private async Task<Boolean> DoesTokenOk() {
            string token = getTokenFromRequest();
            HttpClient client = new HttpClient();
            var hc = Helpers.CouchDBConnect.ConnectionMethod(5001);
            var response = await hc.GetAsync("ValidateSession/"+token);
            string result = response.Content.ReadAsStringAsync().Result;
            if (result.Equals("true")) {
                return true;
            }

            return false;
        }

        //GET update park
        [HttpGet("/DoesParkExisted/{park}")]
        public async Task<Boolean> DoesParkExisted(string park) {
            /*var userId = string.Empty;
            Microsoft.Extensions.Primitives.StringValues headerValues;
            var request = Request.Headers.TryGetValue("Token",out headerValues);
            userId = headerValues.FirstOrDefault();
            Console.WriteLine(userId);*/
            if(await DoesTokenOk()){
                var hc = Helpers.CouchDBConnect.GetDB("parks");
                var response = await hc.GetAsync("parks/"+park);
                if (response.IsSuccessStatusCode) {
                    return true;
                }
            }
            return false;
        }
        static List<Park> Parks = new List<Park>();
        //Post new park
        [HttpPost("/CreatePark/")]
        public async Task<int> CreatePark([FromBody] Park newpark) {
            if(!await DoesTokenOk())
                return -1;
            var doesExist = await DoesParkExist(newpark);
            if (doesExist) {
                return -1;
            }

            var hc = Helpers.CouchDBConnect.GetDB("parks");
            string json = JsonConvert.SerializeObject(newpark);
            var jsonObj = Newtonsoft.Json.Linq.JObject.Parse(json);
            jsonObj.Remove("_rev");
            jsonObj.GetValue("_id");
            json = jsonObj.ToString();
            HttpContent htc = new StringContent(json,System.Text.Encoding.UTF8,"application/json");
            var response = await hc.PostAsync("",htc);
            
            Console.WriteLine(response);
            return 1;
        }

                //Put new box
        // PUT api/values/5
        [HttpPut("/Update/")]
        public async Task<string> UpdatePark([FromBody] Park newpark)
        {
            if(!await DoesTokenOk())
                return "-1";
            //newpark._rev=(GetRev(newpark._id)).ToString();
            var doesExist = await DoesParkExisted(newpark._id);
            if (!doesExist) {
                return "-1";
            }
            newpark._rev=GetRev(newpark._id).Result;
            var hc = Helpers.CouchDBConnect.GetDB("parks");
            string json = JsonConvert.SerializeObject(newpark);
            //var jsonObj = Newtonsoft.Json.Linq.JObject.Parse(json);
            //jsonObj.GetValue("_id");
            //json = jsonObj.ToString();
            HttpContent htc = new StringContent(json,System.Text.Encoding.UTF8,"application/json");
            var response = await hc.PutAsync("parks/"+newpark._id,htc);

            //=============RRABITMQ==========
            //RabbitMQ Post new Log if there is no more room
            if (newpark.FreeSpots()==0){
                var wake = Helpers.CouchDBConnect.ConnectionMethod(5002);
                var up = await wake.GetAsync("LogActive/"); 
                await RabbitMq.PublishAsync(new Email {
                details = "The park "+newpark.Name.ToString()+" is full"
            }
);

            }
            return "True";
        }

        [HttpGet("/GetPark/{park}")]
        public async Task<int> GetPark(string park) {
            /*var userId = string.Empty;
            Microsoft.Extensions.Primitives.StringValues headerValues;
            var request = Request.Headers.TryGetValue("Token",out headerValues);
            userId = headerValues.FirstOrDefault();
            Console.WriteLine(userId);*/
            HttpClient client = new HttpClient();
            var hc = Helpers.CouchDBConnect.GetDB("parks");
            var response = await hc.GetAsync("parks/"+park);
            if (response.IsSuccessStatusCode) {
                string result = response.Content.ReadAsStringAsync().Result;
                var getpark = (Park) JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(),typeof(Park));
                return getpark.FreeSpots();
                
            }
            
            return -1;
        }

        // DELETE api/values/5
        [HttpDelete("Delete/{park}")]
        public void Delete(string park)
        {
        }
    }
}