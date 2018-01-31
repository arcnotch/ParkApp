using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LogService.Models;
using LogService.Helpers;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Net.Http.Headers;
using RawRabbit.Enrichers.MessageContext.Context;
using RawRabbit.Operations.MessageSequence;
using RawRabbit;

namespace LogService.Controllers
{
    [Route("[controller]")]
    public class LogController : Controller
    { 
        IBusClient client;
        public Task logger { get; private set; }
        private async Task LogAsync(Email log)
    {
        // Asynchronously initialize this instance.
            var hc = Helpers.CouchDBConnect.GetDB("logs");
            var response = await hc.GetAsync("logs/"+(log._id));
            string json = JsonConvert.SerializeObject(log);
            var jsonObj = Newtonsoft.Json.Linq.JObject.Parse(json);
            jsonObj.Remove("_rev");
            jsonObj.GetValue("_id");
            json = jsonObj.ToString();
            HttpContent htc = new StringContent(json,System.Text.Encoding.UTF8,"application/json");
            var response2 = await hc.PostAsync("",htc);
            if (response2.IsSuccessStatusCode)
                Console.WriteLine("Logged"); 
    }

    public LogController(IBusClient _client) {
            client = _client;
            client.SubscribeAsync<Email,MessageContext>((log,ctx) => {
            Console.WriteLine("log id: {0} ",log.details);
            Email l = new Email();
            l.Date=DateTime.Now;
            l.details=log.details;
            l._id=l.Date.ToString()+"-"+Guid.NewGuid();
            logger = LogAsync(l);  
            Console.WriteLine("Logged"); 
            return Task.FromResult(0);
        });
    }

        [HttpGet]
        [Route("/LogActive/")]
        public void LogActive() {
            Console.WriteLine("Alive");
        }
    }
}