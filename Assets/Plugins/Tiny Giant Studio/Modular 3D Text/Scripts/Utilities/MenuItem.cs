using System;
using TinyGiantStudio.Layout;
using UnityEditor;
using UnityEngine;

namespace TinyGiantStudio.Text
{
    public class MenuItem : MonoBehaviour //MonoBehaviour is required for destroy immediate/instantiate etc.
    {
#if UNITY_EDITOR
        static void CheckForRayCaster()
        {
            AssetSettings settings = StaticMethods.VerifySettings(null);
#if ENABLE_INPUT_SYSTEM
            if (settings.InputActionAsset == null)
                settings.FindModularTextInputActionAsset();
#endif

            if (!settings.autoCreateSceneInputSystem)
                return;



            var xRToolkitGlobalInputControllerForModularTextAsset = Type.GetType("TinyGiantStudio.Text.XRToolkitGlobalInputControllerForModularTextAsset");
            if (xRToolkitGlobalInputControllerForModularTextAsset != null)
            {
#if UNITY_2023_1_OR_NEWER
                var vrController = UnityEngine.Object.FindFirstObjectByType(Type.GetType("TinyGiantStudio.Text.XRToolkitGlobalInputControllerForModularTextAsset"));
                //XRToolkitGlobalInputControllerForModularTextAsset vrController = (XRToolkitGlobalInputControllerForModularTextAsset)Object.FindFirstObjectByType(typeof(XRToolkitGlobalInputControllerForModularTextAsset), true);
#else
                var vrController = GameObject.FindObjectOfType(Type.GetType("TinyGiantStudio.Text.XRToolkitGlobalInputControllerForModularTextAsset"), true);
#endif
                if (vrController == null)
                {
                    GameObject inputSystemGameObject = new GameObject("MText XR Input Manager");
                    Undo.RegisterCreatedObjectUndo(inputSystemGameObject, "Created MText XR Input Manager");
                    inputSystemGameObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                    //inputSystemGameObject.AddComponent<XRToolkitGlobalInputControllerForModularTextAsset>();
                    inputSystemGameObject.AddComponent(xRToolkitGlobalInputControllerForModularTextAsset);
                }

                if (settings.dontAutoCreateRaycasterOrButtonIfVRtoolkitExists)
                    return;
            }




#if UNITY_2023_1_OR_NEWER
            RaycastSelector inputSystem = (RaycastSelector)UnityEngine.Object.FindFirstObjectByType(typeof(RaycastSelector), FindObjectsInactive.Include);
#else
            RaycastSelector inputSystem = (RaycastSelector)GameObject.FindObjectOfType(typeof(RaycastSelector), true);
#endif
            if (!inputSystem)
                CreateRaycastSelector();

            if (inputSystem) //TODO: //this is to update people's old scenes. Remove later, Added on march 2023
            {
                if (!inputSystem.gameObject.GetComponent<ButtonInputSystemGlobal>())
                {
                    if (Application.isPlaying)
                        inputSystem.gameObject.AddComponent<ButtonInputSystemGlobal>();
                    else
                        Undo.AddComponent<ButtonInputSystemGlobal>(inputSystem.gameObject);
                }
            }


#if ENABLE_INPUT_SYSTEM
            if (!inputSystem)
                return;

            EditorApplication.delayCall += () => UpdateInputActionAsset(settings, inputSystem);
#endif
        }

#if ENABLE_INPUT_SYSTEM
        private static void UpdateInputActionAsset(AssetSettings settings, RaycastSelector inputSystem)
        {
            ButtonInputSystemGlobal buttonInputSystemGlobal = inputSystem.gameObject.GetComponent<ButtonInputSystemGlobal>();

            if (settings && buttonInputSystemGlobal)
            {
                if (settings.InputActionAsset && !buttonInputSystemGlobal.inputActionAsset)
                {
                    buttonInputSystemGlobal.inputActionAsset = settings.InputActionAsset;
                }
            }
        }
#endif

