using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Newtonsoft.Json;

namespace ParkService.Models{
        public class Cell{
            public string _id {get;set;}
            public bool Spot {get; set;} //Free=False
            public float Price {get; set;}
            public float X {get; set;}
            public float Y {get; set;}
            [JsonIgnore]
            private Car Car {get; set;}
            [JsonIgnore]
            private DateTime StartTime {get; set;}

            public bool statusCell(){
                return Spot;
            }
        }
}