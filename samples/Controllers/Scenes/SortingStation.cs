//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using EngineIO;

namespace Controllers
{
    public class SortingStation : Controller
    {
        MemoryBit entryConveyor = MemoryMap.Instance.GetBit("Entry conveyor", MemoryType.Output);
        MemoryBit sorter1Turn = MemoryMap.Instance.GetBit("Sorter 1 turn", MemoryType.Output);
        MemoryBit sorter1Belt = MemoryMap.Instance.GetBit("Sorter 1 belt", MemoryType.Output);
        MemoryBit sorter2Turn = MemoryMap.Instance.GetBit("Sorter 2 turn", MemoryType.Output);
        MemoryBit sorter2Belt = MemoryMap.Instance.GetBit("Sorter 2 belt", MemoryType.Output);
        MemoryBit sorter3Turn = MemoryMap.Instance.GetBit("Sorter 3 turn", MemoryType.Output);
        MemoryBit sorter3Belt = MemoryMap.Instance.GetBit("Sorter 3 belt", MemoryType.Output);
        MemoryBit exitConveyor = MemoryMap.Instance.GetBit("Exit conveyor", MemoryType.Output);

        MemoryInt visionSensor = MemoryMap.Instance.GetInt("Vision sensor", MemoryType.Input);
        MemoryBit atExit = MemoryMap.Instance.GetBit("At exit", MemoryType.Input);

        FTRIG ftAtExit = new FTRIG();

        State feederState = State.State0;
        State sorterState = State.State0;

        public SortingStation()
        {
            entryConveyor.Value = false;
            sorter1Turn.Value = false;
            sorter1Belt.Value = false;
            sorter2Turn.Value = false;
            sorter2Belt.Value = false;
            sorter3Turn.Value = false;
            sorter3Belt.Value = false;
            exitConveyor.Value = false;
        }

        public override void Execute(int elapsedMilliseconds)
        {
            ftAtExit.CLK(!atExit.Value);

            //Feeder
            if (feederState == State.State0)
            {
                entryConveyor.Value = true;

                if (visionSensor.Value != 0)
                {
                    if (sorterState == State.State0)
                    {
                        //Set sorter state
                        if (visionSensor.Value == 1)
                        {
                            //Blue raw material
                            sorterState = State.State1;
                        }
                        else if (visionSensor.Value == 4)
                        {
                            //Green raw material
                            sorterState = State.State2;
                        }
                        else
                        {
                            //Other
                            sorterState = State.State3;
                        }

                        feederState = State.State1;
                    }
                    else
                    {
                        entryConveyor.Value = false;
                    }
                }
            }
            else if (feederState == State.State1)
            {
                entryConveyor.Value = true;

                if (visionSensor.Value == 0)
                {
                    feederState = State.State0;
                }
            }

            //Sorters
            if (sorterState == State.State0)
            {
                sorter1Belt.Value = false;
                sorter1Turn.Value = false;
                sorter2Belt.Value = false;
                sorter2Turn.Value = false;
                sorter3Belt.Value = false;
                sorter3Turn.Value = false;
                exitConveyor.Value = false;
            }
            else if (sorterState == State.State1) //First sorter
            {
                exitConveyor.Value = true;

                sorter1Belt.Value = true;
                sorter1Turn.Value = true;
                sorter2Belt.Value = false;
                sorter2Turn.Value = false;
                sorter3Belt.Value = false;
                sorter3Turn.Value = false;

                if (ftAtExit.Q)
                    sorterState = State.State0;
            }
            else if (sorterState == State.State2) //Second sorter
            {
                exitConveyor.Value = true;

                sorter1Belt.Value = false;
                sorter1Turn.Value = false;
                sorter2Belt.Value = true;
                sorter2Turn.Value = true;
                sorter3Belt.Value = false;
                sorter3Turn.Value = false;

                if (ftAtExit.Q)
                    sorterState = State.State0;
            }
            else if (sorterState == State.State3) //Third sorter
            {
                exitConveyor.Value = true;

                sorter1Belt.Value = false;
                sorter1Turn.Value = false;
                sorter2Belt.Value = false;
                sorter2Turn.Value = false;
                sorter3Belt.Value = true;
                sorter3Turn.Value = true;

                if (ftAtExit.Q)
                    sorterState = State.State0;
            }
        }
    }
}
