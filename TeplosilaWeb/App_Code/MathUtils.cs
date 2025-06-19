using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// математические функции
/// </summary>
public static class MathUtils
{
    public static void Prgl(double t, double d, ref double rr, /*ref double nn, ref double ll, ref double pr,*/ ref double cp)
    {
        //ll = (0.5613 - 0.00363 * d) + (20.1 - 0.372 * d) * t * 0.0001 - (7.39 + 0.071 * d - 0.00326 * d * d) * 0.000001 * t * t;
        //nn = 1.4 * 0.001 * 0.00025 * Math.Exp(233.16 / (d + 51.48)) * Math.Exp((640.24 + 19.235 * d - 0.11645 * d * d) / (t + 150));
        rr = (999.63 + 1.4 * d - 0.0073 * d * d) - (0.08446 + 0.01715 * d - 0.0001277 * d * d) * t - 0.0029 * t * t;
        cp = 1000 * ((4.222 - 0.00752 * d - 0.00011 * d * d) + (-9.712 + 0.5964 * d) * t / 10000 + (8.383 + 0.0877 * d - 0.001457 * d * d) * 0.000001 * t * t);
        //nn = nn / rr;
        //pr = nn * cp * rr / ll;
    }

    public static void Etgl(double t, double d, ref double rr, /*ref double nn, ref double ll, ref double pr,*/ ref double cp)
    {
        //ll = (0.565 - 0.00303 * d) + (8.9 - 0.21 * d) * t * 0.0001;
        //nn = 0.001 / 1.05 * (0.472 - 0.00238 * d + 0.00024 * d * d + (4.65 - 0.212 * d + 0.00595 * d * d) * Math.Exp(-t / 20.5));
        rr = (1001.9 + 1.73 * d) - (0.1784 + 0.0062 * d) * t - 0.00107 * t * t;
        cp = 1000 * ((4.289 - 0.0212 * d) + (-8.84 + 1.404 * d - 0.00987 * d * d) * t / 10000);
        //nn = nn / rr;
        //pr = nn * cp * rr / ll;
    }

    public static void Water(double t, ref double rr) //, ref double nn, ref double ll, ref double pr, ref double cp)
    {
        rr = 1001.37102 - 0.18338 * t - 0.000683948 * Math.Pow(t, 2) - 0.0000290502 * Math.Pow(t, 3) + 0.000000145068 * Math.Pow(t, 4) - 0.000000000231169 * Math.Pow(t, 5);
        //nn = 1.05 * 0.000001 * (1.76764 - 0.05177 * t + 0.000800965 * Math.Pow(t, 2) - 0.00000598494 * Math.Pow(t, 3) + 0.0000000167612 * Math.Pow(t, 4));
        //cp = 1000000 / rr * (4.23825 - 0.00331 * t + 0.0000352307 * Math.Pow(t, 2) - 0.000000216299 * Math.Pow(t, 3));
        //ll = 0.01 * (55.12817 + 0.26992 * t - 0.00179 * Math.Pow(t, 2) + 0.00000482756 * Math.Pow(t, 3) - 0.00000000632 * Math.Pow(t, 4));
        //pr = 13.04883 - 0.40964 * t + 0.00639 * Math.Pow(t, 2) - 0.0000474596 * Math.Pow(t, 3) + 0.000000132131 * Math.Pow(t, 4);
    }

    /// <summary>
    /// Расчет удельной теплоемкости воды для заданной температуры.
    /// </summary>
    /// <param name="t">Средняя температура</param>
    /// <returns></returns>
    public static double WaterCP(double t)
    {
        double rr = 1001.37102 - 0.18338 * t - 0.000683948 * Math.Pow(t, 2) - 0.0000290502 * Math.Pow(t, 3) + 0.000000145068 * Math.Pow(t, 4) - 0.000000000231169 * Math.Pow(t, 5);
        //double nn = 1.05 * 0.000001 * (1.76764 - 0.05177 * t + 0.000800965 * Math.Pow(t, 2) - 0.00000598494 * Math.Pow(t, 3) + 0.0000000167612 * Math.Pow(t, 4));
        double cp = 1000000 / rr * (4.23825 - 0.00331 * t + 0.0000352307 * Math.Pow(t, 2) - 0.000000216299 * Math.Pow(t, 3));
        //double ll = 0.01 * (55.12817 + 0.26992 * t - 0.00179 * Math.Pow(t, 2) + 0.00000482756 * Math.Pow(t, 3) - 0.00000000632 * Math.Pow(t, 4));
        //double pr = 13.04883 - 0.40964 * t + 0.00639 * Math.Pow(t, 2) - 0.0000474596 * Math.Pow(t, 3) + 0.000000132131 * Math.Pow(t, 4);
        return cp;
    }

    public static double getPSbyT(double t)
    {
        return Math.Pow(t / 103, 1 / 0.242) - 0.892;
    }

    public static double getTbyPS(double ps)
    {
        return 103 * Math.Pow(ps + 0.892, 0.242);
    }
}