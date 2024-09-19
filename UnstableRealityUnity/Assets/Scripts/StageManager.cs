using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StageManager : MonoBehaviour
{
    private static StageManager _instance;
    public static StageManager Instance {  get { return _instance; } }
    private void Awake()
    {
        _instance = this;
    }


    public bool PlayerIsControllable = false;
    public Action ChangingStage;

    [SerializeField]
    private Transform Player;
    [SerializeField]
    List<Stage> Stages = new List<Stage>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Stages.Count; i++)
        {
            Stages[i].StageId = i;
        }

        StartCoroutine(Opening(0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Opening(int stage)
    {
        PlayerIsControllable = false;
        ChangingStage?.Invoke();

        yield return null;
        
        Player.position = Stages[stage].transform.position;
        yield return null;


        ChangingStage?.Invoke();
        PlayerIsControllable = true;
    }

    public IEnumerator LoadStage(int stage)
    {
        yield return StartCoroutine(Opening(stage));
    }
}
