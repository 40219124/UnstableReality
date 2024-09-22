using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StageRotate : Stage
{
    SpriteRenderer PlayerSprite;

    [SerializeField]
    Transform Voidling;
    public override void InitialSetUp()
    {
        base.InitialSetUp();
        Manager.UseLimittedPalette(true);
        PlayerSprite = Player.GetComponentInChildren<SpriteRenderer>();
        Voidling.gameObject.SetActive(false);
    }

    public override void FinishSetUp()
    {
        base.FinishSetUp();
        Manager.UseLimittedPalette(false);
    }

    public override void TearDown()
    {
        base.TearDown();
        PlayerSprite.transform.rotation = Quaternion.identity;
    }
    protected override eDirections ModifyFacingDirection(eDirections facing)
    {
        float zRot = 0f;
        switch (facing)
        {
            case eDirections.right:
                zRot = 90f;
                break;
            case eDirections.up:
                zRot = 180f;
                break;
            case eDirections.left:
                zRot = 270f;
                break;
        }
        Quaternion newRot = Quaternion.Euler(0, 0, zRot);
        PlayerSprite.transform.rotation = newRot;
        return eDirections.down;
    }

    protected override void OnEndEnter(Collider2D collision)
    {
        base.OnEndEnter(collision);
        Player.FreezeAfterMove = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Voidling.gameObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Voidling.gameObject.SetActive(false);
    }
}
