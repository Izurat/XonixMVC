using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class XonixFieldModel
{
    public int fieldWidth = 128;
    public int fieldHeight = 72;
    public int initialBoarder = 2;
    public float updatePathsDelay = 0.1f;

    private List<XonixFieldElement> currentPath;
    private XonixMainModel mainModel;

    private int totalGroundCount;

    public XonixFieldElement[,] fieldArray { get; private set; }

    private XonixEnemysManager enemyManager;

    public delegate void OnIslandRemoved(int scores,float partCompleted);
    public static event OnIslandRemoved OnIslandRemovedEvent = delegate { };

    public delegate void OnFieldElementUpdated(XonixFieldElement fieldElement);
    public static event OnFieldElementUpdated OnFieldElementUpdatedEvent = delegate { };

    public delegate void OnEnemyPositionUpdated(XonixEnemy enemy, XonixFieldElement leavedFieldElement);
    public static event OnEnemyPositionUpdated OnEnemyPositionUpdatedEvent = delegate { };

    public delegate void OnPlayerPositionUpdated(int playerX, int playerY, XonixFieldElement leavedFieldElement);
    public static event OnPlayerPositionUpdated OnPlayerPositionUpdatedEvent = delegate { };


    public XonixFieldModel()
    {
        XonixEnemysManager.OnEnemyPositionUpdateEvent += onEnemyPositionUpdated;
        XonixEnemysManager.OnEnemyRemovedEvent += onEnemyRemoved;
        XonixPlayer.OnPlayerPositionUpdateEvent += onPlayerPositionUpdated;
        XonixMainModel.OnUnloadSceneEvent += onSceneUnloaded;
    }

    private void onSceneUnloaded()
    {
        XonixEnemysManager.OnEnemyPositionUpdateEvent -= onEnemyPositionUpdated;
        XonixEnemysManager.OnEnemyRemovedEvent -= onEnemyRemoved;
        XonixPlayer.OnPlayerPositionUpdateEvent -= onPlayerPositionUpdated;
        XonixMainModel.OnUnloadSceneEvent -= onSceneUnloaded;
    }

    public void init(XonixEnemysManager em)
    {
        enemyManager = em;
        initField();
    }

    private void onEnemyPositionUpdated(XonixEnemy enemy)
    {
        XonixFieldElement leavedFieldElement = getFieldElementByPosition((int)(enemy.previousPosition.x), (int)(enemy.previousPosition.y));
        OnEnemyPositionUpdatedEvent(enemy, leavedFieldElement);
    }

    private void onPlayerPositionUpdated(int playerX,int playerY,int prevPlayerPositionX,int prevPlayerPositionY)
    {
        XonixFieldElement leavedFieldElement = getFieldElementByPosition(prevPlayerPositionX, prevPlayerPositionY);
        OnPlayerPositionUpdatedEvent(playerX, playerY,leavedFieldElement);
    }

    private void onEnemyRemoved(XonixEnemy currEnemy)
    {
        XonixFieldElement enemyPlaceElement = getFieldElementByPosition((int) (currEnemy.currentPosition.x),(int) (currEnemy.currentPosition.y));
        OnFieldElementUpdatedEvent(enemyPlaceElement);
    }

    public void resetField()
    {
        clearPath();
        XonixFieldElement currentElement;
        EFieldCellState currCellState;
        totalGroundCount = 0;
        for (int i = 0; i < fieldWidth; i++)
        {
            for (int j = 0; j < fieldHeight; j++)
            {
                if (i < initialBoarder || i >= fieldWidth - initialBoarder || j < initialBoarder || j >= fieldHeight - initialBoarder)
                {
                    currCellState = EFieldCellState.Sea;
                }
                else
                {
                    totalGroundCount++;
                    currCellState = EFieldCellState.Ground;
                }
                currentElement = new XonixFieldElement(new Vector2(i, j), currCellState);
                fieldArray[i, j] = currentElement;
            }
        }
        
    }

    public void clearPath()
    {
        if (currentPath != null)
        {
            foreach (XonixFieldElement currPathElement in currentPath)
            {
                currPathElement.CurrentState = EFieldCellState.Ground;
                OnFieldElementUpdatedEvent(currPathElement);
            }
        }
        
        currentPath = new List<XonixFieldElement>();
    }

    private void initField()
    { 
        fieldArray = new XonixFieldElement[fieldWidth,fieldHeight];
        resetField();
    }

    public XonixFieldElement getFieldElementByPosition(int x, int y)
    {

        Assert.IsTrue(x >= 0 && y >= 0 && x < fieldWidth && y < fieldHeight, "try to get field elemrnt on unexpected position");
        return fieldArray[x,y];
    }

    public EFieldCellState getFieldElementStateByPosition(int x, int y)
    {
        if (x < 0 || x >= fieldWidth || y < 0 || y >= fieldHeight)
        {
            return EFieldCellState.Void;
        }
        return getFieldElementByPosition(x, y).CurrentState;
    }

    public void notifyPlayerMovement(int oldX,int oldY,int newX,int newY)
    {
        EFieldCellState pathCellCurrentState = getFieldElementStateByPosition(oldX, oldY);
        if (pathCellCurrentState == EFieldCellState.Ground)
        {
            if (currentPath == null)
            {
                currentPath = new List<XonixFieldElement>();
            }
            XonixFieldElement pathElement = getFieldElementByPosition(oldX, oldY);
            pathElement.CurrentState = EFieldCellState.Path;
            OnFieldElementUpdatedEvent(pathElement);
            currentPath.Add(pathElement);

            EFieldCellState pathCellNextState = getFieldElementStateByPosition(newX, newY);
            if (pathCellNextState == EFieldCellState.Sea)
            {
                foreach (XonixFieldElement currentFoundElement in currentPath)
                {
                    currentFoundElement.CurrentState = EFieldCellState.Sea;
                    OnFieldElementUpdatedEvent(currentFoundElement);
                }
                currentPath = new List<XonixFieldElement>();
                removeSmallIslands();
            }
        }
    }

    private void removeSmallIslands()
    {
        for (int i = 0; i < fieldWidth; i++)
        {
            for (int j = 0; j < fieldHeight; j++)
            {
                fieldArray[i, j].isAlreadyChecked = false;
            }
        }
        List<List<XonixFieldElement>> foundIslands = new List<List<XonixFieldElement>>();
        List<XonixFieldElement> currIsland;
        for (int i = 0; i < fieldWidth; i++)
        {
            for (int j = 0; j < fieldHeight; j++)
            {
                if (!fieldArray[i, j].isAlreadyChecked && fieldArray[i, j].CurrentState == EFieldCellState.Ground)
                {
                    currIsland = new List<XonixFieldElement>();
                    getNeighbours(i, j, EFieldCellState.Ground, currIsland);
                    Boolean isEnemyInside = false;
                    foreach (XonixFieldElement currElement in currIsland)
                    {
                        if (enemyManager.isExistsEnemyInPosition(currElement.Position))
                        {
                            isEnemyInside = true;
                            break;
                        }
                    }
                    if (!isEnemyInside)
                    {
                        foundIslands.Add(currIsland);
                    }
                }
            }
        }
        if (foundIslands.Count == 0)
        {
            return;
        }
        if (foundIslands.Count > 0)
        {
            int scores = 0;
            
            foreach (List<XonixFieldElement> currCheckingIsland in foundIslands)
            {
                    foreach (XonixFieldElement currElement in currCheckingIsland)
                    {
                        currElement.CurrentState = EFieldCellState.Sea;
                        OnFieldElementUpdatedEvent(currElement);
                        scores++;
                    }
            }
            OnIslandRemovedEvent(scores,getGroundPart());
        }
    }

    public float getGroundPart()
    {
        int groundCount =0;
        for (int i = 0; i < fieldWidth; i++)
        {
            for (int j = 0; j < fieldHeight; j++)
            {
                if (fieldArray[i, j].CurrentState == EFieldCellState.Ground)
                {
                    groundCount++;
                }
            }
        }

        float res = (float)(groundCount)/(float)(totalGroundCount);
        return res;
    }

    void getNeighbours(int x,int y,EFieldCellState requiredState, List<XonixFieldElement> listToAdd)
    {
        int startX;
        int startY;
        int endX;
        int endY;
        XonixFieldElement currElement;
        startX = x - 1;
        if (startX < 0)
        {
            startX = 0;
        }
        startY = y - 1;
        if (startY < 0)
        {
            startY = 0;
        }
        endX = x + 1;
        if (endX > fieldWidth - 1)
        {
            endX = fieldWidth - 1;
        }
        endY = y + 1;
        if (endY > fieldHeight - 1)
        {
            endY = fieldHeight - 1;
        }
        for (int i = startX; i <= endX; i++)
        {
            for (int j = startY; j <= endY; j++)
            {
                currElement = fieldArray[i, j];
                if (currElement.CurrentState == requiredState && !currElement.isAlreadyChecked)
                {
                    listToAdd.Add(currElement);
                    currElement.isAlreadyChecked = true;
                    getNeighbours(i, j, requiredState, listToAdd);
                }
            }
        }
    }

    public List<XonixFieldElement> getAllElementsWithRequiredState(EFieldCellState requiredState)
    {
        List<XonixFieldElement> res = new List<XonixFieldElement>();
        XonixFieldElement currElement;
        for (int i = 0; i < fieldWidth; i++)
        {
            for (int j = 0; j < fieldHeight; j++)
            {
                currElement = fieldArray[i, j];
                if (currElement.CurrentState == requiredState)
                {
                    res.Add(currElement);
                }
            }
        }
        return res;
    }
}