//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using EngineIO;

namespace Controllers
{
    public class ProductionLine : Controller
    {
        MemoryBit lidsRawConveyor = MemoryMap.Instance.GetBit("Lids raw conveyor", MemoryType.Output);
        MemoryBit lidsCenterStart = MemoryMap.Instance.GetBit("Lids center (start)", MemoryType.Output);
        MemoryInt lidsCounter = MemoryMap.Instance.GetInt("Lids counter", MemoryType.Output);
        MemoryBit lidsExitConveyor1 = MemoryMap.Instance.GetBit("Lids exit conveyor 1", MemoryType.Output);
        MemoryBit lidsExitConveyor2 = MemoryMap.Instance.GetBit("Lids exit conveyor 2", MemoryType.Output);

        MemoryBit lidsAtEntry = MemoryMap.Instance.GetBit("Lids at entry", MemoryType.Input);
        MemoryBit lidsCenterBusy = MemoryMap.Instance.GetBit("Lids center (busy)", MemoryType.Input);
        MemoryBit lidsCenterRunning = MemoryMap.Instance.GetBit("Lids center (running)", MemoryType.Input);

        MemoryBit basesRawConveyor = MemoryMap.Instance.GetBit("Bases raw conveyor", MemoryType.Output);
        MemoryBit basesCenterStart = MemoryMap.Instance.GetBit("Bases center (start)", MemoryType.Output);
        MemoryInt basesCounter = MemoryMap.Instance.GetInt("Bases counter", MemoryType.Output);
        MemoryBit basesExitConveyor1 = MemoryMap.Instance.GetBit("Bases exit conveyor 1", MemoryType.Output);
        MemoryBit basesExitConveyor2 = MemoryMap.Instance.GetBit("Bases exit conveyor 2", MemoryType.Output);

        MemoryBit exitConveyor = MemoryMap.Instance.GetBit("Exit conveyor", MemoryType.Output);

        MemoryBit basesAtEntry = MemoryMap.Instance.GetBit("Bases at entry", MemoryType.Input);
        MemoryBit basesCenterBusy = MemoryMap.Instance.GetBit("Bases center (busy)", MemoryType.Input);
        MemoryBit basesCenterRunning = MemoryMap.Instance.GetBit("Bases center (running)", MemoryType.Input);

        FTRIG ftLidsAtEntry = new FTRIG();
        FTRIG ftLidsCenterBusy = new FTRIG();

        FTRIG ftBasesAtEntry = new FTRIG();
        FTRIG ftBasesCenterBusy = new FTRIG();

        bool feedLidMaterial;
        bool feedBaseMaterial;

        public ProductionLine()
        {
            lidsRawConveyor.Value = false;
            lidsCenterStart.Value = true;

            basesRawConveyor.Value = false;
            basesCenterStart.Value = true;

            feedLidMaterial = true;
            feedBaseMaterial = true;

            lidsCounter.Value = 0;
            basesCounter.Value = 0;

            lidsExitConveyor1.Value = true;
            lidsExitConveyor2.Value = true;
            basesExitConveyor1.Value = true;
            basesExitConveyor2.Value = true;
            exitConveyor.Value = true;
        }

        public override void Execute(int elapsedMilliseconds)
        {
            //Lids
            ftLidsAtEntry.CLK(!lidsAtEntry.Value);
            ftLidsCenterBusy.CLK(lidsCenterBusy.Value);

            if (ftLidsAtEntry.Q)
            {
                feedLidMaterial = false;
            }

            if (ftLidsCenterBusy.Q)
            {
                feedLidMaterial = true;
                lidsCounter.Value++;
            }

            lidsRawConveyor.Value = feedLidMaterial && !lidsCenterBusy.Value;

            //Bases
            ftBasesAtEntry.CLK(!basesAtEntry.Value);
            ftBasesCenterBusy.CLK(basesCenterBusy.Value);

            if (ftBasesAtEntry.Q)
            {
                feedBaseMaterial = false;
            }

            if (ftBasesCenterBusy.Q)
            {
                feedBaseMaterial = true;
                basesCounter.Value++;
            }

            basesRawConveyor.Value = feedBaseMaterial && !basesCenterBusy.Value;
        }
    }
}
