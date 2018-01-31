using System;

namespace LogService.Models {
    public class Email {
        public string _id { get; set; }
        public string _rev { get; set; }
        public string details { get; set; }
        public DateTime Date { get; set; }
    }
}