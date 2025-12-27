using System;

namespace Infrastructure.Random
{
    /// <summary>
    /// <para>
    /// An implementation of the Generalized Feedback Shift Register (GFSR) R250_521 pseudo random number generator
    /// </para>
    /// <remarks>
    /// see: 
    /// http://portal.acm.org/citation.cfm?id=321765.321777&coll=GUIDE&dl=ACM&idx=J401&part=periodical&WantType=periodical&title=Journal%20of%20the%20ACM%20(JACM)
    /// </remarks>
    /// </summary>
    public sealed class R250_521 : RandomBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:R250_521"/> class.
        /// </summary>
        public R250_521() : this(Convert.ToInt32(DateTime.Now.Ticks & 0x000000007FFFFFFF)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:R250_521"/> class.
        /// </summary>
        /// <param name="seed">The seed.</param>
        public R250_521(int seed): base(seed)
        {
            #region Declarations

            uint i, mask1, mask2;

            // Convert.ToUInt32(0x7FFFFFFF)  ==	2147483647U 

            #endregion

            #region Execution

            r250_buffer = new uint[250];
            r521_buffer = new uint[521];
            i = 521;
            mask1 = 0x01;
            mask2 = 2147483647U;

            while (i-- > 250)
            {
                r521_buffer[i] = GetBaseNextUInt32();
            }

            while (i-- > 31)
            {
                r250_buffer[i] = GetBaseNextUInt32();
                r521_buffer[i] = GetBaseNextUInt32();
            }

            // Establish linear independence of the bit columns
            // by setting the diagonal bits and clearing all bits above

            while (i-- > 0)
            {
                r250_buffer[i] = (GetBaseNextUInt32() | mask1) & mask2;
                r521_buffer[i] = (GetBaseNextUInt32() | mask1) & mask2;
                mask2 ^= mask1;
                mask1 >>= 1;
            }

            r250_buffer[0] = mask1;
            r521_buffer[0] = mask2;
            r250_index = 0;
            r521_index = 0;

            #endregion
        }

        #endregion

        #region Member Variables

        private int r250_index, r521_index;
        private uint[] r250_buffer, r521_buffer;

        #endregion

        #region Overrides

        /// <summary>
        /// Returns a nonnegative random number.
        /// </summary>
        /// <returns>
        /// A 32-bit signed uinteger greater than or equal to zero and less than <see cref="F:System.uint32.MaxValue"></see>.
        /// </returns>
        public override int Next()
        {
            #region Declarations

            int i1, i2, j1, j2;
            uint r, s;

            #endregion

            #region Execution

            i1 = r250_index;
            i2 = r521_index;

            j1 = i1 - 146; // (249 - 103)

            if (j1 < 0)
                j1 = i1 + 103;

            j2 = i2 - 352; // (520 - 168)

            if (j2 < 0)
                j2 = i2 + 167;

            r = r250_buffer[j1] ^ r250_buffer[i1];
            r250_buffer[i1] = r;

            s = r521_buffer[j2] ^ r521_buffer[i2];
            r521_buffer[i2] = s;

            i1 = (i1 == 249) ? 0: (i1 + 1);
            r250_index = i1;

            i2 = (i2 == 520) ? 0 :(i2 + 1);
            r521_index = i2;

            return ConvertToInt32(r ^ s);

            #endregion
        }

        #endregion

    }
}