        private static void CreateRaycastSelector()
        {
            GameObject inputSystemGameObject = new GameObject("M3D Input Manager");
            Undo.RegisterCreatedObjectUndo(inputSystemGameObject, "Create M3D Input Manager");
            inputSystemGameObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            inputSystemGameObject.AddComponent<RaycastSelector>();
            inputSystemGameObject.AddComponent<RaycastInputProcessor>().myCamera = Camera.main;
            inputSystemGameObject.AddComponent<ButtonInputSystemGlobal>();
        }


        [UnityEditor.MenuItem("GameObject/3D Object/Tiny Giant Studio/Modular 3D Text/Text", false, 20001)]
        static void CreateText(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject go = CreateText("Modular 3D Text");

            // Ensure it gets re-parented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }


        [UnityEditor.MenuItem("GameObject/3D Object/Tiny Giant Studio/Modular 3D Text/List/Grid Layout", false, 20501)]
        static void CreateGridLayoutList(MenuCommand menuCommand)
        {
            CheckForRayCaster();

            // Create a custom game object
            GameObject go = new GameObject("List (M3D)");
            go.AddComponent<List>();
            go.GetComponent<List>().LoadDefaultSettings();
            go.AddComponent<GridLayoutGroup>();

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        [UnityEditor.MenuItem("GameObject/3D Object/Tiny Giant Studio/Modular 3D Text/List/Linear Layout", false, 20502)]
        static void CreateLinearLayoutList(MenuCommand menuCommand)
        {
            CheckForRayCaster();

            // Create a custom game object
            GameObject go = new GameObject("List (M3D)");
            go.AddComponent<List>();
            go.GetComponent<List>().LoadDefaultSettings();
            go.AddComponent<LinearLayoutGroup>();

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        [UnityEditor.MenuItem("GameObject/3D Object/Tiny Giant Studio/Modular 3D Text/List/Circular Layout", false, 20503)]
        static void CreateCircularLayoutList(MenuCommand menuCommand)
        {
            CheckForRayCaster();

            // Create a custom game object
            GameObject go = new GameObject("List (M3D)");
            go.AddComponent<List>();
            go.GetComponent<List>().LoadDefaultSettings();
            go.AddComponent<CircularLayoutGroup>();

            // Ensure it gets re-parented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }


        [UnityEditor.MenuItem("GameObject/3D Object/Tiny Giant Studio/Modular 3D Text/Button", false, 20002)]
        static void CreateButton(MenuCommand menuCommand)
        {
            CheckForRayCaster();

            GameObject go = new GameObject("Button (M3D)");

            GameObject text = CreateText("Button");
            text.transform.SetParent(go.transform);


            GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bg.name = "Background";
            bg.transform.localScale = new Vector3(15, 2, 1);
            if (bg.GetComponent<BoxCollider>())
                DestroyImmediate(bg.GetComponent<BoxCollider>());
            bg.transform.SetParent(go.transform);
            bg.transform.localPosition = new Vector3(0, 0, 0.55f);


            go.AddComponent<Button>();
            go.GetComponent<Button>().Background = bg.GetComponent<Renderer>();
            go.GetComponent<Button>().Text = text.GetComponent<Modular3DText>();
            go.GetComponent<Button>().LoadDefaultSettings();
            bg.GetComponent<Renderer>().material = go.GetComponent<Button>().NormalBackgroundMaterial;


            go.AddComponent<BoxCollider>();
            go.GetComponent<BoxCollider>().size = new Vector3(15, 2, 1);
            go.GetComponent<BoxCollider>().center = new Vector3(0, 0, 0.5f);

            // Ensure it gets re-parented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        [UnityEditor.MenuItem("GameObject/3D Object/Tiny Giant Studio/Modular 3D Text/Input Field", false, 20003)]
        static void CreateInputField(MenuCommand menuCommand)
        {
            CheckForRayCaster();

            GameObject text = new GameObject("Text");
            text.AddComponent<Modular3DText>();
            text.AddComponent<GridLayoutGroup>();
            LoadDefaultTextSettings(text.GetComponent<Modular3DText>());
            //text.GetComponent<Modular3DText>().UpdateText("Input Field");

            GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bg.name = "Background";
            bg.transform.localScale = new Vector3(15, 2, 1);
            if (bg.GetComponent<BoxCollider>())
                DestroyImmediate(bg.GetComponent<BoxCollider>());

            // Create a custom game object
            GameObject go = new GameObject("Input Field (M3D)");

            bg.transform.SetParent(go.transform);
            bg.transform.localPosition = new Vector3(0, 0, 0.55f);
            text.transform.SetParent(go.transform);


            go.AddComponent<InputField>();
            go.GetComponent<InputField>().background = bg.GetComponent<Renderer>();
            bg.GetComponent<Renderer>().material = go.GetComponent<InputField>().normalBackgroundMaterial;
            go.GetComponent<InputField>().textComponent = text.GetComponent<Modular3DText>();
            go.GetComponent<InputField>().UpdateText();



            go.AddComponent<BoxCollider>();
            go.GetComponent<BoxCollider>().size = new Vector3(15, 2, 1);
            go.GetComponent<BoxCollider>().center = new Vector3(0, 0, 0.5f);

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        [UnityEditor.MenuItem("GameObject/3D Object/Tiny Giant Studio/Modular 3D Text/Slider", false, 20004)]
        static void CreateSlider(MenuCommand menuCommand)
        {
            CheckForRayCaster();

            // Create a custom game object
            GameObject go = new GameObject("Slider (M3D)");
            go.AddComponent<Slider>();
            Slider slider = go.GetComponent<Slider>();


            GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            handle.name = "Handle";
            handle.AddComponent<SliderHandle>();
            handle.GetComponent<SliderHandle>().slider = slider;
            handle.transform.SetParent(go.transform);
            handle.transform.localPosition = Vector3.zero;
            slider.handle = handle.GetComponent<SliderHandle>();
            slider.handleGraphic = handle.GetComponent<Renderer>();
            handle.GetComponent<Renderer>().material = slider.unSelectedHandleMat;

            GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bg.name = "Background";
            bg.transform.SetParent(go.transform);
            bg.transform.localPosition = Vector3.zero;
            bg.transform.localScale = new Vector3(10, 0.25f, 0.25f);
            slider.background = bg.transform;

            // Ensure it gets re-parented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        [UnityEditor.MenuItem("GameObject/3D Object/Tiny Giant Studio/Modular 3D Text/Toggle", false, 20006)]
        static void CreateToggle(MenuCommand menuCommand)
        {
            CheckForRayCaster();

            GameObject go = new GameObject("Toggle (M3D)");
            go.AddComponent<Toggle>();
            go.AddComponent<Button>().PressedBackgroundMaterial = go.GetComponent<Button>().SelectedBackgroundMaterial;
            go.AddComponent<BoxCollider>().size = new Vector3(3, 3, 1);
            go.GetComponent<Button>().NormalTextSize = new Vector3(20, 20, 8);
            go.GetComponent<Button>().SelectedTextSize = new Vector3(20, 20, 8.2f);
            go.GetComponent<Button>().PressedTextSize = new Vector3(20, 20, 8.5f);
            go.GetComponent<Button>().LoadDefaultSettings();



            GameObject text = CreateText("X");
            text.GetComponent<Modular3DText>().FontSize = new Vector3(20, 20, 8);
            text.transform.SetParent(go.transform);
            go.GetComponent<Button>().Text = text.GetComponent<Modular3DText>();
            go.GetComponent<Toggle>().onGraphic = text;



            GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bg.name = "Background";
            bg.transform.localScale = new Vector3(3, 3, 1);
            if (bg.GetComponent<BoxCollider>())
                DestroyImmediate(bg.GetComponent<BoxCollider>());
            bg.transform.SetParent(go.transform);
            bg.transform.localPosition = new Vector3(0, 0, 0.55f);

            go.GetComponent<Button>().Background = bg.GetComponent<Renderer>();
            bg.GetComponent<Renderer>().material = go.GetComponent<Button>().NormalBackgroundMaterial;



            go.GetComponent<Toggle>().AddEventToButton();



            // Ensure it gets re-parented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        [UnityEditor.MenuItem("GameObject/3D Object/Tiny Giant Studio/Modular 3D Text/Progress Bar", false, 20005)]
        static void CreateProgressBar(MenuCommand menuCommand)
        {
            CheckForRayCaster();

            // Create a custom game object
            GameObject go = new GameObject("ProgressBar (M3D)");
            go.AddComponent<Slider>();
            Slider slider = go.GetComponent<Slider>();


            GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            handle.name = "Handle";
            handle.AddComponent<SliderHandle>();
            handle.GetComponent<SliderHandle>().slider = slider;
            handle.transform.SetParent(go.transform);
            handle.transform.localPosition = Vector3.zero;
            slider.handle = handle.GetComponent<SliderHandle>();
            slider.handleGraphic = handle.GetComponent<Renderer>();
            handle.GetComponent<Renderer>().material = slider.unSelectedHandleMat;

            GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bg.name = "Background";
            bg.transform.SetParent(go.transform);
            bg.transform.localPosition = Vector3.zero;
            bg.transform.localScale = new Vector3(10, 0.25f, 0.25f);
            slider.background = bg.transform;

            if (slider.progressBarPrefab)
            {
                GameObject progressBarGraphic = Instantiate(slider.progressBarPrefab);
                slider.progressBar = progressBarGraphic.transform;
                progressBarGraphic.transform.SetParent(go.transform);
                progressBarGraphic.transform.localPosition = new Vector3(-5, 0, 0);
                progressBarGraphic.transform.localScale = new Vector3(5, 0.8f, 0.8f);
            }
            else
            {
                Debug.Log("No progress bar prefab found. Please create one and assign it to Progressbar field");
            }

            // Ensure it gets re-parented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        [UnityEditor.MenuItem("GameObject/3D Object/Tiny Giant Studio/Modular 3D Text/Horizontal Selector", false, 20007)]
        static void CreateHorizontalSelector(MenuCommand menuCommand)
        {
            CheckForRayCaster();

            // Create a custom game object
            GameObject go = new GameObject("Horizontal Selector (M3D)");
            go.AddComponent<HorizontalSelector>();
            HorizontalSelector selector = go.GetComponent<HorizontalSelector>();

            // Create a custom game object
            GameObject text = new GameObject("Text (M3D)");
            text.AddComponent<Modular3DText>();
            text.AddComponent<GridLayoutGroup>();
            if (!text.GetComponent<TextUpdater>()) text.AddComponent<TextUpdater>();
            text.GetComponent<Modular3DText>().UpdateText("Option A");
            LoadDefaultTextSettings(text.GetComponent<Modular3DText>());
            text.transform.SetParent(go.transform);
            text.transform.localPosition = Vector3.zero;

            selector.text = text.GetComponent<Modular3DText>();

            // Ensure it gets re-parented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }




        private static GameObject CreateText(string text)
        {
            GameObject go = new GameObject("Text (M3D)");
            go.AddComponent<Modular3DText>();
            go.AddComponent<GridLayoutGroup>();
            go.GetComponent<Modular3DText>().Text = text;

            if (!go.GetComponent<TextUpdater>())
                go.AddComponent<TextUpdater>();

            LoadDefaultTextSettings(go.GetComponent<Modular3DText>());
            return go;
        }

        static void LoadDefaultTextSettings(Modular3DText text)
        {
            AssetSettings settings = StaticMethods.VerifySettings(null);

            if (settings)
            {
                text.Font = settings.defaultFont;
                text.FontSize = settings.defaultTextSize;
                text.Material = settings.defaultTextMaterial;
            }
        }
#endif
    }
}