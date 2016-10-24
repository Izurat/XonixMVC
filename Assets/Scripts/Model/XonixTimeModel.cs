using System;
using System.Collections;
using UnityEngine;

public class XonixTimeModel : MonoBehaviour
{

    public bool isPaused { get; set; }
    private float updateDelay = 10f;
    private float timePassedFromLastUpdate = 0f;

    private int timeForLevel = 6000;
    private float updateLevelTimeDelay = 1f;
    private int currentTimeForLevel;
    private float timePassedFromLastLevelTimeUpdate = 0f;

    public delegate void OnUpdate();

    public static event OnUpdate OnUpdateEvent;

    public delegate void OnLevelTimeFinished();

    public static event OnLevelTimeFinished OnLevelTimeFinishedEvent = delegate { };

    public delegate void OnLevelTime(int currentLevelTime);

    public static event OnLevelTime OnLevelTimeEvent = delegate { };

    public delegate void OnInitCompleted();

    public static event OnInitCompleted OnInitCompletedEvent = delegate { };

    private const float waitOnInit = 0.1f;


    public XonixTimeModel()
    {
        updateDelay = XonixInitialMenuModel.stepTime;
        timeForLevel = XonixInitialMenuModel.levelTime;
        isPaused = true;
        XonixLifeCycleModel.OnLifeLostEvent += resetLevelTime;
        XonixLifeCycleModel.OnNextLevelEvent += resetLevelTime;
        XonixLifeCycleModel.OnGameOverEvent += resetLevelTime;
        XonixMainModel.OnUnloadSceneEvent += onSceneUnloaded; 
    }

    private void onSceneUnloaded()
    {
        XonixLifeCycleModel.OnLifeLostEvent -= resetLevelTime;
        XonixLifeCycleModel.OnNextLevelEvent -= resetLevelTime;
        XonixLifeCycleModel.OnGameOverEvent -= resetLevelTime;
        XonixMainModel.OnUnloadSceneEvent -= onSceneUnloaded;
    }


    private void resetLevelTime()
    {
        currentTimeForLevel = 0;
        timePassedFromLastLevelTimeUpdate = 0;
    }



    public void init()
    {
        timePassedFromLastUpdate = 0f;
        currentTimeForLevel = 0;
        timePassedFromLastLevelTimeUpdate = 0;
        isPaused = false;
        StartCoroutine(onInitComplete());
    }

    private IEnumerator onInitComplete()
    {
        yield return new WaitForSeconds(waitOnInit);
        isPaused = false;
        OnInitCompletedEvent();
    }

    void Update()
    {
        if (isPaused)
        {
            return;
        }
        timePassedFromLastUpdate += Time.deltaTime;
        if (timePassedFromLastUpdate >= updateDelay )
        {
            timePassedFromLastUpdate = 0;
            if (OnUpdateEvent != null)
            {
                OnUpdateEvent();
            }
           
        }
        timePassedFromLastLevelTimeUpdate += Time.deltaTime;
        if (timePassedFromLastLevelTimeUpdate >= updateLevelTimeDelay)
        {
            timePassedFromLastLevelTimeUpdate = 0;
            currentTimeForLevel++;
            if (currentTimeForLevel >= timeForLevel)
            {
                currentTimeForLevel = 0;
                if (OnLevelTimeFinishedEvent != null)
                {
                    OnLevelTimeFinishedEvent();
                }
                
            }
            if (OnLevelTimeEvent != null)
            {
                OnLevelTimeEvent(currentTimeForLevel);
            }
            
        }
    }

    public void onTogglePause(Boolean pauseState)
    {
        if (isPaused == pauseState)
        {
            return;
        }
        isPaused = !isPaused;
    }
}
