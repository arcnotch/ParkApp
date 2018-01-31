using System;

namespace LoginService.Models
{
    public class User
    {
        public string _id {get; set;}
        
        public string password {get; set;}
    }

    public class Token {
        public string _id {get; set;}
        public int ttl {get ;set;}
        public string Email { get; set; }
        public DateTime create {get; set;}

        public Token(){;
        }
    }
}