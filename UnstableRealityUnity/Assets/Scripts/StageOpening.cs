using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageOpening : Stage
{
    readonly int xLimit = 33;
    int xReached = -1;


    protected override void Start()
    {
        base.Start();
    }

    public override void InitialSetUp()
    {
        base.InitialSetUp();
        MakePlayerSilhouette(true);
        Manager.UseLimittedPalette(true);
    }

    public override void TearDown()
    {
        base.TearDown();
        xReached = -1;
    }

    protected override void StageUpdate()
    {
        xReached = Mathf.FloorToInt(Player.transform.position.x);
        if(PlayerSilhouetted && Player.transform.position.x > ((float)xLimit - 0.5f))
        {
            MakePlayerSilhouette(false);
        }

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
