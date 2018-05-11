using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Kans;
using Kans.ListAlgorithms;

namespace Tests {
	[TestClass]
	public class RNGTests {

		private void TestGenerator(KansBase rng, string name) {
			/* In a way random number generators are poor candidates for unit testing, since there is
			no specific output they must produce. For now, manual inspection of their output will suffice. */

			IList<int> urn = new List<int>() { 1, 2, 4, 5, 6, 7, 8, 9 };

			rng.Seed(12345);

			PrivateObject po = new PrivateObject(rng);
			Console.WriteLine(name + " initial state:");
			Console.WriteLine(
				String.Join(" ", (uint[])po.GetField(rng is ChaChaRNG ? "coreArray" : "state"))
			);

			Console.WriteLine(name + " doubles:");
			for (int i = 1; i <= 10; i++) {
				Console.WriteLine(rng.Random());
			}
			Console.WriteLine(name + " dice rolls:");
			for (int i = 1; i <= 10; i++) {
				Console.WriteLine(rng.RandInt(1, 6));
			}
			Console.WriteLine(name + " indices:");
			for (int i = 1; i <= 10; i++) {
				Console.WriteLine(rng.RandIndex(4));
			}
			Console.WriteLine(name + " urn:");
			for (int i = 1; i <= 10; i++) {
				Console.WriteLine(String.Join(", ", urn.Sample(5, rng)));
			}
		}

		[TestMethod]
		public void WELL1024aTest() {
			TestGenerator(new WELL1024a(), "WELL1024a");
		}

		[TestMethod]
		public void LFIB4Test() {
			TestGenerator(new LFIB4(), "LFIB4");
		}

		[TestMethod]
		public void ChaChaTest() {
			TestGenerator(new ChaChaRNG(), "ChaCha");
		}
	}
}
