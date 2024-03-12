using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TinyGiantStudio.Text.FontCreation;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace TinyGiantStudio.Text.EditorFiles
{
    public class ModularTextEditorWindow : EditorWindow
    {
        #region Variable declaration

        [SerializeField] private VisualTreeAsset visualTreeAsset;

        #region Links

        private string documentationLink = "https://ferdowsur.gitbook.io/modular-3d-text/";
        private string forumLink = "https://forum.unity.com/threads/modular-3d-text-complete-ingame-ui-sytem.821931/";

        #endregion Links

        #region Tabs

        private readonly string selectedTabClassName = "tabSelected";

        private GroupBox tabsHolder;
        private UnityEngine.UIElements.Button informationTabButton;
        private UnityEngine.UIElements.Button settingsTabButton;
        private UnityEngine.UIElements.Button createFontTabButton;
        private UnityEngine.UIElements.Button utilityTabButton;

        #endregion Tabs

        #region Theme

        private EditorThemeManager _themeManager;

        private EditorThemeManager ThemeManager
        {
            get
            {
                if (_themeManager == null)
                    SetupThemeManager();

                return _themeManager;
            }
        }

        private GroupBox rootHolder;
        private UnityEngine.UIElements.Button darkModeButton;
        private UnityEngine.UIElements.Button lightModeButton;
        private VisualElement themeModeBackgroundButton;

        #endregion Theme

        #region Font creation variables

        [SerializeField] private VisualTreeAsset previewTemplate;

        private GroupBox createFontContent;
        private UnityEngine.UIElements.Button createFontButton;
        private Foldout previewFoldout;
        private ScrollView previewScrollview;

        private CharacterGenerator _characterGenerator;

        private CharacterGenerator CharacterGenerator
        {
            get
            {
                if (_characterGenerator == null) _characterGenerator = new CharacterGenerator();

                return _characterGenerator;
            }
        }

        [SerializeField] private string _filePath;

        private string FilePath
        {
            set
            {
                _filePath = value;
                LoadFileContent();
                GetFontAsset();
                UpdateFontInformation();
                Kerning();

                if (value != null && value.Length > 0)
                {
                    createFontButton.SetEnabled(true);
                    previewFoldout.style.display = DisplayStyle.Flex;
                    CleanUpdatePreviews();
                }
                else
                {
                    NoFileSeletected();
                }
            }
            get { return _filePath; }
        }

        private void NoFileSeletected()
        {
            previewFoldout.style.display = DisplayStyle.None;
            createFontButton.SetEnabled(false);
        }

        private byte[] fileContent;
        public UnityEngine.Font font;

        private AssetSettings _settings;

        private int previewCount = 1;
        public List<Preview> previews = new List<Preview>();

        public class Preview
        {
            public Mesh mesh;
            public MeshPreview meshPreview;
            public IMGUIContainer previewContainer;
            public IMGUIContainer previewSettingsContainer;
            public TextField previewCharacterField;
            public Label meshInformationLabel;
        }

        [SerializeField]
        private AssetSettings Settings
        {
            get
            {
                _settings = StaticMethods.VerifySettings(_settings);
                return _settings;
            }
        }

        private SerializedObject _soTarget;

        private SerializedObject soTarget
        {
            get
            {
                if (_soTarget == null)
                    _soTarget = new SerializedObject(Settings);

                return _soTarget;
            }
        }

        private readonly string kerningDocumentationDocURL = "https://ferdowsur.gitbook.io/modular-3d-text/text/fonts/creating-fonts/troubleshoot#no-kerning-found";
        private readonly string fontCreationTroubleShootDocURL = "https://ferdowsur.gitbook.io/modular-3d-text/fonts/creating-fonts/troubleshoot";

        #endregion Font creation variables

        private MaterialPreviewManager _materialPreviewManager;

        private MaterialPreviewManager materialPreviewManager
        {
            get
            {
                if (_materialPreviewManager == null)
                    _materialPreviewManager = new MaterialPreviewManager();

                return _materialPreviewManager;
            }
        }

        #endregion Variable declaration

        #region Unity Things

        [UnityEditor.MenuItem("Tools/Tiny Giant Studio/Modular 3D Text", false, 100)]
        public static void ShowWindow()
        {
            ModularTextEditorWindow wnd = GetWindow<ModularTextEditorWindow>();

            Texture titleIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Utility/Editor Icons/M3D.png") as Texture;
            if (titleIcon)
                wnd.titleContent = new GUIContent("Modular 3D Text", titleIcon);
            else
                wnd.titleContent = new GUIContent("Modular 3D Text", titleIcon);

            wnd.minSize = new Vector2(400, 600);
        }

        private void OnDisable()
        {
            CleanupPreviews();
        }

        private void OnDestroy()
        {
            CleanupPreviews();
        }

        public void CreateGUI()
        {
            visualTreeAsset.CloneTree(rootVisualElement);

            SetupHeader();

            SetupTabs();

            SetupInformationTab();
            SetupSettingsTab();
            SetupFontCreationTab();
            SetupUtilityTab();

            SetupFooter();

            OpenInformationTab();

            #region resume work session after unity session reset after something like a compile script complete

            UpdateFontInformation();

            if (fileContent != null && fileContent.Length > 0)
            {
                createFontButton.SetEnabled(true);
                previewFoldout.style.display = DisplayStyle.Flex;
                CleanUpdatePreviews();
            }
            else
            {
                NoFileSeletected();
            }

            #endregion resume work session after unity session reset after something like a compile script complete
        }

        #endregion Unity Things

        #region Header

        private void SetupHeader()
        {
            string assetLink = "https://bit.ly/Modular3DTextUnityAsset";
            string versionLink = "https://assetstore.unity.com/packages/3d/gui/modular-3d-text-in-game-3d-ui-system-247241?aid=1011ljxWe#releases";

            rootHolder = rootVisualElement.Q<GroupBox>("RootHolder");
            GroupBox header = rootHolder.Q<GroupBox>("Header");

            SetupURLButton(header, "AssetNameButton", assetLink);
            SetupURLButton(header, "CheckUpdateButton", versionLink);

            UpdateTheme();
        }

        private void SetupThemeManager()
        {
            _themeManager = new EditorThemeManager(rootHolder);
        }

        private void UpdateTheme()
        {
            ThemeManager.Start();
        }

        #endregion Header

        #region Tabs

        private void SetupTabs()
        {
            tabsHolder = rootVisualElement.Q<GroupBox>("Tabs");

            informationTabButton = tabsHolder.Q<UnityEngine.UIElements.Button>("InformationTabButton");
            settingsTabButton = tabsHolder.Q<UnityEngine.UIElements.Button>("SettingsTabButton");
            createFontTabButton = tabsHolder.Q<UnityEngine.UIElements.Button>("CreateFontTabButton");
            utilityTabButton = tabsHolder.Q<UnityEngine.UIElements.Button>("UtilityTabButton");

            informationTabButton.clicked += () =>
            {
                OpenInformationTab();
            };
            settingsTabButton.clicked += () =>
            {
                OpenSettingsTab();
            };
            createFontTabButton.clicked += () =>
            {
                OpenCreateFontTab();
            };
            utilityTabButton.clicked += () =>
            {
                OpenUtilityTab();
            };
        }

        private void OpenInformationTab()
        {
            informationTabButton.AddToClassList(selectedTabClassName);
            settingsTabButton.RemoveFromClassList(selectedTabClassName);
            createFontTabButton.RemoveFromClassList(selectedTabClassName);
            utilityTabButton.RemoveFromClassList(selectedTabClassName);
            rootVisualElement.Q<GroupBox>("InformationContent").style.display = DisplayStyle.Flex;
            rootVisualElement.Q<GroupBox>("SettingsContent").style.display = DisplayStyle.None;
            rootVisualElement.Q<GroupBox>("CreateFontContent").style.display = DisplayStyle.None;
            rootVisualElement.Q<GroupBox>("UtilityContent").style.display = DisplayStyle.None;
        }

        private void OpenCreateFontTab()
        {
            createFontTabButton.AddToClassList(selectedTabClassName);
            settingsTabButton.RemoveFromClassList(selectedTabClassName);
            informationTabButton.RemoveFromClassList(selectedTabClassName);
            utilityTabButton.RemoveFromClassList(selectedTabClassName);
            rootVisualElement.Q<GroupBox>("CreateFontContent").style.display = DisplayStyle.Flex;
            rootVisualElement.Q<GroupBox>("SettingsContent").style.display = DisplayStyle.None;
            rootVisualElement.Q<GroupBox>("InformationContent").style.display = DisplayStyle.None;
            rootVisualElement.Q<GroupBox>("UtilityContent").style.display = DisplayStyle.None;
        }

        private void OpenSettingsTab()
        {
            settingsTabButton.AddToClassList(selectedTabClassName);
            informationTabButton.RemoveFromClassList(selectedTabClassName);
            utilityTabButton.RemoveFromClassList(selectedTabClassName);
            createFontTabButton.RemoveFromClassList(selectedTabClassName);
            rootVisualElement.Q<GroupBox>("SettingsContent").style.display = DisplayStyle.Flex;
            rootVisualElement.Q<GroupBox>("UtilityContent").style.display = DisplayStyle.None;
            rootVisualElement.Q<GroupBox>("InformationContent").style.display = DisplayStyle.None;
            rootVisualElement.Q<GroupBox>("CreateFontContent").style.display = DisplayStyle.None;
        }

        private void OpenUtilityTab()
        {
            utilityTabButton.AddToClassList(selectedTabClassName);
            informationTabButton.RemoveFromClassList(selectedTabClassName);
            settingsTabButton.RemoveFromClassList(selectedTabClassName);
            createFontTabButton.RemoveFromClassList(selectedTabClassName);
            rootVisualElement.Q<GroupBox>("UtilityContent").style.display = DisplayStyle.Flex;
            rootVisualElement.Q<GroupBox>("SettingsContent").style.display = DisplayStyle.None;
            rootVisualElement.Q<GroupBox>("InformationContent").style.display = DisplayStyle.None;
            rootVisualElement.Q<GroupBox>("CreateFontContent").style.display = DisplayStyle.None;
        }

        #endregion Tabs

        #region Information Tab

        private void SetupInformationTab()
        {
            string changeLogLink = "https://ferdowsur.gitbook.io/modular-3d-text/history/version-history";
            string forumLink = "https://forum.unity.com/threads/modular-3d-text-complete-ingame-ui-sytem.821931/";
            string reviewsLink = "https://assetstore.unity.com/packages/3d/gui/modular-3d-text-in-game-3d-ui-system-247241?aid=1011ljxWe#reviews";
            string supportEmail = "ferdowsurasif@gmail.com";

            GroupBox informationHolder = rootVisualElement.Q<GroupBox>("InformationContent");
            SetupURLButton(informationHolder, "DocumentationButton", documentationLink);
            SetupURLButton(informationHolder, "ChangeLogButton", changeLogLink);
            SetupURLButton(informationHolder, "ForumButton", forumLink);
            SetupURLButton(informationHolder, "ReviewButton", reviewsLink);
            SetupURLButton(informationHolder, "SendEmailButton", "mailto:" + supportEmail + "?subject=Modular 3D Text Support");

            informationHolder.Q<UnityEngine.UIElements.Button>("CopyEmailButton").clicked += () =>
            {
                GUIUtility.systemCopyBuffer = supportEmail;
                this.ShowNotification(new GUIContent("Email copied"), 1);
            };
        }

        #endregion Information Tab

        #region Settings Tab

        private void SetupSettingsTab()
        {
            GroupBox settingsContent = rootHolder.Q<GroupBox>("SettingsContent");

            SetupInputSetting(settingsContent);

            SetupTextDefaultSettings(settingsContent);
            SetupButtonDefaultSettings(settingsContent);
            SetupListDefaultSettings(settingsContent);
        }

        private void SetupTextDefaultSettings(GroupBox settingsContent)
        {
            Foldout foldout = settingsContent.Q<Foldout>("TextSettingsFoldout");

            ObjectField textDefaultFontField = foldout.Q<ObjectField>("TextDefaultFontField");
            textDefaultFontField.BindProperty(soTarget.FindProperty(nameof(AssetSettings.defaultFont)));

            var applyDefaultTextFontToSceneButton = foldout.Q<UnityEngine.UIElements.Button>("ApplyDefaultTextFontToSceneButton");
            applyDefaultTextFontToSceneButton.clicked += () => { ApplyDefaultFontToScene(); };

            Vector3Field textDefaultSizeField = foldout.Q<Vector3Field>("TextDefaultSizeField");
            textDefaultSizeField.BindProperty(soTarget.FindProperty(nameof(AssetSettings.defaultTextSize)));

            var applyDefaultTextSizeToSceneButton = foldout.Q<UnityEngine.UIElements.Button>("ApplyDefaultTextSizeToSceneButton");
            applyDefaultTextSizeToSceneButton.clicked += () => { ApplyDefaultTextSizeToScene(); };

            materialPreviewManager.Hook(foldout.Q<TemplateContainer>("TextMaterialPreviewField"), soTarget.FindProperty(nameof(AssetSettings.defaultTextMaterial)), "Text Material", Settings.defaultTextMaterial);

            var applyDefaultTextMaterialToSceneButton = foldout.Q<UnityEngine.UIElements.Button>("ApplyDefaultTextMaterialToSceneButton");
            applyDefaultTextMaterialToSceneButton.clicked += () => { ApplyDefaultTextMaterialToScene(); };
        }

        #region Button

        private void SetupButtonDefaultSettings(GroupBox settingsContent)
        {
            Foldout foldout = settingsContent.Q<Foldout>("ButtonSettingsFoldout");

            SetupButtonNormalStyle(foldout);
            SetupButtonSelectedStyle(foldout);
            SetupButtonPressedStyle(foldout);
            SetupButtonDisabledStyle(foldout);
        }

        private void SetupButtonNormalStyle(Foldout foldout)
        {
            GroupBox groupBox = foldout.Q<GroupBox>("NormalStyleGroupBox");

            UnityEvent applyTextSizeEvent = new UnityEvent();
            applyTextSizeEvent.AddListener(ApplyDefaultButtonNormalTextSizeToScene);

            UnityEvent applyTextMaterialEvent = new UnityEvent();
            applyTextMaterialEvent.AddListener(ApplyDefaultButtonNormalTextMaterialToScene);

            UnityEvent applyBackgroundMaterialEvent = new UnityEvent();
            applyBackgroundMaterialEvent.AddListener(ApplyDefaultButtonNormalBackgroundMaterialToScene);

            SetupDefaultSettingForCommonStylePattern
                (
                groupBox,
                nameof(AssetSettings.defaultButtonNormalTextSize), applyTextSizeEvent,
                nameof(AssetSettings.defaultButtonNormalTextMaterial), Settings.defaultButtonNormalTextMaterial,
                nameof(AssetSettings.defaultButtonNormalBackgroundMaterial), Settings.defaultButtonNormalBackgroundMaterial,
                applyTextMaterialEvent, applyBackgroundMaterialEvent
                );
        }

        private void SetupButtonSelectedStyle(Foldout foldout)
        {
            GroupBox groupBox = foldout.Q<GroupBox>("SelectedStyleGroupBox");

            UnityEvent applyTextSizeEvent = new UnityEvent();
            applyTextSizeEvent.AddListener(ApplyDefaultButtonSelectedTextSizeToScene);

            UnityEvent applyTextMaterialEvent = new UnityEvent();
            applyTextMaterialEvent.AddListener(ApplyDefaultButtonSelectedTextMaterialToScene);

            UnityEvent applyBackgroundMaterialEvent = new UnityEvent();
            applyBackgroundMaterialEvent.AddListener(ApplyDefaultButtonSelectedBackgroundMaterialToScene);

            SetupDefaultSettingForCommonStylePattern
                (
                groupBox,
                nameof(AssetSettings.defaultButtonSelectedTextSize), applyTextSizeEvent,
                nameof(AssetSettings.defaultButtonSelectedTextMaterial), Settings.defaultButtonSelectedTextMaterial,
                nameof(AssetSettings.defaultButtonSelectedBackgroundMaterial), Settings.defaultButtonSelectedBackgroundMaterial,
                applyTextMaterialEvent, applyBackgroundMaterialEvent
                );
        }

        private void SetupButtonPressedStyle(Foldout foldout)
        {
            GroupBox groupBox = foldout.Q<GroupBox>("PressedStyleGroupBox");

            UnityEvent applyTextSizeEvent = new UnityEvent();
            applyTextSizeEvent.AddListener(ApplyDefaultButtonPressedTextSizeToScene);

            UnityEvent applyTextMaterialEvent = new UnityEvent();
            applyTextMaterialEvent.AddListener(ApplyDefaultButtonPressedTextMaterialToScene);

            UnityEvent applyBackgroundMaterialEvent = new UnityEvent();
            applyBackgroundMaterialEvent.AddListener(ApplyDefaultButtonPressedBackgroundMaterialToScene);

            SetupDefaultSettingForCommonStylePattern
                (
                groupBox,
                nameof(AssetSettings.defaultButtonPressedTextSize), applyTextSizeEvent,
                nameof(AssetSettings.defaultButtonPressedTextMaterial), Settings.defaultButtonPressedTextMaterial,
                nameof(AssetSettings.defaultButtonPressedBackgroundMaterial), Settings.defaultButtonPressedBackgroundMaterial,
                applyTextMaterialEvent, applyBackgroundMaterialEvent
                );
        }

        private void SetupButtonDisabledStyle(Foldout foldout)
        {
            GroupBox groupBox = foldout.Q<GroupBox>("DisabledStyleGroupBox");

            UnityEvent applyTextSizeEvent = new UnityEvent();
            applyTextSizeEvent.AddListener(ApplyDefaultButtonDisabledTextSizeToScene);

            UnityEvent applyTextMaterialEvent = new UnityEvent();
            applyTextMaterialEvent.AddListener(ApplyDefaultButtonDisabledTextMaterialToScene);

            UnityEvent applyBackgroundMaterialEvent = new UnityEvent();
            applyBackgroundMaterialEvent.AddListener(ApplyDefaultButtonDisabledBackgroundMaterialToScene);

            SetupDefaultSettingForCommonStylePattern
                (
                groupBox,
                nameof(AssetSettings.defaultButtonDisabledTextSize), applyTextSizeEvent,
                nameof(AssetSettings.defaultButtonDisabledTextMaterial), Settings.defaultButtonDisabledTextMaterial,
                nameof(AssetSettings.defaultButtonDisabledBackgroundMaterial), Settings.defaultButtonDisabledBackgroundMaterial,
                applyTextMaterialEvent, applyBackgroundMaterialEvent
                );
        }

        #endregion Button

        #region List

        private void SetupListDefaultSettings(GroupBox settingsContent)
        {
            Foldout foldout = settingsContent.Q<Foldout>("ListSettingsFoldout");

            SetupListNormalStyle(foldout);
            SetupListSelectedStyle(foldout);
            SetupListPressedStyle(foldout);
            SetupListDisabledStyle(foldout);
        }

        private void SetupListNormalStyle(Foldout foldout)
        {
            GroupBox groupBox = foldout.Q<GroupBox>("NormalStyleGroupBox");

            UnityEvent applyTextSizeEvent = new UnityEvent();
            applyTextSizeEvent.AddListener(ApplyDefaultListNormalTextSizeToScene);

            UnityEvent applyTextMaterialEvent = new UnityEvent();
            applyTextMaterialEvent.AddListener(ApplyDefaultListNormalTextMaterialToScene);

            UnityEvent applyBackgroundMaterialEvent = new UnityEvent();
            applyBackgroundMaterialEvent.AddListener(ApplyDefaultListNormalBackgroundMaterialToScene);

            SetupDefaultSettingForCommonStylePattern
                (
                groupBox,
                nameof(AssetSettings.defaultListNormalTextSize), applyTextSizeEvent,
                nameof(AssetSettings.defaultListNormalTextMaterial), Settings.defaultListNormalTextMaterial,
                nameof(AssetSettings.defaultListNormalBackgroundMaterial), Settings.defaultListNormalBackgroundMaterial,
                applyTextMaterialEvent, applyBackgroundMaterialEvent
                );
        }

        private void SetupListSelectedStyle(Foldout foldout)
        {
            GroupBox groupBox = foldout.Q<GroupBox>("SelectedStyleGroupBox");

            UnityEvent applyTextSizeEvent = new UnityEvent();
            applyTextSizeEvent.AddListener(ApplyDefaultListSelectedTextSizeToScene);

            UnityEvent applyTextMaterialEvent = new UnityEvent();
            applyTextMaterialEvent.AddListener(ApplyDefaultListSelectedTextMaterialToScene);

            UnityEvent applyBackgroundMaterialEvent = new UnityEvent();
            applyBackgroundMaterialEvent.AddListener(ApplyDefaultListSelectedBackgroundMaterialToScene);

            SetupDefaultSettingForCommonStylePattern
                (
                groupBox,
                nameof(AssetSettings.defaultListSelectedTextSize), applyTextSizeEvent,
                nameof(AssetSettings.defaultListSelectedTextMaterial), Settings.defaultListSelectedTextMaterial,
                nameof(AssetSettings.defaultListSelectedBackgroundMaterial), Settings.defaultListSelectedBackgroundMaterial,
                applyTextMaterialEvent, applyBackgroundMaterialEvent
                );
        }

        private void SetupListPressedStyle(Foldout foldout)
        {
            GroupBox groupBox = foldout.Q<GroupBox>("PressedStyleGroupBox");

            UnityEvent applyTextSizeEvent = new UnityEvent();
            applyTextSizeEvent.AddListener(ApplyDefaultListPressedTextSizeToScene);

            UnityEvent applyTextMaterialEvent = new UnityEvent();
            applyTextMaterialEvent.AddListener(ApplyDefaultListPressedTextMaterialToScene);

            UnityEvent applyBackgroundMaterialEvent = new UnityEvent();
            applyBackgroundMaterialEvent.AddListener(ApplyDefaultListPressedBackgroundMaterialToScene);

            SetupDefaultSettingForCommonStylePattern
                (
                groupBox,
                nameof(AssetSettings.defaultListPressedTextSize), applyTextSizeEvent,
                nameof(AssetSettings.defaultListPressedTextMaterial), Settings.defaultListPressedTextMaterial,
                nameof(AssetSettings.defaultListPressedBackgroundMaterial), Settings.defaultListPressedBackgroundMaterial,
                applyTextMaterialEvent, applyBackgroundMaterialEvent
                );
        }

        private void SetupListDisabledStyle(Foldout foldout)
        {
            GroupBox groupBox = foldout.Q<GroupBox>("DisabledStyleGroupBox");

            UnityEvent applyTextSizeEvent = new UnityEvent();
            applyTextSizeEvent.AddListener(ApplyDefaultListDisabledTextSizeToScene);

            UnityEvent applyTextMaterialEvent = new UnityEvent();
            applyTextMaterialEvent.AddListener(ApplyDefaultListDisabledTextMaterialToScene);

            UnityEvent applyBackgroundMaterialEvent = new UnityEvent();
            applyBackgroundMaterialEvent.AddListener(ApplyDefaultListDisabledBackgroundMaterialToScene);

            SetupDefaultSettingForCommonStylePattern
                (
                groupBox,
                nameof(AssetSettings.defaultListDisabledTextSize), applyTextSizeEvent,
                nameof(AssetSettings.defaultListDisabledTextMaterial), Settings.defaultListDisabledTextMaterial,
                nameof(AssetSettings.defaultListDisabledBackgroundMaterial), Settings.defaultListDisabledBackgroundMaterial,
                applyTextMaterialEvent, applyBackgroundMaterialEvent
                );
        }

        #endregion List

        /// <summary>
        /// Couldn't decide what to name this method.
        /// This basically just hooks up anything with two preview and a text size method
        /// </summary>
        private void SetupDefaultSettingForCommonStylePattern
            (
            GroupBox groupBox,
            string textSizePropertyName, UnityEvent applyTextSizeEvent,
            string textMaterialPropertyName, Material textMaterial,
            string backgroundMaterialPropertyName, Material backgroundMaterial,
            UnityEvent applyTextMaterialEvent, UnityEvent applyBackgroundMaterialEvent
            )
        {
            Vector3Field textSize = groupBox.Q<Vector3Field>("TextSize");
            textSize.BindProperty(soTarget.FindProperty(textSizePropertyName));

            var applyTextSizeButton = groupBox.Q<UnityEngine.UIElements.Button>("ApplyTextSizeButton");
            applyTextSizeButton.clicked += () => { applyTextSizeEvent.Invoke(); };

            materialPreviewManager.Hook(groupBox.Q<TemplateContainer>("TextMaterialPreviewField"), soTarget.FindProperty(textMaterialPropertyName), "Text Material", textMaterial);

            var applyTextMaterialToSceneButton = groupBox.Q<UnityEngine.UIElements.Button>("ApplyTextMaterialToSceneButton");
            applyTextMaterialToSceneButton.clicked += () => { applyTextMaterialEvent.Invoke(); };

            materialPreviewManager.Hook(groupBox.Q<TemplateContainer>("BackgroundMaterialPreviewField"), soTarget.FindProperty(backgroundMaterialPropertyName), "Background Material", backgroundMaterial);

            var applyBackgroundMaterialToSceneButton = groupBox.Q<UnityEngine.UIElements.Button>("ApplyBackgroundMaterialToSceneButton");
            applyBackgroundMaterialToSceneButton.clicked += () => { applyBackgroundMaterialEvent.Invoke(); };
        }

        private void SetupInputSetting(GroupBox settingsContent)
        {
            var autoGenerateSceneInputSystemToggle = settingsContent.Q<UnityEngine.UIElements.Toggle>("AutoGenerateSceneInputSystemToggle");
            autoGenerateSceneInputSystemToggle.BindProperty(soTarget.FindProperty(nameof(AssetSettings.autoCreateSceneInputSystem)));

            //XR Toolkit compatibility
            var dontAutoCreateRaycasterOrButtonIfVRtoolkitExists = settingsContent.Q<UnityEngine.UIElements.Toggle>("DontAutoCreateRaycasterOrButtonIfVRtoolkitExists");
            var xRToolkitGlobalInputControllerForModularTextAsset = Type.GetType("TinyGiantStudio.Text.XRToolkitEditorSetup");
            if (xRToolkitGlobalInputControllerForModularTextAsset != null)
            {
                dontAutoCreateRaycasterOrButtonIfVRtoolkitExists.style.display = DisplayStyle.Flex;
                dontAutoCreateRaycasterOrButtonIfVRtoolkitExists.BindProperty(soTarget.FindProperty(nameof(AssetSettings.dontAutoCreateRaycasterOrButtonIfVRtoolkitExists)));
            }
            else
            {
                dontAutoCreateRaycasterOrButtonIfVRtoolkitExists.style.display = DisplayStyle.None;
            }

            GroupBox inputActionAssetGroup = settingsContent.Q<GroupBox>("InputActionAssetGroup");
#if ENABLE_INPUT_SYSTEM
            ObjectField inputActionAssetField = settingsContent.Q<ObjectField>("InputActionAssetField");
            inputActionAssetField.BindProperty(soTarget.FindProperty("_inputActionAsset"));
            var applyInputActionAssetButton = settingsContent.Q<UnityEngine.UIElements.Button>("ApplyInputActionAssetButton");
            applyInputActionAssetButton.clicked += () => { ApplyInputActionAssetToScene(); };
#else
            inputActionAssetGroup.style.display = DisplayStyle.None;
#endif
        }

        #region Apply Settings to Scene methods

        #region Input Settings

#if ENABLE_INPUT_SYSTEM
        private void ApplyInputActionAssetToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.InputActionAsset.name + "' to every player controller active object in the scene?" +
               "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                PlayerInput[] playerInputs = UnityEngine.Object.FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
#else
                PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
#endif
                for (int i = 0; i < playerInputs.Length; i++)
                {
                    playerInputs[i].actions = Settings.InputActionAsset;
                    EditorUtility.SetDirty(playerInputs[i]);
                }
            }
        }
