using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class XonixMainModel {

    private XonixFieldModel fieldModel;
    private XonixEnemysManager enemysManager;
    private XonixPlayer player;
    private XonixLifeCycleModel lifeModel;

    private float updateTimeDelay = 0.1f;

    public delegate void OnRedraw(XonixFieldElement[,] fieldArray, int fieldSizeX, int fieldSizeY, List<XonixEnemy> enemys, int playerX, int playerY);
    public static event OnRedraw OnRedrawEvent = delegate { };

    public delegate void OnUpdateComlete();
    public static event OnUpdateComlete OnUpdateComleteEvent = delegate { };

    public delegate void OnUnloadScene();
    public static event OnUnloadScene OnUnloadSceneEvent = delegate { };

    private int initialEnemysCount = 1;

    private int currentEnemysCount;

    public void Init()
    {
        initialEnemysCount = XonixInitialMenuModel.initialEnemysCount;
        currentEnemysCount = initialEnemysCount;
        fieldModel = new XonixFieldModel();
        enemysManager = new XonixEnemysManager();
        lifeModel = new XonixLifeCycleModel();
        fieldModel.init(enemysManager);
        enemysManager.init(fieldModel);
        enemysManager.initEnemys(initialEnemysCount, initialEnemysCount);
        player = new XonixPlayer();
        player.init(fieldModel);
        player.resetPlayer();
        XonixTimeModel.OnUpdateEvent += onUpdate;
        XonixTimeModel.OnInitCompletedEvent+=onInitComplete;
        XonixLifeCycleModel.OnGameOverEvent += onGameOver;
        XonixLifeCycleModel.OnLifeLostEvent += onLifeLost;
        XonixLifeCycleModel.OnNextLevelEvent += onNextLevel;
        XonixMainModel.OnUnloadSceneEvent += onSceneUnloaded;
    }

    private void onSceneUnloaded()
    {
        XonixTimeModel.OnUpdateEvent -= onUpdate;
        XonixTimeModel.OnInitCompletedEvent -= onInitComplete;
        XonixLifeCycleModel.OnGameOverEvent -= onGameOver;
        XonixLifeCycleModel.OnLifeLostEvent -= onLifeLost;
        XonixLifeCycleModel.OnNextLevelEvent -= onNextLevel;
        OnUnloadSceneEvent -= onSceneUnloaded;
    }

    private void onInitComplete()
    {
        redraw();
    }

    private void onUpdate()
    {
        enemysManager.updateEnemysMovement();
        player.updatePlayerMovement();
        enemysManager.updateDamage(player.currentPositionX,player.currentPositionY);
        OnUpdateComleteEvent();
    }

    private void onLifeLost()
    {
        player.resetPlayer();
        fieldModel.clearPath();
        enemysManager.resetWaterEnemys(initialEnemysCount);
    }

    private void onGameOver()
    {
        currentEnemysCount = initialEnemysCount;
        restartField();
    }

    private void redraw()
    {
        if (OnRedrawEvent != null)
        {
            OnRedrawEvent(fieldModel.fieldArray, fieldModel.fieldWidth, fieldModel.fieldHeight, enemysManager.enemys, player.currentPositionX, player.currentPositionY);
        }
        
    }

    public void notifyPlayerDirection(EPlayerMovementInputType direction)
    {
        player.notifyDirection(direction);
    }

    private void onNextLevel()
    {
        currentEnemysCount++;
        restartField();
    }

    private void restartField()
    {
        fieldModel.resetField();
        enemysManager.initEnemys(currentEnemysCount, initialEnemysCount);
        player.resetPlayer();
        redraw();
    }

    public void exitToMainMenu()
    {
        OnUnloadSceneEvent();
        SceneManager.LoadScene("XonixScene0",LoadSceneMode.Additive);
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
