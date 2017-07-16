using System;
using System.Collections.Generic;

namespace Kans {
	/// <summary>
	/// Represents a reseedable pseudo-random number generator.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Implementations of this interface may be seeded with any amount of data. In comparison to <see cref="System.Random"/>, this interface exposes fewer random number generation methods, but no functionality is lost. On the contrary, instances of implementing classes have the small benefit of (by implementing <see cref="RandInt(int, int)"/> correctly) having the ability to generate an integer from the <em>full</em> range of possible integers, whereas the analogous methods of <see cref="System.Random"/> cannot produce <see cref="int.MaxValue"/>.
	/// </para>
	/// </remarks>
	public interface IAltRandom {

		/// <summary>
		/// Seeds the generator with a single integer.
		/// </summary>
		/// <param name="seed">The seed integer to be used.</param>
		/// <seealso cref="Seed(int[])"/>
		void Seed(int seed);

		/// <summary>
		/// Seeds the generator with an array of integers.
		/// </summary>
		/// <param name="seed">The seed array to be used.</param>
		void Seed(int[] seed);

		/// <summary>
		/// Returns a random double from 0.0 (inclusive) to 1.0 (exclusive).
		/// </summary>
		double Random();

		/// <summary>
		/// Returns a random integer from <paramref name="lower"/> to <paramref name="upper"/>, both inclusive.
		/// </summary>
		/// <param name="lower">The lowest possible return value.</param>
		/// <param name="upper">The highest possible return value.</param>
		int RandInt(int lower, int upper);

		/// <summary>
		/// Returns a random integer from 0 (inclusive) to <paramref name="count"/> (exclusive).
		/// </summary>
		/// <param name="count">One more than the highest possible return value.</param>
		int RandIndex(int count);
	}
}