#endif

        #endregion Input Settings

        #region Default Settings

        private bool ShouldItApplyTextPreference(Modular3DText text)
        {
            if (text.transform.parent)
            {
                var button = text.transform.parent.GetComponent<Button>();
                if (button)
                {
                    var applyNormalStyle = button.ApplyNormalStyle();
                    if (applyNormalStyle.Item1 || applyNormalStyle.Item2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #region Text

        private void ApplyDefaultFontToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultFont.name + "' font to every active 3D text in the scene?\n" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var text in UnityEngine.Object.FindObjectsByType<Modular3DText>(FindObjectsSortMode.None))
#else
                foreach (var text in FindObjectsOfType<Modular3DText>())
#endif
                {
                    text.Font = Settings.defaultFont;
                    EditorUtility.SetDirty(text);
                }
            }
        }

        private void ApplyDefaultTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultTextSize + "' font size to every active text in the scene?\n" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var text in UnityEngine.Object.FindObjectsByType<Modular3DText>(FindObjectsSortMode.None))
#else
                foreach (var text in FindObjectsOfType<Modular3DText>())
#endif
                {
                    if (ShouldItApplyTextPreference(text))
                        continue;

                    text.FontSize = Settings.defaultTextSize;
                    EditorUtility.SetDirty(text);
                }
            }
        }

        private void ApplyDefaultTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultTextMaterial.name + "' material to every active text in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var text in UnityEngine.Object.FindObjectsByType<Modular3DText>(FindObjectsSortMode.None))
