//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

using EngineIO;

namespace Controllers
{
    public class FillingTank : Controller
    {
        MemoryBit fillValve = MemoryMap.Instance.GetBit("Fill valve", MemoryType.Output);
        MemoryBit dischargeValve = MemoryMap.Instance.GetBit("Discharge valve", MemoryType.Output);

        MemoryBit fillButton = MemoryMap.Instance.GetBit("Fill", MemoryType.Input);
        MemoryBit dischargeButton = MemoryMap.Instance.GetBit("Discharge", MemoryType.Input);
        MemoryBit fillingLight = MemoryMap.Instance.GetBit("Filling", MemoryType.Output);
        MemoryBit dischargingLight = MemoryMap.Instance.GetBit("Discharging", MemoryType.Output);

        MemoryInt timer = MemoryMap.Instance.GetInt("Timer", MemoryType.Output);

        TOF tofFill = new TOF();
        TOF tofDisch = new TOF();

        public FillingTank()
        {
            tofFill.PT = 8000;
            tofDisch.PT = 8000;

            fillValve.Value = false;
            dischargeValve.Value = false;

            fillingLight.Value = false;
            dischargingLight.Value = false;
        }

        public override void Execute(int elapsedMilliseconds)
        {
            //Fill
            tofFill.IN = fillButton.Value;

            fillValve.Value = tofFill.Q;
            fillingLight.Value = tofFill.Q;

            //Discharge
            tofDisch.IN = !dischargeButton.Value;

            dischargeValve.Value = tofDisch.Q;
            dischargingLight.Value = tofDisch.Q;

            //HMI
            if (tofFill.Q)
                timer.Value = 8 - tofFill.ET / 1000;
            else
                timer.Value = 8 - tofDisch.ET / 1000;
        }
    }
}
