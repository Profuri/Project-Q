//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/99.Settings/InputSystem/InputControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace InputControl
{
    public partial class @InputControls: IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @InputControls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputControls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""b921f3d0-e5ba-45c5-aee0-ee3ea386063f"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""1e04fd3b-b587-4ca7-947d-6a566fe64a5e"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""2945dd48-1ec2-4a9f-82c1-b1e975524560"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Interaction"",
                    ""type"": ""Button"",
                    ""id"": ""043858ae-c34a-4cf1-a083-a73b2ccd9f34"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""AxisControl"",
                    ""type"": ""Button"",
                    ""id"": ""50db877a-ab43-496e-9f06-083c49564ae2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Click"",
                    ""type"": ""Button"",
                    ""id"": ""3481214d-470b-4f3b-a361-2157ce0688c3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Reload"",
                    ""type"": ""Button"",
                    ""id"": ""bf275e9e-5ea1-40de-b5f6-2cd982452ecf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Up/Down/Left/Right"",
                    ""id"": ""90d27824-50b1-4343-b340-62a50b643bf3"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""0eaeec72-84be-462f-8902-a82eca5f3cb4"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""7c4f4f4b-e8bf-49ce-a704-99e3596018c9"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""264c3c73-b678-4b2f-b4c0-6f445e81b5c4"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""48963a0d-9926-45e2-a6ab-d0f6fc2b9f7b"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""61d54c62-3346-40ef-b5ef-1823ec4e8f58"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3697c80f-d747-4b78-8fd3-45d1ddd08ce1"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Interaction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""649b2138-67b1-4eaa-a07b-d3ce13883325"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""AxisControl"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a95ba4d4-15e5-444e-bed8-08a697c923b5"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""419e203e-9386-499e-a366-8a2099ac6a6b"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""e8b09cf0-1d79-4818-bcf2-91729e97889b"",
            ""actions"": [
                {
                    ""name"": ""LeftClick"",
                    ""type"": ""Button"",
                    ""id"": ""c224120b-d9cb-43c7-8d61-4fc16e85d2d8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MouseMove"",
                    ""type"": ""Value"",
                    ""id"": ""b2e0f273-77d4-40b3-9a4c-8442f15d4510"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""UpArrow"",
                    ""type"": ""Button"",
                    ""id"": ""287ff507-2721-48b9-acc8-99187ca817aa"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""DownArrow"",
                    ""type"": ""Button"",
                    ""id"": ""2d7e8dd9-3bd2-40f7-b324-f6b6fcf51bc4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Enter"",
                    ""type"": ""Button"",
                    ""id"": ""4c080734-d039-448f-8df0-f78f64e66773"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""20e4fd6f-ff4c-4699-8810-f2daacc6051c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TutorialOut"",
                    ""type"": ""Button"",
                    ""id"": ""aef87256-6208-4996-8f6a-6f084caff0c6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""6729ab35-e6f2-495a-a883-b8b8241ccabf"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""LeftClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7a1f45cb-936f-45ac-ae6e-25cfd0ebd06c"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""MouseMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""10efc039-c0d9-4828-8f2a-f74e78dc4b4e"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""UpArrow"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""21a12fd3-d8ad-4ce6-9868-a4b9c1c714ae"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""DownArrow"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b6cf443e-0d4d-4818-a989-a0e77716585f"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Enter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""90f272e2-9c1d-4dce-b92d-f27256866ecc"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Enter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3642fe2b-f046-4776-ad8b-e03386cce0f1"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Enter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ce074dae-954e-4ac3-bf2c-e5df8cb364b7"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f8aecf60-800d-4ccc-bc67-9dbc17164d12"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TutorialOut"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4d82f0b7-66b3-4e3b-9bc2-5a8ed7a09840"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TutorialOut"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""291f57a5-5a3b-4ddb-aac8-f6e1bd58b803"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TutorialOut"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Camera"",
            ""id"": ""c8e89bbc-2487-4e83-8006-4d5c4de9f563"",
            ""actions"": [
                {
                    ""name"": ""ZoomControl"",
                    ""type"": ""Button"",
                    ""id"": ""3da71c75-677c-4e78-aee1-f779698c0978"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""02392352-4d58-48dc-ab3f-d4e26cd57e8a"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""ZoomControl"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Timeline"",
            ""id"": ""c0265da2-9d46-4df9-a253-b917b0810a78"",
            ""actions"": [
                {
                    ""name"": ""SpeedUp"",
                    ""type"": ""Button"",
                    ""id"": ""86559dd8-49f7-4ca0-aef7-5ed64116dc2a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""71255516-8e33-40be-9d6e-ea43696ad6cd"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""SpeedUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KeyboardMouse"",
            ""bindingGroup"": ""KeyboardMouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // Player
            m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
            m_Player_Movement = m_Player.FindAction("Movement", throwIfNotFound: true);
            m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
            m_Player_Interaction = m_Player.FindAction("Interaction", throwIfNotFound: true);
            m_Player_AxisControl = m_Player.FindAction("AxisControl", throwIfNotFound: true);
            m_Player_Click = m_Player.FindAction("Click", throwIfNotFound: true);
            m_Player_Reload = m_Player.FindAction("Reload", throwIfNotFound: true);
            // UI
            m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
            m_UI_LeftClick = m_UI.FindAction("LeftClick", throwIfNotFound: true);
            m_UI_MouseMove = m_UI.FindAction("MouseMove", throwIfNotFound: true);
            m_UI_UpArrow = m_UI.FindAction("UpArrow", throwIfNotFound: true);
            m_UI_DownArrow = m_UI.FindAction("DownArrow", throwIfNotFound: true);
            m_UI_Enter = m_UI.FindAction("Enter", throwIfNotFound: true);
            m_UI_Pause = m_UI.FindAction("Pause", throwIfNotFound: true);
            m_UI_TutorialOut = m_UI.FindAction("TutorialOut", throwIfNotFound: true);
            // Camera
            m_Camera = asset.FindActionMap("Camera", throwIfNotFound: true);
            m_Camera_ZoomControl = m_Camera.FindAction("ZoomControl", throwIfNotFound: true);
            // Timeline
            m_Timeline = asset.FindActionMap("Timeline", throwIfNotFound: true);
            m_Timeline_SpeedUp = m_Timeline.FindAction("SpeedUp", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }

        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // Player
        private readonly InputActionMap m_Player;
        private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
        private readonly InputAction m_Player_Movement;
        private readonly InputAction m_Player_Jump;
        private readonly InputAction m_Player_Interaction;
        private readonly InputAction m_Player_AxisControl;
        private readonly InputAction m_Player_Click;
        private readonly InputAction m_Player_Reload;
        public struct PlayerActions
        {
            private @InputControls m_Wrapper;
            public PlayerActions(@InputControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Movement => m_Wrapper.m_Player_Movement;
            public InputAction @Jump => m_Wrapper.m_Player_Jump;
            public InputAction @Interaction => m_Wrapper.m_Player_Interaction;
            public InputAction @AxisControl => m_Wrapper.m_Player_AxisControl;
            public InputAction @Click => m_Wrapper.m_Player_Click;
            public InputAction @Reload => m_Wrapper.m_Player_Reload;
            public InputActionMap Get() { return m_Wrapper.m_Player; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
            public void AddCallbacks(IPlayerActions instance)
            {
                if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Interaction.started += instance.OnInteraction;
                @Interaction.performed += instance.OnInteraction;
                @Interaction.canceled += instance.OnInteraction;
                @AxisControl.started += instance.OnAxisControl;
                @AxisControl.performed += instance.OnAxisControl;
                @AxisControl.canceled += instance.OnAxisControl;
                @Click.started += instance.OnClick;
                @Click.performed += instance.OnClick;
                @Click.canceled += instance.OnClick;
                @Reload.started += instance.OnReload;
                @Reload.performed += instance.OnReload;
                @Reload.canceled += instance.OnReload;
            }

            private void UnregisterCallbacks(IPlayerActions instance)
            {
                @Movement.started -= instance.OnMovement;
                @Movement.performed -= instance.OnMovement;
                @Movement.canceled -= instance.OnMovement;
                @Jump.started -= instance.OnJump;
                @Jump.performed -= instance.OnJump;
                @Jump.canceled -= instance.OnJump;
                @Interaction.started -= instance.OnInteraction;
                @Interaction.performed -= instance.OnInteraction;
                @Interaction.canceled -= instance.OnInteraction;
                @AxisControl.started -= instance.OnAxisControl;
                @AxisControl.performed -= instance.OnAxisControl;
                @AxisControl.canceled -= instance.OnAxisControl;
                @Click.started -= instance.OnClick;
                @Click.performed -= instance.OnClick;
                @Click.canceled -= instance.OnClick;
                @Reload.started -= instance.OnReload;
                @Reload.performed -= instance.OnReload;
                @Reload.canceled -= instance.OnReload;
            }

            public void RemoveCallbacks(IPlayerActions instance)
            {
                if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IPlayerActions instance)
            {
                foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public PlayerActions @Player => new PlayerActions(this);

        // UI
        private readonly InputActionMap m_UI;
        private List<IUIActions> m_UIActionsCallbackInterfaces = new List<IUIActions>();
        private readonly InputAction m_UI_LeftClick;
        private readonly InputAction m_UI_MouseMove;
        private readonly InputAction m_UI_UpArrow;
        private readonly InputAction m_UI_DownArrow;
        private readonly InputAction m_UI_Enter;
        private readonly InputAction m_UI_Pause;
        private readonly InputAction m_UI_TutorialOut;
        public struct UIActions
        {
            private @InputControls m_Wrapper;
            public UIActions(@InputControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @LeftClick => m_Wrapper.m_UI_LeftClick;
            public InputAction @MouseMove => m_Wrapper.m_UI_MouseMove;
            public InputAction @UpArrow => m_Wrapper.m_UI_UpArrow;
            public InputAction @DownArrow => m_Wrapper.m_UI_DownArrow;
            public InputAction @Enter => m_Wrapper.m_UI_Enter;
            public InputAction @Pause => m_Wrapper.m_UI_Pause;
            public InputAction @TutorialOut => m_Wrapper.m_UI_TutorialOut;
            public InputActionMap Get() { return m_Wrapper.m_UI; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
            public void AddCallbacks(IUIActions instance)
            {
                if (instance == null || m_Wrapper.m_UIActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_UIActionsCallbackInterfaces.Add(instance);
                @LeftClick.started += instance.OnLeftClick;
                @LeftClick.performed += instance.OnLeftClick;
                @LeftClick.canceled += instance.OnLeftClick;
                @MouseMove.started += instance.OnMouseMove;
                @MouseMove.performed += instance.OnMouseMove;
                @MouseMove.canceled += instance.OnMouseMove;
                @UpArrow.started += instance.OnUpArrow;
                @UpArrow.performed += instance.OnUpArrow;
                @UpArrow.canceled += instance.OnUpArrow;
                @DownArrow.started += instance.OnDownArrow;
                @DownArrow.performed += instance.OnDownArrow;
                @DownArrow.canceled += instance.OnDownArrow;
                @Enter.started += instance.OnEnter;
                @Enter.performed += instance.OnEnter;
                @Enter.canceled += instance.OnEnter;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
                @TutorialOut.started += instance.OnTutorialOut;
                @TutorialOut.performed += instance.OnTutorialOut;
                @TutorialOut.canceled += instance.OnTutorialOut;
            }

            private void UnregisterCallbacks(IUIActions instance)
            {
                @LeftClick.started -= instance.OnLeftClick;
                @LeftClick.performed -= instance.OnLeftClick;
                @LeftClick.canceled -= instance.OnLeftClick;
                @MouseMove.started -= instance.OnMouseMove;
                @MouseMove.performed -= instance.OnMouseMove;
                @MouseMove.canceled -= instance.OnMouseMove;
                @UpArrow.started -= instance.OnUpArrow;
                @UpArrow.performed -= instance.OnUpArrow;
                @UpArrow.canceled -= instance.OnUpArrow;
                @DownArrow.started -= instance.OnDownArrow;
                @DownArrow.performed -= instance.OnDownArrow;
                @DownArrow.canceled -= instance.OnDownArrow;
                @Enter.started -= instance.OnEnter;
                @Enter.performed -= instance.OnEnter;
                @Enter.canceled -= instance.OnEnter;
                @Pause.started -= instance.OnPause;
                @Pause.performed -= instance.OnPause;
                @Pause.canceled -= instance.OnPause;
                @TutorialOut.started -= instance.OnTutorialOut;
                @TutorialOut.performed -= instance.OnTutorialOut;
                @TutorialOut.canceled -= instance.OnTutorialOut;
            }

            public void RemoveCallbacks(IUIActions instance)
            {
                if (m_Wrapper.m_UIActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IUIActions instance)
            {
                foreach (var item in m_Wrapper.m_UIActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_UIActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public UIActions @UI => new UIActions(this);

        // Camera
        private readonly InputActionMap m_Camera;
        private List<ICameraActions> m_CameraActionsCallbackInterfaces = new List<ICameraActions>();
        private readonly InputAction m_Camera_ZoomControl;
        public struct CameraActions
        {
            private @InputControls m_Wrapper;
            public CameraActions(@InputControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @ZoomControl => m_Wrapper.m_Camera_ZoomControl;
            public InputActionMap Get() { return m_Wrapper.m_Camera; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(CameraActions set) { return set.Get(); }
            public void AddCallbacks(ICameraActions instance)
            {
                if (instance == null || m_Wrapper.m_CameraActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_CameraActionsCallbackInterfaces.Add(instance);
                @ZoomControl.started += instance.OnZoomControl;
                @ZoomControl.performed += instance.OnZoomControl;
                @ZoomControl.canceled += instance.OnZoomControl;
            }

            private void UnregisterCallbacks(ICameraActions instance)
            {
                @ZoomControl.started -= instance.OnZoomControl;
                @ZoomControl.performed -= instance.OnZoomControl;
                @ZoomControl.canceled -= instance.OnZoomControl;
            }

            public void RemoveCallbacks(ICameraActions instance)
            {
                if (m_Wrapper.m_CameraActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(ICameraActions instance)
            {
                foreach (var item in m_Wrapper.m_CameraActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_CameraActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public CameraActions @Camera => new CameraActions(this);

        // Timeline
        private readonly InputActionMap m_Timeline;
        private List<ITimelineActions> m_TimelineActionsCallbackInterfaces = new List<ITimelineActions>();
        private readonly InputAction m_Timeline_SpeedUp;
        public struct TimelineActions
        {
            private @InputControls m_Wrapper;
            public TimelineActions(@InputControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @SpeedUp => m_Wrapper.m_Timeline_SpeedUp;
            public InputActionMap Get() { return m_Wrapper.m_Timeline; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(TimelineActions set) { return set.Get(); }
            public void AddCallbacks(ITimelineActions instance)
            {
                if (instance == null || m_Wrapper.m_TimelineActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_TimelineActionsCallbackInterfaces.Add(instance);
                @SpeedUp.started += instance.OnSpeedUp;
                @SpeedUp.performed += instance.OnSpeedUp;
                @SpeedUp.canceled += instance.OnSpeedUp;
            }

            private void UnregisterCallbacks(ITimelineActions instance)
            {
                @SpeedUp.started -= instance.OnSpeedUp;
                @SpeedUp.performed -= instance.OnSpeedUp;
                @SpeedUp.canceled -= instance.OnSpeedUp;
            }

            public void RemoveCallbacks(ITimelineActions instance)
            {
                if (m_Wrapper.m_TimelineActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(ITimelineActions instance)
            {
                foreach (var item in m_Wrapper.m_TimelineActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_TimelineActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public TimelineActions @Timeline => new TimelineActions(this);
        private int m_KeyboardMouseSchemeIndex = -1;
        public InputControlScheme KeyboardMouseScheme
        {
            get
            {
                if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("KeyboardMouse");
                return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
            }
        }
        public interface IPlayerActions
        {
            void OnMovement(InputAction.CallbackContext context);
            void OnJump(InputAction.CallbackContext context);
            void OnInteraction(InputAction.CallbackContext context);
            void OnAxisControl(InputAction.CallbackContext context);
            void OnClick(InputAction.CallbackContext context);
            void OnReload(InputAction.CallbackContext context);
        }
        public interface IUIActions
        {
            void OnLeftClick(InputAction.CallbackContext context);
            void OnMouseMove(InputAction.CallbackContext context);
            void OnUpArrow(InputAction.CallbackContext context);
            void OnDownArrow(InputAction.CallbackContext context);
            void OnEnter(InputAction.CallbackContext context);
            void OnPause(InputAction.CallbackContext context);
            void OnTutorialOut(InputAction.CallbackContext context);
        }
        public interface ICameraActions
        {
            void OnZoomControl(InputAction.CallbackContext context);
        }
        public interface ITimelineActions
        {
            void OnSpeedUp(InputAction.CallbackContext context);
        }
    }
}
