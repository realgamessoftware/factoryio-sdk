//-----------------------------------------------------------------------------
// FACTORY I/O (SDK)
//
// Copyright (C) Real Games. All rights reserved.
//-----------------------------------------------------------------------------

namespace Controllers
{
    /// <summary>
    /// Enumeration used to define a state similar to an SFC (Sequential function chart) state.
    /// This enumeration is useful when implementing parallelism, i.e. when different states are active at the same time which can be achived by using the bitwise operator &.
    /// </summary>
    public enum State
    {
        State0 = 0x0,
        State1 = 0x1,
        State2 = 0x2,
        State3 = 0x4,
        State4 = 0x8,
        State5 = 0x10,
        State6 = 0x20,
        State7 = 0x40,
        State8 = 0x80,
        State9 = 0x100,
        State10 = 0x200,
        State11 = 0x400,
        State12 = 0x800,
        State13 = 0x1000,
        State14 = 0x2000,
        State15 = 0x4000,
        State16 = 0x8000,
        State17 = 0x10000,
        State18 = 0x20000,
        State19 = 0x40000,
        State20 = 0x80000,
        State21 = 0x100000,
        State22 = 0x200000,
        State23 = 0x400000,
        State24 = 0x800000,
        State25 = 0x1000000,
        State26 = 0x2000000,
        State27 = 0x4000000,
        State28 = 0x8000000,
        State29 = 0x10000000,
        State30 = 0x20000000,
        State31 = 0x40000000
    }
}
