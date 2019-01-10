using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace magneto
{
    public class Spline32D
    {
        protected double[][][][] s;
        protected double[] x, y;

        public Spline32D() { }

        /// <summary>
        /// Constructor of the class.
        /// </summary>
        /// <param name="x">array of x-data</param>
        /// <param name="y">array of y-data</param>
        /// <param name="f">array of function values. First index represents x-data; second index represents y-data.</param>
        public Spline32D(double[] x, double[] y, double[][] f)
        {
            this.x = x;
            this.y = y;
            int n = x.Length;
            int m = y.Length;

            this.s = new double[n][][][];
            for (int i = 0; i < n; i++)
            {
                this.s[i] = new double[m][][];
                for (int j = 0; j < m; j++)
                {
                    this.s[i][j] = new double[4][];
                    for (int k = 0; k < 4; k++)
                    {
                        this.s[i][j][k] = new double[4];
                    }
                }
            }

            //
            Spline31D spline31d;
            double[] spMas3, spMas2, spMas1, spMas0;
            for (int i = 0; i < n; i++)
            {
                spline31d = new Spline31D(y, f[i]);
                spMas3 = spline31d.masD3;
                spMas2 = spline31d.masD2;
                spMas1 = spline31d.masD1;
                spMas0 = spline31d.masD0;
                for (int j = 0; j < m; j++)
                {
                    this.s[i][j][0][0] = spMas0[j];
                    this.s[i][j][0][1] = spMas1[j];
                    this.s[i][j][0][2] = spMas2[j];
                    this.s[i][j][0][3] = spMas3[j];
                }
            }

            double[] masf = new double[n];
            for (int k = 0; k < 4; k++)
            {
                for (int j = 0; j < m; j++)
                {
                    for (int i = 0; i < n; i++)
                    {
                        masf[i] = this.s[i][j][0][k];
                    }
                    spline31d = new Spline31D(x, masf);
                    spMas0 = spline31d.masD0;
                    spMas1 = spline31d.masD1;
                    spMas2 = spline31d.masD2;
                    spMas3 = spline31d.masD3;
                    for (int i = 0; i < n; i++)
                    {
                        this.s[i][j][0][k] = spMas0[i];
                        this.s[i][j][1][k] = spMas1[i];
                        this.s[i][j][2][k] = spMas2[i];
                        this.s[i][j][3][k] = spMas3[i];
                    }
                }
            }
        }

        public double Interp(double x0, double y0)
        {
            int i;
            for (i = 0; i < x.Length - 2; i++)
            {
                if (x0 <= x[i + 1])
                {
                    break;
                }
            }

            int j;
            for (j = 0; j < y.Length - 2; j++)
            {
                if (y0 <= y[j + 1])
                {
                    break;
                }
            }

            double dx = x0 - x[i];
            double dy = y0 - y[j];
            double dx2 = dx * dx;
            double dy2 = dy * dy;
            double dx3 = dx2 * dx;
            double dy3 = dy2 * dy;

            double f = s[i][j][3][3] * dx3 * dy3 / 36 +
                s[i][j][3][2] * dx3 * dy2 / 12 +
                s[i][j][3][1] * dx3 * dy / 6 +
                s[i][j][3][0] * dx3 / 6 +
                s[i][j][2][3] * dx2 * dy3 / 12 +
                s[i][j][2][2] * dx2 * dy2 / 4 +
                s[i][j][2][1] * dx2 * dy / 2 +
                s[i][j][2][0] * dx2 / 2 +
                s[i][j][1][3] * dx * dy3 / 6 +
                s[i][j][1][2] * dx * dy2 / 2 +
                s[i][j][1][1] * dx * dy +
                s[i][j][1][0] * dx +
                s[i][j][0][3] * dy3 / 6 +
                s[i][j][0][2] * dy2 / 2 +
                s[i][j][0][1] * dy +
                s[i][j][0][0];

            return f;
        }
    }
}
