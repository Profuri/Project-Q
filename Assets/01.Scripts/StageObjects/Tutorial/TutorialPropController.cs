using System.Collections.Generic;
using System.Linq;
using AxisConvertSystem;
using UnityEngine;

public class TutorialPropController : MonoBehaviour
{
      private enum TutorialType
      {
            None,
            Movement,
            CompressX,
            CompressY,
            CompressZ,
      }
      
      [SerializeField] private TutorialType _tutorialType;
      
      private List<ObjectUnit> _props;
      [SerializeField] private StoryData _storyData;

      private Stage _ownerStage;
      private UIComponent _tutorialUI;

      private void Awake()
      {
            _props = new List<ObjectUnit>();
            _props = GetComponentsInChildren<ObjectUnit>(true).ToList();

            _ownerStage = GetComponentInParent<Stage>();
            _ownerStage.OnEnterSectionEvent += UnShowProps;
      }

      public void ShowProps()
      {
            foreach (var unit in _props)
            {
                  unit.Activate(true);
            }

            if (_tutorialType != TutorialType.None && StoryManager.Instance.IsPlay)
            {
                  StoryManager.Instance.OnStoryRealesed += () =>
                  {
                        ShowTutorialUI();
                        SceneControlManager.Instance.Player.OnApplyUnitInfoEvent += AxisChangeHandle;
                  };
            }
      }

      public void UnShowProps()
      {
            foreach (var unit in _props)
            {
                  unit.Activate(false);
            }
            DisappearTutorialUI();
            _ownerStage.OnEnterSectionEvent -= UnShowProps;
            SceneControlManager.Instance.Player.OnApplyUnitInfoEvent -= AxisChangeHandle;
      }

      public void ReloadPlayer()
      {
            SceneControlManager.Instance.Player.ReloadUnit();
      }

      public void PlayStory()
      {
            StoryManager.Instance.StartStory(_storyData);
      }

      private void AxisChangeHandle(AxisType newAxis)
      {
            if (newAxis == AxisType.None)
            {
                  ShowTutorialUI();
            }
            else
            {
                  DisappearTutorialUI();
            }
      }

      private void ShowTutorialUI()
      {
            if (_tutorialUI != null)
            {
                  return;
            }

            _tutorialUI = UIManager.Instance.GenerateUI($"{_tutorialType}TutorialPanel");
      }

      private void DisappearTutorialUI()
      {
            if (_tutorialUI == null)
            {
                  return;
            }

            _tutorialUI.Disappear();
            _tutorialUI = null;
      }
}