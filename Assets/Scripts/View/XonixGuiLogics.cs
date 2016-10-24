using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class XonixGuiLogics : MonoBehaviour
{
    public GameObject levelMenu;
    public GameObject infoMenu;

    public Text infoMenuText;

    public delegate void OnPauseStateChanged(bool isPaused);
    public static event OnPauseStateChanged OnPauseStateChangedEvent = delegate { };

    public delegate void OnExitToMainMenu();
    public static event OnExitToMainMenu OnExitToMainMenuEvent = delegate { };

    public delegate void OnExitGame();
    public static event OnExitGame OnExitGameEvent = delegate { };

    private void Start()
    {
        XonixLifeCycleModel.OnGameOverEvent += onGameOver;
        XonixLifeCycleModel.OnLifeLostEvent += onLifeLost;
        XonixLifeCycleModel.OnNextLevelEvent += onLevelPassed;
        InputManager.OnTogglePauseEvent += onTogglePause;
        XonixMainModel.OnUnloadSceneEvent += onSceneUnloaded; 

        levelMenu.SetActive(false);
        infoMenu.SetActive(false);
    }

    private void onSceneUnloaded()
    {
        XonixLifeCycleModel.OnGameOverEvent -= onGameOver;
        XonixLifeCycleModel.OnLifeLostEvent -= onLifeLost;
        XonixLifeCycleModel.OnNextLevelEvent -= onLevelPassed;
        InputManager.OnTogglePauseEvent -= onTogglePause;
        XonixMainModel.OnUnloadSceneEvent -= onSceneUnloaded;
    }

    public void onExitToMainMenu()
    {
        OnExitToMainMenuEvent();
    }
    public void onExitGame()
    {
        OnExitGameEvent();
    }

    private void updateIsPause()
    {
        bool isPaused = false;
        if (isAnyMenuActive())
        {
            isPaused = true;
        }
        OnPauseStateChangedEvent(isPaused);
    }

    public void closAllMenues()
    {
        levelMenu.SetActive(false);
        infoMenu.SetActive(false);
        updateIsPause();
    }

    public void exitToMainMenu()
    {
        OnExitToMainMenuEvent();
    }

    private void onTogglePause()
    {
        if (isAnyMenuActive())
        {
            return;
        }

        levelMenu.SetActive(true);
        updateIsPause();
    }

    private Boolean isAnyMenuActive()
    {
        return levelMenu.activeSelf || infoMenu.activeSelf;
    }

    private void onLevelPassed()
    {
        infoMenuText.text = "Level passed";
        openInfoMenu();
    }

    private void onLifeLost()
    {
        infoMenuText.text = "Life lost";
        openInfoMenu();
    }

    private void onGameOver()
    {
        infoMenuText.text = "All lifes lost. Restart?";
        openInfoMenu();
    }

    private void openInfoMenu()
    {
        infoMenu.SetActive(true);
    }

    private void closeInfoMenu()
    {
        infoMenu.SetActive(false);
    }
}
