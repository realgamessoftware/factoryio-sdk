//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using EngineIO;

namespace Controllers
{
    public class BufferStation : Controller
    {
        MemoryFloat bufferConveyor = MemoryMap.Instance.GetFloat("Buffer conveyor", MemoryType.Output);
        MemoryFloat exitConveyor = MemoryMap.Instance.GetFloat("Exit conveyor", MemoryType.Output);
        MemoryBit stopBlade = MemoryMap.Instance.GetBit("Stop blade", MemoryType.Output);

        MemoryBit atBufferExit = MemoryMap.Instance.GetBit("At buffer exit", MemoryType.Input);
        MemoryBit atExit = MemoryMap.Instance.GetBit("At exit", MemoryType.Input);
        MemoryFloat bufferVel = MemoryMap.Instance.GetFloat("Buffer Vel.", MemoryType.Input);

        FTRIG ftAtBufferExit = new FTRIG();

        State state = State.State0;

        public BufferStation()
        {
            bufferConveyor.Value = 0;
            exitConveyor.Value = 10;
            stopBlade.Value = false;
        }

        public override void Execute(int elapsedMilliseconds)
        {
            ftAtBufferExit.CLK(!atBufferExit.Value);

            if (state == 0)
            {
                bufferConveyor.Value = bufferVel.Value;
                //bufferConveyor.Value = 7;
                exitConveyor.Value = 0;
                stopBlade.Value = true;

                if (!atBufferExit.Value)
                {
                    stopBlade.Value = false;
                    exitConveyor.Value = 10;
                    state = State.State1;
                }
            }
            else if (state == State.State1)
            {
                if (ftAtBufferExit.Q)
                {
                    stopBlade.Value = true;
                    state = State.State2;
                }
            }
            else if (state == State.State2)
            {
                bufferConveyor.Value = atBufferExit.Value ? bufferVel.Value : 0;
                //bufferConveyor.Value = atBufferExit.Value ? 7 : 0;

                if (!atExit.Value)
                    state = State.State0;
            }
        }
    }
}
