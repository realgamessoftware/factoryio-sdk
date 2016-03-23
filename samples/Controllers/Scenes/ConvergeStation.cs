//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using EngineIO;

namespace Controllers
{
    public class ConvergeStation : Controller
    {
        MemoryBit conveyor1 = MemoryMap.Instance.GetBit("Conveyor 1", MemoryType.Output);
        MemoryBit conveyor2 = MemoryMap.Instance.GetBit("Conveyor 2", MemoryType.Output);
        MemoryBit load1 = MemoryMap.Instance.GetBit("Load 1", MemoryType.Output);
        MemoryBit transferLeft1 = MemoryMap.Instance.GetBit("Transfer left 1", MemoryType.Output);
        MemoryBit load2 = MemoryMap.Instance.GetBit("Load 2", MemoryType.Output);
        MemoryBit transferLeft2 = MemoryMap.Instance.GetBit("Transfer left 2", MemoryType.Output);
        MemoryBit exitConveyor = MemoryMap.Instance.GetBit("Exit conveyor", MemoryType.Output);

        MemoryBit atEntry2 = MemoryMap.Instance.GetBit("At entry 2", MemoryType.Input);
        MemoryBit atTransfer2 = MemoryMap.Instance.GetBit("At transfer 2", MemoryType.Input);
        MemoryBit atEntry1 = MemoryMap.Instance.GetBit("At entry 1", MemoryType.Input);
        MemoryBit atTransfer1 = MemoryMap.Instance.GetBit("At transfer 1", MemoryType.Input);
        MemoryBit atExit = MemoryMap.Instance.GetBit("At exit", MemoryType.Input);

        State mngState = State.State0;
        State trfState1 = State.State0;
        State trfState2 = State.State0;

        bool doTransfer1;
        bool doTransfer2;

        RTRIG rtAtEntry1 = new RTRIG();
        RTRIG rtAtEntry2 = new RTRIG();
        RTRIG rtAtExit = new RTRIG();
        FTRIG ftAtExit = new FTRIG();

        public ConvergeStation()
        {
            conveyor1.Value = false;
            conveyor2.Value = false;
            load1.Value = false;
            transferLeft1.Value = false;
            load2.Value = false;
            transferLeft2.Value = false;
            exitConveyor.Value = false;
        }

        public override void Execute(int elapsedMilliseconds)
        {
            rtAtEntry1.CLK(atEntry1.Value);
            rtAtEntry2.CLK(atEntry2.Value);
            rtAtExit.CLK(atExit.Value);
            ftAtExit.CLK(atExit.Value);

            //Master controller
            if (mngState == State.State0)
            {
                if (atEntry1.Value)
                    mngState = State.State1;
                else if (atEntry2.Value)
                    mngState = State.State2;

            }
            else if (mngState == State.State1)
            {
                doTransfer1 = true;

                if (ftAtExit.Q)
                    mngState = State.State0;
            }
            else if (mngState == State.State2)
            {
                doTransfer2 = true;

                if (ftAtExit.Q)
                   mngState = State.State0;
            }

            //Transfer 1
            if (doTransfer1)
            {
                if (trfState1 == State.State0)
                {
                    load1.Value = false;

                    if (atEntry1.Value)
                        trfState1 = State.State1;
                }
                else if (trfState1 == State.State1)
                {
                    load1.Value = true;

                    if (ftAtExit.Q)
                    {
                        doTransfer1 = false;
                        trfState1 = State.State0;
                    }
                }
            }

            //Transfer 2
            if (doTransfer2)
            {
                if (trfState2 == State.State0)
                {
                    load2.Value = false;

                    if (atEntry2.Value)
                        trfState2 = State.State1;
                }
                else if (trfState2 == State.State1)
                {
                    load2.Value = true;

                    if (atTransfer2.Value)
                        trfState2 = State.State2;
                }
                else if (trfState2 == State.State2)
                {
                    load2.Value = false;

                    transferLeft1.Value = true;
                    transferLeft2.Value = true;

                    if (atTransfer1.Value)
                        trfState2 = State.State3;
                }
                else if (trfState2 == State.State3)
                {
                    load1.Value = true;

                    transferLeft1.Value = false;
                    transferLeft2.Value = false;

                    if (ftAtExit.Q)
                    {
                        doTransfer2 = false;
                        trfState2 = State.State0;
                    }
                }
            }

            conveyor1.Value = !atEntry1.Value || (load1.Value && doTransfer1);
            conveyor2.Value = !atEntry2.Value || load2.Value;
            exitConveyor.Value = atExit.Value || load1.Value;
        }
    }
}
