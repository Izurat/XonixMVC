using System;
using UnityEngine;

public enum EFieldCellState {Ground,Sea,Path,Void};

public class XonixFieldElement
{
    private Vector2 _position;
    public Vector2 Position { get; private set; }

    public Boolean isAlreadyChecked { get; set; }

    private EFieldCellState _currentState;
    public EFieldCellState CurrentState {
        get { return _currentState; }
        set { _currentState = value; }
    }

    public XonixFieldElement(Vector2 position, EFieldCellState state)
    {
        Position = position;
        CurrentState = state;
    }
}
