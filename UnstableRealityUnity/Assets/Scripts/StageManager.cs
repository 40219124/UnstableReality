using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TrackByEnum
{
    public eMusic Key;
    public AudioClip Value;
}

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
    private Texture DefaultTransTex;
    [SerializeField]
    private Material RenderCanvas;
    private readonly string TransTexName = "_TransitionTexture";
    private readonly string TransProgName = "_TransProgress";

    [SerializeField]
    private Transform Player;
    private EntityMover PlayerMover;

    [SerializeField]
    AudioSource MusicPlayer;
    float TimeOfLastMusicChange;
    [SerializeField]
    public List<TrackByEnum> MusicTracks = new List<TrackByEnum>();
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
        RenderCanvas.SetTexture(TransTexName, DefaultTransTex);
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

    public void SetTransProgress(float value)
    {
        RenderCanvas.SetFloat(TransProgName, value);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeMusic(eMusic newMusic)
    {
        StartCoroutine(ChangeMusicCo(newMusic));
    }

    public AudioClip GetMusic(eMusic music)
    {
        return MusicTracks.Find(x => x.Key == music).Value;
    }

    IEnumerator ChangeMusicCo(eMusic newMusic)
    {
        float personalStart = TimeOfLastMusicChange = Time.deltaTime;
        yield return null;
        if (MusicPlayer.isPlaying)
        {
            float fadeTime = 0.5f;
            for (float t = fadeTime; t > 0f; t -= Time.deltaTime)
            {
                if(personalStart != TimeOfLastMusicChange)
                {
                    yield break;
                }
                MusicPlayer.volume = Mathf.Clamp01(t / fadeTime);
                yield return null;
            }
            MusicPlayer.Stop();
        }
        if(newMusic == eMusic.none || personalStart != TimeOfLastMusicChange)
        {
            yield break;
        }
        MusicPlayer.clip = GetMusic(newMusic);
        MusicPlayer.volume = 1f;
        yield return null;
        MusicPlayer.Play();
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

        for (float t = 1f; t > 0; t -= Time.deltaTime)
        {
            SetTransProgress(t);
            yield return null;
        }

        Stages[ActiveStage].FinishSetUp();
    }

    public IEnumerator LoadStage(int stage)
    {
        yield return StartCoroutine(ChangeStage(stage));
    }
}
