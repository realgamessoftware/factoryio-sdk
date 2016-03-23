//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using EngineIO;

using System.Collections.Generic;

namespace Controllers
{
    public class SortingHeightAdvanced : Controller
    {
        MemoryBit feederConveyor = MemoryMap.Instance.GetBit("Feeder conveyor", MemoryType.Output);
        MemoryBit entryConveyor = MemoryMap.Instance.GetBit("Entry conveyor", MemoryType.Output);
        MemoryBit load = MemoryMap.Instance.GetBit("Load", MemoryType.Output);
        MemoryBit unload = MemoryMap.Instance.GetBit("Unload", MemoryType.Output);
        MemoryBit turn = MemoryMap.Instance.GetBit("Turn", MemoryType.Output);
        MemoryBit rightConveyor = MemoryMap.Instance.GetBit("Right conveyor", MemoryType.Output);
        MemoryBit leftConveyor = MemoryMap.Instance.GetBit("Left conveyor", MemoryType.Output);

        MemoryBit atEntry = MemoryMap.Instance.GetBit("At entry", MemoryType.Input);
        MemoryBit highBox = MemoryMap.Instance.GetBit("High box", MemoryType.Input);
        MemoryBit lowBox = MemoryMap.Instance.GetBit("Low box", MemoryType.Input);
        MemoryBit atTurntableEntry = MemoryMap.Instance.GetBit("At turntable entry", MemoryType.Input);
        MemoryBit atFront = MemoryMap.Instance.GetBit("At front", MemoryType.Input);
        MemoryBit atBack = MemoryMap.Instance.GetBit("At back", MemoryType.Input);
        MemoryBit atRightEntry = MemoryMap.Instance.GetBit("At right entry", MemoryType.Input);
        MemoryBit atLeftEntry = MemoryMap.Instance.GetBit("At left entry", MemoryType.Input);
        MemoryBit atRightExit = MemoryMap.Instance.GetBit("At right exit", MemoryType.Input);
        MemoryBit atLeftExit = MemoryMap.Instance.GetBit("At left exit", MemoryType.Input);
        MemoryBit atLoadPosition = MemoryMap.Instance.GetBit("At load position", MemoryType.Input);
        MemoryBit atUnloadPosition = MemoryMap.Instance.GetBit("At unload position", MemoryType.Input);

        RTRIG rtLow = new RTRIG();
        RTRIG rtAtTurntableEntry = new RTRIG();
        RTRIG rtAtFront = new RTRIG();
        RTRIG rtAtBack = new RTRIG();
        RTRIG rtAtRightEntry = new RTRIG();
        RTRIG rtAtLeftEntry = new RTRIG();

        FTRIG ftAtBack = new FTRIG();
        FTRIG ftAtFront = new FTRIG();
        FTRIG ftAtRightEntry = new FTRIG();
        FTRIG ftAtLeftEntry = new FTRIG();
        FTRIG ftAtRightExit = new FTRIG();
        FTRIG ftAtLeftExit = new FTRIG();
        FTRIG ftAtTurntableEntry = new FTRIG();

        Queue<bool> partsFIFO = new Queue<bool>();

        State turnTableState = State.State0;
        State entryConveyorState = State.State0;

        public SortingHeightAdvanced()
        {
            feederConveyor.Value = false;
            entryConveyor.Value = false;
            load.Value = false;
            unload.Value = false;
            turn.Value = false;
            rightConveyor.Value = false;
            leftConveyor.Value = false;
        }

        public override void Execute(int elapsedMilliseconds)
        {
            rtLow.CLK(lowBox.Value);
            rtAtTurntableEntry.CLK(atTurntableEntry.Value);
            rtAtFront.CLK(atFront.Value);
            rtAtBack.CLK(atBack.Value);
            rtAtRightEntry.CLK(atRightEntry.Value);
            rtAtLeftEntry.CLK(atLeftEntry.Value);

            ftAtBack.CLK(atBack.Value);
            ftAtFront.CLK(atFront.Value);
            ftAtRightEntry.CLK(atRightEntry.Value);
            ftAtLeftEntry.CLK(atLeftEntry.Value);
            ftAtRightExit.CLK(atRightExit.Value);
            ftAtLeftExit.CLK(atLeftExit.Value);
            ftAtTurntableEntry.CLK(atTurntableEntry.Value);

            //Conveyors
            if (entryConveyorState == State.State0)
            {
                entryConveyor.Value = !atTurntableEntry.Value;
            }
            else if (entryConveyorState == State.State1)
            {
                entryConveyor.Value = true;

                if (ftAtTurntableEntry.Q)
                    entryConveyorState = State.State0;
            }

            feederConveyor.Value = entryConveyor.Value;

            if (!atEntry.Value)
                feederConveyor.Value = true;

            if (!atTurntableEntry.Value)
                entryConveyor.Value = true;

            if (rtAtRightEntry.Q)
                rightConveyor.Value = true;

            if (ftAtRightExit.Q)
                rightConveyor.Value = false;

            if (rtAtLeftEntry.Q)
                leftConveyor.Value = true;

            if (ftAtLeftExit.Q)
                leftConveyor.Value = false;

            //FIFO
            if (rtLow.Q)
                partsFIFO.Enqueue(highBox.Value);

            //Turntable
            if (turnTableState == State.State0)
            {
                if (atTurntableEntry.Value)
                {
                    if (partsFIFO.Count > 0)
                    {
                        bool isHigh = partsFIFO.Dequeue();

                        if (isHigh)
                        {
                            turnTableState = State.State10; //High boxes
                            entryConveyorState = State.State1;
                        }
                        else
                        {
                            turnTableState = State.State20; //Low boxes
                            entryConveyorState = State.State1;
                        }
                    }
                }
            }
            else if (turnTableState == State.State10) //High boxes go right
            {
                load.Value = true;

                if (ftAtBack.Q)
                {
                    load.Value = false;
                    turn.Value = true;
                    turnTableState = State.State11;
                }
            }
            else if (turnTableState == State.State11)
            {
                if (atUnloadPosition.Value)
                {
                    unload.Value = true;
                    turnTableState = State.State12;
                }
            }
            else if (turnTableState == State.State12)
            {
                if (ftAtRightEntry.Q)
                {
                    turn.Value = false;
                    unload.Value = false;
                    turnTableState = State.State13;
                }
            }
            else if (turnTableState == State.State13)
            {
                if (atLoadPosition.Value)
                    turnTableState = State.State0;
            }
            else if (turnTableState == State.State20)
            {
                entryConveyor.Value = true;
                load.Value = true;

                if (ftAtBack.Q)
                {
                    entryConveyor.Value = false;
                    turn.Value = true;
                    turnTableState = State.State21;
                }
            }
            else if (turnTableState == State.State21)
            {
                if (atFront.Value)
                {
                    load.Value = false;
                    turnTableState = State.State22;
                }
            }
            else if (turnTableState == State.State22)
            {
                if (atUnloadPosition.Value)
                {
                    load.Value = true;
                    turnTableState = State.State23;
                }
            }
            else if (turnTableState == State.State23)
            {
                if (ftAtLeftEntry.Q)
                {
                    load.Value = false;
                    turn.Value = false;
                    turnTableState = State.State24;
                }
            }
            else if (turnTableState == State.State24)
            {
                if (atLoadPosition.Value)
                    turnTableState = State.State0;
            }


        }
    }
}
