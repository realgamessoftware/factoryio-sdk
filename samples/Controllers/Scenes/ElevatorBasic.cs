//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using EngineIO;

namespace Controllers
{
    public class ElevatorBasic : Controller
    {
        //Actuators
        MemoryBit entryConveyor = MemoryMap.Instance.GetBit("Entry conveyor", MemoryType.Output);
        MemoryBit exitConveyor = MemoryMap.Instance.GetBit("Exit conveyor", MemoryType.Output);

        MemoryBit loadLeft = MemoryMap.Instance.GetBit("Load left", MemoryType.Output);
        MemoryBit loadRight = MemoryMap.Instance.GetBit("Load right", MemoryType.Output);

        MemoryBit leftUp = MemoryMap.Instance.GetBit("Left up", MemoryType.Output);
        MemoryBit leftSlow = MemoryMap.Instance.GetBit("Left slow", MemoryType.Output);
        MemoryBit leftDown = MemoryMap.Instance.GetBit("Left down", MemoryType.Output);

        MemoryBit rightUp = MemoryMap.Instance.GetBit("Right up", MemoryType.Output);
        MemoryBit rightSlow = MemoryMap.Instance.GetBit("Right slow", MemoryType.Output);
        MemoryBit rightDown = MemoryMap.Instance.GetBit("Right down", MemoryType.Output);

        MemoryBit conveyor0 = MemoryMap.Instance.GetBit("Conveyor 0", MemoryType.Output);
        MemoryBit conveyor1 = MemoryMap.Instance.GetBit("Conveyor 1", MemoryType.Output);
        MemoryBit conveyor2 = MemoryMap.Instance.GetBit("Conveyor 2", MemoryType.Output);

        //Sensors
        MemoryBit atLeftLow0 = MemoryMap.Instance.GetBit("At left low 0", MemoryType.Input);
        MemoryBit atLeftHigh0 = MemoryMap.Instance.GetBit("At left high 0", MemoryType.Input);
        MemoryBit atLeftLow1 = MemoryMap.Instance.GetBit("At left low 1", MemoryType.Input);
        MemoryBit atLeftHigh1 = MemoryMap.Instance.GetBit("At left high 1", MemoryType.Input);
        MemoryBit atLeftLow2 = MemoryMap.Instance.GetBit("At left low 2", MemoryType.Input);
        MemoryBit atLeftHigh2 = MemoryMap.Instance.GetBit("At left high 2", MemoryType.Input);

        MemoryBit atRightLow0 = MemoryMap.Instance.GetBit("At right low 0", MemoryType.Input);
        MemoryBit atRightHigh0 = MemoryMap.Instance.GetBit("At right high 0", MemoryType.Input);
        MemoryBit atRightLow1 = MemoryMap.Instance.GetBit("At right low 1", MemoryType.Input);
        MemoryBit atRightHigh1 = MemoryMap.Instance.GetBit("At right high 1", MemoryType.Input);
        MemoryBit atRightLow2 = MemoryMap.Instance.GetBit("At right low 2", MemoryType.Input);
        MemoryBit atRightHigh2 = MemoryMap.Instance.GetBit("At right high 2", MemoryType.Input);

        MemoryBit atEntryLeft = MemoryMap.Instance.GetBit("At entry left", MemoryType.Input);
        MemoryBit atEntry0 = MemoryMap.Instance.GetBit("At entry 0", MemoryType.Input);
        MemoryBit atEntry1 = MemoryMap.Instance.GetBit("At entry 1", MemoryType.Input);
        MemoryBit atEntry2 = MemoryMap.Instance.GetBit("At entry 2", MemoryType.Input);
        MemoryBit atExit0 = MemoryMap.Instance.GetBit("At exit 0", MemoryType.Input);
        MemoryBit atExit1 = MemoryMap.Instance.GetBit("At exit 1", MemoryType.Input);
        MemoryBit atExit2 = MemoryMap.Instance.GetBit("At exit 2", MemoryType.Input);
        MemoryBit atExitRight = MemoryMap.Instance.GetBit("At exit right", MemoryType.Input);
        MemoryBit atExit = MemoryMap.Instance.GetBit("At exit", MemoryType.Input);

