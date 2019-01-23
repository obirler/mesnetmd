/*
========================================================================
    Copyright (C) 2016 Omer Birler.
    
    This file is part of Mesnet.

    Mesnet is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Mesnet is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Mesnet.  If not, see <http://www.gnu.org/licenses/>.
========================================================================
*/

using System;
using System.Linq;

namespace MesnetMD.Classes.Math
{
    /// <summary>
    /// 
    /// </summary>
    public static class Algebra
    {
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

        /// <summary>
        /// Multiplies a matrix and a vector.
        /// </summary>
        /// <param name="m">The matrix.</param>
        /// <param name="v">The vector.</param>
        /// <returns>Resulting multiplication vector.</returns>
        /// <exception cref="InvalidOperationException">Throws an invalid operaiton exception when the column of the matrix and the row of the vector is different.</exception>
        public static double[] DotProduct(double[,] m, double[] v)
        {
            if (m.GetLength(1) != v.GetLength(0))
            {
                throw new InvalidOperationException("This Matrix and this vector can not be multiplied!");
            }

            var result = new double[m.GetLength(0)];
            double sum = 0;

            ; for (int i = 0; i < m.GetLength(0); i++)
            {
                sum = 0;
                for (int j = 0; j < m.GetLength(1); j++)
                {
                    sum += m[i, j] * v[j];
                }

                result[i] = sum;
            }

            return result;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The summation of two vectors</returns>
        /// <exception cref="InvalidOperationException">Throws an invalid operaiton exception when the row of the vectors are different</exception>
        public static double[] AddVectors(double[] v1, double[] v2)
        {
            if (v1.GetLength(0) != v2.GetLength(0))
            {
                throw new InvalidOperationException("These two vectors can not be added");
            }

            var result = new double[v1.GetLength(0)];

            for (int i = 0; i < v1.GetLength(0); i++)
            {
                result[i] = v1[i] + v2[i];
            }

            return result;
        }

        /// <summary>
        /// Multiplies given two matrices.
        /// </summary>
        /// <param name="m1">The matrix 1.</param>
        /// <param name="m2">The matrix.</param>
        /// <returns>the multiplied matrix</returns>
        /// <exception cref="InvalidOperationException">This matrices can not be multiplied!</exception>
        public static double[,] MultiplyMatrix(double[,] m1, double[,] m2)
        {
            int rm1 = m1.GetLength(0);
            int cm1 = m1.GetLength(1);
            int rm2 = m2.GetLength(0);
            int cm2 = m2.GetLength(1);
            double temp = 0;
            double[,] result = new double[rm1, cm2];
            if (cm1 != rm2)
            {
                throw new InvalidOperationException("This matrices can not be multiplied!");
            }
            else
            {
                for (int i = 0; i < rm1; i++)
                {
                    for (int j = 0; j < cm2; j++)
                    {
                        temp = 0;
                        for (int k = 0; k < cm1; k++)
                        {
                            temp += m1[i, k] * m2[k, j];
                        }
                        result[i, j] = temp;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Returns Transpose of the specified matrix.
        /// </summary>
        /// <param name="matrix">The matrix to be transposed.</param>
        /// <returns>The transposed matrix</returns>
        public static double[,] Transpose(double[,] matrix)
        {
            int w = matrix.GetLength(0);
            int h = matrix.GetLength(1);

            double[,] result = new double[h, w];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    result[j, i] = matrix[i, j];
                }
            }

            return result;
        }

        /// <summary>
        /// Makes a number positive.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        public static double Positive(double number)
        {
            if (number < 0)
            {
                return -1 * number;
            }
            else
            {
                return number;
            }
        }

        /// <summary>
        /// Makes a number negative.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        public static double Negative(double number)
        {
            if (number > 0)
            {
                return -1 * number;
            }
            else
            {
                return number;
            }
        }

        public static double Combination(int n, int k)
        {
            double sum = 0;
            for (long i = 0; i < k; i++)
            {
                sum += System.Math.Log10(n - i);
                sum -= System.Math.Log10(i + 1);
            }
            return System.Math.Pow(10, sum);
        }

        /// <summary>
        /// Returns the radian of a degree value.
        /// </summary>
        /// <param name="degree">The degree to be converted</param>
        /// <returns></returns>
        public static double ToRadian(double degree)
        {
            return degree * System.Math.PI / 180;
        }

        /// <summary>
        /// Returns sine of an angle in degree.
        /// </summary>
        /// <param name="degree">The degree.</param>
        /// <returns></returns>
        public static double SinD(double degree)
        {
            return System.Math.Sin(ToRadian(degree));
        }

        /// <summary>
        /// Returns cosine of an angle in degree.
        /// </summary>
        /// <param name="degree">The degree.</param>
        /// <returns></returns>
        public static double CosD(double degree)
        {
            return System.Math.Cos(ToRadian(degree));
        }
    }
}
