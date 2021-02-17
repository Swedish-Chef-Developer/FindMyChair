using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindMyChair.Models.Meetings
{
    public interface IAddress
    {
        int Id { get; set; }
        string City { get; set; }
        List<District> Districts { get; set; }
        string Street { get; set; }
        string LocationLink { get; set; }
    }
}
