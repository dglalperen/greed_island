using System.Collections.Generic;
using GreedIsland.Abilities;
using GreedIsland.Aura;
using GreedIsland.Camera;
using GreedIsland.Character;
using GreedIsland.Combat;
using GreedIsland.Input;
using GreedIsland.Stats;
using GreedIsland.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GreedIsland.Core
{
    public sealed class PrototypeArenaInstaller : MonoBehaviour
    {
        private const string InstallerRootName = "PrototypeRuntimeRoot";
        private static bool runtimeHookRegistered;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetRuntimeHooks()
        {
            runtimeHookRegistered = false;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterRuntimeHooks()
        {
            if (runtimeHookRegistered)
            {
                return;
            }

            runtimeHookRegistered = true;
            SceneManager.sceneLoaded += OnSceneLoaded;
            TryInstallForScene(SceneManager.GetActiveScene());
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInstallerAfterInitialSceneLoad()
        {
            TryInstallForScene(SceneManager.GetActiveScene());
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            TryInstallForScene(scene);
        }

        private static void TryInstallForScene(Scene scene)
        {
            if (!scene.IsValid() || !scene.isLoaded)
            {
                return;
            }

            if (!ShouldInstallForScene(scene.name))
            {
                return;
            }

            if (FindInstallerInScene(scene) != null)
            {
                return;
            }

            var installerRoot = new GameObject(InstallerRootName);
            SceneManager.MoveGameObjectToScene(installerRoot, scene);
            installerRoot.AddComponent<PrototypeArenaInstaller>();
        }

        private static PrototypeArenaInstaller FindInstallerInScene(Scene scene)
        {
            var roots = scene.GetRootGameObjects();
            for (var i = 0; i < roots.Length; i++)
            {
                if (roots[i].name != InstallerRootName)
                {
                    continue;
                }

                var installer = roots[i].GetComponent<PrototypeArenaInstaller>();
                if (installer != null)
                {
                    return installer;
                }

                return roots[i].AddComponent<PrototypeArenaInstaller>();
            }

            return null;
        }

        private void Start()
        {
            Install();
        }

        private void Install()
        {
            CreateEnvironmentIfMissing();
            var playerRefs = CreatePlayerIfMissing();
            var dummy = CreateTargetDummyIfMissing();
            CreatePhysicsPropsIfMissing();
            CreateHudIfMissing(playerRefs);
            CreateEventSystemIfMissing();
            PlaceCameraNearPlayer(playerRefs.PlayerRoot);
            EnsureCameraGameplaySettings();

            if (dummy != null)
            {
                dummy.transform.LookAt(playerRefs.PlayerRoot.transform.position);
            }
        }

        private static bool ShouldInstallForScene(string sceneName)
        {
            return sceneName == GameConstants.PrototypeSceneName || sceneName == "AbilitySandbox" || sceneName == "AnimationTest";
        }

        private void CreateEnvironmentIfMissing()
        {
            if (GameObject.Find("ArenaRoot") != null)
            {
                return;
            }

            var arenaRoot = new GameObject("ArenaRoot");

            var groundLayer = LayerMaskConfig.Ground;
            var worldLayer = LayerMaskConfig.WorldProp;

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "ArenaGround";
            ground.transform.SetParent(arenaRoot.transform);
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(GameConstants.ArenaSize.x / 10f, 1f, GameConstants.ArenaSize.z / 10f);
            SetLayerSafe(ground, groundLayer);
            ApplyColor(ground, new Color(0.17f, 0.2f, 0.22f, 1f));

            CreateWall(arenaRoot.transform, new Vector3(0f, 2f, 14f), new Vector3(30f, 4f, 1f), worldLayer);
            CreateWall(arenaRoot.transform, new Vector3(0f, 2f, -14f), new Vector3(30f, 4f, 1f), worldLayer);
            CreateWall(arenaRoot.transform, new Vector3(14f, 2f, 0f), new Vector3(1f, 4f, 30f), worldLayer);
            CreateWall(arenaRoot.transform, new Vector3(-14f, 2f, 0f), new Vector3(1f, 4f, 30f), worldLayer);

            CreateRamp(arenaRoot.transform, new Vector3(-6f, 0.5f, -5f), new Vector3(4f, 1f, 8f), Quaternion.Euler(0f, 0f, 15f), groundLayer);
            CreateRamp(arenaRoot.transform, new Vector3(6f, 0.75f, 5f), new Vector3(4f, 1.5f, 8f), Quaternion.Euler(0f, 0f, -15f), groundLayer);

            CreateColumn(arenaRoot.transform, new Vector3(-4f, 1.5f, 4f), 3f, worldLayer);
            CreateColumn(arenaRoot.transform, new Vector3(4f, 1.5f, -4f), 3f, worldLayer);
            CreateColumn(arenaRoot.transform, new Vector3(0f, 1.5f, 8f), 3f, worldLayer);
        }

        private PlayerRuntimeRefs CreatePlayerIfMissing()
        {
            var existingBrain = FindAnyObjectByType<PlayerBrain>();
            if (existingBrain != null)
            {
                EnsurePlayerPresentation(existingBrain.gameObject);
                return new PlayerRuntimeRefs(existingBrain.gameObject);
            }

            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.tag = GameTags.Player;
            SetLayerSafe(player, LayerMaskConfig.Player);
            player.transform.position = new Vector3(0f, 1f, -8f);

            var primitiveCollider = player.GetComponent<Collider>();
            if (primitiveCollider != null)
            {
                Destroy(primitiveCollider);
            }

            ApplyColor(player, new Color(0.25f, 0.66f, 1f, 1f));

            var characterController = player.AddComponent<CharacterController>();
            characterController.height = 2f;
            characterController.radius = 0.34f;
            characterController.center = new Vector3(0f, 1f, 0f);
            characterController.stepOffset = 0.32f;
            characterController.slopeLimit = 55f;
            characterController.skinWidth = 0.05f;
            characterController.minMoveDistance = 0f;

            var health = player.AddComponent<HealthComponent>();
            var auraPool = player.AddComponent<AuraPool>();
            auraPool.Configure(GameConstants.DefaultAuraMax, GameConstants.DefaultAuraRegen, GameConstants.DefaultAuraRegenDelay);

            var auraController = player.AddComponent<AuraController>();
            player.AddComponent<PlayerStatsProvider>();
            player.AddComponent<PlayerMotor>();
            player.AddComponent<PlayerGroundChecker>();
            player.AddComponent<PlayerJumpController>();
            player.AddComponent<PlayerDashController>();
            player.AddComponent<PlayerRotationController>();
            player.AddComponent<PlayerAnimatorController>();
            var targetProvider = player.AddComponent<CameraTargetProvider>();
            player.AddComponent<LockOnTargetResolver>();
            var brain = player.AddComponent<PlayerBrain>();
            var inputReader = player.AddComponent<PlayerInputReader>();
            var abilityRunner = player.AddComponent<AbilityRunner>();
            player.AddComponent<PrototypeAbilityLoadout>();
            player.AddComponent<PlayerCameraRigInstaller>();
            var inputRouter = player.AddComponent<InputActionRouter>();
            player.AddComponent<PrototypeLimbAnimator>();
            player.AddComponent<AuraVisualController>();

            // Force references that are otherwise optional-at-runtime so startup is deterministic.
            health.Heal(GameConstants.DefaultHealthMax);
            _ = auraController.CurrentMode;
            _ = brain.CurrentMoveState;
            _ = inputReader.Move;
            _ = inputRouter.enabled;
            _ = abilityRunner.enabled;
            _ = targetProvider.CameraRoot;

            EnsurePlayerPresentation(player);
            return new PlayerRuntimeRefs(player);
        }

        private TargetDummyController CreateTargetDummyIfMissing()
        {
            var existingDummy = FindAnyObjectByType<TargetDummyController>();
            if (existingDummy != null)
            {
                return existingDummy;
            }

            var dummy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            dummy.name = "TargetDummy";
            dummy.tag = GameTags.Enemy;
            SetLayerSafe(dummy, LayerMaskConfig.Target >= 0 ? LayerMaskConfig.Target : LayerMaskConfig.Enemy);
            dummy.transform.position = new Vector3(0f, 1f, 6f);
            ApplyColor(dummy, new Color(1f, 0.56f, 0.32f, 1f));

            var rigidBody = dummy.AddComponent<Rigidbody>();
            rigidBody.mass = 60f;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;

            dummy.AddComponent<ForceReceiver>();
            dummy.AddComponent<HealthComponent>();
            dummy.AddComponent<DamageableActor>();
            var targetDummy = dummy.AddComponent<TargetDummyController>();
            dummy.AddComponent<Hurtbox>();

            return targetDummy;
        }

        private void CreatePhysicsPropsIfMissing()
        {
            if (GameObject.Find("PhysicsPropsRoot") != null)
            {
                return;
            }

            var root = new GameObject("PhysicsPropsRoot");
            var positions = new[]
            {
                new Vector3(-5f, 0.5f, 2f),
                new Vector3(-3f, 0.5f, 2.5f),
                new Vector3(3f, 0.5f, -1f),
                new Vector3(4.5f, 0.5f, -1.25f),
                new Vector3(1.5f, 0.5f, 3f),
                new Vector3(-1f, 0.5f, -3f)
            };

            for (var i = 0; i < positions.Length; i++)
            {
                var crate = GameObject.CreatePrimitive(PrimitiveType.Cube);
                crate.name = $"Crate_{i + 1:00}";
                crate.transform.SetParent(root.transform);
                crate.transform.position = positions[i];
                crate.transform.localScale = new Vector3(1f, 1f, 1f);
                crate.tag = GameTags.Interactable;
                SetLayerSafe(crate, LayerMaskConfig.WorldProp);
                ApplyColor(crate, new Color(0.74f, 0.68f, 0.53f, 1f));

                var rb = crate.AddComponent<Rigidbody>();
                rb.mass = 2.2f;
                rb.angularDamping = 0.35f;
                rb.linearDamping = 0.15f;
            }
        }

        private void CreateHudIfMissing(PlayerRuntimeRefs refs)
        {
            if (FindAnyObjectByType<PlayerHudPresenter>() != null)
            {
                return;
            }

            var canvasObject = new GameObject("HUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            var leftPanel = CreatePanel(canvas.transform, "TopLeftPanel", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(20f, -20f), new Vector2(420f, 230f));
            var health = CreateLabeledSlider(leftPanel.transform, "Health", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(10f, -20f), new Color(0.87f, 0.16f, 0.16f, 1f));
            var aura = CreateLabeledSlider(leftPanel.transform, "Aura", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(10f, -70f), new Color(0.95f, 0.95f, 0.95f, 1f));

            var modeText = CreateText(leftPanel.transform, "AuraMode", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(12f, -116f), new Vector2(260f, 28f), 18, TextAnchor.MiddleLeft);
            modeText.text = "Mode: Neutral";

            var statusText = CreateText(leftPanel.transform, "AbilityStatus", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(12f, -148f), new Vector2(400f, 50f), 14, TextAnchor.UpperLeft);
            statusText.text = "1 Burst | 2 Guard toggle | 3 Sense Pulse";

            var cooldownContainer = CreatePanel(canvas.transform, "CooldownPanel", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-180f, 24f), new Vector2(360f, 110f));
            var cooldownViews = new List<CooldownBarView>(3);

            for (var i = 0; i < 3; i++)
            {
                var rowY = -10f - i * 34f;
                var cooldownRow = CreateLabeledSlider(cooldownContainer.transform, $"Ability {i + 1}", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(10f, rowY), new Color(0.95f, 0.87f, 0.22f, 1f));
                var cooldownView = cooldownRow.Root.AddComponent<CooldownBarView>();
                cooldownView.Configure(i + 1, cooldownRow.Slider, cooldownRow.Label, 8f);
                cooldownViews.Add(cooldownView);
            }

            var controlsPanel = CreatePanel(canvas.transform, "ControlsPanel", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-20f, -20f), new Vector2(430f, 290f));
            var controlsText = CreateInstructionText(controlsPanel.transform, "ControlsText", 14);
            controlsText.text =
                "Controls (F1 hide/show)\n" +
                "------------------------\n" +
                "WASD / Left Stick: Move\n" +
                "Mouse / Right Stick: Look\n" +
                "Space: Jump\n" +
                "Left Shift: Sprint\n" +
                "Left Ctrl: Dash\n" +
                "1: Aura Burst (offense)\n" +
                "2: Aura Guard (toggle defense)\n" +
                "3: Sense Pulse (utility)\n" +
                "Q: Cycle Aura Mode (Perception auto-pulses)\n" +
                "Tab: Lock target\n" +
                "Backquote (`): Toggle debug panel\n" +
                "Esc: Unlock cursor, LMB: lock cursor again";

            var instructionsPanel = canvasObject.AddComponent<QuickInstructionsPanel>();
            instructionsPanel.Configure(controlsPanel, true);

            var debugRoot = CreatePanel(canvas.transform, "DebugPanel", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-20f, 20f), new Vector2(360f, 250f));
            var debugText = CreateText(debugRoot.transform, "DebugText", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(10f, -10f), new Vector2(-20f, -220f), 14, TextAnchor.UpperLeft);

            var crosshair = CreateText(canvas.transform, "Crosshair", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-10f, -10f), new Vector2(20f, 20f), 20, TextAnchor.MiddleCenter);
            crosshair.text = "+";

            var healthView = health.Root.AddComponent<HealthBarView>();
            healthView.Configure(health.Slider, health.Label);

            var auraView = aura.Root.AddComponent<AuraBarView>();
            auraView.Configure(aura.Slider, aura.Label);

            var debugPanel = debugRoot.AddComponent<DebugStatePanel>();
            debugPanel.Configure(debugRoot.gameObject, debugText, true);

            var presenter = canvasObject.AddComponent<PlayerHudPresenter>();
            presenter.ConfigureViews(
                healthView,
                auraView,
                cooldownViews.ToArray(),
                modeText,
                statusText,
                debugPanel);
            presenter.ConfigureSources(
                refs.Health,
                refs.AuraPool,
                refs.AuraController,
                refs.AbilityRunner);

            var crosshairView = crosshair.gameObject.AddComponent<CrosshairView>();
            crosshairView.SetFocused(false);

            var dummyPresenter = CreateDummyHealthUi(canvas.transform);
            var dummyHealth = FindAnyObjectByType<TargetDummyController>()?.GetComponent<HealthComponent>();
            if (dummyHealth != null)
            {
                dummyPresenter.gameObject.SetActive(true);
            }
        }

        private DummyHealthPresenter CreateDummyHealthUi(Transform canvas)
        {
            var panel = CreatePanel(canvas, "DummyHealthPanel", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-180f, -20f), new Vector2(360f, 40f));
            var row = CreateLabeledSlider(panel.transform, "Dummy", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(10f, -10f), new Color(1f, 0.43f, 0.43f, 1f));
            var presenter = row.Root.AddComponent<DummyHealthPresenter>();
            var dummyHealth = FindAnyObjectByType<TargetDummyController>()?.GetComponent<HealthComponent>();

            row.Label.alignment = TextAnchor.MiddleRight;
            presenter.Configure(dummyHealth, row.Slider);

            return presenter;
        }

        private void CreateEventSystemIfMissing()
        {
            if (FindAnyObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            eventSystemObject.transform.SetAsFirstSibling();
        }

        private static void PlaceCameraNearPlayer(GameObject player)
        {
            var mainCamera = UnityEngine.Camera.main;
            if (mainCamera == null || player == null)
            {
                return;
            }

            if (mainCamera.transform.position.sqrMagnitude < 0.001f)
            {
                mainCamera.transform.position = player.transform.position + new Vector3(0f, 2.2f, -4.8f);
            }
        }

        private static void EnsureCameraGameplaySettings()
        {
            var mainCamera = UnityEngine.Camera.main;
            if (mainCamera == null)
            {
                return;
            }

            if (mainCamera.GetComponent<GameplayCursorLock>() == null)
            {
                mainCamera.gameObject.AddComponent<GameplayCursorLock>();
            }
        }

        private static void EnsurePlayerPresentation(GameObject player)
        {
            if (player == null)
            {
                return;
            }

            if (player.GetComponent<PrototypeLimbAnimator>() == null)
            {
                player.AddComponent<PrototypeLimbAnimator>();
            }

            if (player.GetComponent<AuraVisualController>() == null)
            {
                player.AddComponent<AuraVisualController>();
            }

        }

        private static GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            var panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);

            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = anchorMax;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;

            var image = panel.GetComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.45f);

            return panel;
        }

        private static SliderRow CreateLabeledSlider(Transform parent, string labelPrefix, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Color fillColor)
        {
            var root = new GameObject($"{labelPrefix}Row", typeof(RectTransform));
            root.transform.SetParent(parent, false);

            var rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = anchorMin;
            rootRect.anchorMax = anchorMax;
            rootRect.pivot = anchorMax;
            rootRect.anchoredPosition = anchoredPosition;
            rootRect.sizeDelta = new Vector2(340f, 28f);

            var label = CreateText(root.transform, "Label", new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(88f, 0f), 14, TextAnchor.MiddleLeft);
            label.text = labelPrefix;

            var sliderRoot = new GameObject("Slider", typeof(RectTransform), typeof(Slider));
            sliderRoot.transform.SetParent(root.transform, false);
            var sliderRect = sliderRoot.GetComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0f, 0f);
            sliderRect.anchorMax = new Vector2(1f, 1f);
            sliderRect.offsetMin = new Vector2(92f, 3f);
            sliderRect.offsetMax = new Vector2(-4f, -3f);

            var background = new GameObject("Background", typeof(RectTransform), typeof(Image));
            background.transform.SetParent(sliderRoot.transform, false);
            var bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            var bgImage = background.GetComponent<Image>();
            bgImage.color = new Color(1f, 1f, 1f, 0.22f);

            var fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(sliderRoot.transform, false);
            var fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0f, 0f);
            fillAreaRect.anchorMax = new Vector2(1f, 1f);
            fillAreaRect.offsetMin = new Vector2(2f, 2f);
            fillAreaRect.offsetMax = new Vector2(-2f, -2f);

            var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(fillArea.transform, false);
            var fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0f, 0f);
            fillRect.anchorMax = new Vector2(1f, 1f);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            var fillImage = fill.GetComponent<Image>();
            fillImage.color = fillColor;

            var slider = sliderRoot.GetComponent<Slider>();
            slider.targetGraphic = fillImage;
            slider.fillRect = fillRect;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;

            return new SliderRow(root, slider, label);
        }

        private static Text CreateText(
            Transform parent,
            string name,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 anchoredPosition,
            Vector2 sizeDelta,
            int fontSize,
            TextAnchor alignment)
        {
            var textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);

            var rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = anchorMax;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;

            var text = textObject.GetComponent<Text>();
            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            text.font = font;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;

            return text;
        }

        private static Text CreateInstructionText(Transform parent, string name, int fontSize)
        {
            var textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);

            var rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = new Vector2(12f, 10f);
            rect.offsetMax = new Vector2(-12f, -10f);

            var text = textObject.GetComponent<Text>();
            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            text.font = font;
            text.fontSize = fontSize;
            text.alignment = TextAnchor.UpperLeft;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        private static void CreateWall(Transform parent, Vector3 position, Vector3 scale, int layer)
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.SetParent(parent);
            wall.transform.position = position;
            wall.transform.localScale = scale;
            SetLayerSafe(wall, layer);
            ApplyColor(wall, new Color(0.24f, 0.26f, 0.3f, 1f));
        }

        private static void CreateRamp(Transform parent, Vector3 position, Vector3 scale, Quaternion rotation, int layer)
        {
            var ramp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ramp.transform.SetParent(parent);
            ramp.transform.position = position;
            ramp.transform.localScale = scale;
            ramp.transform.rotation = rotation;
            SetLayerSafe(ramp, layer);
            ApplyColor(ramp, new Color(0.2f, 0.24f, 0.27f, 1f));
        }

        private static void CreateColumn(Transform parent, Vector3 position, float height, int layer)
        {
            var column = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            column.transform.SetParent(parent);
            column.transform.position = position;
            column.transform.localScale = new Vector3(1f, height * 0.5f, 1f);
            SetLayerSafe(column, layer);
            ApplyColor(column, new Color(0.3f, 0.3f, 0.35f, 1f));
        }

        private static void ApplyColor(GameObject target, Color color)
        {
            var renderer = target.GetComponentInChildren<Renderer>();
            if (renderer == null)
            {
                return;
            }

            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            if (shader == null)
            {
                return;
            }

            var material = new Material(shader);
            material.SetColor("_BaseColor", color);
            renderer.sharedMaterial = material;
        }

        private static void SetLayerSafe(GameObject target, int layer)
        {
            if (layer < 0)
            {
                return;
            }

            target.layer = layer;
        }

        private readonly struct SliderRow
        {
            public SliderRow(GameObject root, Slider slider, Text label)
            {
                Root = root;
                Slider = slider;
                Label = label;
            }

            public GameObject Root { get; }
            public Slider Slider { get; }
            public Text Label { get; }
        }

        private readonly struct PlayerRuntimeRefs
        {
            public PlayerRuntimeRefs(GameObject playerRoot)
            {
                PlayerRoot = playerRoot;
                Health = playerRoot.GetComponent<HealthComponent>();
                AuraPool = playerRoot.GetComponent<AuraPool>();
                AuraController = playerRoot.GetComponent<AuraController>();
                AbilityRunner = playerRoot.GetComponent<AbilityRunner>();
            }

            public GameObject PlayerRoot { get; }
            public HealthComponent Health { get; }
            public AuraPool AuraPool { get; }
            public AuraController AuraController { get; }
            public AbilityRunner AbilityRunner { get; }
        }

    }
}
