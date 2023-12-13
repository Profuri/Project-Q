using System;
using System.Reflection;
using InteractableSystem;
using Singleton;
using StageStructureConvertSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoSingleton<GameManager>
{
    public delegate void UnityEventListener();
    public delegate void ObserverListener<in T>(T value);
    
    public event UnityEventListener OnStartEvent = null;
    public event UnityEventListener OnUpdateEvent = null;

    private void Start()
    {
        OnStartEvent?.Invoke();
    }

    private void Update()
    {
        OnUpdateEvent?.Invoke();
    }
}