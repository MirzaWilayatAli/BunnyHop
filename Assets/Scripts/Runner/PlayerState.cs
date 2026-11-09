// PHOEBE FAITH (1033478)
// PLAYERSTATE.CS

using System;

public abstract class PlayerState
{
    public abstract void Enter(EventArgs player);
    public abstract void Execute(EventArgs player);
    public abstract void Exit(EventArgs player);
}
