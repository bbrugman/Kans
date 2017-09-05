using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests {
	[TestClass]
	public class KeccakTests {
		[TestMethod]
		public void KeccakDigestsCorrect() {
			//https://github.com/gvanas/KeccakCodePackage/tree/master/TestVectors
			// Clearly these tests are far from exhaustive, and not sufficient for a crypto library.
			// At the same time, a faulty implementation is very unlikely to preduce the results below.
			Assert.AreEqual(0x56C5094Du, 
				Kans.Keccak.Hash(new uint[] { }, 1, 288 / 32)[0]);
			Assert.AreEqual(0x0E23B8BDu, 
				Kans.Keccak.Hash(new uint[] { 0xFCFDECC1u }, 1, 288 / 32)[0]);
			Assert.AreEqual(0xDB4F78CEu, 
				Kans.Keccak.Hash(new uint[] { 0x24204F4Au, 0x26255184u}, 2, 544 / 32)[1]);
		}
	}
}