#else
                foreach (var text in FindObjectsOfType<Modular3DText>())
#endif
                {
                    if (ShouldItApplyTextPreference(text))
                        continue;

                    text.Material = Settings.defaultTextMaterial;
                    EditorUtility.SetDirty(text);
                }
            }
        }

        #endregion Text

        #region Button

        private void ApplyDefaultButtonNormalTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultButtonNormalTextSize + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.NormalTextSize = Settings.defaultButtonNormalTextSize;
                    if (!Application.isPlaying)
                        button.UnselectedButtonVisualUpdate();
                    EditorUtility.SetDirty(button);
                }
            }
        }

        private void ApplyDefaultButtonNormalTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultButtonNormalTextMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.NormalTextMaterial = Settings.defaultButtonNormalTextMaterial;
                    if (!Application.isPlaying)
                        button.UnselectedButtonVisualUpdate();
                    EditorUtility.SetDirty(button);
                }
            }
        }

        private void ApplyDefaultButtonNormalBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultButtonNormalBackgroundMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.NormalBackgroundMaterial = Settings.defaultButtonNormalBackgroundMaterial;
                    if (!Application.isPlaying)
                        button.UnselectedButtonVisualUpdate();
                    EditorUtility.SetDirty(button);
                }
            }
        }

        private void ApplyDefaultButtonSelectedTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultButtonSelectedTextSize + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.SelectedTextSize = Settings.defaultButtonSelectedTextSize;
                    if (!Application.isPlaying)
                        button.UnselectedButtonVisualUpdate();
                    EditorUtility.SetDirty(button);
                }
            }
        }

        private void ApplyDefaultButtonSelectedTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultButtonSelectedTextMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.SelectedTextMaterial = Settings.defaultButtonSelectedTextMaterial;
                    EditorUtility.SetDirty(button);
                }
            }
        }

        private void ApplyDefaultButtonSelectedBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultButtonSelectedBackgroundMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.SelectedBackgroundMaterial = Settings.defaultButtonSelectedBackgroundMaterial;
                    EditorUtility.SetDirty(button);
                }
            }
        }

        private void ApplyDefaultButtonPressedTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultButtonPressedTextSize + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.PressedTextSize = Settings.defaultButtonPressedTextSize;
                    EditorUtility.SetDirty(button);
                }
            }
        }

        private void ApplyDefaultButtonPressedTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultButtonPressedTextMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.PressedTextMaterial = Settings.defaultButtonPressedTextMaterial;
                    EditorUtility.SetDirty(button);
                }
            }
        }

        private void ApplyDefaultButtonPressedBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultButtonPressedBackgroundMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.PressedBackgroundMaterial = Settings.defaultButtonPressedBackgroundMaterial;
                    EditorUtility.SetDirty(button);
                }
            }
        }

        private void ApplyDefaultButtonDisabledTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultButtonDisabledTextSize + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.DisabledTextSize = Settings.defaultButtonDisabledTextSize;
                    EditorUtility.SetDirty(button);
                }
            }
        }

        private void ApplyDefaultButtonDisabledTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultButtonDisabledTextMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.DisabledTextMaterial = Settings.defaultButtonDisabledTextMaterial;
                    EditorUtility.SetDirty(button);
                }
            }
        }

        private void ApplyDefaultButtonDisabledBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultButtonDisabledBackgroundMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var button in UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
