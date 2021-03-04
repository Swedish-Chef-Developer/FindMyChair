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
        [Description("Oppet sista")]
        OpenLast = 11,
        [Description("Djurfritt")]
        AnimalFree = 12,
        [Description("Rullstols vänligt")]
        HandicapFriendly = 13,
        [Description("Barnfritt")]
        KidFree = 14,
        [Description("Barnvänligt")]
        KidFriendly = 15,
        [Description("Öppet första")]
        OpenFirst = 16,
        [Description("Ickebinära")]
        NonBinary = 17,
        [Description("Persiskt")]
        Persian = 18,
        [Description("Spanska")]
        Spanish = 19,
        [Description("Engelska")]
        English = 20,
        [Description("HBQTmöte")]
        HBQT = 21,
        [Description("")]
        NotSet = 22
    }
}