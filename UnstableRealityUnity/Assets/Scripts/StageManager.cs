using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StageManager : MonoBehaviour
{
    private static StageManager _instance;
    public static StageManager Instance { get { return _instance; } }
    private void Awake()
    {
        _instance = this;
    }


    public bool PlayerIsControllable = false;
    public Action ChangingStage;
    public bool IsChangingStage { get; private set; }
    public int ActiveStage { get; private set; }

    [SerializeField]
    private Transform Player;
    private EntityMover PlayerMover;
    [SerializeField]
    List<Stage> Stages = new List<Stage>();
    // Start is called before the first frame update
    void Start()
    {
        ActiveStage = -1;
        for (int i = 0; i < Stages.Count; i++)
        {
            Stages[i].StageId = i;
        }

        StartCoroutine(PrepareTheatre());
    }

    IEnumerator PrepareTheatre()
    {
        PlayerIsControllable = false;
        Player.gameObject.SetActive(true);
        yield return null;
        yield return null;
        PlayerMover = Player.GetComponent<EntityMover>();
        StartCoroutine(ChangeStage(0));
    }

    public EntityMover GetPlayerMover()
    {
        return PlayerMover;
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ChangeStage(int stage)
    {
        if (stage > Stages.Count || stage < 0)
        {
            yield break;
        }

        IsChangingStage = true;
        PlayerIsControllable = false;
        ChangingStage?.Invoke();
        if (ActiveStage != -1)
        {
            Stages[ActiveStage].SetCurrentStage(false);
        }
        yield return null;

        ActiveStage = -1;
        Player.position = Stages[stage].transform.position;
        Stages[stage].SetCurrentStage(true);
        yield return null;

        ActiveStage = stage;
        IsChangingStage = false;
        PlayerIsControllable = true;
        Stages[ActiveStage].FinishSetUp();
    }

    public IEnumerator LoadStage(int stage)
    {
        yield return StartCoroutine(ChangeStage(stage));
    }
}
