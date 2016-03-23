//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using EngineIO;

namespace Controllers
{
    public class FromAToBSetReset : Controller
    {
        MemoryBit conveyor = MemoryMap.Instance.GetBit("Conveyor", MemoryType.Output);

        MemoryBit sensorA = MemoryMap.Instance.GetBit("Sensor A", MemoryType.Input);
        MemoryBit sensorB = MemoryMap.Instance.GetBit("Sensor B", MemoryType.Input);

        public FromAToBSetReset()
        {
            conveyor.Value = false;
        }

        public override void Execute(int elapsedMilliseconds)
        {
            if (!sensorA.Value)
                conveyor.Value = true;

            if (!sensorB.Value)
                conveyor.Value = false;
        }
    }
}
