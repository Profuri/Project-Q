using System.Collections.Generic;
using System.Linq;
using AxisConvertSystem;
using UnityEngine;

public class TutorialPropController : MonoBehaviour
{
      private List<ObjectUnit> _props;
      [SerializeField] private StoryData _storyData;

      private void Awake()
      {
            _props = new List<ObjectUnit>();
            _props = GetComponentsInChildren<ObjectUnit>(true).ToList();

            var ownerStage = GetComponentInParent<Stage>();
            ownerStage.OnEnterSectionEvent += UnShowProps;
      }

      public void ShowProps()
      {
            foreach (var unit in _props)
            {
                  unit.Activate(true);
            }
      }

      public void UnShowProps()
      {
            foreach (var unit in _props)
            {
                  unit.Activate(false);
            }
      }

      public void ReloadPlayer()
      {
            SceneControlManager.Instance.Player.ReloadUnit();
      }

      public void PlayStory()
      {
            StoryManager.Instance.StartStory(_storyData);
      }
}