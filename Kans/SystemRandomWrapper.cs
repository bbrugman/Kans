using System;

namespace Kans {
	/// <summary>
	/// Represents a <see cref="System.Random"/> instance that encapsulates an underlying <see cref="IAltRandom"/> instance. The underlying instance must be seeded prior to wrapping.
	/// </summary>
	public class SystemRandomWrapper : Random {

		private IAltRandom rng;

		/// <summary>
		/// Constructs a wrapper around an <see cref="IAltRandom"/> object.
		/// The resulting objects behaves as an ordinary <see cref="System.Random"/> instance as much as possible,
		/// except for using better algorithms.
		/// </summary>
		/// <param name="wrapped">The <see cref="IAltRandom"/> object to be encapsulated.</param>
		public SystemRandomWrapper(IAltRandom wrapped) {
			rng = wrapped;
		}

		public override int Next() {
			return rng.RandIndex(int.MaxValue);
		}

		public override int Next(int maxValue) {
			if (maxValue == 0) return 0;
			return rng.RandIndex(maxValue);
		}

		public override int Next(int minValue, int maxValue) {
			if (minValue > maxValue) {
				throw new ArgumentOutOfRangeException("minValue", "minValue > maxValue");
			} else if(minValue == maxValue) {
				return minValue;
			}
			return rng.RandInt(minValue, maxValue-1);
		}

		public override double NextDouble() {
			return rng.Random();
		}

		public override void NextBytes(byte[] buffer) {
			if (buffer == null) throw new ArgumentNullException("buffer");
			for (int i = 0; i < buffer.Length; i++) {
				buffer[i] = (byte)rng.RandInt(byte.MinValue, byte.MaxValue);
			}
		}
	}
}
