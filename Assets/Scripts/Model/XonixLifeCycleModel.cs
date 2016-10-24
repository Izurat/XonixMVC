using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class XonixLifeCycleModel
{
    public int scoresValue { get; set; }

    public const int initialLivesCount = 5;
    private float groundLeftForLevelFinishing = 0.25f;


    public int currentLivesCount { get; private set; }

    public delegate void OnLifeLost();
    public static event OnLifeLost OnLifeLostEvent = delegate { };

    public delegate void OnNextLevel();
    public static event OnNextLevel OnNextLevelEvent = delegate { };

    public delegate void OnGameOver();
    public static event OnGameOver OnGameOverEvent = delegate { };

    public delegate void OnInfoUpdated(int currentLifesCount, int initialLifesCount, int scoresValue, float levelPartLeft);
    public static event OnInfoUpdated OnInfoUpdatedEvent = delegate { };


    public XonixLifeCycleModel()
    {
        groundLeftForLevelFinishing = XonixInitialMenuModel.levelPartToRemove;
        XonixEnemysManager.OnDamageEvent+=notifyDamage;
        XonixFieldModel.OnIslandRemovedEvent += notifyIslandRemoved;
        XonixMainModel.OnUnloadSceneEvent += onSceneUnloaded; ;
        reset();
    }

    private void onSceneUnloaded()
    {
        XonixEnemysManager.OnDamageEvent -= notifyDamage;
        XonixFieldModel.OnIslandRemovedEvent -= notifyIslandRemoved;
        XonixMainModel.OnUnloadSceneEvent -= onSceneUnloaded;
    }

    public void reset()
    {
        currentLivesCount = initialLivesCount;
        scoresValue = 0;
        onInfoUpdatedd();
    }

    public void onScoresAdded(int scores)
    {
        scoresValue += scores*XonixInitialMenuModel.scoresMultiplyer;
        onInfoUpdatedd();
    }

    private void notifyDamage()
    {
        currentLivesCount--;
        if (currentLivesCount <= 0)
        {
            OnGameOverEvent();
            return;
        }
        OnLifeLostEvent();
        onInfoUpdatedd();
    }

    private float levelPartPassed= 0;
    public void notifyIslandRemoved(int scores,float groundPart)
    {
        levelPartPassed = 1f - (float)(groundPart - groundLeftForLevelFinishing) / (float)(1f - groundLeftForLevelFinishing);
        if (levelPartPassed < 0)
        {
            levelPartPassed = 0;
        }
        if (groundPart < groundLeftForLevelFinishing)
        {
            OnNextLevelEvent();
        }
        onInfoUpdatedd();
    }

    private void onInfoUpdatedd()
    {
        OnInfoUpdatedEvent(currentLivesCount, initialLivesCount, scoresValue, 1 - levelPartPassed);
    }
}
