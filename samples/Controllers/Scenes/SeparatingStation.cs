//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using EngineIO;

namespace Controllers
{
    public class SeparatingStation : Controller
    {
        MemoryBit entryConveyor1 = MemoryMap.Instance.GetBit("Entry conveyor 1", MemoryType.Output);
        MemoryBit entryConveyor2 = MemoryMap.Instance.GetBit("Entry conveyor 2", MemoryType.Output);
        MemoryBit pusher1 = MemoryMap.Instance.GetBit("Pusher 1", MemoryType.Output);
        MemoryBit pusher2 = MemoryMap.Instance.GetBit("Pusher 2", MemoryType.Output);
        MemoryBit conveyor1 = MemoryMap.Instance.GetBit("Conveyor 1", MemoryType.Output);
        MemoryBit conveyor2 = MemoryMap.Instance.GetBit("Conveyor 2", MemoryType.Output);

        MemoryInt visionSensor1 = MemoryMap.Instance.GetInt("Vision sensor 1", MemoryType.Input);
        MemoryInt visionSensor2 = MemoryMap.Instance.GetInt("Vision sensor 2", MemoryType.Input);
        MemoryBit sensor1 = MemoryMap.Instance.GetBit("Sensor 1", MemoryType.Input);
        MemoryBit sensor2 = MemoryMap.Instance.GetBit("Sensor 2", MemoryType.Input);
        MemoryBit atPusher1Exit = MemoryMap.Instance.GetBit("At pusher 1 exit", MemoryType.Input);
        MemoryBit atPusher2Exit = MemoryMap.Instance.GetBit("At pusher 2 exit", MemoryType.Input);
        MemoryBit pusherBack1 = MemoryMap.Instance.GetBit("Pusher 1 back", MemoryType.Input);
        MemoryBit pusherFront1 = MemoryMap.Instance.GetBit("Pusher 1 front", MemoryType.Input);
        MemoryBit pusherBack2 = MemoryMap.Instance.GetBit("Pusher 2 back", MemoryType.Input);
        MemoryBit pusherFront2 = MemoryMap.Instance.GetBit("Pusher 2 front", MemoryType.Input);

        FTRIG ftSensor1 = new FTRIG();
        FTRIG ftSensor2 = new FTRIG();
        FTRIG ftAtPusher1Exit = new FTRIG();
        FTRIG ftAtPusher2Exit = new FTRIG();

        State state = State.State0;

        int counter;

        public SeparatingStation()
        {
            entryConveyor1.Value = false;
            entryConveyor2.Value = false;
            pusher1.Value = false;
            pusher2.Value = false;
            conveyor1.Value = false;
            conveyor2.Value = false;
        }

        public override void Execute(int elapsedMilliseconds)
        {
            ftSensor1.CLK(sensor1.Value);
            ftSensor2.CLK(sensor2.Value);
            ftAtPusher1Exit.CLK(atPusher1Exit.Value);
            ftAtPusher2Exit.CLK(atPusher2Exit.Value);

            if (state == State.State0)
            {
                entryConveyor1.Value = (visionSensor1.Value == 0);
                entryConveyor2.Value = (visionSensor2.Value == 0);
                conveyor1.Value = false;
                conveyor2.Value = false;

                if (visionSensor1.Value != 0 && visionSensor2.Value != 0)
                {
                    if (visionSensor1.Value == 1 && visionSensor2.Value == 4)
                    {
                        entryConveyor1.Value = true;
                        entryConveyor2.Value = true;
                        conveyor1.Value = true;
                        conveyor2.Value = true;

                        counter = 0;

                        state = State.State1;
                    }
                    else
                    {
                        state = State.State10;
                    }
                }
            }
            else if (state == State.State1)
            {
                if (ftSensor1.Q)
                    entryConveyor1.Value = false;

                if (ftSensor2.Q)
                    entryConveyor2.Value = false;

                if (ftAtPusher1Exit.Q)
                {
                    conveyor1.Value = false;
                    counter++;
                }

                if (ftAtPusher2Exit.Q)
                {
                    conveyor2.Value = false;
                    counter++;
                }

                if (counter > 1)
                    state = State.State0;
            }
            else if (state == State.State10)
            {
                if (visionSensor1.Value == 4) //Green
                {
                    entryConveyor1.Value = true;
                    conveyor1.Value = true;

                    state = State.State11;
                }
                else if (visionSensor2.Value == 1) //Blue
                {
                    entryConveyor2.Value = true;
                    conveyor2.Value = true;

                    state = State.State20;
                }
            }
            else if (state == State.State11)
            {
                if (ftSensor1.Q)
                {
                    entryConveyor1.Value = false;
                    conveyor1.Value = false;
                    pusher1.Value = true;

                    state = State.State12;
                }
            }
            else if (state == State.State12)
            {
                if (pusherFront1.Value)
                {
                    pusher1.Value = false;
                    conveyor2.Value = true;
                }

                if (ftAtPusher2Exit.Q)
                    state = State.State0;
            }
            else if (state == State.State20)
            {
                if (ftSensor2.Q)
                {
                    entryConveyor2.Value = false;
                    conveyor2.Value = false;
                    pusher2.Value = true;

                    state = State.State21;
                }
            }
            else if (state == State.State21)
            {
                if (pusherFront2.Value)
                {
                    pusher2.Value = false;
                    conveyor1.Value = true;
                }

                if (ftAtPusher1Exit.Q)
                    state = State.State0;
            }
        }
    }
}
