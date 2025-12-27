using System;

namespace Infrastructure.Random
{
    /// <summary>
    /// implements the tGFSR based on
    /// x_n = ( x_(n-N) >> 1 ) ^ x_(n-M)
    /// plus an extra twist,
    /// which has period 2^(32*N) - 1
    /// <remarks>
    /// see: http://www.shadlen.org/ichbin/random/generators.htm#tt800
    /// </remarks>
    /// </summary>
    public class TT800 : RandomBase
    {
        #region Constructors

        public TT800() : this(Convert.ToInt32(DateTime.Now.Ticks & 0x000000007FFFFFFF)) { }

        public TT800(int seed)
            : base(seed)
        {
            uint[] xx;
            int i;

            x = new uint[N];

            xx = new uint[]{
                0x95f24dab, 0x0b685215, 0xe76ccae7, 0xaf3ec239, 0x715fad23,
                0x24a590ad, 0x69e4b5ef, 0xbf456141, 0x96bc1b7b, 0xa7bdf825,
                0xc1de75b7, 0x8858a9c9, 0x2da87693, 0xb657f9dd, 0xffdc8a9f,
                0x8121da71, 0x8b823ecb, 0x885d05f5, 0x4e20cd47, 0x5a9ad5d9,
                0x512c0c03, 0xea857ccd, 0x4cc1d30f, 0x8891a8a1, 0xa6b7aadb };


            for (i = 0; i < N; i++)
            {
                x[i] = xx[i] ^ GetBaseNextUInt32();
            }

            p = N - 1;
            q = N - M - 1;

        }

        #endregion

        #region Member Variables

        private static readonly int N = 25;
        private static readonly int M = 18;

        private static readonly uint a = 0x8ebfd028U;
        private static readonly int s = 7;
        private static readonly uint b = 0x2b5b2500U;
        private static readonly int t = 15;
        private static readonly uint c = 0xdb8b0000U;
        private static readonly int l = 16;

        private uint[] x;
        private int p, q;

        #endregion

        #region Methods

        public override int Next()
        {
            #region Declarations

            uint y, z;

            #endregion

            #region Execution

            if (p == N - 1)
            {
                p = 0;
            }
            else
            {
                (p)++;
            }
            
            if (q == N - 1)
            {
                q = 0;
            }
            else
            {
                (q)++;
            }

            z = x[(p)];
            y = x[(q)] ^ (z >> 1);

            if (z % 2 != 0) 
            { 
                y ^= a; 
            }

            if (p == N - 1)
            {
                x[0] = y;
            }
            else
            {
                x[(p) + 1] = y;
            }

            y ^= ((y << s) & b);
            y ^= ((y << t) & c);
            y ^= (y >> l); // improves bits

            return ConvertToInt32(y & 0x7FFFFFFF);

            #endregion

        }

        #endregion
    }
}
