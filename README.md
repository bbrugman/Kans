# Kans: better random numbers for .NET

## Motivation

At least as of .NET 4.7, the default implementation of `System.Random` has a number of deficiencies that may make it unsuitable in situations where high-quality (but not necessarily cryptographic-quality) random numbers are required, i.e. in simulation. These are:

* The underlying pseudo-random number generator (PRNG) is a small "lagged Fibonacci generator". While fast, this generator is not ideal from a statistical point of view, already failing two tests from [TestU01](http://simul.iro.umontreal.ca/testu01/tu01.html)'s SmallCrush.
* The algorithm used to generate random integers is not uniform, i.e. some integers may be up two twice as likely to be chosen than others, depending on how large the range is from which the random integer is chosen.
* Seeding (initializing) the generator can be performed only on construction of the `Random` object, and only a single integer may be supplied.

*Kans* provides an alternative source of pseudo-randomness that is free from these deficiencies. It also offers an easy means of creating drop-in replacements for `System.Random` and some useful list extension methods.

## Examples

Basic use:

```csharp
// construct new generator
Kans.IAltRandom rng = new Kans.WELL1024a();
// time-based seeding, as in System.Random
// note this is not done automatically
rng.Seed(Environment.TickCount);

// random double in [0.0, 1.0)
Console.WriteLine(rng.Random());
// dice roll
Console.WriteLine(rng.RandInt(1, 6));

IList<int> myList = new List<int>() { 1, 1, 2, 3, 5, 8, 13 };
// alias of RandInt(0, n-1) for random list items
Console.WriteLine(myList[rng.RandIndex(myList.Count)]);
```

Specific seeding and list methods:

```csharp
using Kans.ListAlgorithms;

Kans.IAltRandom rng = new Kans.WELL1024a();
rng.Seed(new int[] {12345, 67890});

IList<int> myList = new List<int>() { 1, 1, 2, 3, 5, 8, 13 };
// shuffle the list
myList.Shuffle(rng);
// choose a random subset of size 3 of the list
IList<int> choice = myList.Sample(3, rng);
```

Use in place of `System.Random`:

```csharp
Kans.IAltRandom rng = new Kans.WELL1024a();
rng.Seed(Environment.TickCount);
Random replacement = new Kans.SystemRandomWrapper(rng);
// use as you would
byte[] buffer = new byte[16];
replacement.nextBytes(buffer);
```

## Details

### Available generators

*Kans* offers three pseudo-random number generation algorithms:

* `WELL1024a`, a generator based on linear recurrences on bit strings, like the Mersenne Twister. Period of 2^1024-1, excellent statistical properties and decently fast. Good for general use.
* `LFIB4`, a multiple recursive generator. Similar in concept to the lagged Fibonacci generator of `System.Random`, but of much higher quality. Useful if WELL's bit linearity is somehow of concern.
* A generator based on the stream cipher `ChaCha`. For the paranoid.

### Seeding

Seeding (i.e. initializing the state of) a generator can be tricky, since poor seeding procedures may yield biases in the generator's output that may last for a great many iterations. *Kans* takes a low-risk approach: it initializes the state of its generators by feeding the input seed (that may be arbitrarily large) into a [Keccak](http://keccak.noekeon.org/) hash function and extracting the initial state from the output, making some corrections to ensure a valid starting state. This should provide a good "random" starting point for the generator regardless of the specific input seed.

However, *Kans* PRNG's are **not** seeded automatically with `Environment.TickCount` like `System.Random` instances are. Time-based seeding is not always a good choice, and since the hash-based initialization *is* more expensive than usual, you are to seed any `KansBase` instances explicitly (by calling either of the two `Seed` functions) before using them.

## License

*Kans* is subject to the MIT license (see the LICENSE file).