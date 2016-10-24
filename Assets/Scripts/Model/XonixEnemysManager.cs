using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.SceneManagement;


public enum EBoarderCollisionType
{
    VerticalBoarder,HorizontalBoarder,InnerAngle,OuterAngle,Clamp,None
}

public class XonixEnemysManager{

    private XonixFieldModel fieldModel;
    
    public float updateEnemysDelay = 0.2f;

    public List<XonixEnemy> enemys { get; private set; }

    public delegate void OnEnemyPositionUpdate(XonixEnemy enemy);
    public static event OnEnemyPositionUpdate OnEnemyPositionUpdateEvent = delegate { };

    public delegate void OnEnemyRemoved(XonixEnemy enemy);
    public static event OnEnemyRemoved OnEnemyRemovedEvent = delegate { };

    public delegate void OnDamage();
    public static event OnDamage OnDamageEvent = delegate { };

    public void init(XonixFieldModel fm)
    {
        fieldModel = fm;
        XonixTimeModel.OnLevelTimeFinishedEvent += addWaterEnemy;
        XonixMainModel.OnUnloadSceneEvent += onSceneUnloaded; ;
    }

    private void onSceneUnloaded()
    {
        XonixTimeModel.OnLevelTimeFinishedEvent -= addWaterEnemy;
        XonixMainModel.OnUnloadSceneEvent -= onSceneUnloaded;
    }

    public void initEnemys( int groundEnemysCount, int waterEnemysCount)
    {
        if (enemys != null)
        {
            foreach (XonixEnemy currExistingEnemy in enemys)
            {
                OnEnemyRemovedEvent(currExistingEnemy);
            }
        }
        enemys = new List<XonixEnemy>();
        List<Vector2> EnemysPositions = getEnemysPlaces(EEnemyType.Ground, groundEnemysCount);
        XonixEnemy currEnemy;
        foreach (Vector2 currEnemyPosition in EnemysPositions)
        {
            currEnemy = new XonixEnemy(EEnemyType.Ground, currEnemyPosition);
            enemys.Add(currEnemy);
        }
        for (int i = 0; i < waterEnemysCount; i++)
        {
            Vector2 currWaterEnemyPosition = new Vector2(fieldModel.fieldWidth / 2+i, 0);
            currEnemy = new XonixEnemy(EEnemyType.Water, currWaterEnemyPosition);
            enemys.Add(currEnemy);
        }
    }

    public void resetWaterEnemys(int waterEnemysCount)
    {
        if (enemys != null)
        {
            foreach (XonixEnemy currExistingEnemy in enemys)
            {
                if (currExistingEnemy.EnemyType == EEnemyType.Water)
                {
                    OnEnemyRemovedEvent(currExistingEnemy);
                }    
            }
        }
        List<XonixEnemy> newEnemys = new List<XonixEnemy>();
        foreach (XonixEnemy currEnemy in enemys)
        {
            if (currEnemy.EnemyType == EEnemyType.Ground)
            {
                newEnemys.Add(currEnemy);
            }
        }
        XonixEnemy currWaterEnemy;
        for (int i = 0; i < waterEnemysCount; i++)
        {
            Vector2 currWaterEnemyPosition = new Vector2(fieldModel.fieldWidth / 2 + i, 0);
            currWaterEnemy = new XonixEnemy(EEnemyType.Water, currWaterEnemyPosition);
            newEnemys.Add(currWaterEnemy);
        }
        enemys = newEnemys;
    }

    public void updateEnemysMovement()
    {
        foreach (XonixEnemy currEnemy in enemys)
        {
            updateEnemyPosition(currEnemy);
            OnEnemyPositionUpdateEvent(currEnemy);
        }
    }

    public void updateDamage(int playerPositionX,int playerPositionY)
    {
        EFieldCellState currCellState;
        foreach (XonixEnemy currEnemy in enemys)
        {
            for (int i = (int)(currEnemy.currentPosition.x - 1); i <= (int)(currEnemy.currentPosition.x + 1); i++)
            {
                for (int j = (int)(currEnemy.currentPosition.y - 1); j <= (int)(currEnemy.currentPosition.y + 1); j++)
                {
                    currCellState = fieldModel.getFieldElementStateByPosition(i, j);
                    if (currCellState == EFieldCellState.Path || (playerPositionY == j && playerPositionX == i))
                    {
                        OnDamageEvent();
                        return;
                    }
                }
            }
        }
        currCellState = fieldModel.getFieldElementStateByPosition(playerPositionX, playerPositionY);
        if (currCellState == EFieldCellState.Path)
        {
            OnDamageEvent();
        }
    }


