using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StageEndCollider : MonoBehaviour
{

    public Action<Collider2D> OnZoneEnter;
    public Action<Collider2D> OnZoneExit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnZoneEnter?.Invoke(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnZoneExit?.Invoke(collision);
    }
}
