using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    Transform FollowTarget;

    float OwnZ;
    // Start is called before the first frame update
    void Start()
    {
        OwnZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = FollowTarget.position;
        newPos.z = OwnZ;
        transform.position = newPos;
    }
}