    private void updateEnemyPosition(XonixEnemy enemy)
    {
        EEnemyDirectionType nextDirectionType = updateEnemyDirectionType(enemy);
        enemy.directionType = nextDirectionType;
        int nextX = (int)(enemy.currentPosition.x);
        int nextY = (int)(enemy.currentPosition.y);
        switch (nextDirectionType)
        {
            case EEnemyDirectionType.DownLeft:
                nextX --;
                nextY --;
                break;
            case EEnemyDirectionType.DownRight:
                nextX++;
                nextY--;
                break;
            case EEnemyDirectionType.UpLeft:
                nextX--;
                nextY++;
                break;
            case EEnemyDirectionType.UpRight:
                nextX++;
                nextY++;
                break;
            default:
                break;
        }
        enemy.currentPosition = new Vector2(nextX,nextY);
    }
    private EEnemyDirectionType updateEnemyDirectionType(XonixEnemy enemy)
    {
        EBoarderCollisionType currentCollisionType = getCollisionType(enemy);
        if (currentCollisionType == EBoarderCollisionType.None)
        {
            return enemy.directionType;
        }
        if (currentCollisionType == EBoarderCollisionType.Clamp)
        {
            return EEnemyDirectionType.Clamped;
        }
        switch (enemy.directionType)
        {
            case EEnemyDirectionType.DownLeft:
                switch (currentCollisionType)
                {
                    case EBoarderCollisionType.HorizontalBoarder:
                        return EEnemyDirectionType.UpLeft;
                    case EBoarderCollisionType.VerticalBoarder:
                        return EEnemyDirectionType.DownRight;
                    case EBoarderCollisionType.InnerAngle:
                    case EBoarderCollisionType.OuterAngle:
                        return EEnemyDirectionType.UpRight;
                }
                break;
            case EEnemyDirectionType.DownRight:
                switch (currentCollisionType)
                {
                    case EBoarderCollisionType.HorizontalBoarder:
                        return EEnemyDirectionType.UpRight;
                    case EBoarderCollisionType.VerticalBoarder:
                        return EEnemyDirectionType.DownLeft;
                    case EBoarderCollisionType.InnerAngle:
                    case EBoarderCollisionType.OuterAngle:
                        return EEnemyDirectionType.UpLeft;
                }
                break;
            case EEnemyDirectionType.UpLeft:
                switch (currentCollisionType)
                {
                    case EBoarderCollisionType.HorizontalBoarder:
                        return EEnemyDirectionType.DownLeft;
                    case EBoarderCollisionType.VerticalBoarder:
                        return EEnemyDirectionType.UpRight;
                    case EBoarderCollisionType.InnerAngle:
                    case EBoarderCollisionType.OuterAngle:
                        return EEnemyDirectionType.DownRight;
                }
                break;
            case EEnemyDirectionType.UpRight:
                switch (currentCollisionType)
                {
                    case EBoarderCollisionType.HorizontalBoarder:
                        return EEnemyDirectionType.DownRight;
                    case EBoarderCollisionType.VerticalBoarder:
                        return EEnemyDirectionType.UpLeft;
                    case EBoarderCollisionType.InnerAngle:
                    case EBoarderCollisionType.OuterAngle:
                        return EEnemyDirectionType.DownLeft;
                }
                break;
            default:break;
        }
        return EEnemyDirectionType.Clamped;
    }

