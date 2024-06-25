using System;
using System.Collections.Generic;
using AxisConvertSystem;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class KeyInfoPanel : UIComponent
{
    [SerializeField] private Image _yAxis;
    [SerializeField] private Image _xAxis;
    [SerializeField] private Image _zAxis;

    private Dictionary<AxisType,Image> _axisImageDictionary = new Dictionary<AxisType, Image>();


    protected override void Awake()
    {
        base.Awake();
        _axisImageDictionary.Add(AxisType.Y,_yAxis);
        _axisImageDictionary.Add(AxisType.Z,_zAxis);
        _axisImageDictionary.Add(AxisType.X,_xAxis);
    }
    public override void Appear(Transform parentTrm, Action callback = null)
    {
        base.Appear(parentTrm, callback);

        SceneControlManager.Instance.Player.OnControllingAxisEvent += ChangeAxisUI;
    }

    public override void Disappear(Action callback = null)
    {
        base.Disappear(callback);
        SceneControlManager.Instance.Player.OnControllingAxisEvent -= ChangeAxisUI;
    }
    public void ChangeAxisUI(AxisType axisType)
    {
        Action<float> dictionaryChangeAction = (value)  => 
        {
            foreach(Image image in _axisImageDictionary.Values)
            {
                image.transform.DOScale(new Vector3(value,value,value),0.3f);
            }
        };

        dictionaryChangeAction(0.8f);
        if(axisType == AxisType.None)
        {
            dictionaryChangeAction(1f);
            return;
        }
        _axisImageDictionary[axisType].transform.DOScale(1.3f,0.3f);
    }
}
