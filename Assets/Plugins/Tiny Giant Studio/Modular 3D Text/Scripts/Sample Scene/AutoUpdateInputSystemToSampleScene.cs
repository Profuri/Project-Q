using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace TinyGiantStudio.Text.SampleScene
{
    [ExecuteAlways]
    public class AutoUpdateInputSystemToSampleScene : MonoBehaviour
    {
#if UNITY_EDITOR
        void Awake()
        {
#if ENABLE_INPUT_SYSTEM
            if (!gameObject.GetComponent<PlayerInput>())
                gameObject.AddComponent<PlayerInput>();

            AssetSettings settings = StaticMethods.VerifySettings(null);

            if (settings)
            {
                if (!settings.InputActionAsset)
                {
                    settings.FindModularTextInputActionAsset();
                }
                gameObject.GetComponent<PlayerInput>().actions = settings.InputActionAsset;
            }
#endif
        }
#endif
    }
}