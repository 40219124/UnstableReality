using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum eMusic
{
    none = -1,
    BasicDrone,
    Movement,
    LowMelody,
    HighMelody
}
public class Stage : MonoBehaviour
{
    [SerializeField]
    Stage NextStage;
    [SerializeField]
    StageEndCollider EndZone;

    protected static StageManager Manager;
    protected EntityMover Player;

    public int StageId;
    protected bool IsCurrentStage = false;

    [SerializeField]
    protected float ExitDelay = 1f;
    protected float ExitRemaining = -1f;
    protected bool PlayerOnExit = false;

    [SerializeField]
    protected eMusic OpeningMusic = eMusic.BasicDrone;
    [SerializeField]
    protected Texture OpeningTransImage;
    public Texture GetOpeningTransImage() { return OpeningTransImage; }
    [SerializeField]
    protected Texture ClosingTransImage;
    public Texture GetClosingTransImage() { return ClosingTransImage; }

    protected virtual void Awake()
    {
        ExitRemaining = ExitDelay;
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (Manager == null)
        {
            Manager = StageManager.Instance;
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!IsCurrentStage)
        {
            return;
        }
        StageUpdate();
    }

    protected virtual void StageUpdate()
    {
        ExitingUpdate();
    }

    protected virtual void ExitingUpdate()
    {
        if (PlayerOnExit)
        {
            ExitRemaining -= Time.deltaTime;
        }
        else if (ExitRemaining < ExitDelay && ExitRemaining != 0f)
        {
            ExitRemaining += Time.deltaTime;
        }
        ExitRemaining = Mathf.Clamp(ExitRemaining, 0f, ExitDelay);
        Manager.SetTransProgress(1f - (ExitRemaining / ExitDelay));

        if (ExitRemaining == 0f)
        {
            if (!Player.IsFrozen)
            {
                Player.FreezeAfterMove = true;
            }
            else if (Player.IsFrozen)
            {
                if (PlayerOnExit)
                {
                    EndRoutine();
                }
                else
                {
                    Player.IsFrozen = false;
                    ExitRemaining += Time.deltaTime;
                }
            }
        }
    }

    public virtual void SetCurrentStage(bool isCurrent)
    {
        IsCurrentStage = isCurrent;
        if (isCurrent)
        {
            OnEnable();
            Manager.ChangeMusic(OpeningMusic);
        }
        else
        {
            OnDisable();
            TearDown();
        }
    }

    public virtual void FinishSetUp()
    {
        Player.IsFrozen = false;
        Manager.SetTransImage(ClosingTransImage);
    }

    public virtual void TearDown()
    {
        ExitRemaining = ExitDelay;
    }

    protected virtual Vector2 ModifyDesiredDirection(Vector2 dir)
    {
        return dir;
    }

    protected virtual eDirections ModifyFacingDirection(eDirections facing)
    {
        return facing;
    }

    protected void OnEnable()
    {
        if (!IsCurrentStage)
        {
            return;
        }
        EndZone.OnZoneEnter += OnEndEnter;
        EndZone.OnZoneExit += OnEndExit;

        Player = Manager.GetPlayerMover();
        Player.ModifyDesiredDirection += ModifyDesiredDirection;
        Player.ModifyFacingDirection += ModifyFacingDirection;
    }

    protected void OnDisable()
    {
        EndZone.OnZoneEnter -= OnEndEnter;
        EndZone.OnZoneExit -= OnEndExit;

        Player.ModifyDesiredDirection -= ModifyDesiredDirection;
        Player.ModifyFacingDirection -= ModifyFacingDirection;
        Player = null;
    }

    protected virtual void EndRoutine()
    {
        StartCoroutine(StageManager.Instance.LoadStage(NextStage.StageId));
        PlayerOnExit = false;
    }

    protected void OnEndEnter(Collider2D collision)
    {
        PlayerOnExit = true;
    }

    protected void OnEndExit(Collider2D collision)
    {
        PlayerOnExit = false;
    }
}
