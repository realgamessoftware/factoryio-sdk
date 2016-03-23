//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using EngineIO;

namespace Controllers
{
    public class FromAToB : Controller
    {
        MemoryBit conveyor = MemoryMap.Instance.GetBit("Conveyor", MemoryType.Output);

        MemoryBit sensor = MemoryMap.Instance.GetBit("Sensor", MemoryType.Input);

        public FromAToB()
        {
            conveyor.Value = false;
        }

        public override void Execute(int elapsedMilliseconds)
        {
            conveyor.Value = sensor.Value;
        }
    }
}