#else
                foreach (var button in FindObjectsOfType<Button>())
#endif
                {
                    button.DisabledBackgroundMaterial = Settings.defaultButtonDisabledBackgroundMaterial;
                    EditorUtility.SetDirty(button);
                }
            }
        }

        #endregion Button

        #region List

        private void ApplyDefaultListNormalTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultListNormalTextSize + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in UnityEngine.Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.NormalTextSize = Settings.defaultListNormalTextSize;
                    EditorUtility.SetDirty(list);
                }
            }
        }

        private void ApplyDefaultListNormalTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultButtonNormalTextMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in UnityEngine.Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.NormalTextMaterial = Settings.defaultListNormalTextMaterial;
                    EditorUtility.SetDirty(list);
                }
            }
        }

        private void ApplyDefaultListNormalBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultListNormalBackgroundMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in UnityEngine.Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.NormalBackgroundMaterial = Settings.defaultListNormalBackgroundMaterial;
                    EditorUtility.SetDirty(list);
                }
            }
        }

        private void ApplyDefaultListSelectedTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultListSelectedTextSize + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in UnityEngine.Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.SelectedTextSize = Settings.defaultListSelectedTextSize;
                    EditorUtility.SetDirty(list);
                }
            }
        }

        private void ApplyDefaultListSelectedTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultListSelectedTextMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in UnityEngine.Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.SelectedTextMaterial = Settings.defaultListSelectedTextMaterial;
                    EditorUtility.SetDirty(list);
                }
            }
        }

        private void ApplyDefaultListSelectedBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultListSelectedBackgroundMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in UnityEngine.Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.SelectedBackgroundMaterial = Settings.defaultListSelectedBackgroundMaterial;
                    EditorUtility.SetDirty(list);
                }
            }
        }

        private void ApplyDefaultListPressedTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultListPressedTextSize + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in UnityEngine.Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.PressedTextSize = Settings.defaultListPressedTextSize;
                    EditorUtility.SetDirty(list);
                }
            }
        }

        private void ApplyDefaultListPressedTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultListPressedTextMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in UnityEngine.Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.PressedTextMaterial = Settings.defaultListPressedTextMaterial;
                    EditorUtility.SetDirty(list);
                }
            }
        }

        private void ApplyDefaultListPressedBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultListPressedBackgroundMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in UnityEngine.Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.PressedBackgroundMaterial = Settings.defaultListPressedBackgroundMaterial;
                    EditorUtility.SetDirty(list);
                }
            }
        }

        private void ApplyDefaultListDisabledTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultListDisabledTextSize + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in UnityEngine.Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.DisabledTextSize = Settings.defaultListDisabledTextSize;
                    EditorUtility.SetDirty(list);
                }
            }
        }

        private void ApplyDefaultListDisabledTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultListDisabledTextMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in UnityEngine.Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.DisabledTextMaterial = Settings.defaultListDisabledTextMaterial;
                    EditorUtility.SetDirty(list);
                }
            }
        }

        private void ApplyDefaultListDisabledBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + Settings.defaultButtonDisabledBackgroundMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
