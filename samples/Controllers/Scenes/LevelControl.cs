//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using System;

using EngineIO;

namespace Controllers
{
    public class LevelControl : Controller
    {
        MemoryFloat fillValve = MemoryMap.Instance.GetFloat("Fill valve", MemoryType.Output);
        MemoryInt spDisplay = MemoryMap.Instance.GetInt("SP", MemoryType.Output);
        MemoryInt pvDisplay = MemoryMap.Instance.GetInt("PV", MemoryType.Output);
        MemoryFloat levelMeter = MemoryMap.Instance.GetFloat("Level meter", MemoryType.Input);
        MemoryFloat setpoint = MemoryMap.Instance.GetFloat("Setpoint", MemoryType.Input);

        PID controller;

        public LevelControl()
        {
            controller = new PID(10, 0.35, 0);
            controller.CVLow = 0;
            controller.CVHigh = 10;

            fillValve.Value = 0;
        }

        public override void Execute(int elapsedTime)
        {
            controller.CLK(setpoint.Value, levelMeter.Value, elapsedTime);

            fillValve.Value = (float)controller.OV;

            spDisplay.Value = (int)(setpoint.Value * 300 / 10);
            pvDisplay.Value = (int)(levelMeter.Value * 300 / 10);

            Console.WriteLine("OV: " + controller.OV.ToString());
        }
    }
}
