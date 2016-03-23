//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Controllers
{
    #region Triggers

    /// <summary>
    /// Falling edge detection.
    /// </summary>
    public class FTRIG
    {
        bool clk = false;
        bool q = false;

        /// <summary>
        /// Input signal.
        /// </summary>
        public void CLK(bool val)
        {
            q = clk && !val;

            clk = val;
        }

        /// <summary>
        /// Falling edge detection result.
        /// </summary>
        public bool Q { get { return q; } }
    }

    /// <summary>
    /// Rising edge detection.
    /// </summary>
    public class RTRIG
    {
        bool clk = true;
        bool q = true;

        /// <summary>
        /// Input signal.
        /// </summary>
        public void CLK(bool val)
        {
            q = !clk && val;

            clk = val;
        }

        /// <summary>
        /// Rising edge detection result.
        /// </summary>
        public bool Q { get { return q; } }
    }

    #endregion

    #region Counters

    /// <summary>
    /// Up counter.
    /// </summary>
    public class CTU
    {
        bool cu = false;
        bool reset = false;
        int cv = 0;

        /// <summary>
        /// Increments counter by 1 on rising edge provided CV is smaller than PV.
        /// </summary>
        public void CU(bool val)
        {
            if (!cu && val && !reset && CV < PV)
                ++cv;

            cu = val;
        }

        /// <summary>
        /// Resets counter to 0 when true.
        /// </summary>
        public void RESET(bool val)
        {
            if (val)
                cv = 0;

            reset = val;
        }

        /// <summary>
        /// Counter limit.
        /// </summary>
        public int PV { get; set; }

        /// <summary>
        /// Counter reached the limit.
        /// </summary>
        public bool Q { get { return CV >= PV && PV > 0; } }

        /// <summary>
        /// Current counter value.
        /// </summary>
        public int CV { get { return reset ? 0 : cv; } }
    }

    /// <summary>
    /// Down counter.
    /// </summary>
    public class CTD
    {
        bool cd = false;
        bool load = false;
        int cv = 0;

        /// <summary>
        /// Decrements the counter by 1 on rising edge.
        /// </summary>
        public void CD(bool val)
        {
            if (!cd && val && !load && CV > 0 )
                --cv;

            cd = val;
        }

        /// <summary>
        /// Loads the initial value when true.
        /// </summary>
        public void LOAD(bool val)
        {
            if (val)
                cv = PV;

            load = val;
        }

        /// <summary>
        /// Counter initial value.
        /// </summary>
        public int PV { private get; set; }

        /// <summary>
        /// Counter reached 0.
        /// </summary>
        public bool Q { get { return CV <= 0 && PV > 0; } }

        /// <summary>
        /// Current counter value.
        /// </summary>
        public int CV { get { return load ? PV : cv; } }
    }

    /// <summary>
    /// Up/down counter.
    /// </summary>
    public class CTUD
    {
        bool cu = false;
        bool cd = false;
        bool reset = false;
        bool load = false;
        int cv = 0;

        /// <summary>
        /// Increments counter by 1 on rising edge provided CV is smaller than PV.
        /// </summary>
        public void CU(bool val)
        {
            if (!cu && val && !reset && CV < PV)
                ++cv;

            cu = val;
        }

        /// <summary>
        /// Decrements the counter by 1 on rising edge.
        /// </summary>
        public void CD(bool val)
        {
            if (!cd && val && !load && !QD)
                --cv;

            cd = val;
        }

        /// <summary>
        /// Resets counter to 0 when true.
        /// </summary>
        public void RESET(bool val)
        {
            if (val)
                cv = 0;

            reset = val;
        }

        /// <summary>
        /// Loads the initial value when true.
        /// </summary>
        public void LOAD(bool val)
        {
            if (val)
                cv = PV;

            load = val;
        }

        /// <summary>
        /// Counter initial value / Counter limit.
        /// </summary>
        public int PV { private get; set; }

        /// <summary>
        /// True then the counter reached the limit.
        /// </summary>
        public bool QU { get { return CV >= PV && PV > 0; } }

        /// <summary>
        /// Counter reached 0.
        /// </summary>
        public bool QD { get { return CV <= 0 && PV > 0; } }

        /// <summary>
        /// Current counter value.
        /// </summary>
        public int CV { get { return reset ? 0 : cv; } }
    }

    #endregion

    #region Timers

    /// <summary>
    /// Off delay timer.
    /// </summary>
    public class TOF
    {
        Stopwatch sw = new Stopwatch();

        bool input = false;
        bool q = false;
        int et = 0;

        void CheckElapsedTime()
        {
            et = (int)sw.Elapsed.TotalMilliseconds;

            if (et > PT)
            {
                q = false;
                et = PT;
            }
        }

        /// <summary>
        /// Preset time (ms). If IN becomes TRUE before ET reaches the preset time, the elapsed time resets.
        /// </summary>
        public int PT { private get; set; }

        /// <summary>
        /// Timer operation condition.
        /// </summary>
        public bool IN
        {
            set
            {
                if (!input && value)
                {
                    sw.Reset();
                    q = true;
                }

                if (input && !value)
                    sw.Start();

                input = value;
            }
        }

        /// <summary>
        /// Timer output. Output Q becomes FALSE when IN is FALSE and the preset time is reached (ET > PT).
        /// </summary>
        public bool Q { get { CheckElapsedTime(); return q; } }

        /// <summary>
        /// Gets the elapsed time span.
        /// </summary>
        public int ET { get { CheckElapsedTime(); return et; } }
    }

    /// <summary>
    /// On delay timer.
    /// </summary>
    public class TON
    {
        Stopwatch sw = new Stopwatch();

        bool input = false;

        /// <summary>
        /// Preset time (ms). If IN becomes false before ET reaches the preset time, the elapsed time resets.
        /// </summary>
        public int PT { get; set; }

        /// <summary>
        /// Timer operation condition.
        /// </summary>
        public bool IN
        {
            set
            {
                if (value && !input)
                    sw.Start();

                if (!value && input)
                    sw.Reset();

                input = value;
            }
        }

        /// <summary>
        /// Timer output. Output Q becomes true when IN is true and the preset time is reached (ET > PT).
        /// </summary>
        public bool Q { get { return sw.Elapsed.TotalMilliseconds > PT; } }

        /// <summary>
        /// Elapsed time (ms). Elapsed time ET increases after IN becomes true.
        /// </summary>
        public int ET { get { return Q ? PT : (int)sw.Elapsed.TotalMilliseconds; } }
    }

    /// <summary>
    /// Pulse timer.
    /// </summary>
    public class TP
    {
        Stopwatch sw = new Stopwatch();

        bool input = false;
        int et = 0;

        void CheckElapsedTime()
        {
            if (sw.IsRunning)
                et = (int)sw.Elapsed.TotalMilliseconds;

            if (et > PT)
            {
                sw.Reset();
                et = PT;
            }
        }

        /// <summary>
        /// Length of the pulse (ms).
        /// </summary>
        public int PT { private get; set; }

        /// <summary>
        /// Timer operation condition.
        /// </summary>
        public bool IN
        {
            set
            {
                if (sw.IsRunning) return;

                if (!input && value && !sw.IsRunning)
                    sw.Start();

                if (input && !value && !sw.IsRunning)
                    et = 0;

                input = value;
            }
        }

        /// <summary>
        /// Output Q (pulse) becomes true when IN is true and remains true for the pulse duration specified by PT.
        /// </summary>
        public bool Q { get { CheckElapsedTime(); return sw.IsRunning; } }

        /// <summary>
        /// Elapsed time (ms). Elapsed time ET increases after IN becomes true.
        /// </summary>
        public int ET { get { CheckElapsedTime(); return et; } }
    }

    #endregion

    #region Control

    /// <summary>
    /// A proportional–integral–derivative controller (PID controller).
    /// </summary>
    public class PID
    {
        /// <summary>
        /// Integral term with Ki applied allowing changes to Ki while running.
        /// </summary>
        double iTerm;

        /// <summary>
        /// Previous PV (process variable) for the derivative term without the huge changes caused by changing the SP (setpoint).
        /// </summary>
        double prvPV;

        public PID()
        {
            this.Kp = 1.0f;
            this.Ki = 0.0f;
            this.Kd = 0.0f;

            CVLow = double.MinValue;
            CVHigh = double.MaxValue;
        }

        public PID(double kp, double ki, double kd)
        {
            this.Kp = kp;
            this.Ki = ki;
            this.Kd = kd;

            CVLow = double.MinValue;
            CVHigh = double.MaxValue;
        }

        static double Clamp(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        public void CLK(double sp, double pv, int elapsedMilliseconds)
        {
            SP = sp;
            PV = pv;

            //Convert milliseconds to seconds
            double dt = elapsedMilliseconds * 0.001; 

            //Calculate errors
            double error = SP - PV;

            //Remove derivative kick by using just the input to calculate the error, pay attention to the signals!
            double dError = (dt == 0.0) ? 0.0 : (prvPV - PV) / dt;

            prvPV = PV;

            //Softer tunning changes by putting Ki inside the integral
            iTerm += error * dt * Ki;

            //Clamp iTerm for reset windup
            iTerm = PID.Clamp(iTerm, CVLow, CVHigh);

            if (Ki == 0.0)
                iTerm = 0.0;

            OV = PID.Clamp(Kp * error + iTerm + Kd * dError, CVLow, CVHigh);
        }

        /// <summary>
        /// Setpoint.
        /// </summary>
        public double SP { get; private set; }

        /// <summary>
        /// Process variable.
        /// </summary>
        public double PV { get; private set; }

        /// <summary>
        /// Output.
        /// </summary>
        public double OV { get; private set; }

        /// <summary>
        /// Proportional gain.
        /// </summary>
        public double Kp { get; set; }

        /// <summary>
        /// Integrative gain.
        /// </summary>
        public double Ki { get; set; }

        /// <summary>
        /// Derivative gain.
        /// </summary>
        public double Kd { get; set; }

        /// <summary>
        /// Maximum value allowed for the output.
        /// </summary>
        public double CVHigh { get; set; }

        /// <summary>
        /// Minimum value allowed for the output.
        /// </summary>
        public double CVLow { get; set; }
    }

    #endregion
}
