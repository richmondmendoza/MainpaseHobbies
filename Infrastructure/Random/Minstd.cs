using System;

namespace Infrastructure.Random
{
    public class Minstd: RandomBase
    {
        #region Constructors

        public Minstd() : this(Convert.ToInt32(DateTime.Now.Ticks & 0x000000007FFFFFFF)) { }

        public Minstd(int seed)
            : base(seed)
        {
            i = GetBaseNextInt32();
        }

        #endregion

        #region Member Variables

        private static readonly int a = 48271; // multiplier (16807 also works);
        private static readonly int m = 2147483647; // modulus 2^31-1, a prime
        private static readonly int q = m / a; // m / a
        private static readonly int r = m % a; // m % a

        private int i;

        #endregion


        public override int Next()
        {
            #region Execution

            i = a * (i % q) - r * (i / q);

            if (i < 0) 
                i = i + m; 

            return (i);

            #endregion
        }
    }
}
