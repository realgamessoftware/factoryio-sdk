//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using EngineIO;

namespace Controllers
{
    public class AssemblerAnalog : Controller
    {
        MemoryBit lidsConveyor1 = MemoryMap.Instance.GetBit("Lids conveyor 1", MemoryType.Output);
        MemoryBit lidsConveyor2 = MemoryMap.Instance.GetBit("Lids conveyor 2", MemoryType.Output);
        MemoryBit stopBlade1 = MemoryMap.Instance.GetBit("Stop blade 1", MemoryType.Output);

        MemoryBit basesConveyor1 = MemoryMap.Instance.GetBit("Bases conveyor 1", MemoryType.Output);
        MemoryBit basesConveyor2 = MemoryMap.Instance.GetBit("Bases conveyor 2", MemoryType.Output);
        MemoryBit stopBlade2 = MemoryMap.Instance.GetBit("Stop blade 2", MemoryType.Output);

        MemoryFloat setX = MemoryMap.Instance.GetFloat("Set X", MemoryType.Output);
        MemoryFloat setZ = MemoryMap.Instance.GetFloat("Set Z", MemoryType.Output);
        MemoryBit grab = MemoryMap.Instance.GetBit("Grab", MemoryType.Output);
        MemoryInt counter = MemoryMap.Instance.GetInt("Counter", MemoryType.Output);

        MemoryBit lidAtPlace = MemoryMap.Instance.GetBit("Lid at place", MemoryType.Input);
        MemoryBit baseAtPlace = MemoryMap.Instance.GetBit("Base at place", MemoryType.Input);
        MemoryBit partLeaving = MemoryMap.Instance.GetBit("Part leaving", MemoryType.Input);
        MemoryBit itemDetected = MemoryMap.Instance.GetBit("Item detected", MemoryType.Input);
        MemoryFloat positionX = MemoryMap.Instance.GetFloat("X", MemoryType.Input);
        MemoryFloat positionZ = MemoryMap.Instance.GetFloat("Z", MemoryType.Input);

        RTRIG rtLidAtPlace = new RTRIG();
        RTRIG rtBaseAtPlace = new RTRIG();
        FTRIG ftPartLeaving = new FTRIG();

        State stateLids = State.State0;
        State stateBases = State.State0;

        TON grabTimer = new TON();
        TON watchDogTimer = new TON();

        public AssemblerAnalog()
        {
            lidsConveyor1.Value = false;
            lidsConveyor2.Value = false;

            stopBlade1.Value = false;
            stopBlade2.Value = false;

            basesConveyor1.Value = true;
            basesConveyor2.Value = true;

            setX.Value = 0;
            setZ.Value = 0;
            grab.Value = false;

            counter.Value = 0;
        }

        public override void Execute(int elapsedMilliseconds)
        {
            rtLidAtPlace.CLK(lidAtPlace.Value);

            if (stateLids == State.State0)
            {
                lidsConveyor1.Value = true;
                stopBlade1.Value = true;

                setX.Value = 1.1f;

                if (rtLidAtPlace.Q)
                    stateLids = State.State1;
            }
            else if (stateLids == State.State1)
            {
                lidsConveyor1.Value = false;
                setZ.Value = 9;

                if (itemDetected.Value)
                {
                    grabTimer.PT = 1000;
                    grabTimer.IN = true;

                    stateLids = State.State2;
                }
            }
            else if (stateLids == State.State2)
            {
                grab.Value = true;

                if (grabTimer.Q)
                {
                    grabTimer.IN = false;

                    stateLids = State.State3;
                }
            }
            else if (stateLids == State.State3)
            {
                setZ.Value = 0;
                setX.Value = 8.85f;

                if (positionX.Value > 8.84f)
                {
                    watchDogTimer.PT = 3000;
                    watchDogTimer.IN = true;

                    stateLids = State.State4;
                }
            }
            else if (stateLids == State.State4)
            {
                setZ.Value = 10;
                stopBlade2.Value = false;

                watchDogTimer.IN = true;

                if (watchDogTimer.Q || positionZ.Value > 8.4f)
                {
                    if (!watchDogTimer.Q)
                        counter.Value++;

                    watchDogTimer.IN = false;

                    stateLids = State.State5;
                }
            }
            else if (stateLids == State.State5)
            {
                setZ.Value = 0;
                setX.Value = 0;
                grab.Value = false;

                if (stateBases != State.State0)
                    stateBases = State.State2;

                if (positionX.Value < 0.1f && positionZ.Value < 0.1f)
                    stateLids = State.State0;
            }

            //Bases
            rtBaseAtPlace.CLK(baseAtPlace.Value);
            ftPartLeaving.CLK(partLeaving.Value);

            if (stateBases == State.State0)
            {
                basesConveyor1.Value = true;
                basesConveyor2.Value = false;
                stopBlade2.Value = true;

                if (rtBaseAtPlace.Q)
                    stateBases = State.State1;
            }
            else if (stateBases == State.State1)
            {
                basesConveyor1.Value = false;
            }
            else if (stateBases == State.State2)
            {
                stopBlade2.Value = false;

                basesConveyor1.Value = true;
                basesConveyor2.Value = true;

                if (ftPartLeaving.Q)
                    stateBases = State.State0;
            }
        }
    }
}
