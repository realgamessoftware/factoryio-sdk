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
        //In this sample we are showing how to use the InputsNameChanged, InputsValueChange, OutputsNameChanged and OutputsValueChange events.
        //Add, change and remove Tags in Factory I/O to get notified about its memories changes (no Saved Scene needed).
        static void Main(string[] args)
        {
            //Registering on the events
            MemoryMap.Instance.InputsNameChanged += new MemoriesChangedEventHandler(Instance_NameChanged);
            MemoryMap.Instance.InputsValueChanged += new MemoriesChangedEventHandler(Instance_ValueChanged);
            MemoryMap.Instance.OutputsNameChanged += new MemoriesChangedEventHandler(Instance_NameChanged);
            MemoryMap.Instance.OutputsValueChanged += new MemoriesChangedEventHandler(Instance_ValueChanged);

            Console.WriteLine("Press any key to exit...");

            //Calling the Update method will fire events if any memory value or name changed.
            //When a Tag is created in Factory I/O a name is given to its memory, firing the name changed event, and when a tag's value is changed, it is fired the value changed event.
            //In this case we are updating the MemoryMap each 16 milliseconds (the typical update rate of Factory I/O).
            while (!Console.KeyAvailable)
            {
                MemoryMap.Instance.Update();

                Thread.Sleep(16);
            }  

            //When we no longer need the MemoryMap we should call the Dispose method to release all the allocated resources.
            MemoryMap.Instance.Dispose();
        }

        static void Instance_NameChanged(MemoryMap sender, MemoriesChangedEventArgs value)
        {
            //Display any changed MemoryBit
            foreach (MemoryBit mem in value.MemoriesBit)
            {
                if (mem.HasName)
                    Console.WriteLine(string.Format("{0} Bit({1}) name changed to: {2}", mem.MemoryType.ToString(), mem.Address, mem.Name));
                else
                    Console.WriteLine(string.Format("{0} Bit({1}) name cleared", mem.MemoryType.ToString(), mem.Address));
            }

            //Display any changed MemoryFloat
            foreach (MemoryFloat mem in value.MemoriesFloat)
            {
                if (mem.HasName)
                    Console.WriteLine(string.Format("{0} Float({1}) name changed to: {2}", mem.MemoryType.ToString(), mem.Address, mem.Name));
                else
                    Console.WriteLine(string.Format("{0} Float({1}) name cleared", mem.MemoryType.ToString(), mem.Address));
            }

            //Display any changed MemoryInt
            foreach (MemoryInt mem in value.MemoriesInt)
            {
                if (mem.HasName)
                    Console.WriteLine(string.Format("{0} Int({1}) name changed to: {2}", mem.MemoryType.ToString(), mem.Address, mem.Name));
                else
                    Console.WriteLine(string.Format("{0} Int({1}) name cleared", mem.MemoryType.ToString(), mem.Address));
            }
        }

        static void Instance_ValueChanged(MemoryMap sender, MemoriesChangedEventArgs value)
        {
            //Display any changed MemoryBit
            foreach (MemoryBit mem in value.MemoriesBit)
            {
                Console.WriteLine(string.Format("{0} Bit{1} value changed to: {2}", mem.MemoryType.ToString(), mem.Address, mem.Value.ToString()));
            }

            //Display any changed MemoryFLoat
            foreach (MemoryFloat mem in value.MemoriesFloat)
            {
                Console.WriteLine(string.Format("{0} Float{1} value changed to: {2}", mem.MemoryType.ToString(), mem.Address, mem.Value.ToString()));
            }

            //Display any changed MemoryInt
            foreach (MemoryInt mem in value.MemoriesInt)
            {
                Console.WriteLine(string.Format("{0} Int{1} value changed to: {2}", mem.MemoryType.ToString(), mem.Address, mem.Value.ToString()));
            }
        }
    }
}
