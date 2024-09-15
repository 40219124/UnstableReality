using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityMover))]
public class PlayerController : MonoBehaviour
{
    private EntityMover Mover;

    // Start is called before the first frame update
    void Start()
    {
        Mover = GetComponent<EntityMover>();
    }

    // Update is called once per frame
    void Update()
    {
        InputUpdate();
    }

    void InputUpdate()
    {
        Vector2 moveDir = Input.GetAxisRaw("Horizontal") * Vector2.right + Input.GetAxisRaw("Vertical") * Vector2.up;
        
        if (Mathf.Abs(moveDir.y) >= Mathf.Abs(moveDir.x))
        {
            moveDir.x = 0;
        }
        else
        {
            moveDir.y = 0;
        }

        if(moveDir != Vector2.zero)
        {
            Mover.SetDesiredDirection(moveDir);
        }

    }
}
