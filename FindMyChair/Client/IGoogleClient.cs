using FindMyChair.Models.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindMyChair.Client
{
	interface IGoogleClient
	{
		List<LongitudeAndLatitude> GetLongitudeAndLatitudeList(List<string> addresses);
		string GetLongitudeAndLatitudeListToJasonString(List<LongitudeAndLatitude> addresses);
	}
}
