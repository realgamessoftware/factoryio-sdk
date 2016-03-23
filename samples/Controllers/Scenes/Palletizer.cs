//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using EngineIO;

namespace Controllers
{
    public class Palletizer : Controller
    {
        MemoryBit palletFeeder = MemoryMap.Instance.GetBit("Pallet feeder", MemoryType.Output);
        MemoryBit loadPallet = MemoryMap.Instance.GetBit("Load pallet", MemoryType.Output);
        MemoryBit elevatorUp = MemoryMap.Instance.GetBit("Elevator up", MemoryType.Output);
        MemoryBit elevatorDown = MemoryMap.Instance.GetBit("Elevator down", MemoryType.Output);
        MemoryBit moveToLimit = MemoryMap.Instance.GetBit("Move to limit", MemoryType.Output);
        MemoryBit warningLight = MemoryMap.Instance.GetBit("Warning light", MemoryType.Output);
        MemoryBit boxFeeder = MemoryMap.Instance.GetBit("Box feeder", MemoryType.Output);
        MemoryBit loadBelt = MemoryMap.Instance.GetBit("Load belt", MemoryType.Output);
        MemoryBit push = MemoryMap.Instance.GetBit("Push", MemoryType.Output);
        MemoryBit clamp = MemoryMap.Instance.GetBit("Clamp", MemoryType.Output);
        MemoryBit openPlate = MemoryMap.Instance.GetBit("Open plate", MemoryType.Output);
        MemoryBit exitConveyor = MemoryMap.Instance.GetBit("Exit conveyor", MemoryType.Output);
        MemoryBit turn = MemoryMap.Instance.GetBit("Turn", MemoryType.Output);

        MemoryBit palletLoaded = MemoryMap.Instance.GetBit("Pallet loaded", MemoryType.Input);
        MemoryBit palletAtEntry = MemoryMap.Instance.GetBit("Pallet at entry", MemoryType.Input);
        MemoryBit palletAtExit = MemoryMap.Instance.GetBit("Pallet at exit", MemoryType.Input);
        MemoryBit elevatorMoving = MemoryMap.Instance.GetBit("Elevator moving", MemoryType.Input);
        MemoryBit boxAtEntry = MemoryMap.Instance.GetBit("Box at entry", MemoryType.Input);
        MemoryBit pusherLimit = MemoryMap.Instance.GetBit("Pusher limit", MemoryType.Input);
        MemoryBit clamped = MemoryMap.Instance.GetBit("Clamped", MemoryType.Input);
        MemoryBit plateLimit = MemoryMap.Instance.GetBit("Plate limit", MemoryType.Input);

        bool loadingPallet = false;
        bool elevatorBusy = false;
        bool doLayer = false;

        int movingToLayer = 0;
        int boxCount = 0;
        int boxesInLayer = 0;

        State palletizingState = State.State0;
        State unloadingState = State.State0;

        bool oddEven = false;

        FTRIG ftElevatorMoving = new FTRIG();
        FTRIG ftAtEntry = new FTRIG();
        RTRIG rtPusherLimit = new RTRIG();
        RTRIG rtPlateLimit = new RTRIG();
        FTRIG ftPalletAtExit = new FTRIG();

        TON ton_loadBoxes = new TON();

        public Palletizer()
        {
            palletFeeder.Value = false;
            elevatorUp.Value = false;
            elevatorDown.Value = false;
            moveToLimit.Value = false;
            warningLight.Value = false;
            boxFeeder.Value = false;
            loadBelt.Value = false;
            push.Value = false;
            clamp.Value = false;
            openPlate.Value = false;

            doLayer = true;

            ton_loadBoxes.PT = 2000;
        }

