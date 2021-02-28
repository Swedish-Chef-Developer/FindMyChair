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
        public IEnumerable<District> Districts{ get; set; }
        public double Longitude { get; set; }
        public double Lattitude { get; set; }
        public string LocalizedAddress { get; set; }
    }
}