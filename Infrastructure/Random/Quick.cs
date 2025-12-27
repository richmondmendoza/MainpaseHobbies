using System;

namespace Infrastructure.Random
{
    /// <summary>
    /// <para>
    /// very quick linear congruential generator which takes its modulus
    /// operation from the unflagged overflow of a 32-bit register
    /// </para>
    /// <remarks>
    /// see: http://www.shadlen.org/ichbin/random/generators.htm#quick
    /// </remarks>
    /// </summary>
    public class Quick : RandomBase
    {
        #region Constructors

        public Quick() : this(Convert.ToInt32(DateTime.Now.Ticks & 0x000000007FFFFFFF)) { }

        public Quick(int seed)
            : base(seed)
        {
            i = Convert.ToUInt64(GetBaseNextInt32());
        }

        #endregion

        #region Member Variables

        private static readonly uint a = 1099087573;

        private ulong i;

        #endregion

        #region Methods

        public override int Next()
        {
            #region Execution

            i = a * i; // overflow occurs here!
            return ConvertToInt32(i);

            #endregion

        }

        #endregion
    }
}
