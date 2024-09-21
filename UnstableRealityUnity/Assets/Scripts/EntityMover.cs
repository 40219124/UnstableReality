using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Linq;

public static class ExtMets
{
    public static Vector3 V2to3(this Vector2 vec, float ZValue)
    {
        return new Vector3(vec.x, vec.y, ZValue);
    }

    public static Vector2 V3to2(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.y);
    }

    public static Vector2Int V2toI2(this Vector2 vec)
    {
        return new Vector2Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));
    }

    public static Vector2 V2ItoF(this Vector2Int vec)
    {
        return new Vector2(vec.x, vec.y);
    }

    public static Vector3 V2Ito3(this Vector2Int vec, float ZValue)
    {
        return vec.V2ItoF().V2to3(ZValue);
    }

    public static Vector2Int V3toI2(this Vector3 vec)
    {
        return vec.V3to2().V2toI2();
    }

    public static Vector3Int V3toI3(this Vector3 vec)
    {
        return new Vector3Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));
    }

    public static eDirections Opposite(this eDirections dir)
    {
        switch (dir)
        {
            case eDirections.up:
                return eDirections.down;
            case eDirections.right:
                return eDirections.left;
            case eDirections.down:
                return eDirections.up;
            case eDirections.left:
                return eDirections.right;
            default:
                return eDirections.none;
        }
    }
}

public enum eDirections
{
    none = -1, up, right, down, left
}


public class EntityMover : MonoBehaviour
{
    public enum eSetMoveResult
    {
        tooSoon = -1,
        success = 0,
        blocked = 1
    }

    public Vector2Int? Destination { get; private set; }
    public eDirections TravelDir { get; private set; }
    public eDirections FacingDir { get; private set; }
    public Func<eDirections, eDirections> ModifyFacingDirection;
    [Range(0, 5)]
    public float MoveSpeed = 1f;
    public float SprintMod = 2f;
    public bool IsSprinting = false;
    public float ExternalSpeedMod = 1f;
    protected float TimeSinceDestSet = -1f;

    public bool FreezeAfterMove = false;
    public bool IsFrozen = false;

    protected float ZValue;

    public Func<Vector2, Vector2> ModifyDesiredDirection;
    protected Vector2? AttemptedDesDir = null;
    protected float TimeAttemptedSet = -1f;

    [SerializeField]
    protected LayerMask MoveBlocker;

    [SerializeField]
    protected Tilemap Tilemap;


    protected void OnEnable()
    {
        StageManager.Instance.ChangingStage += ClearAll;
    }
    protected void OnDisable()
    {
        StageManager.Instance.ChangingStage -= ClearAll;
    }

    protected void ClearAll()
    {
        Destination = null;
        TravelDir = eDirections.none;
        FacingDir = eDirections.down;

        ModifyDesiredDirection = null;
        ModifyFacingDirection = null;

        IsSprinting = false;
        ExternalSpeedMod = 1f;

        TimeSinceDestSet = -1f;

        AttemptedDesDir = null;
        TimeAttemptedSet = -1f;
    }


    public eSetMoveResult SetDesiredDirection(Vector2 dir)
    {
        eSetMoveResult outcome = eSetMoveResult.tooSoon;

        if (Destination != null)
        {
            AttemptedDesDir = dir;
            TimeAttemptedSet = Time.time;
            return outcome;
        }

        if (ModifyDesiredDirection != null)
        {
            foreach (Func<Vector2, Vector2> f in (ModifyDesiredDirection?.GetInvocationList()).Cast<Func<Vector2, Vector2>>())
            {
                dir = f(dir);
            }
        }
        Vector3 desPos = (transform.position + dir.V2to3(0f));

        Vector3Int tilePos = Tilemap.WorldToCell(desPos);
        TileBase tile = Tilemap.GetTile(tilePos);


        if (tile != null)
        {
            outcome = eSetMoveResult.blocked;
        }

        /* RaycastHit2D hit = Physics2D.Raycast(desPos, Vector2.zero);//, 100f, MoveBlocker.value);
        if (hit.collider != null && !hit.collider.CompareTag("MoveBlock"))
        {
            outcome = eSetMoveResult.blocked;
        }*/

        if (outcome != eSetMoveResult.blocked)
        {
            Destination = desPos.V3toI2();
            TimeSinceDestSet = 0f;
        }

        // dertermine enum
        eDirections newDir = FacingDir;
        if (dir.x != 0f)
        {
            if (dir.x > 0f)
            {
                newDir = eDirections.right;
            }
            else
            {
                newDir = eDirections.left;
            }
        }
        else
        {
            if (dir.y > 0f)
            {
                newDir = eDirections.up;
            }
            else
            {
                newDir = eDirections.down;
            }
        }
        // set move enum if moving
        if (outcome != eSetMoveResult.blocked)
        {
            TravelDir = newDir;
        }
        // modify enum for facing different to moving
        if (ModifyFacingDirection != null)
        {
            foreach (Func<eDirections, eDirections> f in ModifyFacingDirection.GetInvocationList().Cast<Func<eDirections, eDirections>>())
            {
                newDir = f(newDir);
            }
        }
        FacingDir = newDir;



        if (outcome != eSetMoveResult.blocked)
        {
            outcome = eSetMoveResult.success;
        }
        return outcome;
    }

    // Start is called before the first frame update
    void Start()
    {
        ZValue = transform.position.z;
        FacingDir = eDirections.down;
    }

    // Update is called once per frame
    void Update()
    {
        MoveUpdate();
    }


    void MoveUpdate()
    {
        TimeSinceDestSet += Time.deltaTime;

        if (Destination == null || IsFrozen)
        {
            return;
        }
        Vector3 vecToDest = Destination.Value.V2Ito3(0f) - transform.position;
        vecToDest.z = 0;
        Vector3 moveVec = vecToDest.normalized * MoveSpeed * Time.deltaTime * (IsSprinting ? SprintMod : 1f) * ExternalSpeedMod;
        if (moveVec.sqrMagnitude > vecToDest.sqrMagnitude)
        {
            moveVec = vecToDest;
        }
        transform.position += moveVec;

        if (CloseEnough())
        {
            transform.position = Destination.Value.V2Ito3(ZValue);
            Destination = null;
            TravelDir = eDirections.none;
            if (FreezeAfterMove)
            {
                IsFrozen = true;
                FreezeAfterMove = false;
            }
            else if (AttemptedDesDir != null)
            {
                if (Time.time == TimeAttemptedSet)
                {
                    SetDesiredDirection(AttemptedDesDir.Value);
                }
                AttemptedDesDir = null;
            }
        }
    }

    protected bool CloseEnough()
    {
        if (Destination == null) { return false; }

        return (Destination.Value.V2Ito3(transform.position.z) - transform.position).sqrMagnitude < 0.001f;
    }
}
