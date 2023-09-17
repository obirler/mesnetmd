using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace algebratest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var coeffs = new double[2, 2] { { 2, 3 }, { 1, 2 } };
            var vec = new double[2] { 1, 2 };

            var res = LinearEquationSolver(coeffs, vec);
            Console.WriteLine("Finished");
        }

        /// <summary>
        /// Solves the linear equation system using Gauss Elimination Method like
        /// Ax + By = C
        /// Dx + Ey = F
        /// where A, B, D, E are the coefficients array members and C and F are the results array members.
        /// </summary>
        /// <param name="coefficients">The coefficients array.</param>
        /// <param name="results">The results array.</param>
        /// <returns>The solution array.</returns>
        /// <exception cref="System.ArgumentException">Throws Argument Exception when coefficients and results sizes are different.</exception>
        public static double[] LinearEquationSolver(double[,] coefficients, double[] results)
        {
            if (coefficients.GetLength(0) != coefficients.GetLength(1) && coefficients.GetLength(0) != results.Length)
            {
                throw new ArgumentException("Different array sizes");
            }

            int count = coefficients.GetLength(0);

            for (int i = 0; i < count - 1; i++)
            {
                for (int j = i + 1; j < count; j++)
                {
                    double s = coefficients[j, i] / coefficients[i, i];
                    for (int k = i; k < count; k++)
                    {
                        coefficients[j, k] -= coefficients[i, k] * s;
                    }
                    results[j] -= results[i] * s;
                }
            }

            for (int i = count - 1; i >= 0; i--)
            {
                results[i] /= coefficients[i, i];
                coefficients[i, i] /= coefficients[i, i];
                for (int j = i - 1; j >= 0; j--)
                {
                    double s = coefficients[j, i] / coefficients[i, i];
                    coefficients[j, i] -= s;
                    results[j] -= results[i] * s;
                }
            }

            return Enumerable.Range(0, count).Select(i => results[i] / coefficients[i, i]).ToArray();
        }
    }
}
