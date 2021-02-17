using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindMyChair.Models.Meetings
{
    interface IDistrict
    {
        int Id { get; set; }
        string DistrictName { get; set; }
    }
}
