using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace FindMyChair.Types
{
    public enum MeetingTypes
    {
        [Description("Öppet möte")]
        Open = 1,
        [Description("Stängt möte")]
        Closed = 2,
        [Description("Mans möte")]
        Mens = 3,
        [Description("Kvinno möte")]
        Womens = 4,
        [Description("Big Book möte")]
        BigBook = 5,
        [Description("Litteratur möte")]
        Litterature = 6,
        [Description("Ungdoms möte")]
        YPAA = 7,
        [Description("Online möte")]
        Online = 8,
        [Description("")]
        NotSet = 9
    }
}