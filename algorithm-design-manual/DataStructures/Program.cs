using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStructures
{
	class Program
	{
		static void Main(string[] args)
		{
            var dict = new MyDictionary<string, int>();

            foreach (var entry in MyDictionaryTests.BaseEntries)
                dict.Add(entry.Item1, entry.Item2);

		}
	}
}
