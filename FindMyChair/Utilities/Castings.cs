using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FindMyChair.Utilities
{
	public static class Castings
	{
		public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null)
			{
				throw new Exception("source");
			}
			return new List<TSource>(source);
		}
	}
}