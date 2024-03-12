using AxisConvertSystem;
using InteractableSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialObjectUnit : InteractableObject
{
    [Header("Tutorial System")]
    [SerializeField] private TutorialSO _tutorialSO;
    [SerializeField] private Transform _markAppearTransform;

    public bool IsOn { get; private set; } = false;
    private TutorialMark _rotateTarget;

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
        IsOn = false;
        LoadTutorialMark();
    }

    public override void OnPush()
    {
        base.OnPush();
        if(_rotateTarget != null)
        {
            SceneControlManager.Instance.DeleteObject(_rotateTarget);
        }
    }

    public override void Convert(AxisType axis)
    {
        if(axis != AxisType.None)
        {
            Activate(false);
        }
        else
        {
            Activate(true);
        }
        base.Convert(axis);
    }

    public override void UnitSetting(AxisType axis)
    {
        base.UnitSetting(axis);

        if(axis != AxisType.None)
        {
            SceneControlManager.Instance.DeleteObject(_rotateTarget);
            _rotateTarget = null;
        }
        else
        {
            if(_rotateTarget == null)
            {
                LoadTutorialMark();
            }
        }
    }

    private void LoadTutorialMark()
    {
        _rotateTarget = SceneControlManager.Instance.AddObject("TutorialMark") as TutorialMark;

        if (_markAppearTransform != null)
        {
            _rotateTarget.transform.position = _markAppearTransform.position;
        }
        else
        {
            _rotateTarget.transform.position = Collider.bounds.center + Vector3.up * 2.0f;
        }
    }


    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        if (!IsOn)
        {
            TutorialManager.Instance.StartTutorial(_tutorialSO);
            InputManager.Instance.SetEnableInputWithout(EInputCategory.Interaction, true);
        }
        else
        {
            TutorialManager.Instance.StopTutorial();
            InputManager.Instance.SetEnableInputWithout(EInputCategory.Interaction, false);
        }
        IsOn = !IsOn;
    }
}
