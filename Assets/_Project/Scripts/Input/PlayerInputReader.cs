using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GreedIsland.Input
{
    public sealed class PlayerInputReader : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActionsAsset;

        public event Action JumpPressed;
        public event Action DashPressed;
        public event Action BasicAttackPressed;
        public event Action<int> AbilityPressed;
        public event Action LockOnPressed;
        public event Action InteractPressed;
        public event Action ToggleFocusPressed;
        public event Action PausePressed;
        public event Action ToggleDebugPressed;

        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool SprintHeld { get; private set; }
        public bool IsLookInputFromGamepad { get; private set; }

        private InputActionMap playerMap;
        private InputActionMap debugMap;

        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction jumpAction;
        private InputAction sprintAction;
        private InputAction dashAction;
        private InputAction basicAttackAction;
        private InputAction ability1Action;
        private InputAction ability2Action;
        private InputAction ability3Action;
        private InputAction lockOnAction;
        private InputAction interactAction;
        private InputAction toggleFocusAction;
        private InputAction pauseAction;
        private InputAction toggleDebugAction;
        private bool callbacksRegistered;

        private void Awake()
        {
            BuildOrResolveMaps();
            CacheActions();
        }

        private void OnEnable()
        {
            if (playerMap == null)
            {
                BuildOrResolveMaps();
                CacheActions();
            }

            RegisterCallbacks();
            playerMap?.Enable();
            debugMap?.Enable();
        }

        private void OnDisable()
        {
            UnregisterCallbacks();
            playerMap?.Disable();
            debugMap?.Disable();
            Move = Vector2.zero;
            Look = Vector2.zero;
            SprintHeld = false;
            IsLookInputFromGamepad = false;
            callbacksRegistered = false;
        }

        private void BuildOrResolveMaps()
        {
            if (inputActionsAsset != null)
            {
                playerMap = inputActionsAsset.FindActionMap("Player", true);
                debugMap = inputActionsAsset.FindActionMap("Debug", false);
                return;
            }

            var runtimeAsset = ScriptableObject.CreateInstance<InputActionAsset>();
            playerMap = new InputActionMap("Player");
            debugMap = new InputActionMap("Debug");

            AddPlayerActions(playerMap);
            AddDebugActions(debugMap);

            runtimeAsset.AddActionMap(playerMap);
            runtimeAsset.AddActionMap(debugMap);
            inputActionsAsset = runtimeAsset;
        }

        private void CacheActions()
        {
            moveAction = playerMap.FindAction("Move", true);
            lookAction = playerMap.FindAction("Look", true);
            jumpAction = playerMap.FindAction("Jump", true);
            sprintAction = playerMap.FindAction("Sprint", true);
            dashAction = playerMap.FindAction("Dash", true);
            basicAttackAction = playerMap.FindAction("BasicAttack", true);
            ability1Action = playerMap.FindAction("Ability1", true);
            ability2Action = playerMap.FindAction("Ability2", true);
            ability3Action = playerMap.FindAction("Ability3", true);
            lockOnAction = playerMap.FindAction("LockOn", true);
            interactAction = playerMap.FindAction("Interact", true);
            toggleFocusAction = playerMap.FindAction("ToggleFocus", true);
            pauseAction = playerMap.FindAction("Pause", true);

            if (debugMap != null)
            {
                toggleDebugAction = debugMap.FindAction("ToggleDebug", false);
            }
        }

        private void RegisterCallbacks()
        {
            if (callbacksRegistered)
            {
                return;
            }

            if (moveAction != null)
            {
                moveAction.performed += OnMovePerformed;
                moveAction.canceled += OnMoveCanceled;
            }

            if (lookAction != null)
            {
                lookAction.performed += OnLookPerformed;
                lookAction.canceled += OnLookCanceled;
            }

            if (sprintAction != null)
            {
                sprintAction.performed += OnSprintPerformed;
                sprintAction.canceled += OnSprintCanceled;
            }

            if (jumpAction != null)
            {
                jumpAction.performed += OnJumpPerformed;
            }

            if (dashAction != null)
            {
                dashAction.performed += OnDashPerformed;
            }

            if (basicAttackAction != null)
            {
                basicAttackAction.performed += OnBasicAttackPerformed;
            }

            if (ability1Action != null)
            {
                ability1Action.performed += OnAbility1Performed;
            }

            if (ability2Action != null)
            {
                ability2Action.performed += OnAbility2Performed;
            }

            if (ability3Action != null)
            {
                ability3Action.performed += OnAbility3Performed;
            }

            if (lockOnAction != null)
            {
                lockOnAction.performed += OnLockOnPerformed;
            }

            if (interactAction != null)
            {
                interactAction.performed += OnInteractPerformed;
            }

            if (toggleFocusAction != null)
            {
                toggleFocusAction.performed += OnToggleFocusPerformed;
            }

            if (pauseAction != null)
            {
                pauseAction.performed += OnPausePerformed;
            }

            if (toggleDebugAction != null)
            {
                toggleDebugAction.performed += OnToggleDebugPerformed;
            }

            callbacksRegistered = true;
        }

        private void UnregisterCallbacks()
        {
            if (!callbacksRegistered)
            {
                return;
            }

            if (moveAction != null)
            {
                moveAction.performed -= OnMovePerformed;
                moveAction.canceled -= OnMoveCanceled;
            }

            if (lookAction != null)
            {
                lookAction.performed -= OnLookPerformed;
                lookAction.canceled -= OnLookCanceled;
            }

            if (sprintAction != null)
            {
                sprintAction.performed -= OnSprintPerformed;
                sprintAction.canceled -= OnSprintCanceled;
            }

            if (jumpAction != null)
            {
                jumpAction.performed -= OnJumpPerformed;
            }

            if (dashAction != null)
            {
                dashAction.performed -= OnDashPerformed;
            }

            if (basicAttackAction != null)
            {
                basicAttackAction.performed -= OnBasicAttackPerformed;
            }

            if (ability1Action != null)
            {
                ability1Action.performed -= OnAbility1Performed;
            }

            if (ability2Action != null)
            {
                ability2Action.performed -= OnAbility2Performed;
            }

            if (ability3Action != null)
            {
                ability3Action.performed -= OnAbility3Performed;
            }

            if (lockOnAction != null)
            {
                lockOnAction.performed -= OnLockOnPerformed;
            }

            if (interactAction != null)
            {
                interactAction.performed -= OnInteractPerformed;
            }

            if (toggleFocusAction != null)
            {
                toggleFocusAction.performed -= OnToggleFocusPerformed;
            }

            if (pauseAction != null)
            {
                pauseAction.performed -= OnPausePerformed;
            }

            if (toggleDebugAction != null)
            {
                toggleDebugAction.performed -= OnToggleDebugPerformed;
            }

            callbacksRegistered = false;
        }

        private void OnMovePerformed(InputAction.CallbackContext context) => Move = context.ReadValue<Vector2>();
        private void OnMoveCanceled(InputAction.CallbackContext context) => Move = Vector2.zero;

        private void OnLookPerformed(InputAction.CallbackContext context)
        {
            Look = context.ReadValue<Vector2>();
            IsLookInputFromGamepad = context.control?.device is Gamepad;
        }

        private void OnLookCanceled(InputAction.CallbackContext context)
        {
            Look = Vector2.zero;
            IsLookInputFromGamepad = false;
        }

        private void OnSprintPerformed(InputAction.CallbackContext context) => SprintHeld = context.ReadValueAsButton();
        private void OnSprintCanceled(InputAction.CallbackContext context) => SprintHeld = false;

        private void OnJumpPerformed(InputAction.CallbackContext context) => JumpPressed?.Invoke();
        private void OnDashPerformed(InputAction.CallbackContext context) => DashPressed?.Invoke();
        private void OnBasicAttackPerformed(InputAction.CallbackContext context) => BasicAttackPressed?.Invoke();
        private void OnAbility1Performed(InputAction.CallbackContext context) => AbilityPressed?.Invoke(1);
        private void OnAbility2Performed(InputAction.CallbackContext context) => AbilityPressed?.Invoke(2);
        private void OnAbility3Performed(InputAction.CallbackContext context) => AbilityPressed?.Invoke(3);
        private void OnLockOnPerformed(InputAction.CallbackContext context) => LockOnPressed?.Invoke();
        private void OnInteractPerformed(InputAction.CallbackContext context) => InteractPressed?.Invoke();
        private void OnToggleFocusPerformed(InputAction.CallbackContext context) => ToggleFocusPressed?.Invoke();
        private void OnPausePerformed(InputAction.CallbackContext context) => PausePressed?.Invoke();
        private void OnToggleDebugPerformed(InputAction.CallbackContext context) => ToggleDebugPressed?.Invoke();

        private static void AddPlayerActions(InputActionMap map)
        {
            var move = map.AddAction("Move", InputActionType.Value);
            move.expectedControlType = "Vector2";
            move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            move.AddBinding("<Gamepad>/leftStick");

            var look = map.AddAction("Look", InputActionType.Value);
            look.expectedControlType = "Vector2";
            look.AddBinding("<Mouse>/delta");
            look.AddBinding("<Gamepad>/rightStick");

            var jump = map.AddAction("Jump", InputActionType.Button);
            jump.AddBinding("<Keyboard>/space");
            jump.AddBinding("<Gamepad>/buttonSouth");

            var sprint = map.AddAction("Sprint", InputActionType.Button);
            sprint.AddBinding("<Keyboard>/leftShift");
            sprint.AddBinding("<Gamepad>/leftStickPress");

            var dash = map.AddAction("Dash", InputActionType.Button);
            dash.AddBinding("<Keyboard>/leftCtrl");
            dash.AddBinding("<Gamepad>/buttonEast");

            var attack = map.AddAction("BasicAttack", InputActionType.Button);
            attack.AddBinding("<Mouse>/leftButton");
            attack.AddBinding("<Gamepad>/rightTrigger");

            map.AddAction("Ability1", InputActionType.Button).AddBinding("<Keyboard>/1");
            map.FindAction("Ability1", true).AddBinding("<Gamepad>/buttonWest");

            map.AddAction("Ability2", InputActionType.Button).AddBinding("<Keyboard>/2");
            map.FindAction("Ability2", true).AddBinding("<Gamepad>/buttonNorth");

            map.AddAction("Ability3", InputActionType.Button).AddBinding("<Keyboard>/3");
            map.FindAction("Ability3", true).AddBinding("<Gamepad>/leftShoulder");

            var lockOn = map.AddAction("LockOn", InputActionType.Button);
            lockOn.AddBinding("<Keyboard>/tab");
            lockOn.AddBinding("<Gamepad>/rightShoulder");

            var interact = map.AddAction("Interact", InputActionType.Button);
            interact.AddBinding("<Keyboard>/e");
            interact.AddBinding("<Gamepad>/buttonNorth");

            var toggleFocus = map.AddAction("ToggleFocus", InputActionType.Button);
            toggleFocus.AddBinding("<Keyboard>/q");
            toggleFocus.AddBinding("<Gamepad>/dpad/up");

            var pause = map.AddAction("Pause", InputActionType.Button);
            pause.AddBinding("<Keyboard>/escape");
            pause.AddBinding("<Gamepad>/start");
        }

        private static void AddDebugActions(InputActionMap map)
        {
            var toggleDebug = map.AddAction("ToggleDebug", InputActionType.Button);
            toggleDebug.AddBinding("<Keyboard>/backquote");
            toggleDebug.AddBinding("<Gamepad>/select");
        }
    }
}
