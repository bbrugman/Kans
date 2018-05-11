using System;

namespace Kans {
	// Keccak f-800
	internal class Keccak {

		private static readonly int WORDS = 25; // state has 25 * w = 800 bits

		private static readonly int ROUNDS = 22; // 22

		private static readonly uint[] RC = { // truncated round constants
			0x00000001,
			0x00008082,
			0x0000808a,
			0x80008000,
			0x0000808b,
			0x80000001,
			0x80008081,
			0x00008009,
			0x0000008a,
			0x00000088,
			0x80008009,
			0x8000000a,
			0x8000808b,
			0x0000008b,
			0x00008089,
			0x00008003,
			0x00008002,
			0x00000080,
			0x0000800a,
			0x8000000a,
			0x80008081,
			0x00008080
		};

		private static readonly int[] rxy = { // rotation constants mod 32
			0, 1, 30, 28, 27,
			4, 12, 6, 23, 20,
			3, 10, 11, 25, 7,
			9, 13, 15, 21, 8,
			18, 2, 29, 24, 14
		};

		private static void Keccakf800(uint[] array) {
			for (int i = 0; i < ROUNDS; i++) {
                Round(array, RC[i]);
			}
		}

		private static void Round(uint[] array, uint RC) {
			uint[, ] B = new uint[5, 5];
			uint[] C = new uint[5];
			uint[] D = new uint[5];

			// theta step
			for (int i = 0; i < 5; i++) {
				C[i] = array[i] ^ array[i + 5] ^ array[i + 10] ^ array[i + 15] ^ array[i + 20];
			}
			for (int i = 0; i < 5; i++) {
				D[i] = C[(i + 4) % 5] ^ ((C[(i + 1) % 5] << 1) | (C[(i + 1) % 5] >> 31));
			}
			for (int i = 0; i < 25; i++) {
				array[i] = array[i] ^ D[i % 5];
			}
			// rho and pi step
			for (int i = 0; i < 25; i++) {
				int x = i % 5;
				int y = i / 5;
				B[y, (2 * x + 3 * y) % 5] = (array[i] << rxy[i]) | (array[i] >> (32 - rxy[i]));
			}
			// chi step
			for (int i = 0; i < 25; i++) {
				int x = i % 5;
				int y = i / 5;
				array[i] = B[x, y] ^ (~B[(x + 1) % 5, y] & B[(x + 2) % 5, y]);
			}
			// iota step
			array[0] = array[0] ^ RC;

		}

		public static uint[] Hash(uint[] input, int outputLength, int wordRate) {
			uint[] state = new uint[WORDS];

			/* Since we have input consisting of entire words, an extra block is
			always needed for padding if input length matches the word rate */
			int requiredInputLength = ((input.Length / wordRate) + 1) * wordRate;

			uint[] P = new uint[requiredInputLength];
			Array.Copy(input, P, input.Length);
			// Now follows 10*1 padding
			// Compensating for endianness and bit order means the operands look different
			P[input.Length] = 0x00000001u;
			P[requiredInputLength - 1] ^= 0x80000000u;

			int absorbIndex = 0;
			for(int i = 0; i < requiredInputLength; i++) {
				state[absorbIndex++] ^= P[i];
				if(absorbIndex >= wordRate) {
					Keccakf800(state);
					absorbIndex = 0;
				}
			}

			uint[] output = new uint[outputLength];
			int squeezeIndex = 0;
			for(int i = 0; i < outputLength; i++) {
				output[i] = state[squeezeIndex++];
				if (squeezeIndex >= wordRate) {
					Keccakf800(state);
					squeezeIndex = 0;
				}
			}
			return output;
		}
	}
}
