using System;
using System.Collections.Generic;
using UnityEngine;

public class Scene : PoolableMono
{
    private List<Transform> _sceneObjs;

    private void Awake()
    {
        _sceneObjs = new List<Transform>();
    }

    public override void Init()
    {
    }
}