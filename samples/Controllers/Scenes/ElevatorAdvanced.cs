//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

using EngineIO;

namespace Controllers
{
    public class ElevatorAdvanced : Controller
    {
        const int ItemCount = 4;

        //Actuators
        MemoryBit conveyor0 = MemoryMap.Instance.GetBit("Conveyor 0", MemoryType.Output);
        MemoryBit conveyor1 = MemoryMap.Instance.GetBit("Conveyor 1", MemoryType.Output);
        MemoryBit conveyor2 = MemoryMap.Instance.GetBit("Conveyor 2", MemoryType.Output);
        MemoryBit conveyor3 = MemoryMap.Instance.GetBit("Conveyor 3", MemoryType.Output);
        MemoryBit load = MemoryMap.Instance.GetBit("Load", MemoryType.Output);
        MemoryBit unload = MemoryMap.Instance.GetBit("Unload", MemoryType.Output);
        MemoryBit exitConveyor = MemoryMap.Instance.GetBit("Exit conveyor", MemoryType.Output);

        MemoryBit up = MemoryMap.Instance.GetBit("Up", MemoryType.Output);
        MemoryBit slow= MemoryMap.Instance.GetBit("Slow", MemoryType.Output);
        MemoryBit down = MemoryMap.Instance.GetBit("Down", MemoryType.Output);
        MemoryBit warningLight = MemoryMap.Instance.GetBit("Warning light", MemoryType.Output);

        MemoryBit floor1Light = MemoryMap.Instance.GetBit("Floor 1", MemoryType.Output);
        MemoryBit floor2Light = MemoryMap.Instance.GetBit("Floor 2", MemoryType.Output);
        MemoryBit floor3Light = MemoryMap.Instance.GetBit("Floor 3", MemoryType.Output);

        //Sensors
        MemoryBit atElevator = MemoryMap.Instance.GetBit("At elevator", MemoryType.Input);
        MemoryBit atEntry = MemoryMap.Instance.GetBit("At entry", MemoryType.Input);
        MemoryBit atExit = MemoryMap.Instance.GetBit("At exit", MemoryType.Input);
        MemoryBit at1 = MemoryMap.Instance.GetBit("At 1", MemoryType.Input);
        MemoryBit at2 = MemoryMap.Instance.GetBit("At 2", MemoryType.Input);
        MemoryBit at3 = MemoryMap.Instance.GetBit("At 3", MemoryType.Input);
        MemoryBit at0Low = MemoryMap.Instance.GetBit("At 0 (low)", MemoryType.Input);
        MemoryBit at0High = MemoryMap.Instance.GetBit("At 0 (high)", MemoryType.Input);
        MemoryBit at1Low = MemoryMap.Instance.GetBit("At 1 (low)", MemoryType.Input);
        MemoryBit at1High = MemoryMap.Instance.GetBit("At 1 (high)", MemoryType.Input);
        MemoryBit at2Low = MemoryMap.Instance.GetBit("At 2 (low)", MemoryType.Input);
        MemoryBit at2High = MemoryMap.Instance.GetBit("At 2 (high)", MemoryType.Input);
        MemoryBit at3Low = MemoryMap.Instance.GetBit("At 3 (low)", MemoryType.Input);
        MemoryBit at3High = MemoryMap.Instance.GetBit("At 3 (high)", MemoryType.Input);

        MemoryBit floor0 = MemoryMap.Instance.GetBit("Floor 0", MemoryType.Input);
        MemoryBit floor1 = MemoryMap.Instance.GetBit("Floor 1", MemoryType.Input);
        MemoryBit floor2 = MemoryMap.Instance.GetBit("Floor 2", MemoryType.Input);
        MemoryBit floor3 = MemoryMap.Instance.GetBit("Floor 3", MemoryType.Input);

        State loadUnloadState;
        State getItemsState;
        State elevatorState;

        int elevatorFloor;

        int itemsInBox;

        RTRIG rtAtElevator = new RTRIG();
        RTRIG rtAtExit = new RTRIG();
        FTRIG ftAtEntry = new FTRIG();
        FTRIG ftAtExit = new FTRIG();
        FTRIG ftAt1 = new FTRIG();
        FTRIG ftAt2 = new FTRIG();
        FTRIG ftAt3 = new FTRIG();
        RTRIG rtAt1 = new RTRIG();
        RTRIG rtAt2 = new RTRIG();
        RTRIG rtAt3 = new RTRIG();

        /// <summary>
        /// Each item ready to be picked up by the elevator adds an elevator state to this queue.
        /// An elevator state describes which floor to go to; state 2 = floor 1, state 3 = floor 2, state 4 = floor 3.
        /// </summary>
        Queue<State> itemsQueue = new Queue<State>();

