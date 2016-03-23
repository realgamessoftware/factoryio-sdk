//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using EngineIO;

namespace Controllers
{
    public class Assembler : Controller
    {
        MemoryBit lidsConveyor = MemoryMap.Instance.GetBit("Lids conveyor", MemoryType.Output);
        MemoryBit moveX = MemoryMap.Instance.GetBit("Move X", MemoryType.Output);
        MemoryBit moveZ = MemoryMap.Instance.GetBit("Move Z", MemoryType.Output);
        MemoryBit grab = MemoryMap.Instance.GetBit("Grab", MemoryType.Output);
        MemoryBit basesConveyor = MemoryMap.Instance.GetBit("Bases conveyor", MemoryType.Output);
        MemoryBit clampLid = MemoryMap.Instance.GetBit("Clamp lid", MemoryType.Output);
        MemoryBit posRaiseLid = MemoryMap.Instance.GetBit("Pos. raise (lids)", MemoryType.Output);
        MemoryBit clampBase = MemoryMap.Instance.GetBit("Clamp base", MemoryType.Output);
        MemoryBit posRaiseBase = MemoryMap.Instance.GetBit("Pos. raise (bases)", MemoryType.Output);
        MemoryInt counter = MemoryMap.Instance.GetInt("Counter", MemoryType.Output);

        MemoryBit lidAtPlace = MemoryMap.Instance.GetBit("Lid at place", MemoryType.Input);
        MemoryBit baseAtPlace = MemoryMap.Instance.GetBit("Base at place", MemoryType.Input);
        MemoryBit partLeaving = MemoryMap.Instance.GetBit("Part leaving", MemoryType.Input);
        MemoryBit itemDetected = MemoryMap.Instance.GetBit("Item detected", MemoryType.Input);
        MemoryBit movingZ = MemoryMap.Instance.GetBit("Moving Z", MemoryType.Input);
        MemoryBit movingX = MemoryMap.Instance.GetBit("Moving X", MemoryType.Input);
        MemoryBit lidClamped = MemoryMap.Instance.GetBit("Lid clamped", MemoryType.Input);
        MemoryBit posAtLimitLids = MemoryMap.Instance.GetBit("Pos. at limit (lids)", MemoryType.Input);
        MemoryBit baseClamped = MemoryMap.Instance.GetBit("Base clamped", MemoryType.Input);
        MemoryBit posAtLimitBases = MemoryMap.Instance.GetBit("Pos. at limit (bases)", MemoryType.Input);

        FTRIG ftLidAtPlace = new FTRIG();
        FTRIG ftBaseAtPlace = new FTRIG();
        FTRIG ftMovingZ = new FTRIG();
        FTRIG ftMovingX = new FTRIG();
        RTRIG rtMxmz = new RTRIG();
        FTRIG ftPartLeaving = new FTRIG();

        State stateLids = State.State0;
        State stateBases = State.State0;

        public Assembler()
        {
            lidsConveyor.Value = false;
            basesConveyor.Value = true;
            moveZ.Value = false;
            moveX.Value = false;
            grab.Value = false;
            clampLid.Value = false;
            posRaiseLid.Value = false;
            clampBase.Value = false;
            posRaiseBase.Value = false;

            counter.Value = 0;
        }

        public override void Execute(int elapsedMilliseconds)
        {
            ftLidAtPlace.CLK(lidAtPlace.Value);
            ftMovingZ.CLK(movingZ.Value);
            ftMovingX.CLK(movingX.Value);
            rtMxmz.CLK(movingZ.Value && movingX.Value);

            //Lids
            if (stateLids == State.State0)
            {
                lidsConveyor.Value = true;

                if (ftLidAtPlace.Q)
                    stateLids = State.State1;
            }
            else if (stateLids == State.State1)
            {
                lidsConveyor.Value = false;
                clampLid.Value = true;

                if (lidClamped.Value)
                    stateLids = State.State2;
            }
            else if (stateLids == State.State2)
            {
                moveZ.Value = true;

                if (ftMovingZ.Q)
                    stateLids = State.State3;
            }
            else if (stateLids == State.State3)
            {
                if (itemDetected.Value)
                    stateLids = State.State4;
            }
            else if (stateLids == State.State4)
            {
                grab.Value = true;
                moveZ.Value = false;

                clampLid.Value = false;

                if (ftMovingZ.Q)
                    stateLids = State.State5;
            }
            else if (stateLids == State.State5)
            {
                moveX.Value = true;

                if (ftMovingX.Q)
                    stateLids = State.State6;
            }
            else if (stateLids == State.State6)
            {
                moveZ.Value = true;

                if (ftMovingZ.Q)
                    stateLids = State.State7;
            }
            else if (stateLids == State.State7)
            {
                grab.Value = false;
                moveZ.Value = false;

                stateBases = State.State2;

                if (ftMovingZ.Q && !itemDetected.Value)
                {
                    counter.Value++;

                    stateLids = State.State8;
                }
            }
            else if (stateLids == State.State8)
            {
                if (!itemDetected.Value)
                    stateLids = State.State9;     
            }
            else if (stateLids == State.State9)
            {
                moveX.Value = false;

                if (ftMovingX.Q)
                    stateLids = State.State0;
            }

            //Bases
            ftBaseAtPlace.CLK(baseAtPlace.Value);
            ftPartLeaving.CLK(partLeaving.Value);

            if (stateBases == State.State0)
            {
                basesConveyor.Value = true;
                posRaiseBase.Value = false;

                if (ftBaseAtPlace.Q)
                    stateBases = State.State1;
            }
            else if (stateBases == State.State1)
            {
                basesConveyor.Value = false;
                clampBase.Value = true;
            }
            else if (stateBases == State.State2)
            {
                basesConveyor.Value = true;
                clampBase.Value = false;
                posRaiseBase.Value = true;

                if (ftPartLeaving.Q || baseAtPlace.Value)
                    stateBases = State.State0;
            }
        }
    }
}
