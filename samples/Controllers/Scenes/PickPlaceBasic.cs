//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using EngineIO;

namespace Controllers
{
    public class PickPlaceBasic : Controller
    {
        MemoryBit moveZ = MemoryMap.Instance.GetBit("Move Z", MemoryType.Output);
        MemoryBit moveX = MemoryMap.Instance.GetBit("Move X", MemoryType.Output);
        MemoryBit grab = MemoryMap.Instance.GetBit("Grab", MemoryType.Output);
        MemoryBit entryConveyor = MemoryMap.Instance.GetBit("Entry conveyor", MemoryType.Output);
        MemoryBit exitConveyor = MemoryMap.Instance.GetBit("Exit conveyor", MemoryType.Output);
        MemoryInt counterDisplay = MemoryMap.Instance.GetInt("Counter", MemoryType.Output);

        MemoryBit movingZ = MemoryMap.Instance.GetBit("Moving Z", MemoryType.Input);
        MemoryBit movingX = MemoryMap.Instance.GetBit("Moving X", MemoryType.Input);
        MemoryBit itemDetected = MemoryMap.Instance.GetBit("Item detected", MemoryType.Input);
        MemoryBit itemAtEntry = MemoryMap.Instance.GetBit("Item at entry", MemoryType.Input);
        MemoryBit itemAtExit = MemoryMap.Instance.GetBit("Item at exit", MemoryType.Input);

        FTRIG ftMovingZ = new FTRIG();
        FTRIG ftMovingX = new FTRIG();
        RTRIG rtItemAtEntry = new RTRIG();
        RTRIG rtItemAtExit = new RTRIG();
        RTRIG rtItemDetected = new RTRIG();
        FTRIG ftMxmz = new FTRIG();

        State state = State.State0;

        int counter = 0;

        public PickPlaceBasic()
        {
            moveZ.Value = false;
            moveX.Value = false;
            grab.Value = false;
            entryConveyor.Value = true;
        }

        public override void Execute(int elapsedMilliseconds)
        {
            ftMovingZ.CLK(movingZ.Value);
            ftMovingX.CLK(movingX.Value);
            rtItemAtEntry.CLK(itemAtEntry.Value);
            rtItemAtExit.CLK(itemAtExit.Value);
            rtItemDetected.CLK(itemDetected.Value);
            ftMxmz.CLK(movingZ.Value && movingX.Value);

            if (state == State.State0)
            {
                if (itemAtEntry.Value)
                    state = State.State1;
            }
            else if (state == State.State1)
            {
                moveZ.Value = true;

                if (rtItemDetected.Q)
                    state = State.State2;
            }
            else if (state == State.State2)
            {
                grab.Value = true;
                moveZ.Value = false;

                if (ftMovingZ.Q)
                    state = State.State3;
            }
            else if (state == State.State3)
            {
                moveX.Value = true;

                if (ftMovingX.Q)
                {
                    entryConveyor.Value = true;
                        
                    exitConveyor.Value = false;

                    state = State.State4;
                }
            }
            else if (state == State.State4)
            {
                moveZ.Value = true;

                if (rtItemAtExit.Q)
                {
                    state = State.State5;
                }
            }
            else if (state == State.State5)
            {
                grab.Value = false;
                moveZ.Value = false;

                if (ftMovingZ.Q)
                {
                    counter++;
                    exitConveyor.Value = true;

                    state = State.State6;
                }
            }
            else if (state == State.State6)
            {
                moveX.Value = false;

                if (ftMovingX.Q)
                    state = State.State0;
            }

            counterDisplay.Value = counter;

            if (rtItemAtEntry.Q)
                entryConveyor.Value = false;
        }
    }
}
