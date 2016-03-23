//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Threading;

namespace EngineIO.Samples
{
    class Program
    {
        //In this sample we are powering a roller conveyor on and off 10 times.
        //Use the Saved Scene "SDK Write Sample" provided in the Saved Scene folder.
        static void Main(string[] args)
        {
            //We are using a MemoryBit which we get from the MemoryMap.
            //You can find the memory addresses of a specific Factory I/O Tag by toggling the visibility of the Tags Address in the View Menu, or by docking the Tag.
            MemoryBit rollerConveyor = MemoryMap.Instance.GetBit(0, MemoryType.Output);

            for (int i = 0; i < 10; i++)
            {
                rollerConveyor.Value = !rollerConveyor.Value;

                //When using a memory value before calling the Update method we are using a cached value.
                Console.WriteLine("Is the roller conveyor on? " + rollerConveyor.Value);

                //Calling the Update method will write the rollerConveyor.Value to the memory map.
                MemoryMap.Instance.Update();

                Thread.Sleep(1000);
            }

            //When we no longer need the MemoryMap we should call the Dispose method to release all the allocated resources.
            MemoryMap.Instance.Dispose();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
