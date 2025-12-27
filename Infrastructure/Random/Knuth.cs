using System;
using Infrastructure.Random;

namespace Infrastructure.Random
{
    /// <summary>
    /// <para>
    /// Based on subtractive generalized Fibonacci sequence
    /// </para>
    /// <remarks> see: http://www.shadlen.org/ichbin/random/generators.htm#knuth</remarks>
    /// </summary>
    public sealed class Knuth : RandomBase
    {
        #region Constructors

        public Knuth() : this(Convert.ToInt32(DateTime.Now.Ticks & 0x000000007FFFFFFF)) { }

        public Knuth(int seed)
            : base(seed)
        {
            #region Declarations

            int i, j, v, w;

            #endregion

            #region Initialization

            m_x = new int[N];
            v = seed;
            w = 1;

            #endregion

            #region Execution

            for (i = 0; i < N; i++)
            {
                j = (21 * i) % N; // randomize order a bit 

                m_x[j] = w;

                w = v - w;

                if (w < 1) 
                    w = w + (m - 1); 

                v = m_x[j];
            }

            // set the pointers
            m_p = N - 1;
            m_q = N - M - 1;

            // prime the pump
            for (i = 0; i < 3 * N; i++)
            {
                v = Next();
            }

            #endregion

        }

        #endregion

        #region Member Variables

        private static readonly int N = 55;
        private static readonly int M = 24;
        private static readonly int m = 2147483647; // 2^31 - 1
        private static readonly double mi = 1.0 / m;

        private int[] m_x;
        private int m_p, m_q;

        #endregion

        #region Methods

        public override int Next()
        {
            #region Declarations

            int y;

            #endregion

            #region Execution

            if (m_p == N - 1)
            {
                m_p = 0;
            }
            else
            {
                (m_p)++;
            }

            if (m_q == N - 1)
            {
                m_q = 0;
            }
            else
            {
                (m_q)++;
            }

            y = m_x[m_p] - m_x[m_q];

            if (y < 1)
            {
                y = y + (m - 1);
            }

            m_x[m_p] = y;

            return y;

            #endregion

        }

        #endregion

    }
}
