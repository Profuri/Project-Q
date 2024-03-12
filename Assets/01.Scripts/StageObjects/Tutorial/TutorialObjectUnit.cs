using AxisConvertSystem;
using InteractableSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]    
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

        gameObject.layer = LayerMask.NameToLayer("Interactable");
        IsOn = false;
        LoadTutorialMark();
    }

    private void OnDisable()
    {
        if(_rotateTarget != null)
        {
            _rotateTarget.Off();
        }
    }

    public override void OnPush()
    {
        base.OnPush();
        if(_rotateTarget != null)   
        {
            _rotateTarget.Off();
        }
    }

    public override void Convert(AxisType axis)
    {
        if(axis != AxisType.None)
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
        base.Convert(axis);
    }

    public override void UnitSetting(AxisType axis)
    {
        base.UnitSetting(axis);

        if(axis != AxisType.None)
        {
            //SceneControlManager.Instance.DeleteObject(_rotateTarget);
            _rotateTarget.Off();
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

        _rotateTarget.transform.SetParent(Section.transform);
    }


    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        if (!IsOn)
        {
            TutorialManager.Instance.StartTutorial(_tutorialSO);
            InputManager.Instance.SetEnableInputWithout(EInputCategory.Interaction, false);
        }
        else
        {
            InputManager.Instance.SetEnableInputAll(true);
            TutorialManager.Instance.StopTutorial();
        }
        IsOn = !IsOn;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Collider collider = GetComponent<Collider>();
        if(collider != null)
        {
            Vector3 center = collider.bounds.center;
            Vector3 size = collider.bounds.size;
            Gizmos.DrawCube(center,size);
        }
    }
#endif
}
