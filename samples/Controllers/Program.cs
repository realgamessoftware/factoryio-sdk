//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Threading;
using System.Diagnostics;

using EngineIO;

namespace Controllers
{
    class Program
    {
        /// <summary>
        /// Cycle time in milliseconds.
        /// </summary>
        public const int CycleTime = 8;

        /// <summary>
        /// The idea of this sample is to demonstrate that Microsoft Visual Studio can be used as a soft PLC to
        /// control FACTORY I/O (requires Ultimate Edition).
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //Stopwatch used to measure elapsed time between cycles
            Stopwatch stopwatch = new Stopwatch();

            //MemoryBit used to switch FACTORY I/O between edit and run mode
            MemoryBit start = MemoryMap.Instance.GetBit(MemoryMap.BitCount - 16, MemoryType.Output);

            //MemoryBit used to detect if FACTORY I/O is edit or run mode
            MemoryBit running = MemoryMap.Instance.GetBit(MemoryMap.BitCount - 16, MemoryType.Input);

            //Forcing a rising edge on the start MemoryBit so FACTORY I/O can detect it
            SwitchToRun(start);

            //Uncomment ONLY one of the following lines to control the corresponding scene in FACTORY I/O
            Controller controller = new FromAToB();
            //Controller controller = new FromAToBSetReset();
            //Controller controller = new FillingTank();
            //Controller controller = new QueueOfItems();
            //Controller controller = new Assembler();
            //Controller controller = new AssemblerAnalog();
            //Controller controller = new AutomatedWarehouse();
            //Controller controller = new BufferStation();
            //Controller controller = new ConvergeStation();
            //Controller controller = new ElevatorAdvanced();
            //Controller controller = new ElevatorBasic();
            //Controller controller = new LevelControl();
            //Controller controller = new Palletizer();
            //Controller controller = new PickPlaceBasic();
            //Controller controller = new PickPlaceXYZ();
            //Controller controller = new ProductionLine();
            //Controller controller =  new SeparatingStation();
            //Controller controller = new SortingHeightAdvanced();
            //Controller controller = new SortingHeightBasic();
            //Controller controller = new SortingWeight();
            //Controller controller = new SortingStation();

            Console.WriteLine(string.Format("Running controller: {0}", controller.GetType().Name));
            Console.WriteLine("Press Escape to shutdown...");

            stopwatch.Start();

            Thread.Sleep(CycleTime);

            while (!(Console.KeyAvailable && (Console.ReadKey(false).Key == ConsoleKey.Escape)))
            {
                //Update the memory map before executing the controller
                MemoryMap.Instance.Update();

                if (running.Value)
                {
                    stopwatch.Stop();

                    controller.Execute((int)stopwatch.ElapsedMilliseconds);

                    stopwatch.Restart();
                }

                Thread.Sleep(CycleTime);
            }

            Shutdown(start);
        }

        static void SwitchToRun(MemoryBit start)
        {
            start.Value = false;
            MemoryMap.Instance.Update();
            Thread.Sleep(500);

            start.Value = true;
            MemoryMap.Instance.Update();
            Thread.Sleep(500);
        }

        static void Shutdown(MemoryBit start)
        {
            start.Value = false;

            MemoryMap.Instance.Update();
            MemoryMap.Instance.Dispose();
        }
    }
}
