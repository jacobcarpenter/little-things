using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
	public struct Comparison<T>
	{
		public Comparison(IComparer<T> comparer, T value)
		{
			m_comparer = comparer;
			m_value = value;
		}

		public bool GreaterThan(T compareTo)
		{
			int comparison = m_comparer.Compare(m_value, compareTo);
			return comparison > 0;
		}

		public bool GreaterThanOrEqualTo(T compareTo)
		{
			int comparison = m_comparer.Compare(m_value, compareTo);
			return comparison >= 0;
		}

		public bool LessThan(T compareTo)
		{
			int comparison = m_comparer.Compare(m_value, compareTo);
			return comparison < 0;
		}

		public bool LessThanOrEqualTo(T compareTo)
		{
			int comparison = m_comparer.Compare(m_value, compareTo);
			return comparison <= 0;
		}

		readonly IComparer<T> m_comparer;
		readonly T m_value;
	}

	public static class ComparerUtility
	{
		public static Comparison<T> Is<T>(this IComparer<T> comparer, T value)
		{
			return new Comparison<T>(comparer, value);
		}
	}
}
