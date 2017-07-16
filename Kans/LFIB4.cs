using System;

namespace Kans {
	/// <summary>
	/// The LFIB4 generator.
	/// </summary>
	/// <remarks>
	/// Proposed by George Marsaglia in 1999.
	/// <para>
	/// LFIB4 is a multiple recursive generator. It is fast and appears to pass all tests in TestU01. However, it requires a lot of seed information for its not spectacularly long period, and generators of this type are known to be sensitive to initialization.
	/// </para>
	/// <para>
	/// The generator has a period of about 2^287 and has 8192 bits of state that must be initialized.
	/// </para>
	/// </remarks>
	[Serializable]
	public class LFIB4 : KansBase {

		private const int SIZE = 256;

		private uint[] state = new uint[SIZE];
		private int index;

		/// <summary>
		/// Constructs the generator.
		/// </summary>
		public LFIB4() {
		}

		protected override int seedWordsRequired {
			get {
				return SIZE;
			}
		}

		protected override uint NextU() {
			state[index] = state[(index - 55 + SIZE) & 255] + state[(index - 119 + SIZE) & 255] + state[(index - 179 + SIZE) & 255] + state[index];
			uint result = state[index];
			index = (index + 1) & 255;
			return result;
        }

		protected override void InternalSeed(uint[] seed) {
			Array.Copy(seed, state, SIZE);
			state[0] |= 0x00000001; // guarantees a valid (nonzero, one state word odd) starting state
			index = 0;
		}
	}
}
