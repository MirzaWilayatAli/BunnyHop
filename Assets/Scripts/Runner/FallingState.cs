// PHOEBE FAITH (1033478)
// FALLINGSTATE.CS

using System;
using UnityEngine;

public class FallingState : PlayerState
{
    public override void Enter(EventArgs args)
    {
        // Logger.Log(this, "FallingState entered.");
    }

    public override void Exit(EventArgs args)
    {
        // Logger.Log(this, "FallingState exited.");
        BaseStateEventArgs argsCast = (BaseStateEventArgs)args;
        argsCast.player.animator.SetBool("isRunning", true);
    }

    public override void Execute(EventArgs args)
    {
        BaseStateEventArgs argsCast = (BaseStateEventArgs)args;

        if (Input.GetMouseButtonDown(0) && argsCast.player.IsGrounded() == false && argsCast.player.hasSlammed == false)
        {
            argsCast.player.rb2d.AddForce(Vector2.down * argsCast.player.slamForce, ForceMode2D.Impulse);
            argsCast.player.hasSlammed = true;
        }
    }
}
