//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Settings/InputSystem/InputControls.inputactions
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
                    ""name"": ""AxisControlToggle"",
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
                    ""id"": ""cf55c8a6-9973-415e-b90d-e3e2210739d5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
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
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""AxisControlToggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""520047b9-0ba8-492e-a862-a2ff4f97b34e"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Editor"",
            ""id"": ""7b46995b-e0a1-4ec3-bebc-975bd55a947e"",
            ""actions"": [
                {
                    ""name"": ""X-CameraSwitcher"",
                    ""type"": ""Button"",
                    ""id"": ""6957f8aa-6d32-4a0b-a5e5-a680c3c3a24e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Y-CameraSwitcher"",
                    ""type"": ""Button"",
                    ""id"": ""cf2594af-30de-41a3-9855-5d3454d6831c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Z-CameraSwitcher"",
                    ""type"": ""Button"",
                    ""id"": ""6f354320-3be8-4f69-a5ba-ac357d02bf8f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Reset"",
                    ""type"": ""Button"",
                    ""id"": ""ff3a679a-1385-4731-a944-a2fbbd4afb78"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""e0a84256-0f2b-45a8-a6d3-fc5a8879dfd6"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Z-CameraSwitcher"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""953e3991-cc4e-434f-8b00-2e931c7c2da1"",
                    ""path"": ""<Keyboard>/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Y-CameraSwitcher"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""36ec9dff-c5de-44e6-90fb-b94a3a47d6db"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""X-CameraSwitcher"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""876079f7-015a-423a-918d-76606a6733c3"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Reset"",
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
            m_Player_AxisControlToggle = m_Player.FindAction("AxisControlToggle", throwIfNotFound: true);
            m_Player_Click = m_Player.FindAction("Click", throwIfNotFound: true);
            // Editor
            m_Editor = asset.FindActionMap("Editor", throwIfNotFound: true);
            m_Editor_XCameraSwitcher = m_Editor.FindAction("X-CameraSwitcher", throwIfNotFound: true);
            m_Editor_YCameraSwitcher = m_Editor.FindAction("Y-CameraSwitcher", throwIfNotFound: true);
            m_Editor_ZCameraSwitcher = m_Editor.FindAction("Z-CameraSwitcher", throwIfNotFound: true);
            m_Editor_Reset = m_Editor.FindAction("Reset", throwIfNotFound: true);
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
        private readonly InputAction m_Player_AxisControlToggle;
        private readonly InputAction m_Player_Click;
        public struct PlayerActions
        {
            private @InputControls m_Wrapper;
            public PlayerActions(@InputControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Movement => m_Wrapper.m_Player_Movement;
            public InputAction @Jump => m_Wrapper.m_Player_Jump;
            public InputAction @Interaction => m_Wrapper.m_Player_Interaction;
            public InputAction @AxisControlToggle => m_Wrapper.m_Player_AxisControlToggle;
            public InputAction @Click => m_Wrapper.m_Player_Click;
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
                @AxisControlToggle.started += instance.OnAxisControlToggle;
                @AxisControlToggle.performed += instance.OnAxisControlToggle;
                @AxisControlToggle.canceled += instance.OnAxisControlToggle;
                @Click.started += instance.OnClick;
                @Click.performed += instance.OnClick;
                @Click.canceled += instance.OnClick;
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
                @AxisControlToggle.started -= instance.OnAxisControlToggle;
                @AxisControlToggle.performed -= instance.OnAxisControlToggle;
                @AxisControlToggle.canceled -= instance.OnAxisControlToggle;
                @Click.started -= instance.OnClick;
                @Click.performed -= instance.OnClick;
                @Click.canceled -= instance.OnClick;
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

        // Editor
        private readonly InputActionMap m_Editor;
        private List<IEditorActions> m_EditorActionsCallbackInterfaces = new List<IEditorActions>();
        private readonly InputAction m_Editor_XCameraSwitcher;
        private readonly InputAction m_Editor_YCameraSwitcher;
        private readonly InputAction m_Editor_ZCameraSwitcher;
        private readonly InputAction m_Editor_Reset;
        public struct EditorActions
        {
            private @InputControls m_Wrapper;
            public EditorActions(@InputControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @XCameraSwitcher => m_Wrapper.m_Editor_XCameraSwitcher;
            public InputAction @YCameraSwitcher => m_Wrapper.m_Editor_YCameraSwitcher;
            public InputAction @ZCameraSwitcher => m_Wrapper.m_Editor_ZCameraSwitcher;
            public InputAction @Reset => m_Wrapper.m_Editor_Reset;
            public InputActionMap Get() { return m_Wrapper.m_Editor; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(EditorActions set) { return set.Get(); }
            public void AddCallbacks(IEditorActions instance)
            {
                if (instance == null || m_Wrapper.m_EditorActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_EditorActionsCallbackInterfaces.Add(instance);
                @XCameraSwitcher.started += instance.OnXCameraSwitcher;
                @XCameraSwitcher.performed += instance.OnXCameraSwitcher;
                @XCameraSwitcher.canceled += instance.OnXCameraSwitcher;
                @YCameraSwitcher.started += instance.OnYCameraSwitcher;
                @YCameraSwitcher.performed += instance.OnYCameraSwitcher;
                @YCameraSwitcher.canceled += instance.OnYCameraSwitcher;
                @ZCameraSwitcher.started += instance.OnZCameraSwitcher;
                @ZCameraSwitcher.performed += instance.OnZCameraSwitcher;
                @ZCameraSwitcher.canceled += instance.OnZCameraSwitcher;
                @Reset.started += instance.OnReset;
                @Reset.performed += instance.OnReset;
                @Reset.canceled += instance.OnReset;
            }

            private void UnregisterCallbacks(IEditorActions instance)
            {
                @XCameraSwitcher.started -= instance.OnXCameraSwitcher;
                @XCameraSwitcher.performed -= instance.OnXCameraSwitcher;
                @XCameraSwitcher.canceled -= instance.OnXCameraSwitcher;
                @YCameraSwitcher.started -= instance.OnYCameraSwitcher;
                @YCameraSwitcher.performed -= instance.OnYCameraSwitcher;
                @YCameraSwitcher.canceled -= instance.OnYCameraSwitcher;
                @ZCameraSwitcher.started -= instance.OnZCameraSwitcher;
                @ZCameraSwitcher.performed -= instance.OnZCameraSwitcher;
                @ZCameraSwitcher.canceled -= instance.OnZCameraSwitcher;
                @Reset.started -= instance.OnReset;
                @Reset.performed -= instance.OnReset;
                @Reset.canceled -= instance.OnReset;
            }

            public void RemoveCallbacks(IEditorActions instance)
            {
                if (m_Wrapper.m_EditorActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IEditorActions instance)
            {
                foreach (var item in m_Wrapper.m_EditorActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_EditorActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public EditorActions @Editor => new EditorActions(this);
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
            void OnAxisControlToggle(InputAction.CallbackContext context);
            void OnClick(InputAction.CallbackContext context);
        }
        public interface IEditorActions
        {
            void OnXCameraSwitcher(InputAction.CallbackContext context);
            void OnYCameraSwitcher(InputAction.CallbackContext context);
            void OnZCameraSwitcher(InputAction.CallbackContext context);
            void OnReset(InputAction.CallbackContext context);
        }
    }
}
