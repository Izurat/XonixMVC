using UnityEngine;
using System.Collections;


public enum EEnemyType
{
    Ground,
    Water
};

public enum EEnemyDirectionType
{
    UpLeft,UpRight,DownLeft,DownRight,Clamped
}

public class XonixEnemy {

    public EEnemyType EnemyType { get; private set; }

    private Vector2 _currentPositiion;

    public Vector2 currentPosition {
        get
        {
            return _currentPositiion;
        }
        set
        {
            previousPosition = _currentPositiion;
            _currentPositiion = value;
        }
    }
    public Vector2 previousPosition { get; private set; }

    public EEnemyDirectionType directionType { get; set; }

    public XonixEnemy(EEnemyType type, Vector2 startPosition)
    {
        EnemyType = type;
        currentPosition = startPosition;
    }

    public void updatePosition(Vector2 newPosition)
    {
        currentPosition = newPosition;
    }
}
