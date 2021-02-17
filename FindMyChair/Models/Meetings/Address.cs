using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FindMyChair.Models.Meetings
{
    public class Address
    {
        public Address()
        {
            Districts = new List<District>();
        }

        public int Id { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string LocationLink { get; set; }
       public List<District> Districts{ get; set; }
    }
}