
using System;

namespace HashMap
{
	class Program
	{
		static void Main()
		{
			MyDictionary<string, int> myDictionary = new MyDictionary<string, int>();
			myDictionary["zero"] = 0;
			myDictionary["one"] = 1;
			myDictionary["two"] = 2;
			myDictionary["three"] = 3;
			myDictionary["four"] = 4;
			Console.WriteLine(myDictionary.Count);
			Console.WriteLine("{0}, {1}, {2}, {3}, {4}", myDictionary["zero"], myDictionary["one"], myDictionary["two"], myDictionary["three"], myDictionary["four"]);

			myDictionary["five"] = 5;
			Console.WriteLine(myDictionary.Count);
			Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}", myDictionary["zero"], myDictionary["one"], myDictionary["two"], myDictionary["three"], myDictionary["four"], myDictionary["five"]);

			myDictionary.Remove("two");
			myDictionary.Remove("five");
			myDictionary.Remove("one");
			Console.WriteLine(myDictionary.Count);
			Console.WriteLine("{0}, {1}, {2}", myDictionary["zero"], myDictionary["three"], myDictionary["four"]);
		}
	}
}
