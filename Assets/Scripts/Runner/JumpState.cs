// PHOEBE FAITH (1033478)
// JUMPSTATE.CS

using System;
using UnityEngine;

public class JumpState : PlayerState
{
    public override void Enter(EventArgs args)
    {
        BaseStateEventArgs argsCast = (BaseStateEventArgs)args;

        // Logger.Log(this, "JumpState entered.");
        argsCast.player.rb2d.AddForce(Vector2.up * argsCast.player.jumpForce, ForceMode2D.Impulse);
        argsCast.player.animator.SetBool("isRunning", false);
    }

    public override void Exit(EventArgs args)
    {
        // Logger.Log(this, "JumpState exited.");
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
