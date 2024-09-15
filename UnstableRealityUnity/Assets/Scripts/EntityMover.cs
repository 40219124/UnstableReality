using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class ExtMets
{
    public static Vector3 V2to3(this Vector2 vec)
    {
        return new Vector3(vec.x, 0f, vec.y);
    }

    public static Vector2 V3to2(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }

    public static Vector2Int V2toI(this Vector2 vec)
    {
        return new Vector2Int((int)vec.x, (int)vec.y);
    }

    public static Vector2 V2ItoF(this Vector2Int vec)
    {
        return new Vector2(vec.x, vec.y);
    }

    public static Vector3 V2Ito3(this Vector2Int vec)
    {
        return vec.V2ItoF().V2to3();
    }

    public static Vector2Int V3toI(this Vector3 vec)
    {
        return vec.V3to2().V2toI();
    }
}

public class EntityMover : MonoBehaviour
{

    public Vector2Int? Destination { get; private set; }
    [Range(0,5)]
    public float MoveSpeed = 1f;
    protected float TimeSinceDestSet = -1f;

    public void SetDesiredDirection(Vector2 dir)
    {
        Destination = (transform.position + dir.V2to3()).V3toI();
        TimeSinceDestSet = 0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveUpdate();
    }

    void MoveUpdate()
    {
        TimeSinceDestSet += Time.deltaTime;

        if(Destination == null)
        {
            return;
        }
        Vector3 moveDir = Destination.Value.V2Ito3() - transform.position;

        transform.position += moveDir.normalized * MoveSpeed * Time.deltaTime;

        if (transform.position == Destination.Value.V2Ito3())
        {
            Destination = null;
        }
    }
}
