/// Created by Ferdowsur Asif @ Tiny Giant Studios
/// This code was made with the purpose of demonstration only for sample scene
/// Not optimized and not intended to be used with real projects
/// Like using camera.man and checking name string == are bad practices

using TinyGiantStudio.Text.Example;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
#endif

namespace TinyGiantStudio.Text.SampleScene
{
    public class MText_SampleScene_Cursor : MonoBehaviour
    {
        [SerializeField] Transform crosshair = null;
        [SerializeField] float rotationSpeed = 0.1f;
        [SerializeField] ParticleSystem hitEffect = null;
        [SerializeField] StatusToolTip statusToolTip = null;
        [SerializeField] Camera myCamera;

        void Awake()
        {
#if ENABLE_INPUT_SYSTEM
            EnhancedTouchSupport.Enable();
#endif
        }

        void Start()
        {
            Cursor.visible = false;
            if (myCamera == null)
                myCamera = Camera.main;
        }

        void Update()
        {
            if (!crosshair)
                return;
#if ENABLE_INPUT_SYSTEM
            Ray ray = myCamera.ScreenPointToRay(Pointer.current.position.ReadValue());
#else
            Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);
#endif
            if (Physics.Raycast(ray, out RaycastHit hit, 1000))
            {
                crosshair.position = hit.point;

#if ENABLE_INPUT_SYSTEM
                if (MouseClicked() || Tapped())
#else
                if (Input.GetMouseButtonDown(0))
#endif
                {
                    if (hit.transform.gameObject.name == "Target") ///checking name string == is bad practice. This is just for sample scene
                    {
                        int damage = Random.Range(1, 50);
                        int style = 0;
                        if (hit.transform.position.x > 0) style = 1;
                        else if (hit.transform.position.x < 0) style = 2;
                        statusToolTip.ShowToolTip("-" + damage.ToString(), style, hit.point, Quaternion.Euler(0, 0, 0), true);

                        float currentHealth = hit.transform.GetChild(0).gameObject.GetComponent<Slider>().CurrentValue - damage;
                        if (currentHealth < 0) currentHealth = 0;
                        hit.transform.GetChild(0).gameObject.GetComponent<Slider>().UpdateValue(currentHealth);

                        hitEffect.transform.position = hit.point;
                        hitEffect.Play();
                    }
                }
            }
            crosshair.eulerAngles += new Vector3(0, rotationSpeed, 0);
        }

#if ENABLE_INPUT_SYSTEM
        bool MouseClicked()
        {
            if (Mouse.current != null)
                return Mouse.current.leftButton.wasPressedThisFrame;

            return false;
        }

        bool Tapped()
        {
            if (Touch.activeTouches.Count > 0)
                return Touch.activeTouches[0].ended;

            return false;
        }
#endif
    }
}