        TON waitForItemTimer = new TON();

        public ElevatorAdvanced()
        {
            loadUnloadState = State.State1;
            getItemsState = State.State0;
            elevatorState = State.State0;

            conveyor0.Value = false;
            conveyor1.Value = true;
            conveyor2.Value = true;
            conveyor3.Value = true;
            exitConveyor.Value = false;

            up.Value = false;
            down.Value = false;
            slow.Value = false;

            waitForItemTimer.PT = 2000;
        }

        public override void Execute(int elapsedMilliseconds)
        {
            rtAtElevator.CLK(!atElevator.Value);
            rtAtExit.CLK(!atExit.Value);
            ftAtEntry.CLK(!atEntry.Value);
            ftAtExit.CLK(!atExit.Value);
            ftAt1.CLK(!at1.Value);
            ftAt2.CLK(!at2.Value);
            ftAt3.CLK(!at3.Value);
            rtAt1.CLK(!at1.Value);
            rtAt2.CLK(!at2.Value);
            rtAt3.CLK(!at3.Value);

            #region Elevator

            if (elevatorState == State.State0)
            {
                up.Value = false;
                slow.Value = false;
                down.Value = false;

                if (at0Low.Value && at0High.Value)
                {
                    elevatorFloor = 0;
                }
                else if (at1Low.Value && at1High.Value)
                {
                    elevatorFloor = 1;
                }
                else if (at2Low.Value && at2High.Value)
                {
                    elevatorFloor = 2;
                }
                else if (at3Low.Value && at3High.Value)
                {
                    elevatorFloor = 3;
                }
            }
            else if (elevatorState == State.State1) //Floor 0
            {
                if (elevatorFloor > 0)
                {
                    slow.Value = at0High.Value;
                    down.Value = true;

                    if (at0Low.Value && at0High.Value)
                        elevatorState = State.State0;
                }
                else
                {
                    elevatorState = State.State0;
                }
            }
            else if (elevatorState == State.State2) //Floor 1
            {
                if (elevatorFloor > 1)
                {
                    slow.Value = at1High.Value;
                    down.Value = true;

                    if (at1Low.Value && at1High.Value)
                        elevatorState = State.State0;
                }
                else if (elevatorFloor < 1)
                {
                    up.Value = true;
                    slow.Value = at1Low.Value;

                    if (at1Low.Value && at1High.Value)
                        elevatorState = State.State0;
                }
                else
                {
                    elevatorState = State.State0;
                }
            }
            else if (elevatorState == State.State3) //Floor 2
            {
                if (elevatorFloor > 2)
                {
                    slow.Value = at2High.Value;
                    down.Value = true;

                    if (at2Low.Value && at2High.Value)
                        elevatorState = State.State0;
                }
                else if (elevatorFloor < 2)
                {
                    up.Value = true;
                    slow.Value = at2Low.Value;

                    if (at2Low.Value && at2High.Value)
                        elevatorState = State.State0;
                }
                else
                {
                    elevatorState = State.State0;
                }
            }
            else if (elevatorState == State.State4) //Floor 3
            {
                if (elevatorFloor < 3)
                {
                    up.Value = true;
                    slow.Value = at3Low.Value;

                    if (at3Low.Value && at3High.Value)
                        elevatorState = State.State0;
                }
                else
                {
                    elevatorState = State.State0;
                }
            }

            #endregion

            #region Load and Unload

            if (loadUnloadState == State.State0) //Reset state
            {
                conveyor0.Value = false;
                load.Value = false;
                exitConveyor.Value = false;
            }
            else if (loadUnloadState == State.State1) //Load Start
            {
                conveyor0.Value = true;
                load.Value = false;
                exitConveyor.Value = false;

                if (rtAtElevator.Q)
                    loadUnloadState = State.State2;
            }
            else if (loadUnloadState == State.State2) //Load
            {
                load.Value = true;

                //Load is finished start the get items process
                if (ftAtEntry.Q)
                {
                    getItemsState = State.State1;
                    loadUnloadState = State.State0;
                }
            }
            else if (loadUnloadState == State.State3) //Unload Start
            {
                load.Value = true;

                if (rtAtExit.Q)
                    loadUnloadState = State.State4;
            }
            else if (loadUnloadState == State.State4) //Unload
            {
                exitConveyor.Value = true;

                if (ftAtExit.Q)
                {
                    //Load another
                    loadUnloadState = State.State1;
                }
            }

            #endregion

            #region Get Items

            if (getItemsState == State.State0)  //Switch between load and unload
            {
                //If we have all the items unload them
                if (itemsInBox == ItemCount)
                {
                    //Wait for elevator to reach floor 0
                    if (elevatorState == State.State0)
                    {
                        loadUnloadState = State.State3;

                        itemsInBox = 0;
                    }
                }
            }
            else if (getItemsState == State.State1)
            {
                //If the elevator is at destination
                if (elevatorState == State.State0)
                {
                    //If there are any items pick them up
                    if (itemsQueue.Count > 0)
                    {
                        elevatorState = itemsQueue.Dequeue();

                        if (elevatorState == State.State2)
                        {
                            Console.WriteLine("Getting item from floor 1");
                            getItemsState = State.State2;
                        }
                        else if (elevatorState == State.State3)
                        {
                            Console.WriteLine("Getting item from floor 2");
                            getItemsState = State.State4;
                        }
                        else if (elevatorState == State.State4)
                        {
                            Console.WriteLine("Getting item from floor 3");
                            getItemsState = State.State6;
                        }
                    }
                }
            }
            else if (getItemsState == State.State2) //Get from floor 1
            {
                //Wait for elevator
                if (elevatorState == State.State0)
                {
                    conveyor1.Value = true;

                    if (ftAt1.Q)
                    {
                        itemsInBox++;
                        getItemsState = State.State3;

                        Console.WriteLine("Got item from floor 1 (total:" + itemsInBox + ")");
                    }
                }
            }
            else if (getItemsState == State.State3) //Wait in floor 1
            {
                waitForItemTimer.IN = true;

                if (waitForItemTimer.Q)
                {
                    Console.WriteLine("Waited 2s at floor 1");

                    waitForItemTimer.IN = false;

                    if (itemsInBox == ItemCount)
                    {
                        getItemsState = State.State0;
                        elevatorState = State.State1;
                    }
                    else
                    {
                        getItemsState = State.State1;
                    }
                }
            }
            else if (getItemsState == State.State4) //Get from floor 2
            {
                //Wait for elevator
                if (elevatorState == State.State0)
                {
                    conveyor2.Value = true;

                    if (ftAt2.Q)
                    {
                        itemsInBox++;
                        getItemsState = State.State5;

                        Console.WriteLine("Got item from floor 2 (total:" + itemsInBox + ")");
                    }
                }
            }
            else if (getItemsState == State.State5) //Wait in floor 2
            {
                waitForItemTimer.IN = true;

                if (waitForItemTimer.Q)
                {
                    Console.WriteLine("Waited 2s at floor 2");

                    waitForItemTimer.IN = false;

                    if (itemsInBox == ItemCount)
                    {
                        getItemsState = State.State0;
                        elevatorState = State.State1;
                    }
                    else
                    {
                        getItemsState = State.State1;
                    }
                }
            }
            else if (getItemsState == State.State6) //Get from floor 3
            {
                //Wait for elevator
                if (elevatorState == State.State0)
                {
                    conveyor3.Value = true;

                    if (ftAt3.Q)
                    {
                        itemsInBox++;
                        getItemsState = State.State7;

                        Console.WriteLine("Got item from floor 3 (total:" + itemsInBox + ")");
                    }
                }
            }
            else if (getItemsState == State.State7) //Wait in floor 3
            {
                waitForItemTimer.IN = true;

                if (waitForItemTimer.Q)
                {
                    Console.WriteLine("Waited 2s at floor 3");

                    waitForItemTimer.IN = false;

                    if (itemsInBox == ItemCount)
                    {
                        getItemsState = State.State0;
                        elevatorState = State.State1;
                    }
                    else
                    {
                        getItemsState = State.State1;
                    }
                }
            }

            #endregion

            #region FIFO

            if (rtAt1.Q)
                itemsQueue.Enqueue(State.State2);

            if (rtAt2.Q)
                itemsQueue.Enqueue(State.State3);

            if (rtAt2.Q)
                itemsQueue.Enqueue(State.State4);

            #endregion

            #region Conveyors

            if (rtAt1.Q)
                conveyor1.Value = false;

            if (rtAt2.Q)
                conveyor2.Value = false;

            if (rtAt3.Q)
                conveyor3.Value = false;

            #endregion

            #region Floor Lights

            floor1Light.Value = at1Low.Value && at1High.Value;
            floor2Light.Value = at2Low.Value && at2High.Value;
            floor3Light.Value = at3Low.Value && at3High.Value;

            #endregion

            warningLight.Value = elevatorState != State.State0;
        }
    }
}
