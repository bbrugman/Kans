using System;
using System.Collections.Generic;

namespace Kans.ListAlgorithms {
	/// <summary>
	/// Useful <see cref="IList{T}"/> extension methods for common operations on lists that require randomness.
	/// </summary>
	public static class ListAlgorithms {
		/// <summary>
		/// Randomizes the order of the list in-place with the Fisher-Yates shuffle.
		/// </summary>
		/// <param name="rng">The random number generator to use.</param>
		/// <returns>The shuffled list.</returns>
		public static IList<T> Shuffle<T>(this IList<T> list, IAltRandom rng) {
			for (int i = 0; i < list.Count - 2; i++) {
				int j = rng.RandInt(i, list.Count - 1);
				T temp = list[i];
				list[i] = list[j];
				list[j] = temp;
			}
			return list;
		}

		/// <summary>
		/// Samples from the list <em>without replacement</em>.
		/// </summary>
		/// <param name="number">The number of samples to take.</param>
		/// <param name="rng">The random number generator to use.</param>
		/// <returns>A list of elements from this list with unique indices.</returns>
		public static IList<T> Sample<T>(this IList<T> list, int number, IAltRandom rng) {
			if (list == null) throw new ArgumentNullException("list");
			if (list.Count < number) {
				throw new ArgumentException(
					String.Format("List has only {0} items; cannot take {1} samples", list.Count, number)
				);
			}

			IList<T> pool = new List<T>(list);
			IList<T> result = new List<T>(new T[number]);
			for (int i = 0; i < number; i++) {
				int j = rng.RandIndex(pool.Count - i);
				result[i] = pool[j];
				pool[j] = pool[pool.Count - i - 1];
			}
			return result;
		}
	}
}
