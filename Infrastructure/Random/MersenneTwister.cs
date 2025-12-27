using System;

namespace Infrastructure.Random
{
    /// <summary>
    /// <para>
    /// An implementation of the Mersenne Twister pseudo random number generator
    /// </para>
    /// <remarks>
    /// see:
    /// http://www.qbrundage.com/michaelb/pubs/essays/random_number_generation.html
    /// and
    /// http://portal.acm.org/citation.cfm?id=321765.321777&coll=GUIDE&dl=ACM&idx=J401&part=periodical&WantType=periodical&title=Journal%20of%20the%20ACM%20(JACM)
    /// </remarks>
    /// </summary>
    public sealed class MersenneTwister : RandomBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MersenneTwister"/> class.
        /// </summary>
        public MersenneTwister() : this(Convert.ToInt32(DateTime.Now.Ticks & 0x000000007FFFFFFF)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MersenneTwister"/> class.
        /// </summary>
        /// <param name="seed">The seed.</param>
        public MersenneTwister(int seed):base(seed)
        {
            #region Declarations

            int i;

            #endregion

            #region Execution

            mt_buffer = new uint[624];

            for (i = 0; i < 624; i++)
                mt_buffer[i] = GetBaseNextUInt32();

            mt_index = 0;

            #endregion
        }

        #endregion

        #region Member Variables

        private uint mt_index;
        private uint[] mt_buffer;

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

            uint i, s;

            // Convert.ToUInt32(0x80000000) ==	2147483648U
            // Convert.ToUInt32(0x7FFFFFFF) ==	2147483647U
            // Convert.ToUInt32(0x9908B0DF)	==  2567483615U

            #endregion

            #region Execution

            if (mt_index == 624)
            {
                mt_index = 0;
                i = 0;

                for (; i < 624 - 397; i++)
                {
                    s = (mt_buffer[i] & 2147483648U) | (mt_buffer[i + 1] & 2147483647U);


                    mt_buffer[i] = mt_buffer[i + 397] ^ (s >> 1) ^ ((s & 1U) * 2567483615U);
                }
                for (; i < 623; i++)
                {
                    s = (mt_buffer[i] & 2147483648U) | (mt_buffer[i + 1] & 2147483647U);
                    mt_buffer[i] = mt_buffer[i - (624 - 397)] ^ (s >> 1) ^ ((s & 1U) * 2567483615U);
                }

                s = (mt_buffer[623] & 2147483648U) | (mt_buffer[0] & 2147483647);
                mt_buffer[623] = mt_buffer[396] ^ (s >> 1) ^ ((s & 1U) * 2567483615U);
            }

            return ConvertToInt32(mt_buffer[mt_index++] & 2147483647U);

            #endregion
        }

        #endregion

    }
}
