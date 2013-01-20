using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DataStructures
{
	class Program
	{
		static void Main(string[] args)
		{
			InsertionTimings();
			
			Console.WriteLine();
			
			RandomItemRetrievalTimings(showChecksums: true);
		}

		// InsertionTimings -- time the insertion of 1000 items into inreasingly large collections.
		//   All collections timed should exhibit [amortized] O(1) performance.
		static void InsertionTimings()
		{
			Console.WriteLine("\tList<T>\tMyDictionary<T>\tDictionary<T>");

			foreach (var numItems in new[] { 1000, 5000, 10000, 50000, 100000, 500000, 1000000 })
			{
				var seq = Enumerable.Range(1, numItems)
					.Select(x => Tuple.Create(x, (int)Math.Log10(x)))
					.Select(x => Tuple.Create(x.Item1.ToString(), x.Item2))
					.ToList();

				var toInsert = seq.Take(1000).ToList();
				var existingCollectionContents = seq.Skip(1000).ToList();

				Stopwatch sw1;
				{
					var list = new List<Tuple<string, int>>();

					foreach (var item in existingCollectionContents)
						list.Add(item);

					sw1 = Stopwatch.StartNew();

					foreach (var item in toInsert)
						list.Add(item);

					sw1.Stop();
				}

				Stopwatch sw2;
				{
					var dict = new MyDictionary<string, int>();

					foreach (var item in existingCollectionContents)
						dict.Add(item.Item1, item.Item2);

					sw2 = Stopwatch.StartNew();

					foreach (var item in toInsert)
						dict.Add(item.Item1, item.Item2);

					sw2.Stop();
				}

				Stopwatch sw3;
				{
					var dict2 = new Dictionary<string, int>();

					foreach (var item in existingCollectionContents)
						dict2.Add(item.Item1, item.Item2);

					sw3 = Stopwatch.StartNew();

					foreach (var item in toInsert)
						dict2.Add(item.Item1, item.Item2);

					sw3.Stop();
				}

				Console.WriteLine("{0}\t{1}\t{2}\t{3}", numItems, sw1.ElapsedMilliseconds, sw2.ElapsedMilliseconds, sw3.ElapsedMilliseconds);
			}
		}

		// RandomItemRetrievalTimings -- time the lookup of 1000 random elements from increasingly
		//   large collections. List should exhibit O(n) perf; Dictionaries should be O(1).
		static void RandomItemRetrievalTimings(bool showChecksums = false)
		{
			Console.WriteLine("\tList<T>\tMyDictionary<T>\tDictionary<T>");

			foreach (var numItems in new[] { 1000, 5000, 10000, 50000, 100000, 500000, 1000000 })
			{
				var seq = Enumerable.Range(1, numItems)
					.Select(x => Tuple.Create(x, (int)Math.Log10(x)))
					.Select(x => Tuple.Create(x.Item1.ToString(), x.Item2))
					.ToList();

				Random rand = new Random(0);
				var itemsToLookUp = Enumerable.Range(0, 1000)
					.Select(x => rand.Next(numItems))
					.Select(x => x.ToString())
					.ToList();

				Stopwatch sw1;
				int checksum1 = 0;
				{
					var list = new List<Tuple<string, int>>();
					foreach (var entry in seq)
						list.Add(entry);

					sw1 = Stopwatch.StartNew();
					foreach (var lookup in itemsToLookUp)
					{
						var found = list.FirstOrDefault(x => x.Item1 == lookup);
						if (found != null)
							checksum1 += found.Item2;
					}
					sw1.Stop();
				}

				Stopwatch sw2;
				int checksum2 = 0;
				{
					var dict = new MyDictionary<string, int>();
					foreach (var entry in seq)
						dict.Add(entry.Item1, entry.Item2);

					sw2 = Stopwatch.StartNew();
					foreach (var lookup in itemsToLookUp)
					{
						int value;
						if (dict.TryGetValue(lookup, out value))
							checksum2 += value;
					}
					sw2.Stop();
				}

				Stopwatch sw3;
				int checksum3 = 0;
				{
					var dict2 = new Dictionary<string, int>();
					foreach (var entry in seq)
						dict2.Add(entry.Item1, entry.Item2);
					
					sw3 = Stopwatch.StartNew();
					foreach (var lookup in itemsToLookUp)
					{
						int value;
						if (dict2.TryGetValue(lookup, out value))
							checksum3 += value;
					}
					sw3.Stop();
				}

				if (showChecksums)
				{
					var oldColor = Console.ForegroundColor;
					Console.ForegroundColor = ConsoleColor.DarkGreen;
					Console.WriteLine("\t{0}\t{1}\t{2}", checksum1, checksum2, checksum3);
					Console.ForegroundColor = oldColor;
				}

				Console.WriteLine("{0}\t{1}\t{2}\t{3}", numItems, sw1.ElapsedMilliseconds, sw2.ElapsedMilliseconds, sw3.ElapsedMilliseconds);
			}
		}
	}
}
