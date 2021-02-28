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
        [Description("Mansmöte")]
        Mens = 3,
        [Description("Kvinnomöte")]
        Womens = 4,
        [Description("Big Book möte")]
        BigBook = 5,
        [Description("Litteraturmöte")]
        Litterature = 6,
        [Description("Ungdomsmöte")]
        YPAA = 7,
        [Description("Onlinemöte")]
        Online = 8,
        [Description("Stegmöte")]
        Step = 9,
        [Description("Traditionsmöte")]
        Tradition = 10,
        [Description("")]
        NotSet = 11
    }
}