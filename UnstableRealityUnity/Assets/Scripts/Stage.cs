using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Stage : MonoBehaviour
{
    [SerializeField]
    Stage NextStage;
    [SerializeField]
    StageEndCollider EndZone;

    public int StageId;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        EndZone.OnZoneEnter += OnEndEnter;
        EndZone.OnZoneExit += OnEndExit;
    }

    private void OnDisable()
    {
        EndZone.OnZoneEnter -= OnEndEnter;
        EndZone.OnZoneExit -= OnEndExit;
    }

    private void OnEndEnter(Collider2D collision)
    {
        StartCoroutine(StageManager.Instance.LoadStage(NextStage.StageId));
    }

    private void OnEndExit(Collider2D collision)
    {

    }
}
