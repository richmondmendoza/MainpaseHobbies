using System;

namespace Infrastructure.Random
{
    /// <summary>
    /// <para>
    /// implements the GFSR ]
    /// x_n = x_(n-P) ^ x_(n-Q)
    //// which has period 2^P - 1
    /// </para>
    /// <remarks>
    /// see: http://www.shadlen.org/ichbin/random/generators.htm#r250
    /// </remarks>
    /// </summary>
    public class R250 : RandomBase
    {

        #region Constructors

        public R250() : this(Convert.ToInt32(DateTime.Now.Ticks & 0x000000007FFFFFFF)) { }

        public R250(int seed)
            : base(seed)
        {

            #region Declarations

            int i, j, k;
            uint[] e, a;

            #endregion

            #region Initialization

            x = new uint[P];

            // get the memory we need for initialization
            e = new uint[P]; // new unsigned long int[P];
            a = new uint[2 * P]; // new unsigned short int[2*P];


            for (i = 0; i < 7; i++)
                x[i] = GetBaseNextUInt32();

            p = Convert.ToInt32(P * GetBaseNextDouble());

            #endregion

            #region Execution

            // construct P linearly independent basis vectors
            for (i = 0; i < P; i++)
            {
                //mini_rand(s);
                k = L * i / P;
                e[i] = GetBaseNextUInt32() << k; // zeros to right of bit k
                e[i] = e[i] | (0x01U << k); // and a one at bit k
            }

            // construct 2P-1 coefficient bits
            for (i = 0; i < P; i++)
            {
                if (GetBaseNextDouble() > 0.5)
                {
                    a[i] = 1;
                }
                else
                {
                    a[i] = 0;
                }
            }
            for (i = P; i < 2 * P; i++)
            {
                a[i] = a[i - P] ^ a[i - Q];
            }

            // construct first P-1 entries (``matrix seed'') by
            // combining basis vectors according to coefficient bits
            for (i = 0; i < P; i++)
            {
                x[i] = 0;

                for (j = 0; j < P; j++)
                {
                    if (a[i + j] != 0)
                    {
                        x[i] = x[i] ^ e[j];
                    }
                }
            }

            // set pointer to last element
            p = P - 1;

            #endregion

        }

        #endregion

        #region Member Variables

        private static readonly int P = 250; // degree of larger term (250 or 7)
        private static readonly int Q = 103; // degree of smaller term (103 or 4)
        private static readonly int L = 32; // word length (32 or 3)

        private uint[] x;
        private int p;

        //static const int N = 250;
        //static const int M = 147;

        #endregion

        #region Methods

        public override int Next()
        {
            #region Declarations

            uint ret;
            int newP;

            #endregion

            #region Initialization

            newP = p;

            #endregion

            #region Execution

            // advance pointer
            if (newP == P - 1)
            {
                newP = 0;
            }
            else
            {
                newP++;
            }

            // compute next value
            if (newP < Q)
            {
                ret = x[newP] ^ x[(newP - Q + P)];
            }
            else
            {
                ret = x[newP] ^ x[(newP - Q)];
            }

            // replace value and pointer and return
            p = newP;
            x[p] = ret;

            return ConvertToInt32(ret & 0x7fffffffU);

            #endregion

        }

        #endregion
    }
}
