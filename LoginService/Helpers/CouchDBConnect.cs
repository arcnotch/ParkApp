using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using LoginService.Models;

namespace LoginService.Helpers
{
    //https://7894186b-7b5d-4cb6-88e2-7eb85298db66-bluemix:139f30f2155fbf4dcd667ef6a46464f1b38cc7a5269d1b7cad859230ed046fb8@7894186b-7b5d-4cb6-88e2-7eb85298db66-bluemix.cloudant.com/{0}
    public static class CouchDBConnect
    {
        private static string host = "https://2a023f6f-892a-4294-865c-b809aa3a1bc3-bluemix:ef782369d006e54008e55e5a53b6839aae92c9683789cef11a7255d01fc4d621@2a023f6f-892a-4294-865c-b809aa3a1bc3-bluemix.cloudant.com/{0}";
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