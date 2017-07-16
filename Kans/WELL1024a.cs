using System;

namespace Kans {
	/// <summary>
	/// The WELL1024a generator.
	/// </summary>
	/// <remarks>
	/// Reference: "Improved Long-Period Generators Based on Linear Recurrences Modulo 2". François Panneton, Pierre l’Ecuyer and Makoto Matsumoto, 2006.
	/// <para>
	/// The WELL (Well Equidistributed Long-period Linear) generators are designed to fix some problems with the ubiquitous Mersenne Twister. In particular, the WELL generators fare better in terms of their "equidistribution", and are much less sensitive to poor initialization. WELL1024a is a compact generator that nonetheless has a huge period and excellent statistical properties. It is the "workhorse" of Kans.
	/// </para>
	/// <para>
	/// The generator has a period of 2^1024 - 1 and has 1024 bits of state that must be initialized.
	/// </para>
	/// </remarks>
	[Serializable]
	public class WELL1024a : KansBase {

		private const int SIZE = 32;
        private uint[] state = new uint[SIZE];
		private uint i, z0, z1, z2;

		/// <summary>
		/// Construct the generator.
		/// </summary>
		public WELL1024a() { }

		protected override int seedWordsRequired {
			get {
				return SIZE;
			}
		}

		protected override uint NextU() {
			z0 = state[i + 31 & 31];
			z1 = state[i] ^ state[i + 3 & 31] ^ (state[i + 3 & 31] >> 8);
			z2 = state[i + 24 & 31] ^ (state[i + 24 & 31] << 19) ^ state[i + 10 & 31] ^ (state[i + 10 & 31] << 14);
			state[i] = z1 ^ z2;
			state[i + 31 & 31] = z0 ^ (z0 << 11) ^ z1 ^ (z1 << 7) ^ z2 ^ (z2 << 13);
			i = i + 31 & 31;
			return state[i];
		}

		protected override void InternalSeed(uint[] seed) {
			Array.Copy(seed, state, SIZE);
			state[0] |= 0x00000001; // guarantees a valid (nonzero) starting state
			i = 0;
		}
	}
}
