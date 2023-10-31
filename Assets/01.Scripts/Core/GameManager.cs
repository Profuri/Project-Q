using System;
using Singleton;
using UnityEngine.InputSystem;

public class GameManager : MonoSingleton<GameManager>
{
    public delegate void UnityEventListener();
    public delegate void ObserverListener<in T>(T value);
    
    public event UnityEventListener OnStartEvent = null;
    public event UnityEventListener OnUpdateEvent = null;
    public event ObserverListener<EAxisType> OnAxisTypeChangeEvent = null;

    private void Start()
    {
        OnStartEvent?.Invoke();
    }

    private void Update()
    {
        OnUpdateEvent?.Invoke();

        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            ChangeAxis(EAxisType.NONE);
        }

        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            ChangeAxis(EAxisType.EXPRESSION_X);
        }

        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            ChangeAxis(EAxisType.EXPRESSION_Y);
        }

        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            ChangeAxis(EAxisType.EXPRESSION_Z);
        }
    }

    public void ChangeAxis(EAxisType nextType)
    {
        OnAxisTypeChangeEvent?.Invoke(nextType);
    }
}