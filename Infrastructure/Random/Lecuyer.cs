using System;

namespace Infrastructure.Random
{
    /// <summary>
    /// <para>
    /// define two MCGs both implementable with Schrage's method on a 32-bit word
    /// </para>
    /// <remarks>
    /// see: http://www.shadlen.org/ichbin/random/generators.htm#lecuyer
    /// </remarks>
    /// </summary>
    public sealed class Lecuyer : RandomBase
    {
        #region Constructors

        public Lecuyer() : this(Convert.ToInt32(DateTime.Now.Ticks & 0x000000007FFFFFFF)) { }

        public Lecuyer(int seed)
            : base(seed)
        {
            x1 = seed % m1;
            x2 = seed % m2;
        }

        #endregion

        #region Member Variables

        private int x1, x2;

        private static readonly int a1 = 26756; // 2^2 * 6689
        private static readonly int m1 = 2147483647; // 2^31 - 1, a prime
        private static readonly int q1 = m1 / a1; // m / a
        private static readonly int r1 = m1 % a1; // m % a

        private static readonly int a2 = 30318; // 2 * 3 * 31 * 163
        private static readonly int m2 = 2145483479; // 2^31 - 2000169, a prime
        private static readonly int q2 = m2 / a2; // m / a
        private static readonly int r2 = m2 % a2; // m % a

        #endregion

        #region Methods

        public override int Next()
        {
            #region Declarations

            int x;

            #endregion

            #region Execution

            // advance first generator
            x1 = a1 * (x1 % q1) - r1 * (x1 / q1);

            if (x1 < 0)
                x1 += m1;

            // advance second generator
            x2 = a2 * (x2 % q2) - r2 * (x2 / q2);

            if (x2 < 0)
                x2 += m2;

            // combine results
            x = x1 - x2;

            if (x < 1)
                x += m1 - 1;

            return x;

            #endregion
        }

        #endregion

    }
}
