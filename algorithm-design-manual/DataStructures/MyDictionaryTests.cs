using System;
using System.Collections.Generic;
using System.Linq;
using Combinatorics.Collections;
using NUnit.Framework;

namespace DataStructures
{
	[TestFixture]
	public sealed class MyDictionaryTests
	{
		[Test]
		public void Add_DuplicateKey_Throws()
		{
			var dict = new MyDictionary<string, int>();
			dict.Add("Jacob", 1);

			Assert.Throws<ArgumentException>(() => dict.Add("Jacob", 2));
		}

		[Test]
		public void Add_ItemWithNegativeHashCode_DoesNotThrow()
		{
			var dict = new MyDictionary<string, int>(comparer: TestEqualityComparer<string>.Instance);
			dict.Add("test", 1);
			int value;
			Assert.IsTrue(dict.TryGetValue("test", out value));
			Assert.AreEqual(1, value);

			dict.Add("test 2", 2);
			Assert.IsTrue(dict.TryGetValue("test 2", out value));
			Assert.AreEqual(2, value);
		}

		[TestCaseSource("PermutationsOfEntries")]
		public void TryGetValue_ForValidKey_ReturnsExpectedValue(Tuple<string, int>[] entries)
		{
			var dict = new MyDictionary<string, int>();
			foreach (var pair in entries)
				dict.Add(pair.Item1, pair.Item2);

			for (int i = 0; i < entries.Length; i++)
			{
				int value;
				Assert.IsTrue(dict.TryGetValue(BaseEntries[i].Item1, out value));
				Assert.AreEqual(BaseEntries[i].Item2, value);
			}
		}

		[TestCaseSource("PermutationsOfEntries")]
		public void TryGetValue_ForMissingKey_ReturnsFalse(Tuple<string, int>[] entries)
		{
			var dict = new MyDictionary<string, int>();
			foreach (var pair in entries)
				dict.Add(pair.Item1, pair.Item2);

			int value;
			Assert.False(dict.TryGetValue("not a key", out value));
		}

		[TestCaseSource("PermutationsOfEntries")]
		public void Remove_ForValidKey_RemovesEntry(Tuple<string, int>[] entries)
		{
			var dict = new MyDictionary<string, int>();
			foreach (var pair in entries)
				dict.Add(pair.Item1, pair.Item2);

			for (int i = 0; i < entries.Length; i++)
			{
				int value;
				Assert.IsTrue(dict.TryGetValue(BaseEntries[i].Item1, out value));
				Assert.IsTrue(dict.Remove(BaseEntries[i].Item1));
				Assert.IsFalse(dict.TryGetValue(BaseEntries[i].Item1, out value));
				Assert.IsFalse(dict.Remove(BaseEntries[i].Item1));
			}
		}

		public IEnumerable<Tuple<string, int>[]> PermutationsOfEntries
		{
			get
			{
				foreach (var list in Entries)
				{
					var permutations = new Permutations<Tuple<string, int>>(list, Comparer<Tuple<string, int>>.Default);
					foreach (var p in permutations.Take(c_maxPermutations))
						yield return p.ToArray();
				}
			}
		}

		public IEnumerable<Tuple<string, int>[]> Entries
		{
			get
			{
				for (int minItems = c_minEntriesToTest; minItems <= BaseEntries.Count; minItems++)
				{
					var result = new Tuple<string, int>[minItems];
					s_baseEntries.CopyTo(0, result, 0, minItems);
					yield return result;
				}
			}
		}

		sealed class TestEqualityComparer<T> : IEqualityComparer<T>
		{
			public static readonly IEqualityComparer<T> Instance = new TestEqualityComparer<T>();

			public bool Equals(T x, T y)
			{
				return EqualityComparer<T>.Default.Equals(x, y);
			}

			public int GetHashCode(T obj)
			{
				return -1;
			}

			private TestEqualityComparer()
			{
			}
		}

		// NOTE: to add more test runs, decrease this number...
		const int c_minEntriesToTest = 3;

		// NOTE: to add more test runs, increase this number...
		const int c_maxPermutations = 50;

		// NOTE: using List for the CopyTo method; do not mutate!
		private static readonly List<Tuple<string, int>> s_baseEntries =
			new List<Tuple<string, int>>
            {
                Tuple.Create("one", 1),
                Tuple.Create("two", 2),
                Tuple.Create("three", 3),
                Tuple.Create("four", 4),
                Tuple.Create("five", 5),
				Tuple.Create("six", 6),
				Tuple.Create("seven", 7),
            };

		public static IReadOnlyList<Tuple<string, int>> BaseEntries
		{
			get { return s_baseEntries; }
		}
	}
}
