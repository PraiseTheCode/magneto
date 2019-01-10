using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace magneto
{
    class Program
    {
        const double const_c = 2.99792458e10, e = 4.8032e-10, const_me = 9.10953e-28, const_k = 1.38064853e-16, eV_to_erg = 1.60218e-12; 
        static double[] ww, thetas, temps, lambdas;
        static double[][][] ac_o, ac_e;

        static void Load(string path)
        {
            StreamReader sr = new StreamReader(path);
            string s;
            s = sr.ReadLine();
            s = sr.ReadLine();
            string[] mas = s.Split(new string[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            double ww1 = Convert.ToDouble(mas[0].Replace(".", ","));
            double step = Convert.ToDouble(mas[1].Replace(".", ","));
            ww = new double[Convert.ToInt32(mas[2])];
            for (int i = 0; i < ww.Length; i++)
                ww[i] = ww1 + i * step;

            s = sr.ReadLine();
            mas = s.Split(new string[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            temps = new double[Convert.ToInt32(mas[0])];
            for (int i = 0; i < temps.Length; i++)
                temps[i] = Convert.ToDouble(mas[i + 1].Replace(".", ","));

            s = sr.ReadLine();
            mas = s.Split(new string[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            thetas = new double[Convert.ToInt32(mas[0])];
            for (int i = 0; i < thetas.Length; i++)
                thetas[i] = Convert.ToDouble(mas[i + 1].Replace(".", ","));

            ac_o = new double[temps.Length][][];
            ac_e = new double[temps.Length][][];
            for (int i = 0; i < temps.Length; i++)
            {
                ac_o[i] = new double[thetas.Length][];
                ac_e[i] = new double[thetas.Length][];
                for (int j = 0; j < thetas.Length; j++)
                {
                    ac_o[i][j] = new double[ww.Length];
                    ac_e[i][j] = new double[ww.Length];
                }
            }

            for (int i = 0; i < temps.Length; i++)
            {
                for (int j = 0; j < ww.Length; j++)
                {
                    s = sr.ReadLine();
                    mas = s.Split(new string[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int k = 0; k < thetas.Length; k++)
                    {
                        ac_o[i][k][j] = Convert.ToDouble(mas[k + 1].Replace(".", ","));
                        ac_e[i][k][j] = Convert.ToDouble(mas[k + 1 + thetas.Length].Replace(".", ","));

                        ac_o[i][k][j] = (ac_o[i][k][j] > 0) ? Math.Log(ac_o[i][k][j]) : 1e-200;
                        ac_e[i][k][j] = (ac_e[i][k][j] > 0) ? Math.Log(ac_e[i][k][j]) : 1e-200;
                    }
                }
            }
            sr.Close();
        }

        static void Main(string[] args)
        {
            string file_ac; // file with ac
            string file_sp; // output file
            int iT; // No. Te
            double ang_tet, B, size_par; // parameters
            double left_wl; // shortest wavelength
            double right_wl; // longest wavelength
            double step_wl; // step

            StreamReader nsr = new StreamReader("var.txt");

            file_ac = nsr.ReadLine();
            file_sp = nsr.ReadLine();
            iT = Convert.ToInt32(nsr.ReadLine().Replace(".", ","));
            /*ang_tet = Convert.ToDouble(nsr.ReadLine().Replace(".", ",")); 
            ang_tet = ang_tet * Math.PI / 180;
            B = Convert.ToDouble(nsr.ReadLine().Replace(".", ","));
            B = B * 1e6;
            size_par = Convert.ToDouble(nsr.ReadLine().Replace(".", ","));
            left_wl = Convert.ToDouble(nsr.ReadLine().Replace(".", ",")); 
            left_wl = left_wl * 1e-8;
            right_wl = Convert.ToDouble(nsr.ReadLine().Replace(".", ","));
            right_wl = right_wl * 1e-8;
            step_wl = Convert.ToDouble(nsr.ReadLine().Replace(".", ","));
            step_wl = step_wl * 1e-8;

            nsr.Close();*/

            double lB = 38e6, rB = 40e6, lTheta = 77 * Math.PI / 180, rTheta = 87 * Math.PI / 180, lL = 210, rL = 410;
            double dB = 0.1e6, dTheta = 0.5*Math.PI/180, dL = 10;

            int n_B = (int)((rB - lB) / dB);
            int n_theta = (int)((rTheta - lTheta) / dTheta);
            int n_L = (int)((rL - lL) / dL);
            int nn = n_B * n_theta * n_L;
            Console.WriteLine(nn);
            double[][] ini = new double[nn][];
            for (int i = 0; i < nn; i++)
                ini[i] = new double[4];
            int p = 0;
            for (int i = 0; i < n_B; i++)
            {
                for (int j = 0; j < n_theta; j++)
                {
                    for (int s = 0; s < n_L; s++)
                    {
                        ini[p][0] = lB + i * dB;
                        ini[p][1] = lTheta + j * dTheta;
                        ini[p][2] = lL + s * dL;
                        ini[p][3] = p;
                        p++;
                    }
                }
            }
            Console.WriteLine(ini[4408][0]);
            Console.WriteLine(ini[4408][1]*180/Math.PI);
            Console.WriteLine(ini[4408][2]);
            /*
            Load(file_ac);
            left_wl = 3000 * 1e-8;
            right_wl = 8000 * 1e-8;
            step_wl = 1 * 1e-8;
            double wc, w, Irj, II, I_o, I_e, pe_o, pe_e;
            lambdas = new double[Convert.ToInt32(1 + (right_wl - left_wl) / step_wl)];
            
            Spline32D spline_o = new Spline32D(thetas, ww, ac_o[iT]);
            Spline32D spline_e = new Spline32D(thetas, ww, ac_e[iT]);

            double[] x = new double[3];

            int pp = 0;
            for (int ii = 0; ii < nn; ii++)
            {
                x = ini[ii];
                B = x[0];
                ang_tet = x[1];
                size_par = x[2];

                file_sp = "D:/spec0911/izv/28/" + pp + ".txt";

                StreamWriter sw = new StreamWriter(file_sp);

                for (int i = 0; i < lambdas.Length; i++)
                {
                    lambdas[i] = left_wl + step_wl * i;
                    wc = e * B / const_me / const_c;
                    w = 2 * Math.PI * const_c / lambdas[i];
                    double a_o = spline_o.Interp(ang_tet, w / wc), a_e = spline_e.Interp(ang_tet, w / wc);
                    Irj = const_k * temps[iT] * 1e3 * eV_to_erg * Math.Pow(w, 2) / (const_k * 8 * Math.Pow(Math.PI, 3) * Math.Pow(const_c, 2));
                    pe_o = -1 * Math.Exp(a_o) * size_par;
                    pe_e = -1 * Math.Exp(a_e) * size_par;
                    I_o = Irj * (1 - Math.Exp(pe_o));
                    I_e = Irj * (1 - Math.Exp(pe_e));
                    //II = I_o + I_e;   // per Hz
                    II = (I_o + I_e) * const_c / Math.Pow(lambdas[i], 2); // per angstrom
                    lambdas[i] = lambdas[i] * 1e8;
                    sw.WriteLine(lambdas[i].ToString().Replace(",", ".") + "\t" + II.ToString().Replace(",", "."));
                }
                Console.WriteLine(pp);
                sw.Close();
                pp++;
            }
            //*/

            Console.WriteLine();
            Console.Write("happy end");
            Console.ReadKey();
        }
    }
}
