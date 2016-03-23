//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using System;

namespace EngineIO.Samples
{
    class Program
    {
        //In this sample we are reading the Diffuse Sensor 1 value. The value is only sampled once.
        //Use the Saved Scene "SDK Read Sample" provided in the Saved Scene folder.
        static void Main(string[] args)
        {
            //We are using a MemoryBit which we get from the MemoryMap.
            //You can find the memory addresses of a specific Factory I/O Tag by toggling the visibility of the Tags Address in the View Menu, or by docking the Tag.
            MemoryBit diffuseSensor = MemoryMap.Instance.GetBit(0, MemoryType.Input);

            //We must call the Update method each time we want to access the latest value.
            MemoryMap.Instance.Update();

            Console.WriteLine("Diffuse Sensor 1 value: " + diffuseSensor.Value.ToString());

            //When we no longer need the MemoryMap we should call the Dispose method to release all the allocated resources.
            MemoryMap.Instance.Dispose();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