#if UNITY_2023_1_OR_NEWER
                foreach (var list in UnityEngine.Object.FindObjectsByType<List>(FindObjectsSortMode.None))
#else
                foreach (var list in FindObjectsOfType<List>())
#endif
                {
                    list.DisabledBackgroundMaterial = Settings.defaultButtonDisabledBackgroundMaterial;
                    EditorUtility.SetDirty(list);
                }
            }
        }

        #endregion List

        #endregion Default Settings

        #endregion Apply Settings to Scene methods

        #endregion Settings Tab

        #region Font Creation Tab

        private void SetupFontCreationTab()
        {
            createFontContent = rootVisualElement.Q<GroupBox>("CreateFontContent");

            previewFoldout = createFontContent.Q<Foldout>("PreviewFoldout");
            previewScrollview = createFontContent.Q<ScrollView>("PreviewScrollView");

            var selectFileButton = createFontContent.Q<UnityEngine.UIElements.Button>("SelectFileButton");
            selectFileButton.clicked += () => GetTTFFile();

            createFontButton = createFontContent.Q<UnityEngine.UIElements.Button>("CreateFontButton");
            createFontButton.SetEnabled(false);
            createFontButton.clicked += () => CreateFont();

            var resetMeshSettingsButton = createFontContent.Q<UnityEngine.UIElements.Button>("ResetMeshSettingsButton");
            resetMeshSettingsButton.clicked += () => Settings.ResetFontCreationMeshSettings();

            var resetPrebuiltCharactersButton = createFontContent.Q<UnityEngine.UIElements.Button>("ResetPrebuiltCharactersButton");
            resetPrebuiltCharactersButton.clicked += () => Settings.ResetFontCreationPrebuiltSettings();

            var LogPrebuiltCharactersButton = createFontContent.Q<UnityEngine.UIElements.Button>("LogPrebuiltCharactersButton");
            LogPrebuiltCharactersButton.clicked += () => TestCharacterList();

            var KerningDocumentation = createFontContent.Q<UnityEngine.UIElements.Button>("KerningDocumentation");
            KerningDocumentation.clicked += () => Application.OpenURL(kerningDocumentationDocURL);

            var CommonIssuesButton = createFontContent.Q<UnityEngine.UIElements.Button>("CommonIssuesButton");
            CommonIssuesButton.clicked += () => Application.OpenURL(fontCreationTroubleShootDocURL);

            NoFileSeletected();

            var styleEnumField = createFontContent.Q<EnumField>("StyleEnumField");
            styleEnumField.BindProperty(soTarget.FindProperty(nameof(AssetSettings.meshExportStyle)));

            var vartexDensityField = createFontContent.Q<IntegerField>("VertexDensityField");
            vartexDensityField.BindProperty(soTarget.FindProperty(nameof(AssetSettings.vertexDensity)));
            vartexDensityField.RegisterValueChangedCallback(evt =>
            {
                if (Settings.vertexDensity < 1) Settings.vertexDensity = 1;
                UpdateMeshSettingsNote();
                UpdatePreviews();
            });

            var smoothingAngleField = createFontContent.Q<IntegerField>("SmoothingAngleField");
            smoothingAngleField.BindProperty(soTarget.FindProperty(nameof(AssetSettings.smoothingAngle)));
            smoothingAngleField.RegisterValueChangedCallback(evt =>
            {
                if (Settings.smoothingAngle < 0) Settings.smoothingAngle = 0;
                UpdateMeshSettingsNote();
                UpdatePreviews();
            });

            var sizeXYField = createFontContent.Q<FloatField>("SizeXYField");
            sizeXYField.BindProperty(soTarget.FindProperty(nameof(AssetSettings.sizeXY)));
            sizeXYField.RegisterValueChangedCallback(evt =>
            {
                if (Settings.sizeXY <= 0) Settings.sizeXY = 1;
                UpdateMeshSettingsNote();
                UpdatePreviews();
            });

            var sizeZField = createFontContent.Q<FloatField>("SizeZField");
            sizeZField.BindProperty(soTarget.FindProperty(nameof(AssetSettings.sizeZ)));
            sizeZField.RegisterValueChangedCallback(evt =>
            {
                if (Settings.sizeZ <= 0) Settings.sizeZ = 1;
                UpdateMeshSettingsNote();
                UpdatePreviews();
            });

            var PreviewAmount = createFontContent.Q<IntegerField>("PreviewAmount");
            PreviewAmount.BindProperty(soTarget.FindProperty(nameof(AssetSettings.previewAmount)));
            PreviewAmount.RegisterValueChangedCallback(evt =>
            {
                previewCount = evt.newValue;
                UpdatePreviews();
            });
            previewCount = PreviewAmount.value;

            UpdateMeshSettingsNote();

            var charInputStyle = createFontContent.Q<EnumField>("CharInputStyle");
            charInputStyle.BindProperty(soTarget.FindProperty(nameof(AssetSettings.charInputStyle)));
            charInputStyle.RegisterValueChangedCallback(evt =>
            {
                UpdatePrebuiltCharacterFields();
            });
            var startPrebuiltCharacter = createFontContent.Q<TextField>("StartPrebuiltCharacter");
            startPrebuiltCharacter.BindProperty(soTarget.FindProperty(nameof(AssetSettings.startChar)));

            var endPrebuiltCharacter = createFontContent.Q<TextField>("EndPrebuiltCharacter");
            endPrebuiltCharacter.BindProperty(soTarget.FindProperty(nameof(AssetSettings.endChar)));

            var startUnicode = createFontContent.Q<TextField>("StartUnicode");
            startUnicode.BindProperty(soTarget.FindProperty(nameof(AssetSettings.startUnicode)));

            var endUnicode = createFontContent.Q<TextField>("EndUnicode");
            endUnicode.BindProperty(soTarget.FindProperty(nameof(AssetSettings.endUnicode)));

            var customCharacters = createFontContent.Q<TextField>("CustomCharacters");
            customCharacters.BindProperty(soTarget.FindProperty(nameof(AssetSettings.customCharacters)));

            var unicodeSequence = createFontContent.Q<TextField>("UnicodeSequence");
            unicodeSequence.BindProperty(soTarget.FindProperty(nameof(AssetSettings.unicodeSequence)));
            UpdatePrebuiltCharacterFields();
        }

        private void UpdateMeshSettingsNote()
        {
            TemplateContainer templateContainer = createFontContent.Q<TemplateContainer>("MeshSettingsNote");
            templateContainer.style.display = DisplayStyle.None;
            var headLine = templateContainer.Q<Label>("Headline");
            headLine.text = "";
            headLine.tooltip = "";
            var details = templateContainer.Q<Label>("Details");
            details.text = "";

            if (fileContent == null)
                return;
            if (fileContent.Length == 0)
                return;

            if (Settings.vertexDensity > 5)
            {
                templateContainer.style.display = DisplayStyle.Flex;
                string detailsText = "A high value can introduce a lot of problems including performance issue by creating too much vertices, intersecting faces etc." +
                    "\nThat's why, it is recommended to check the preview and use the lowest number that gives acceptable result." +
                    "\nThe default fonts are created with 1 density.";

                headLine.text += "Tip: Keep vertex density low.";
                headLine.tooltip += detailsText;
                details.text += detailsText;
            }
        }

        private void GetTTFFile()
        {
            FilePath = EditorUtility.OpenFilePanel("Select TTF font", "Assets", "ttf");

            //typeFace = new TypeFace(fileContent);
        }

        private void CreateFont()
        {
            EditorUtility.DisplayProgressBar("Creating font", "Preparing", 0 / 100);
            bool exportAsObj = ExportAs();
            List<char> listOfChar = GetCharacterList();

            GameObject fontHolderObject = new GameObject();

            EditorUtility.DisplayProgressBar("Creating font", "Creating pre-built meshes", 10 / 100);

            CharacterGenerator.GetFontObject(fontHolderObject, fileContent, listOfChar, Settings.sizeXY, Settings.sizeZ, Settings.vertexDensity, Settings.smoothingAngle, Settings.defaultTextMaterial);
            fontHolderObject.name = CharacterGenerator.typeFace.nameEntry;
            EditorUtility.DisplayProgressBar("Creating font", "Creating pre-built meshes", 10 / 100);

            if (fontHolderObject.transform.childCount > 0)
            {
                if (exportAsObj)
                {
                    MText_ObjExporter objExporter = new MText_ObjExporter();
                    string prefabPath = objExporter.DoExport(fontHolderObject, true);
                    if (string.IsNullOrEmpty(prefabPath))
                    {
                        Debug.Log("Object save failed");
                        EditorUtility.ClearProgressBar();
                        return;
                    }
                    MText_FontExporter fontExporter = new MText_FontExporter();
                    fontExporter.CreateFontFile(prefabPath, fontHolderObject.name, CharacterGenerator, fileContent);
                }
                else
                {
                    MText_MeshAssetExporter meshAssetExporter = new MText_MeshAssetExporter();
                    meshAssetExporter.DoExport(fontHolderObject);
                }
            }

            EditorUtility.ClearProgressBar();
            if (Application.isPlaying) Destroy(fontHolderObject);
            else DestroyImmediate(fontHolderObject);
        }

        #region TTF Data

        private void LoadFileContent()
        {
            if (!string.IsNullOrEmpty(FilePath))
                fileContent = File.ReadAllBytes(FilePath);
        }

        private void GetFontAsset()
        {
            if (FilePath == null) return;
            if (FilePath.Length == 0) return;

            if (FilePath.StartsWith(Application.dataPath))
            {
                string relativepath = "Assets" + FilePath.Substring(Application.dataPath.Length);
                try
                {
                    font = (UnityEngine.Font)AssetDatabase.LoadAssetAtPath(relativepath, typeof(UnityEngine.Font));
                }
                catch
                {
                }
            }
        }

        private void UpdateFontInformation()
        {
            if (FilePath == null)
            {
                HideFoldouts();
                return;
            }
            if (FilePath.Length == 0)
            {
                HideFoldouts();
                return;
            }

            CharacterGenerator.CreateTypeFace(fileContent);

            if (CharacterGenerator.typeFace == null)
            {
                HideFoldouts();
                return;
            }

            ShowFoldouts();

            createFontContent.Q<Label>("FontNameLabel").text = CharacterGenerator.typeFace.nameEntry;
            createFontContent.Q<Label>("FontLocationLabel").text = "File location : " + FilePath;
            if (CharacterGenerator.typeFace.kernTable != null)
                createFontContent.Q<Label>("FontKernCountLabel").text = "" + CharacterGenerator.typeFace.kernTable.Count;
            else
                createFontContent.Q<Label>("FontKernCountLabel").text = "0";
        }

        private void HideFoldouts()
        {
            rootHolder.Q<Foldout>("InformationFoldout").style.display = DisplayStyle.None;
            rootHolder.Q<Foldout>("MeshSettingsFoldout").style.display = DisplayStyle.None;
            rootHolder.Q<Foldout>("PrebuiltCharactersFoldout").style.display = DisplayStyle.None;
        }

        private void ShowFoldouts()
        {
            rootHolder.Q<Foldout>("InformationFoldout").style.display = DisplayStyle.Flex;
            rootHolder.Q<Foldout>("MeshSettingsFoldout").style.display = DisplayStyle.Flex;
            rootHolder.Q<Foldout>("PrebuiltCharactersFoldout").style.display = DisplayStyle.Flex;
        }

        #endregion TTF Data

        #region Export Settings

        private bool ExportAs()
        {
            return true;
        }

        #endregion Export Settings

        #region Preview

        private void CleanUpdatePreviews()
        {
            CleanupPreviews();

            UpdatePreviews();
        }

        private void UpdatePreviews()
        {
            if (previewCount < 1) previewCount = 1;
            else if (previewCount > 50) previewCount = 50;

            if (previews.Count > previewCount)
            {
                //for (int i = previews.Count - 1; i > previewCount - 1; i--)
                //{
                //    Debug.Log(i);
                //    //CleanUpPreview(previews[i]); //this crashes the editor
                //    //previews.Remove(previews[i]);
                //}
                CleanupPreviews();
                if (previewCount < 1) previewCount = 1;
            }

            if (previews.Count < previewCount)
            {
                for (int i = previews.Count; i < previewCount; i++)
                {
                    previews.Add(new Preview());
                }
            }

            for (int i = 0; i < previews.Count; i++)
            {
                UpdatePreview(previews[i]);
            }
        }

        /// <summary>
        /// This is the main method that handles preview
        /// </summary>
        /// <param name="preview"></param>
        private void UpdatePreview(Preview preview)
        {
            if (preview.previewContainer == null)
            {
                VisualElement previewHolder = new VisualElement();
                previewScrollview.Add(previewHolder);
                previewTemplate.CloneTree(previewHolder);

                preview.meshInformationLabel = previewHolder.Q<Label>("MeshInformationLabel");
                preview.previewContainer = previewHolder.Q<IMGUIContainer>("PreviewContainer");
                preview.previewSettingsContainer = previewHolder.Q<IMGUIContainer>("PreviewSettingsContainer");
                preview.previewCharacterField = previewHolder.Q<TextField>("PreviewCharacterField");
                preview.previewCharacterField.RegisterValueChangedCallback(evt =>
                {
                    UpdatePreview(preview);
                });
            }

            char c;

            if (preview.previewCharacterField.text == null)
                c = 'O';
            else if (preview.previewCharacterField.text.Length <= 0)
                c = 'A';
            else
                c = preview.previewCharacterField.text.ToCharArray()[0];

            preview.mesh = CharacterGenerator.GetMesh(fileContent, Settings.sizeXY, Settings.sizeZ, Settings.smoothingAngle, 0, Settings.vertexDensity, c);
            if (preview.mesh == null) return;

            //preview.meshPreview?.Dispose();
            if (preview.meshPreview == null)
                preview.meshPreview = new MeshPreview(preview.mesh);
            else
                preview.meshPreview.mesh = (preview.mesh);

            preview.meshInformationLabel.text = MeshPreview.GetInfoString(preview.mesh);

            preview.previewContainer.onGUIHandler = null;
            preview.previewContainer.onGUIHandler += () =>
            {
                preview.meshPreview.OnPreviewGUI(preview.previewContainer.contentRect, null);
            };
            preview.previewSettingsContainer.onGUIHandler = null;
            preview.previewSettingsContainer.onGUIHandler += () =>
            {
                EditorGUILayout.BeginHorizontal();
                preview.meshPreview.OnPreviewSettings();
                EditorGUILayout.EndHorizontal();
            };
        }

        private void CleanupPreviews()
        {
            for (int i = 0; i < previews.Count; i++)
            {
                CleanUpPreview(previews[i]);
            }

            previews.Clear();
            if (previewScrollview != null)
                previewScrollview.Clear();
        }

        private void CleanUpPreview(Preview preview)
        {
            preview.meshPreview?.Dispose();
            preview.previewContainer?.Dispose();
            preview.previewSettingsContainer?.Dispose();

            try
            {
                if (preview.mesh)
                {
                    if (!Application.isPlaying)
                        DestroyImmediate(preview.mesh);
                    else
                        Destroy(preview.mesh);
                }
            }
            catch { }
        }

        #endregion Preview

        #region Prebuilt characters

        private void UpdatePrebuiltCharacterFields()
        {
            var characterRangeSelection = createFontContent.Q<GroupBox>("CharacterRangeSelection");
            characterRangeSelection.style.display = DisplayStyle.None;

            var unicodeRangeSelection = createFontContent.Q<GroupBox>("UnicodeRangeSelection");
            unicodeRangeSelection.style.display = DisplayStyle.None;

            var customCharacters = createFontContent.Q<TextField>("CustomCharacters");
            customCharacters.style.display = DisplayStyle.None;

            var unicodeSequence = createFontContent.Q<TextField>("UnicodeSequence");
            unicodeSequence.style.display = DisplayStyle.None;

            switch (Settings.charInputStyle)
            {
                case AssetSettings.CharInputStyle.CharacterRange:
                    characterRangeSelection.style.display = DisplayStyle.Flex;
                    break;

                case AssetSettings.CharInputStyle.UnicodeRange:
                    unicodeRangeSelection.style.display = DisplayStyle.Flex;
                    break;

                case AssetSettings.CharInputStyle.CustomCharacters:
                    customCharacters.style.display = DisplayStyle.Flex;
                    break;

                case AssetSettings.CharInputStyle.UnicodeSequence:
                    unicodeSequence.style.display = DisplayStyle.Flex;
                    break;

                default:
                    break;
            }
        }

        private void TestCharacterList()
        {
            List<char> myCharacters = GetCharacterList();
            Debug.Log("Character count: " + myCharacters.Count);
            for (int i = 0; i < myCharacters.Count; i++)
            {
                Debug.Log(myCharacters[i]);
            }
        }

        private List<char> GetCharacterList()
        {
            List<char> myChars = new List<char>();

            if (Settings.charInputStyle == AssetSettings.CharInputStyle.CharacterRange)
            {
                myChars = GetCharacterFromRange(Settings.startChar, Settings.endChar);
            }
            else if (Settings.charInputStyle == AssetSettings.CharInputStyle.UnicodeRange)
            {
                char start = ConvertCharFromUnicode(Settings.startUnicode);
                char end = ConvertCharFromUnicode(Settings.endUnicode);

                myChars = GetCharacterFromRange(start, end);
            }
            else if (Settings.charInputStyle == AssetSettings.CharInputStyle.CustomCharacters)
            {
                myChars = Settings.customCharacters.ToCharArray().ToList();
            }
            else if (Settings.charInputStyle == AssetSettings.CharInputStyle.UnicodeSequence)
            {
                NewFontCharacterRange characterRange = new NewFontCharacterRange();
                myChars = characterRange.RetrieveCharacterListFromUnicodeSequence(Settings.unicodeSequence);
            }
            myChars = myChars.Distinct().ToList();

            return myChars;
        }

        private List<char> GetCharacterFromRange(char start, char end)
        {
            NewFontCharacterRange characterRange = new NewFontCharacterRange();
            List<char> characterList = characterRange.RetrieveCharactersList(start, end);
            return characterList;
        }

        private char ConvertCharFromUnicode(string unicode)
        {
            string s = System.Text.RegularExpressions.Regex.Unescape("\\u" + unicode);
            s.ToCharArray();
            if (s.Length > 0)
                return s[0];
            else
                return ' ';
        }

        #endregion Prebuilt characters

        private void Kerning()
        {
        }

        #endregion Font Creation Tab

        #region Utility Tab

        private void SetupUtilityTab()
        {
            GroupBox informationHolder = rootVisualElement.Q<GroupBox>("UtilityContent");
            informationHolder.Q<UnityEngine.UIElements.Button>("SetupAssemblyButton").clicked += () => SetupAssemblyDefinitionFile();
        }

        private void SetupAssemblyDefinitionFile()
        {
            string path = "Assets/Tiny Giant Studio/Modular 3D Text/Setup Files/Assembly Definition.unitypackage";
            //#if ENABLE_INPUT_SYSTEM
            //            path = "Assets/Tiny Giant Studio/Modular 3D Text/Setup Files/Assembly Definition with New Input System.unitypackage";
            //#endif

            if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path))) //package do not exist.
            {
                string msg = "Can't find package in the correct folder. If it has been moved, please manually install it from:\n" + path + "\nIf it has been removed, please add redownload it from the asset store.";
                Debug.Log(msg);
                EditorUtility.DisplayDialog("Failed!", msg, "OK");
            }
            else
            {
                AssetDatabase.ImportPackage(path, false);
                EditorUtility.DisplayDialog("Success !", "Assembly definition files have been installed.", "OK");
            }
        }

        #endregion Utility Tab

        #region Footer

        private void SetupFooter()
        {
            #region Variables

            string publisherStoreLink = "https://assetstore.unity.com/publishers/45848?aid=1011ljxWe";
            string facebookLink = "https://www.facebook.com/tinygiantstudio";
            string redditLink = "https://www.reddit.com/r/tinygiantstudio";

            #endregion Variables

            GroupBox footer = rootVisualElement.Q<GroupBox>("Footer");

            SetupURLButton(footer, "GetMoreAssetsButton", publisherStoreLink);
            SetupURLButton(footer, "DocumentationButton", documentationLink);
            SetupURLButton(footer, "ForumButton", forumLink);
            SetupURLButton(footer, "FaceBookButton", facebookLink);
            SetupURLButton(footer, "RedditButton", redditLink);
        }

        private void SetupURLButton(GroupBox holder, string buttonName, string targetURL)
        {
            holder.Q<UnityEngine.UIElements.Button>(buttonName).clicked += () => Application.OpenURL(targetURL);
        }

        #endregion Footer
    }
}