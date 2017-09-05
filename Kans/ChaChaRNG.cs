using System;

namespace Kans {
	/// <summary>
	/// A generator based on the ChaCha stream cipher.
	/// </summary>
	/// <remarks>
	/// Reference: "ChaCha, a variant of Salsa20." D. J. Bernstein, 2008.
	/// <para>
	/// This generator uses the seed to initialize the key and nonce of the ChaCha cipher, after which output is obtained by traversing the cipher stream. Once the stream has been exhausted (which happens after 2^68 outputs), the nonce is incremented, and once the nonce rolls over to zero, the key is incremented. This is not quite a cryptographically sound operation, but it guarantees the generator a period of 2^388.
	/// </para>
	/// <para>
	/// The generator has a free parameter, which is the number of "rounds" that are to be performed every 16 outputs. This number determines speed versus quality, and should be a multiple of four. The default is 12; 8 and 20 could also be considered.
	/// </para>
	/// <para>
	/// Because this generator is based on a cryptographic cipher, distinguishing its output from a "truly random" sequence is expected to be very difficult, and essentially impossible with any statistical test such as those in TestU01. But beware that Kans is not a cryptography library, and a cipher is only a small part of the solution to a cryptographic problem.
	/// </para>
	/// <para>
	/// The generator has 320 bits of state that must be initialized.
	/// </para>
	/// </remarks>
	[Serializable]
	public class ChaChaRNG : KansBase {
		private const int SIZE = 16;
		private const int CONSTANTS_SIZE = 4;
		private const int KEY_SIZE = 8;
		private const int NONCE_SIZE = 2;

		private readonly int numRounds;
		private uint[] coreArray = new uint[SIZE];
		private uint[] output = new uint[SIZE];
		private int remaining;

		/// <summary>
		/// Constructs the generator with a custom number of ChaCha rounds.
		/// </summary>
		/// <param name="rounds">The number of ChaCha rounds to be used by the generator.</param>
		public ChaChaRNG(int rounds) {
			numRounds = rounds;
			coreArray[0] = 0x61707865u; // "expa"
			coreArray[1] = 0x3120646eu; // "nd 3"
			coreArray[2] = 0x79622d36u; // "2-by"
			coreArray[3] = 0x6b206574u; // "te k"
		}

		/// <summary>
		/// Constructs the generator with 12 ChaCha rounds.
		/// </summary>
		public ChaChaRNG() : this(12) { }

		protected override int seedWordsRequired {
			get {
				return KEY_SIZE + NONCE_SIZE;
			}
		}

		private void QuarterRound(uint[] x, int a, int b, int c, int d) {
			x[a] += x[b];
			x[d] = ((x[d] ^ x[a]) << 16) | ((x[d] ^ x[a]) >> (32 - 16));
			x[c] += x[d];
			x[b] = ((x[b] ^ x[c]) << 12) | ((x[b] ^ x[c]) >> (32 - 12));
			x[a] += x[b];
			x[d] = ((x[d] ^ x[a]) << 8) | ((x[d] ^ x[a]) >> (32 - 8));
			x[c] += x[d];
			x[b] = ((x[b] ^ x[c]) << 7) | ((x[b] ^ x[c]) >> (32 - 7));
		}

		private void Cipher() {
			Array.Copy(coreArray, output, 16);
			for(int i = numRounds; i > 0; i -= 2) {
				QuarterRound(output, 0, 4, 8, 12);
				QuarterRound(output, 1, 5, 9, 13);
				QuarterRound(output, 2, 6, 10, 14);
				QuarterRound(output, 3, 7, 11, 15);
				QuarterRound(output, 0, 5, 10, 15);
				QuarterRound(output, 1, 6, 11, 12);
				QuarterRound(output, 2, 7, 8, 13);
				QuarterRound(output, 3, 4, 9, 14);
			}
			for(int i = 0; i < SIZE; i++) {
				output[i] += coreArray[i];
			}
		}

		protected override uint NextU() {
			if(remaining <= 0) {
				int overflowIndex = 15;
				while(true) {
					coreArray[overflowIndex]++;
					// increment higher ints on overflow to achieve 384-bit incrementing
					if(coreArray[overflowIndex] <= 0 && overflowIndex > CONSTANTS_SIZE) {
						overflowIndex--;
					} else {
						break;
					}
				}
				Cipher();
				remaining = SIZE;
			}

			return output[SIZE-(remaining--)];
		}

		protected override void InternalSeed(uint[] seed) {
			// constants are set at object construction, and are never changed
			Array.Copy(seed, 0, coreArray, CONSTANTS_SIZE, KEY_SIZE + NONCE_SIZE);
			// initialize stream counter to zero
			coreArray[14] = 0;
			coreArray[15] = 0;
			Cipher();
			remaining = 16;
		}
	}
}
