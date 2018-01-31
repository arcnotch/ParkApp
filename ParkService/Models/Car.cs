using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;

namespace ParkService.Models{
        public class Car{
            public string _id {get;set;}
            public int Number {get; set;}
            public string Color {get; set;}
            public string cellid { get; set; }
        }
}