        State loadState;
        State unloadState;
        State rightElevatorState;
        State leftElevatorState;

        int leftElevatorFloor;
        int rightElevatorFloor;

        bool conveyor0Busy;
        bool conveyor1Busy;
        bool conveyor2Busy;

        FTRIG ftAtEntryLeft = new FTRIG();
        FTRIG ftAtExitRight = new FTRIG();
        FTRIG ftAtExit = new FTRIG();
        FTRIG ftAtEntry0 = new FTRIG();
        FTRIG ftAtEntry1 = new FTRIG();
        FTRIG ftAtEntry2 = new FTRIG();

        RTRIG rtAtExit0 = new RTRIG();
        RTRIG rtAtExit1 = new RTRIG();
        RTRIG rtAtExit2 = new RTRIG();
        RTRIG rtAtExitRight = new RTRIG();

        public ElevatorBasic()
        {
            entryConveyor.Value = false;
            exitConveyor.Value = false;

            loadLeft.Value = false;
            loadRight.Value = false;

            leftUp.Value = false;
            leftSlow.Value = false;
            leftDown.Value = false;

            rightUp.Value = false;
            rightSlow.Value = false;
            rightDown.Value = false;

            conveyor0.Value = false;
            conveyor1.Value = false;
            conveyor2.Value = false;
        }

