//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using EngineIO;

namespace Controllers
{
    public class SortingHeightBasic : Controller
    {
        MemoryBit convEntry = MemoryMap.Instance.GetBit("Conveyor entry", MemoryType.Output);
        MemoryBit load = MemoryMap.Instance.GetBit("Load", MemoryType.Output);
        MemoryBit transfRight = MemoryMap.Instance.GetBit("Transf. right", MemoryType.Output);
        MemoryBit transfLeft = MemoryMap.Instance.GetBit("Transf. left", MemoryType.Output);
        MemoryBit conveyorRight = MemoryMap.Instance.GetBit("Conveyor right", MemoryType.Output);
        MemoryBit conveyorLeft = MemoryMap.Instance.GetBit("Conveyor left", MemoryType.Output);
        MemoryInt counterDisplay = MemoryMap.Instance.GetInt("Counter", MemoryType.Output);

        MemoryBit palletSensor = MemoryMap.Instance.GetBit("Pallet sensor", MemoryType.Input);
        MemoryBit lowSensor = MemoryMap.Instance.GetBit("Low sensor", MemoryType.Input);
        MemoryBit highSensor = MemoryMap.Instance.GetBit("High sensor", MemoryType.Input);
        MemoryBit loaded = MemoryMap.Instance.GetBit("Loaded", MemoryType.Input);
        MemoryBit atRightEntry = MemoryMap.Instance.GetBit("At right entry", MemoryType.Input);
        MemoryBit atLeftEntry = MemoryMap.Instance.GetBit("At left entry", MemoryType.Input);

        RTRIG rtPalletSensor = new RTRIG();
        RTRIG rtLowSensor = new RTRIG();
        RTRIG rtHighSensor = new RTRIG();
        RTRIG rtLoaded = new RTRIG();
        RTRIG rtRightEntry = new RTRIG();
        RTRIG rtLeftEntry = new RTRIG();

        bool loadMem;
        bool transferBusy;
        bool transfRightMem;
        bool transfLeftMem;
        bool conveyorRightMem;
        bool conveyorLeftMem;

        bool sendLeft;

        int counter;

        public SortingHeightBasic()
        {
            convEntry.Value = false;
            load.Value = false;
            transfRight.Value = false;
            transfLeft.Value = false;
            conveyorRight.Value = false;
            conveyorLeft.Value = false;
        }

        public override void Execute(int elapsedMilliseconds)
        {
            rtPalletSensor.CLK(palletSensor.Value);
            rtLowSensor.CLK(lowSensor.Value);
            rtHighSensor.CLK(highSensor.Value);
            rtLoaded.CLK(loaded.Value);
            rtRightEntry.CLK(atRightEntry.Value);
            rtLeftEntry.CLK(atLeftEntry.Value);

            //Entry conveyor
            if (rtPalletSensor.Q && !transferBusy)
            {
                loadMem = true;
            }

            //Height measure
            if (rtLowSensor.Q)
            {
                sendLeft = true;
            }

            if (rtHighSensor.Q)
            {
                sendLeft = false;
            }

            //Transfer
            if (rtLoaded.Q)
            {
                loadMem = false;

                if (sendLeft)
                {
                    transfLeftMem = true;
                    conveyorLeftMem = true;
                }
                else
                {
                    transfRightMem = true;
                    conveyorRightMem = true;
                }
 
                transferBusy = true;
            }

            if (rtLeftEntry.Q)
            {
                transfLeftMem = false;

                if (palletSensor.Value)
                    loadMem = true;

                counterDisplay.Value = ++counter;
            }

            if (rtRightEntry.Q)
            {
                transfRightMem = false;

                if (palletSensor.Value)
                    loadMem = true;

                counterDisplay.Value = ++counter;
            }

            convEntry.Value = !palletSensor.Value || (palletSensor.Value && loadMem);
            load.Value = loadMem;
            transfRight.Value = transfRightMem;
            transfLeft.Value = transfLeftMem;
            conveyorRight.Value = conveyorRightMem;
            conveyorLeft.Value = conveyorLeftMem;
        }
    }
}
