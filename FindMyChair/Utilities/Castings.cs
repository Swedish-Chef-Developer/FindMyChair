using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace FindMyChair.Utilities
{
	public static class Castings
	{
		public static List<TSource> CustomToList<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null)
			{
				throw new Exception("source");
			}
			return new List<TSource>(source);
		}

		public static IOrderedQueryable<T> CustomOrderBy<T, TKey>(this IQueryable<T> qry, Expression<Func<T, TKey>> expr)
		{
			return qry.OrderBy(expr);
		}
	}
}