using System;
using System.Collections.Generic;

namespace Kans {
	/// <summary>
	/// The base class for Kans PRNG's.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class provides shared logic for Kans PRNG's, including exact uniform integer sampling and Keccak-based seeding.
	/// </para>
	/// </remarks>
	public abstract class KansBase : IAltRandom {
		/// <summary>
		/// The inverse of 2^32.
		/// </summary>
		protected const double INV_232 = 2.32830643653869628906e-10;

		private const int KeccakCapacity = 256;

		/// <summary>
		/// The amount of data required to fully specify the generator's internal state.
		/// </summary>
		protected abstract int seedWordsRequired { get; }

		public void Seed(int seed) {
			Seed(new int[] { seed });
		}

		public void Seed(int[] seed) {
			uint[] input = new uint[seed.Length];
			for(int i = 0; i < seed.Length; i++) {
				input[i] = (uint)seed[i];
			}
			InternalSeed(Keccak.Hash(input, seedWordsRequired, (800 - 256) / 32));
		}

		/// <summary>
		/// Generator-specific initialization, using an array with length determined by <see cref="seedWordsRequired"/>.
		/// </summary>
		/// <param name="seed">A <see cref="uint"/> array of the required length.</param>
		protected abstract void InternalSeed(uint[] seed);

		/// <summary>
		/// The generator's core method.
		/// <see cref="KansBase"/> implementations should override this method rather than <see cref="Random.Sample"/>.
		/// </summary>
		/// <returns>A random <see cref="uint"/> uniformly distributed from the set {0, 1, ..., <see cref="uint.MaxValue"/>}</returns>
		protected abstract uint NextU();

		public int RandIndex(int count) {
			if (count < 0) {
				throw new ArgumentOutOfRangeException("upper",
					String.Format("Upper bound ({0}) must be strictly positive", count)
				);
			}

			return RandInt(0, count-1);
		}

		public int RandInt(int lower, int upper) {
			if (lower > upper) {
				throw new ArgumentException(
					String.Format("Lower bound less than upper bound ({0} < {1})", lower, upper),
					"lower");
			}
			/* The following rejection sampling algorithm yields an exact uniform distribution if the generator
			is exactly uniform over its period. */
			long n = (upper - lower) + 1;
			long bound = ((long)uint.MaxValue + 1) - (((long)uint.MaxValue + 1) % n);
			uint num;
			do {
				num = NextU();
			} while (num >= bound);
			return (int)((n * (double)num / bound) + lower);
		}

		public double Random() {
			return NextU() * INV_232;
		}
	}
}
