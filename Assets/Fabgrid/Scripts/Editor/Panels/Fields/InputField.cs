using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace PanelEditor
{
    public class InputField<T> : PanelField where T : struct
    {
        //이거 뭐 가져와야 되는지도 고민해봐야 될듯.
        private UnityEngine.UIElements.TextInputBaseField<T> _inputField;
                
        public override void Init(FieldInfo info)
        {
            //이거 t 말고 string으로 가져와야 될 것 같기도함.
            if(_inputField == null)
            {
                Debug.LogError($"InputField is null!");
                return;
            }
            
            string labelName = info.Name;
            _inputField.label = labelName;
        }
        
        public InputField(VisualElement root,VisualTreeAsset field, FieldInfo info) : base(root,field, info)
        {
            _inputField = _fieldRoot.Q<TextInputBaseField<T>>();
            Init(info);
        }
    }

}
