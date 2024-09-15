using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

    public static Vector2Int V2toI(this Vector2 vec)
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

    public static Vector2Int V3toI(this Vector3 vec)
    {
        return vec.V3to2().V2toI();
    }
}

public class EntityMover : MonoBehaviour
{

    public Vector2Int? Destination { get; private set; }
    [Range(0, 5)]
    public float MoveSpeed = 1f;
    protected float TimeSinceDestSet = -1f;

    protected float ZValue;

    public void SetDesiredDirection(Vector2 dir)
    {
        if (Destination != null)
        {
            return;
        }
        Destination = (transform.position + dir.V2to3(ZValue)).V3toI();
        TimeSinceDestSet = 0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        ZValue = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        MoveUpdate();
    }

    void MoveUpdate()
    {
        TimeSinceDestSet += Time.deltaTime;

        if (Destination == null)
        {
            return;
        }
        Vector3 vecToDest = Destination.Value.V2Ito3(ZValue) - transform.position;
        Vector3 moveVec = vecToDest.normalized * MoveSpeed * Time.deltaTime;
        if (moveVec.sqrMagnitude > vecToDest.sqrMagnitude)
        {
            moveVec = vecToDest;
        }
        transform.position += moveVec;

        if (CloseEnough())
        {
            transform.position = Destination.Value.V2Ito3(ZValue);
            Destination = null;
        }
    }

    protected bool CloseEnough()
    {
        if (Destination == null) { return false; }

        return (Destination.Value.V2Ito3(ZValue) - transform.position).sqrMagnitude < 0.001f;
    }
}