        public override void Execute(int elapsedMilliseconds)
        {
            ftAtEntryLeft.CLK(!atEntryLeft.Value);
            ftAtExitRight.CLK(!atExitRight.Value);
            ftAtExit.CLK(!atExit.Value);
            ftAtEntry0.CLK(!atEntry0.Value);
            ftAtEntry1.CLK(!atEntry1.Value);
            ftAtEntry2.CLK(!atEntry2.Value);

            rtAtExit0.CLK(!atExit0.Value);
            rtAtExit1.CLK(!atExit1.Value);
            rtAtExit2.CLK(!atExit2.Value);
            rtAtExitRight.CLK(!atExitRight.Value);

            #region Left Elevator

            if (leftElevatorState == State.State0)
            {
                leftUp.Value = false;
                leftSlow.Value = false;
                leftDown.Value = false;

                if (atLeftLow0.Value && atLeftHigh0.Value)
                {
                    leftElevatorFloor = 0;
                }
                else if (atLeftLow1.Value && atLeftHigh1.Value)
                {
                    leftElevatorFloor = 1;
                }
                else if (atLeftLow2.Value && atLeftHigh2.Value)
                {
                    leftElevatorFloor = 2;
                }
            }
            else if (leftElevatorState == State.State1) //Floor 0
            {
                if (leftElevatorFloor > 0)
                {
                    leftSlow.Value = atLeftHigh0.Value;
                    leftDown.Value = true;

                    if (atLeftLow0.Value && atLeftHigh0.Value)
                        leftElevatorState = State.State0;
                }
                else
                {
                    leftElevatorState = State.State0;
                }
            }
            else if (leftElevatorState == State.State2) //Floor 1
            {
                if (leftElevatorFloor > 1)
                {
                    leftSlow.Value = atLeftHigh1.Value;
                    leftDown.Value = true;

                    if (atLeftLow1.Value && atLeftHigh1.Value)
                        leftElevatorState = State.State0;
                }
                else if (leftElevatorFloor < 1)
                {
                    leftUp.Value = true;
                    leftSlow.Value = atLeftLow1.Value;

                    if (atLeftLow1.Value && atLeftHigh1.Value)
                        leftElevatorState = State.State0;
                }
                else
                {
                    leftElevatorState = State.State0;
                }
            }
            else if (leftElevatorState == State.State3) //Floor 2
            {
                if (leftElevatorFloor < 3)
                {
                    leftUp.Value = true;
                    leftSlow.Value = atLeftLow2.Value;

                    if (atLeftLow2.Value && atLeftHigh2.Value)
                        leftElevatorState = State.State0;
                }
                else
                {
                    leftElevatorState = State.State0;
                }
            }

            #endregion

            #region Right Elevator

            if (rightElevatorState == State.State0)
            {
                rightUp.Value = false;
                rightSlow.Value = false;
                rightDown.Value = false;

                if (atRightLow0.Value && atRightHigh0.Value)
                {
                    rightElevatorFloor = 0;
                }
                else if (atRightLow1.Value && atRightHigh1.Value)
                {
                    rightElevatorFloor = 1;
                }
                else if (atRightLow2.Value && atRightHigh2.Value)
                {
                    rightElevatorFloor = 2;
                }
            }
            else if (rightElevatorState == State.State1) //Floor 0
            {
                if (rightElevatorFloor > 0)
                {
                    rightSlow.Value = atRightHigh0.Value;
                    rightDown.Value = true;

                    if (atRightLow0.Value && atRightHigh0.Value)
                        rightElevatorState = State.State0;
                }
                else
                {
                    rightElevatorState = State.State0;
                }
            }
            else if (rightElevatorState == State.State2) //Floor 1
            {
                if (rightElevatorFloor > 1)
                {
                    rightSlow.Value = atRightHigh1.Value;
                    rightDown.Value = true;

                    if (atRightLow1.Value && atRightHigh1.Value)
                        rightElevatorState = State.State0;
                }
                else if (rightElevatorFloor < 1)
                {
                    rightUp.Value = true;
                    rightSlow.Value = atRightLow1.Value;

                    if (atRightLow1.Value && atRightHigh1.Value)
                        rightElevatorState = State.State0;
                }
                else
                {
                    rightElevatorState = State.State0;
                }
            }
            else if (rightElevatorState == State.State3) //Floor 2
            {
                if (rightElevatorFloor < 3)
                {
                    rightUp.Value = true;
                    rightSlow.Value = atRightLow2.Value;

                    if (atRightLow2.Value && atRightHigh2.Value)
                        rightElevatorState = State.State0;
                }
                else
                {
                    rightElevatorState = State.State0;
                }
            }

            #endregion

            #region Load Into Conveyors

            if (loadState == State.State0) //Load into left elevator
            {
                //If any conveyor is free choose where to go
                if (!conveyor0Busy || !conveyor1Busy || !conveyor2Busy)
                {
                    entryConveyor.Value = true;
                    loadLeft.Value = true;

                    if (ftAtEntryLeft.Q)
                    {
                        entryConveyor.Value = false;
                        loadLeft.Value = false;

                        if (!conveyor0Busy)
                            loadState = State.State1;
                        else if (!conveyor1Busy)
                            loadState = State.State10;
                        else if (!conveyor2Busy)
                            loadState = State.State20;
                    }
                }
            }
            else if (loadState == State.State1) //Send to conveyor 0
            {
                loadLeft.Value = true;
                conveyor0.Value = true;
                conveyor0Busy = true;

                if (ftAtEntry0.Q)
                {
                    conveyor0.Value = false;
                    loadLeft.Value = false;
                    loadState = State.State0;
                }
            }
            else if (loadState == State.State10) //Send to conveyor 1
            {
                leftElevatorState = State.State2;
                loadState = State.State11;
            }
            else if (loadState == State.State11)
            {
                if (leftElevatorState == State.State0) //If the elevator stopped at conveyor 1
                {
                    conveyor1.Value = true;
                    conveyor1Busy = true;
                    loadLeft.Value = true;

                    if (ftAtEntry1.Q)
                    {
                        conveyor1.Value = false;
                        loadLeft.Value = false;

                        leftElevatorState = State.State1; //Send to floor 0
                        loadState = State.State12;
                    }
                }
            }
            else if (loadState == State.State12)
            {
                if (leftElevatorState == State.State0) //If at floor 0 repeat
                    loadState = State.State0;
            }
            else if (loadState == State.State20)  //Send to conveyor 2
            {
                leftElevatorState = State.State3;
                loadState = State.State21;
            }
            else if (loadState == State.State21)
            {
                if (leftElevatorState == State.State0) //If the elevator stopped at conveyor 2
                {
                    conveyor2.Value = true;
                    conveyor2Busy = true;
                    loadLeft.Value = true;

                    if (ftAtEntry2.Q)
                    {
                        conveyor2.Value = false;
                        loadLeft.Value = false;

                        leftElevatorState = State.State1; //Send to floor 0
                        loadState = State.State22;
                    }
                }
            }
            else if (loadState == State.State22)
            {
                if (leftElevatorState == State.State0) //If at floor 0 repeat
                    loadState = State.State0;
            }

            #endregion

            #region Unload From Conveyors

            if (unloadState == State.State0) //Choose where to pick from
            {
                if (conveyor0Busy)
                    unloadState = State.State1;
                else if (conveyor1Busy)
                    unloadState = State.State10;
                else if (conveyor2Busy)
                    unloadState = State.State20;
            }
            else if (unloadState == State.State1) //Send elevator to floor 0
            {
                rightElevatorState = State.State1;
                unloadState = State.State2;
            }
            else if (unloadState == State.State2) //Wait for elevator to reach floor 0
            {
                if (rightElevatorState == State.State0)
                    unloadState = State.State3;
            }
            else if (unloadState == State.State3) //Elevator at floor 0, start unloading
            {
                conveyor0.Value = true;
                loadRight.Value = true;
                exitConveyor.Value = true;

                if (ftAtExitRight.Q)
                {
                    conveyor0.Value = false;
                    conveyor0Busy = false;
                    loadRight.Value = false;

                    unloadState = State.State4;
                }
            }
            else if (unloadState == State.State4) //Unloading finished
            {
                if (ftAtExit.Q)
                {
                    exitConveyor.Value = false;
                    unloadState = State.State0;
                }
            }
            else if (unloadState == State.State10) //Send elevator to floor 1
            {
                rightElevatorState = State.State2;
                unloadState = State.State11;
            }
            else if (unloadState == State.State11) //Wait for elevator to reach floor 1
            {
                if (rightElevatorState == State.State0)
                    unloadState = State.State12;
            }
            else if (unloadState == State.State12) //Elevator at floor 1, start loading
            {
                conveyor1.Value = true;
                loadRight.Value = true;

                if (rtAtExitRight.Q)
                {
                    conveyor1.Value = false;
                    conveyor1Busy = false;
                    loadRight.Value = false;

                    rightElevatorState = State.State1; //Move to floor 0
                    unloadState = State.State13;
                }
            }
            else if (unloadState == State.State13) //Wait for elevator to reach floor 0
            {
                if (rightElevatorState == State.State0)
                    unloadState = State.State14;
            }
            else if (unloadState == State.State14) //Elevator at floor 0, unload
            {
                loadRight.Value = true;
                exitConveyor.Value = true;

                if (ftAtExit.Q)
                {
                    loadRight.Value = false;
                    exitConveyor.Value = false;

                    unloadState = State.State0;
                }
            }
            else if (unloadState == State.State20) //Send elevator to floor 2
            {
                rightElevatorState = State.State3;
                unloadState = State.State21;
            }
            else if (unloadState == State.State21) //Wait for elevator to reach floor 2
            {
                if (rightElevatorState == State.State0)
                    unloadState = State.State22;
            }
            else if (unloadState == State.State22) //Elevator at floor 1, start loading
            {
                conveyor2.Value = true;
                loadRight.Value = true;

                if (rtAtExitRight.Q)
                {
                    conveyor2.Value = false;
                    conveyor2Busy = false;
                    loadRight.Value = false;

                    rightElevatorState = State.State1; //Move to floor 0
                    unloadState = State.State23;
                }
            }
            else if (unloadState == State.State23) //Wait for elevator to reach floor 0
            {
                if (rightElevatorState == State.State0)
                    unloadState = State.State24;
            }
            else if (unloadState == State.State24) //Elevator at floor 0, unload
            {
                loadRight.Value = true;
                exitConveyor.Value = true;

                if (ftAtExit.Q)
                {
                    loadRight.Value = false;
                    exitConveyor.Value = false;

                    unloadState = State.State0;
                }
            }

            #endregion
        }
    }
}
