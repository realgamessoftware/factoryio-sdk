//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using EngineIO;

namespace Controllers
{
    public class SortingWeight : Controller
    {
        MemoryBit entryConveyor = MemoryMap.Instance.GetBit("Entry conveyor", MemoryType.Output);
        MemoryBit loadScale = MemoryMap.Instance.GetBit("Load scale", MemoryType.Output);
        MemoryBit sendLeft = MemoryMap.Instance.GetBit("Send left", MemoryType.Output);
        MemoryBit leftConveyor = MemoryMap.Instance.GetBit("Left conveyor", MemoryType.Output);
        MemoryBit send = MemoryMap.Instance.GetBit("Send forward", MemoryType.Output);
        MemoryBit frontConveyor = MemoryMap.Instance.GetBit("Front conveyor", MemoryType.Output);
        MemoryBit sendRight = MemoryMap.Instance.GetBit("Send right", MemoryType.Output);
        MemoryBit rightConveyor = MemoryMap.Instance.GetBit("Right conveyor", MemoryType.Output);
        MemoryInt weightDisplay = MemoryMap.Instance.GetInt("Weight", MemoryType.Output);

        MemoryBit atScaleEntry = MemoryMap.Instance.GetBit("At scale entry", MemoryType.Input);
        MemoryBit atScale = MemoryMap.Instance.GetBit("At scale", MemoryType.Input);
        MemoryFloat weight = MemoryMap.Instance.GetFloat("Weight", MemoryType.Input);
        MemoryBit atScaleExit = MemoryMap.Instance.GetBit("At scale exit", MemoryType.Input);
        MemoryBit atLeftEntry = MemoryMap.Instance.GetBit("At left entry", MemoryType.Input);
        MemoryBit atForwardEntry = MemoryMap.Instance.GetBit("At forward entry", MemoryType.Input);
        MemoryBit atRightEntry = MemoryMap.Instance.GetBit("At right entry", MemoryType.Input);

        RTRIG rtAtScale = new RTRIG();
        FTRIG ftAtScaleExit = new FTRIG();
        FTRIG ftAtLeftEntry = new FTRIG();
        FTRIG ftAtForwardEntry = new FTRIG();
        FTRIG ftAtRightEntry = new FTRIG();
        FTRIG ftAtScaleEntry = new FTRIG();

        int mWeight = 0;

        State state = State.State0;

        TON scaleTimer = new TON();

        public SortingWeight()
        {
            entryConveyor.Value = false;
            loadScale.Value = false;

            sendLeft.Value = false;
            send.Value = false;
            sendRight.Value = false;

            leftConveyor.Value = true;
            frontConveyor.Value = true;
            rightConveyor.Value = true;

            scaleTimer.PT = 1000;
        }

        public override void Execute(int elapsedMilliseconds)
        {
            rtAtScale.CLK(!atScale.Value);
            ftAtScaleExit.CLK(!atScaleExit.Value);
            ftAtLeftEntry.CLK(atLeftEntry.Value);
            ftAtForwardEntry.CLK(atForwardEntry.Value);
            ftAtRightEntry.CLK(atRightEntry.Value);
            ftAtScaleEntry.CLK(!atScaleEntry.Value);

            entryConveyor.Value = atScaleEntry.Value || (state == State.State1);

            if (state == State.State0)
            {
                frontConveyor.Value = false;
                leftConveyor.Value = false;
                rightConveyor.Value = false;

                if (!atScaleEntry.Value)
                    state = State.State1;
            }
            else if (state == State.State1)
            {
                send.Value = false;
                sendRight.Value = false;
                sendLeft.Value = false;

                loadScale.Value = true;

                if (ftAtScaleEntry.Q)
                {
                    entryConveyor.Value = false;

                    state = State.State2;
                }
            }
            else if (state == State.State2)
            {
                if (atScale.Value)
                {
                    loadScale.Value = false;
                    state = State.State3;
                }
            }
            else if (state == State.State3)
            {
                scaleTimer.IN = true;

                if (scaleTimer.Q)
                {
                    mWeight = (int)(weight.Value * 2f);

                    if (mWeight < 8)
                        state = State.State4;
                    if (mWeight > 8 && mWeight < 10f)
                        state = State.State5;
                    if (mWeight > 10)
                        state = State.State6;

                    scaleTimer.IN = false;
                }
            }

            else if (state == State.State4)
            {
                loadScale.Value = true;
                send.Value = true;
                sendRight.Value = false;
                sendLeft.Value = true;
                leftConveyor.Value = true;

                if (ftAtLeftEntry.Q)
                    state = State.State0;
            }
            else if (state == State.State5)
            {
                loadScale.Value = true;
                send.Value = true;
                sendRight.Value = false;
                sendLeft.Value = false;
                frontConveyor.Value = true;

                if (ftAtForwardEntry.Q)
                    state = State.State0;
            }
            else if (state == State.State6)
            {
                loadScale.Value = true;
                send.Value = true;
                sendRight.Value = true;
                sendLeft.Value = false;
                rightConveyor.Value = true;

                if (ftAtRightEntry.Q)
                    state = State.State0;
            }

            weightDisplay.Value = (int)(weight.Value * 2f);
        }
    }
}
