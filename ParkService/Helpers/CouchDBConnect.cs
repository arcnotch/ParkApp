using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using ParkService.Models;

namespace ParkService.Helpers
{
    //https://loginguid-bluemix.cloudant.com/{0}
    public static class CouchDBConnect
    {
        private static string host = "https://loginguid-bluemix.cloudant.com/{0}";
        private static string url = "http://localhost:";
        public static HttpClient GetDB(string db) {
            var hc = new HttpClient();
            hc.BaseAddress = new Uri(string.Format(host,db));
            hc.DefaultRequestHeaders.Clear();
            hc.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Console.WriteLine(hc.DefaultRequestHeaders);
            return hc;
        }

        public static HttpClient ConnectionMethod(int port) {
            var hc = new HttpClient();
            hc.BaseAddress = new Uri(url+(port.ToString())+"/");
            hc.DefaultRequestHeaders.Clear();
            hc.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Console.WriteLine(hc.DefaultRequestHeaders);
            return hc;
        }
    }
}
