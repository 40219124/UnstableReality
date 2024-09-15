using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityMover))]
[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    EntityMover Mover;
    Animator Anim;
    // Start is called before the first frame update
    void Start()
    {
        Mover = GetComponent<EntityMover>();
        Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Anim.SetInteger("Facing", (int)Mover.FacingDir);
        Anim.SetBool("IsMoving", Mover.Destination != null);
    }
}