    private EBoarderCollisionType getCollisionType(XonixEnemy enemy)
    {
        Boolean isVerticalCollision = false;
        Boolean isHorizontalCollision = false;
        Boolean isBackwardLocked = false;
        Boolean isColliding = false;
        
        switch (enemy.directionType)
        {
            case EEnemyDirectionType.DownLeft:
                isColliding = !isCanMoveTo(enemy.EnemyType, (int) (enemy.currentPosition.x - 1), (int) (enemy.currentPosition.y - 1));
                isHorizontalCollision = !isCanMoveTo(enemy.EnemyType,(int)(enemy.currentPosition.x), (int)(enemy.currentPosition.y - 1));
                isVerticalCollision = !isCanMoveTo(enemy.EnemyType, (int)(enemy.currentPosition.x-1), (int)(enemy.currentPosition.y));
                isBackwardLocked = !isCanMoveTo(enemy.EnemyType, (int) (enemy.currentPosition.x + 1), (int) (enemy.currentPosition.y + 1));
                break;
            case EEnemyDirectionType.DownRight:
                isColliding = !isCanMoveTo(enemy.EnemyType, (int)(enemy.currentPosition.x + 1), (int)(enemy.currentPosition.y - 1));
                isHorizontalCollision = !isCanMoveTo(enemy.EnemyType, (int)(enemy.currentPosition.x), (int)(enemy.currentPosition.y - 1));
                isVerticalCollision = !isCanMoveTo(enemy.EnemyType, (int)(enemy.currentPosition.x + 1), (int)(enemy.currentPosition.y));
                isBackwardLocked = !isCanMoveTo(enemy.EnemyType, (int)(enemy.currentPosition.x - 1), (int)(enemy.currentPosition.y + 1));
                break;
            case EEnemyDirectionType.UpLeft:
                isColliding = !isCanMoveTo(enemy.EnemyType, (int)(enemy.currentPosition.x - 1), (int)(enemy.currentPosition.y + 1));
                isHorizontalCollision = !isCanMoveTo(enemy.EnemyType, (int)(enemy.currentPosition.x), (int)(enemy.currentPosition.y + 1));
                isVerticalCollision = !isCanMoveTo(enemy.EnemyType, (int)(enemy.currentPosition.x - 1), (int)(enemy.currentPosition.y));
                isBackwardLocked = !isCanMoveTo(enemy.EnemyType, (int)(enemy.currentPosition.x + 1), (int)(enemy.currentPosition.y - 1));
                break;
            case EEnemyDirectionType.UpRight:
                isColliding = !isCanMoveTo(enemy.EnemyType, (int)(enemy.currentPosition.x + 1), (int)(enemy.currentPosition.y + 1));
                isHorizontalCollision = !isCanMoveTo(enemy.EnemyType, (int)(enemy.currentPosition.x), (int)(enemy.currentPosition.y + 1));
                isVerticalCollision = !isCanMoveTo(enemy.EnemyType, (int)(enemy.currentPosition.x + 1), (int)(enemy.currentPosition.y));
                isBackwardLocked = !isCanMoveTo(enemy.EnemyType, (int)(enemy.currentPosition.x - 1), (int)(enemy.currentPosition.y - 1));
                break;
            default:
                break;
        }
        if (!isColliding)
        {
            return EBoarderCollisionType.None;
        }
        if (isVerticalCollision && isHorizontalCollision && isBackwardLocked)
        {
            return EBoarderCollisionType.Clamp;
        }
        if (isVerticalCollision && isHorizontalCollision)
        {
            return EBoarderCollisionType.InnerAngle;
        }
        if (isVerticalCollision)
        {
            return EBoarderCollisionType.VerticalBoarder;
        }
        if (isHorizontalCollision)
        {
            return EBoarderCollisionType.HorizontalBoarder;
        }
        return EBoarderCollisionType.OuterAngle;
    }

    private Boolean isCanMoveTo(EEnemyType enemyType, int targetX, int targetY)
    {
        EFieldCellState targetCellState = fieldModel.getFieldElementStateByPosition(targetX, targetY);
        switch (enemyType)
        {
            case EEnemyType.Ground:
                if (targetCellState == EFieldCellState.Ground)
                {
                    return true;
                }
                break;
            case EEnemyType.Water:
                if (targetCellState == EFieldCellState.Sea)
                {
                    return true;
                }
                break;
            default:
                break;
        }
        return false;
    }

    
    private List<Vector2> getEnemysPlaces(EEnemyType type, int count)
    {
        EFieldCellState requiredState = EFieldCellState.Ground;
        if (type == EEnemyType.Water)
        {
            requiredState = EFieldCellState.Sea;
        }
        List<XonixFieldElement> foundPlaces = fieldModel.getAllElementsWithRequiredState(requiredState);
        
        List<Vector2> res = new List<Vector2>();
        System.Random randomGenerator = new System.Random();
        int currTmpIndex;
        XonixFieldElement currPlaceElement;
        for (int i = 0; i < count; i++)
        {
            currTmpIndex = randomGenerator.Next(0, foundPlaces.Count - 1);
            currPlaceElement = foundPlaces[currTmpIndex];
            res.Add(new Vector2(currPlaceElement.Position.x, currPlaceElement.Position.y));
            foundPlaces.Remove(currPlaceElement);
            if (foundPlaces.Count == 0)
            {
                break;
            }
        }
        return res;
    }

    public Boolean isExistsEnemyInPosition(Vector2 positionToCheck)
    {
        foreach (XonixEnemy currEnemy in enemys)
        {
            if (currEnemy.currentPosition.Equals(positionToCheck))
            {
                return true;
            }
        }
        return false;
    }

    public void addWaterEnemy()
    {
        foreach (XonixEnemy currEnemy in enemys)
        {
            if (currEnemy.EnemyType == EEnemyType.Water)
            {
                XonixEnemy newEnemy = new XonixEnemy(EEnemyType.Water, new Vector2(currEnemy.currentPosition.x,currEnemy.currentPosition.y));
                EEnemyDirectionType resDirection = EEnemyDirectionType.DownLeft; 
                switch (currEnemy.directionType)
                {
                    
                    case EEnemyDirectionType.DownLeft:
                        resDirection = EEnemyDirectionType.UpRight;
                        break;
                    case EEnemyDirectionType.DownRight:
                        resDirection = EEnemyDirectionType.UpLeft;
                        break;
                    case EEnemyDirectionType.UpLeft:
                        resDirection = EEnemyDirectionType.DownRight;
                        break;
                    default:
                        break;
                }
                newEnemy.directionType = resDirection;
                enemys.Add(newEnemy);
                return;
            }
        }
    }
}
