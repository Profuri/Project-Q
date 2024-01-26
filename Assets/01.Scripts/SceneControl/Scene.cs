using System.Collections.Generic;
using UnityEngine;

public class Scene : PoolableMono
{
    [SerializeField] private SceneType _type;
    public SceneType Type => _type;
    
    private List<Transform> _sceneObjs;

    private void Awake()
    {
        _sceneObjs = new List<Transform>();
    }

    public override void Init()
    {
    }
    
#if UNITY_EDITOR

    private void OnValidate()
    {
        gameObject.name = $"{_type}Scene";
    }

#endif
}