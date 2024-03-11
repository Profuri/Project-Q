using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


namespace TinyGiantStudio.Text
{
    /// <summary>
    /// This is used by the asset to store default settings shared by different scripts
    /// Default File location: Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Utility/M3D Editor Settings.asset
    /// </summary>
    //[CreateAssetMenu(menuName = "Tiny Giant Studio/Modular 3d Text/Settings")]
    public class AssetSettings : ScriptableObject
    {
        [HideInInspector] public string selectedTab = "Getting Started";

        public Color tabSelectedColor = new Color(176f / 255f, 175f / 255, 175f / 255f);
        public Color tabUnselectedColor = new Color(120f / 255f, 116f / 255f, 116f / 255f);
        [Space]
        public Color gridItemColor = new Color(185f / 255f, 184f / 255f, 184f / 255f);

        public Color importantLabelColor_darkSkin = new Color(255f / 255f, 204f / 255f, 128f / 255f);
        public Color importantLabelColor_lightSkin = new Color(159f / 255f, 97f / 255f, 7f / 255f);

        public Color openedFoldoutTitleColor_darkSkin = new Color(240f / 255f, 241f / 255f, 101f / 255f);
        public Color openedFoldoutTitleColor_lightSkin = new Color(123 / 255f, 120 / 255f, 1 / 255f);


        //Text
        public Font defaultFont = null;
        public Vector3 defaultTextSize = new Vector3(8, 8, 8);
        public Material defaultTextMaterial = null;

        //Button
        public Vector3 defaultButtonNormalTextSize = new Vector3(8, 8, 8);
        public Material defaultButtonNormalTextMaterial = null;
        public Material defaultButtonNormalBackgroundMaterial = null;

        public Vector3 defaultButtonSelectedTextSize = new Vector3(8.2f, 8.2f, 8.2f);
        public Material defaultButtonSelectedTextMaterial = null;
        public Material defaultButtonSelectedBackgroundMaterial = null;

        public Vector3 defaultButtonPressedTextSize = new Vector3(8.2f, 8.2f, 5);
        public Material defaultButtonPressedTextMaterial = null;
        public Material defaultButtonPressedBackgroundMaterial = null;

        public Vector3 defaultButtonDisabledTextSize = new Vector3(8.2f, 8.2f, 5);
        public Material defaultButtonDisabledTextMaterial = null;
        public Material defaultButtonDisabledBackgroundMaterial = null;

        //List
        public Vector3 defaultListNormalTextSize = new Vector3(8, 8, 2);
        public Material defaultListNormalTextMaterial = null;
        public Material defaultListNormalBackgroundMaterial = null;

        public Vector3 defaultListSelectedTextSize = new Vector3(8.2f, 8.2f, 5);
        public Material defaultListSelectedTextMaterial = null;
        public Material defaultListSelectedBackgroundMaterial = null;

        public Vector3 defaultListPressedTextSize = new Vector3(8.2f, 8.2f, 5);
        public Material defaultListPressedTextMaterial = null;
        public Material defaultListPressedBackgroundMaterial = null;

        public Vector3 defaultListDisabledTextSize = new Vector3(8.2f, 8.2f, 5);
        public Material defaultListDisabledTextMaterial = null;
        public Material defaultListDisabledBackgroundMaterial = null;


        //[HideInInspector] public bool createLogTextFile = false;
        //[HideInInspector] public bool createConsoleLogs = false;

        [Header("Inspector field size")]
        public float smallHorizontalFieldSize = 72.5f;
        public float normalHorizontalFieldSize = 100;
        public float largeHorizontalFieldSize = 132.5f;
        public float extraLargeHorizontalFieldSize = 150f;

        public enum MeshExportStyle
        {
            exportAsObj,
            exportAsMeshAsset
        }




        #region Preferences
        public bool autoCreateSceneInputSystem = true;
        public bool dontAutoCreateRaycasterOrButtonIfVRtoolkitExists = true;
        #endregion








        #region Font creation
        //font creation settings
        public enum CharInputStyle
        {
            CharacterRange,
            UnicodeRange,
            CustomCharacters,
            UnicodeSequence
            //CharacterSet
        }

        public CharInputStyle charInputStyle;

        public char startChar = '!'; //default '!'
        public char endChar = '~'; //default '~'
        public string startUnicode = "0021"; //default
        [HideInInspector] public string endUnicode = "007E"; //default 

        [HideInInspector]
        [TextArea(10, 99)]
        public string customCharacters = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~"; //default 

        [HideInInspector]
        [TextArea(10, 99)]
        public string unicodeSequence = "\\u0021-\\u007E"; //default     


        [HideInInspector] public int vertexDensity = 1; //default 1
        [HideInInspector] public float sizeXY = 1; //default 1
        [HideInInspector] public float sizeZ = 1; //default 1
        [HideInInspector] public int smoothingAngle = 30; //default 30

        [HideInInspector] public MeshExportStyle meshExportStyle = MeshExportStyle.exportAsObj;

        [HideInInspector] public int previewAmount;



#if ENABLE_INPUT_SYSTEM
        [SerializeField] InputActionAsset _inputActionAsset;
        public InputActionAsset InputActionAsset
        {
            get
            {
                if (_inputActionAsset == null)
                {
                    FindModularTextInputActionAsset();
                }

                return _inputActionAsset;
            }
            set { _inputActionAsset = value; }
        }

        public void FindModularTextInputActionAsset()
        {
#if UNITY_EDITOR //TODO //Unnecessary, but still writing it to be safe. Unnecessary because only editor scripts call the settings file to get input action asset
            if (!_inputActionAsset)
            {
                string[] guids;

                guids = AssetDatabase.FindAssets("t:inputActionAsset");
                foreach (string guid in guids)
                {
                    if (AssetDatabase.GUIDToAssetPath(guid).Contains("3D Text UI Controls.inputactions"))
                    {
                        InputActionAsset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(InputActionAsset)) as InputActionAsset;
                        EditorUtility.SetDirty(this);
                        break;
                    }
                }
            }
#endif
        }
#endif


        public void ResetFontCreationMeshSettings()
        {
            vertexDensity = 1;
            sizeXY = 1;
            sizeZ = 1;
            smoothingAngle = 30;

            meshExportStyle = MeshExportStyle.exportAsObj;
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
        public void ResetFontCreationPrebuiltSettings()
        {
            charInputStyle = CharInputStyle.CharacterRange;
            startChar = '!';
            endChar = '~';
            startUnicode = "0021";
            endUnicode = "007E";
            customCharacters = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
            unicodeSequence = "\\u0021-\\u007E";
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
        #endregion

    }
}
