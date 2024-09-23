using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageVoidYou : Stage
{
    public override void InitialSetUp()
    {
        base.InitialSetUp();
        Manager.UseLimittedPalette(false);
        MakePlayerSilhouette(true);
    }

    public override void TearDown()
    {
        base.TearDown();
        Manager.UseLimittedPalette(true);
        MakePlayerSilhouette(false);
    }

    protected override void OnEndEnter(Collider2D collision)
    {
        base.OnEndEnter(collision);
        Player.FreezeAfterMove = true;
        StartCoroutine(ReturnToLimitted());
    }

    IEnumerator ReturnToLimitted()
    {
        while(ExitRemaining / ExitDelay > 0.6f)
        {
            yield return null;
        }
        Manager.UseLimittedPalette(true);
    }
}
