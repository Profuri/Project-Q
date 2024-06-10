using System.Collections.Generic;
using System.Linq;
using AxisConvertSystem;
using UnityEngine;

public class TutorialPropController : MonoBehaviour
{
      private List<ObjectUnit> _props;

      private void Awake()
      {
            _props = new List<ObjectUnit>();
            _props = GetComponentsInChildren<ObjectUnit>(true).ToList();
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
}