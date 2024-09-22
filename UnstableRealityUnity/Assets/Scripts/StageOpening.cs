using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageOpening : Stage
{
    readonly int xLimit = 33;
    int xReached = -1;

    public override void TearDown()
    {
        base.TearDown();
        xReached = -1;
    }

    protected override void StageUpdate()
    {
        xReached = Mathf.FloorToInt(Player.transform.position.x);

        base.StageUpdate();
    }

    protected override Vector2 ModifyDesiredDirection(Vector2 dir)
    {
        if (xReached <= xLimit && dir.x < 0)
        {
            dir.x = 0;
        }
        return dir;
    }
}
