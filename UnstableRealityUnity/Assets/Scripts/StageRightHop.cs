using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageRightHop : Stage
{
    bool jumpTime = false;
    float lastX;
    public override void InitialSetUp()
    {
        base.InitialSetUp();
        Manager.UseLimittedPalette(true);
    }
    protected override void StageUpdate()
    {
        if (jumpTime)
        {
            float currX = Player.transform.position.x;
            if (lastX % 1 < 0.5f && currX % 1 > 0.5f)
            {
                Player.transform.position += Vector3.right;
                currX = Player.transform.position.x;
            }
            lastX = currX;
        }

        base.StageUpdate();
    }
    protected override Vector2 ModifyDesiredDirection(Vector2 dir)
    {
        if (dir.x > 0)
        {
            dir.x *= 2f;
            jumpTime = true;
            lastX = Player.transform.position.x;
        }
        else
        {
            jumpTime = false;
        }
        return dir;
    }
}