        public override void Execute(int elapsedMilliseconds)
        {
            ftElevatorMoving.CLK(elevatorMoving.Value);
            ftAtEntry.CLK(!boxAtEntry.Value);
            ftPalletAtExit.CLK(!palletAtExit.Value);
            rtPusherLimit.CLK(pusherLimit.Value);
            rtPlateLimit.CLK(plateLimit.Value);

            if (!palletAtEntry.Value && !elevatorBusy)
            {
                loadingPallet = true;
            }

            if (loadingPallet)
            {
                palletFeeder.Value = true;
                loadPallet.Value = true;
                warningLight.Value = true;

                if (palletLoaded.Value)
                {
                    palletFeeder.Value = false;
                    loadPallet.Value = false;

                    loadingPallet = false;
                    elevatorBusy = true;
                    movingToLayer = 0;
                }
            }

            if (elevatorBusy)
            {
                if (movingToLayer == 0)
                {
                    elevatorDown.Value = false;
                    elevatorUp.Value = true;
                    moveToLimit.Value = true;

                    if (ftElevatorMoving.Q)
                    {
                        elevatorUp.Value = false;
                        moveToLimit.Value = false;
                    }
                }
                else if (movingToLayer == 3)
                {
                    if (unloadingState == 0)
                    {
                        elevatorDown.Value = true;
                        moveToLimit.Value = true;

                        if (ftElevatorMoving.Q)
                        {
                            warningLight.Value = false;
                            unloadingState = State.State1;
                        }
                    }
                    else if (unloadingState == State.State1)
                    {
                        loadPallet.Value = true;
                        exitConveyor.Value = true;

                        if (ftPalletAtExit.Q)
                        {
                            loadPallet.Value = false;
                            exitConveyor.Value = false;

                            unloadingState = 0;

                            elevatorBusy = false;

                            loadingPallet = true;
                        }
                    }
                }
             }

            if (doLayer)
            {
                if (palletizingState == State.State0) //Load 3 boxes
                {
                    loadBelt.Value = true;

                    if (ftAtEntry.Q)
                    {
                        openPlate.Value = false;
                        boxesInLayer++;
                        boxCount++;
                    }

                    if (oddEven)
                    {
                        if (boxCount == 3 || boxCount == 6)
                        {
                            boxCount = 0;
                            palletizingState = State.State1;
                        }
                    }
                    else
                    {
                        if (boxCount == 2 || boxCount == 6)
                        {
                            boxCount = 0;
                            palletizingState = State.State1;
                        }
                    }
                }
                else if (palletizingState == State.State1) //Wait 1s
                { 
                    ton_loadBoxes.IN = true;

                    if (ton_loadBoxes.Q)
                    {
                        ton_loadBoxes.IN = false;

                        loadBelt.Value = false;

                        palletizingState = State.State2;
                    }
                }
                else if (palletizingState == State.State2) //Push
                {
                    if (movingToLayer != 3)
                    {
                        push.Value = true;

                        if (rtPusherLimit.Q)
                        {
                            push.Value = false;

                            palletizingState = State.State3;
                        }
                    }
                }
                else if (palletizingState == State.State3)
                {
                    if (rtPusherLimit.Q)
                    {
                        if (boxesInLayer == 6)
                        {
                            oddEven = !oddEven;
                            palletizingState = State.State4; //If we have 6 boxes continue
                        }
                        else
                        {
                            palletizingState = 0; //If need more 3 boxes repeat
                        }
                    }
                }
                else if (palletizingState == State.State4) //Clamp
                {
                    boxesInLayer = 0;

                    clamp.Value = true;

                    if (clamped.Value)
                    {
                        palletizingState = State.State5;
                    }
                }
                else if (palletizingState == State.State5) //Drop
                {
                    openPlate.Value = true;

                    if (rtPlateLimit.Q)
                    {
                        clamp.Value = false;

                        movingToLayer++;

                        if (movingToLayer < 3)
                            palletizingState = State.State6;
                        else
                            palletizingState = State.State0;
                    }
                }
                else if (palletizingState == State.State6)
                {
                    elevatorUp.Value = false;
                    moveToLimit.Value = false;
                    elevatorDown.Value = true;

                    if (ftElevatorMoving.Q)
                    {
                        elevatorDown.Value = false;
                        openPlate.Value = false;

                        palletizingState = 0;
                    }
                }
            }

            palletFeeder.Value = palletAtEntry.Value || loadingPallet;
            boxFeeder.Value = boxAtEntry.Value || (palletizingState == State.State0);
            turn.Value = oddEven;
        }
    }
}
