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
    private TutorialMark _tutorialMark;

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);

        gameObject.layer = LayerMask.NameToLayer("Interactable");
        IsOn = false;
    }

    private void OnDisable()
    {
        if(_tutorialMark != null)
        {
            _tutorialMark.Off();
        }
    }

    public override void OnPush()
    {
        base.OnPush();
        if(_tutorialMark != null)   
        {
            _tutorialMark.Off();
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
            gameObject.layer = LayerMask.NameToLayer("Interactable");
        }
        base.Convert(axis);
    }

    public override void UnitSetting(AxisType axis)
    {
        base.UnitSetting(axis);

        if(axis != AxisType.None)
        {
            if(_tutorialMark != null)
            {
                _tutorialMark.Off();
                _tutorialMark = null;
            }
        }
    }

    private void LoadTutorialMark()
    {
        _tutorialMark = SceneControlManager.Instance.AddObject("TutorialMark") as TutorialMark;

        if (_markAppearTransform != null)
        {
            _tutorialMark.transform.position = _markAppearTransform.position;
        }
        else
        {
            _tutorialMark.transform.position = Collider.bounds.center + Vector3.up * 2.0f;
        }

        _tutorialMark.transform.SetParent(Section.transform);
        _tutorialMark.On();
    }


    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        if (!IsOn)
        {
            TutorialManager.Instance.StartTutorial(_tutorialSO);
            InputManager.Instance.SetEnableInputWithout(EInputCategory.Interaction, false);

            _tutorialMark.Off();
            _tutorialMark = null;
        }
        else
        {
            InputManager.Instance.SetEnableInputAll(true);
            TutorialManager.Instance.StopTutorial();

            if (_tutorialMark == null)
            {
                LoadTutorialMark();
            }
        }
        IsOn = !IsOn;
    }
    public override void OnDetectedEnter()
    {
        base.OnDetectedEnter();
        if (_tutorialMark == null)
        {
            LoadTutorialMark();
        }
    }

    public override void OnDetectedLeave()
    {
        base.OnDetectedLeave();

        if(_tutorialMark != null)
        {
            _tutorialMark.Off();
            _tutorialMark = null;
        }
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
