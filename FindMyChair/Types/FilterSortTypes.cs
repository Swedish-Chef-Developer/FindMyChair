using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace FindMyChair.Types
{
	public class FilterSortTypes
	{
		public enum FilterTypes
		{
			[Description("Städer")]
			Cities = 1,
			[Description("Mötes typ")]
			Meetings = 2,
			[Description("Tidigaste tid")]
			EarliestTime = 3,
			[Description("Senadte tid")]
			LatestTime = 4,
			[Description("Tid mellan tidig och sen")]
			TimeBetweenEarlyAndLate = 5
		}

		public enum SortingTypes
		{
			[Description("Namn A - Ö")]
			NameAZ = 1,
			[Description("Namn Ö - A")]
			NameZA = 2,
			[Description("Tid tidig till sen")]
			TimeEarlyToLate = 3,
			[Description("Tid sen till tidig")]
			TimeLateToEarly = 4
		}
	}
}