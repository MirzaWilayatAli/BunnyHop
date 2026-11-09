// PHOEBE FAITH (1033478)
// IDLESTATE.CS

using System;

public class IdleState : PlayerState
{
    public override void Enter(EventArgs args)
    {
        // Logger.Log(this, "IdleState entered.");
        BaseStateEventArgs argsCast = (BaseStateEventArgs)args;
        argsCast.player.hasSlammed = false;
    }

    public override void Exit(EventArgs args)
    {
        // Logger.Log(this, "IdleState exited.");
    }

    public override void Execute(EventArgs args)
    {
    }
}
