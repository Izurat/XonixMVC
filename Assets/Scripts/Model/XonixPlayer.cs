using UnityEngine;


public enum EPlayerMovementInputType
{
    NoChange,Up, Down,Left,Right,Idle
};

public class XonixPlayer
{
    public delegate void OnPlayerPositionUpdate(int currPositionX, int currPositionY, int prevPositionX, int prevPositionY);
    public static event OnPlayerPositionUpdate OnPlayerPositionUpdateEvent = delegate { };

    private EPlayerMovementInputType currentMovementType = EPlayerMovementInputType.Idle;

    private int _currentPositionX;
    private int _currentPositionY;
    private int nextX;
    private int nextY;
    public float updatepositionDelay = 0.1f;
    private XonixFieldModel fieldModel;
    public int currentPositionX
    {
        get
        {
            return _currentPositionX;
        }
        private set
        {
            previousPositionX = _currentPositionX;
            _currentPositionX = value;
        }
    }
    public int currentPositionY
    {
        get
        {
            return _currentPositionY;
        }
        private set
        {
            previousPositionY = _currentPositionY;
            _currentPositionY = value;
        }
    }
    public int previousPositionX { get; private set; }
    public int previousPositionY { get; private set; }

    

    public void init(XonixFieldModel fm)
    {
        fieldModel = fm;
    }

    public void resetPlayer()
    {
        currentPositionY = fieldModel.fieldHeight-1;
        currentPositionX = fieldModel.fieldWidth/2;
        nextX = currentPositionX;
        nextY = currentPositionY;
        currentMovementType = EPlayerMovementInputType.Idle;
        OnPlayerPositionUpdateEvent(currentPositionX, currentPositionY,previousPositionX, previousPositionY);
    }

    public void updatePlayerMovement()
    {
        if (currentMovementType == EPlayerMovementInputType.Idle)
        {
            return;
        }

        nextX = currentPositionX;
        nextY = currentPositionY;
        
        switch (currentMovementType)
        {
            case EPlayerMovementInputType.Down:
                nextY--;
                break;
            case EPlayerMovementInputType.Up:
                nextY++;
                break;
            case EPlayerMovementInputType.Left:
                nextX--;
                break;
            case EPlayerMovementInputType.Right:
                nextX++;
                break;
        }

        EFieldCellState nextCellState = fieldModel.getFieldElementStateByPosition(nextX, nextY);
        if (nextCellState != EFieldCellState.Void)
        {
            fieldModel.notifyPlayerMovement(currentPositionX, currentPositionY, nextX, nextY);
            currentPositionX = nextX;
            currentPositionY = nextY;
        }
        OnPlayerPositionUpdateEvent(currentPositionX, currentPositionY, previousPositionX, previousPositionY);
    }


    
    public void notifyDirection(EPlayerMovementInputType direction)
    {
        if (direction == EPlayerMovementInputType.NoChange || direction == currentMovementType)
        {
            return;
        }
        currentMovementType = getDirectionIfCurrentIsNotOpposite(direction);
    }
    private EPlayerMovementInputType getOppositeTo(EPlayerMovementInputType value)
    {
        switch (value)
        {
            case EPlayerMovementInputType.Down:
                return EPlayerMovementInputType.Up;
            case EPlayerMovementInputType.Right:
                return EPlayerMovementInputType.Left;
            case EPlayerMovementInputType.Left:
                return EPlayerMovementInputType.Right;
            case EPlayerMovementInputType.Up:
                return EPlayerMovementInputType.Down;
        }
        return EPlayerMovementInputType.Idle;
    }

    private EPlayerMovementInputType getDirectionIfCurrentIsNotOpposite(EPlayerMovementInputType inputPlayerMovementType)
    {
        if (getOppositeTo(inputPlayerMovementType) == currentMovementType)
        {
            return EPlayerMovementInputType.Idle;
        }
        return inputPlayerMovementType;
    }
}
