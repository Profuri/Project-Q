using System;
using System.Reflection;
using Fabgrid;
using InteractableSystem;
using Singleton;
using StageStructureConvertSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class GameManager : MonoSingleton<GameManager>
{
    public delegate void UnityEventListener();
    public delegate void ObserverListener<in T>(T value);
    
    public event UnityEventListener OnStartEvent = null;
    public event UnityEventListener OnUpdateEvent = null;
    private PlayerController _player = null;
    public PlayerController Player => _player;
    public EAxisType CurAxisType => _player.Converter.AxisType;

    private void Start()
    {
        OnStartEvent?.Invoke();
        if(_player == null)
            _player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        OnUpdateEvent?.Invoke();

    }
}