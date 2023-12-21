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
            _inputField = _fieldRoot.Q<TextInputBaseField<T>>("unity-text-input");
        }
        
        public InputField(VisualElement root,VisualTreeAsset field, FieldInfo info) : base(root,field, info)
        {
            //base(field, info);
            //만약 안된다면 부무 생성자가 실행이 안 되는지 확인.
            Init(info);
        }
    }

}
