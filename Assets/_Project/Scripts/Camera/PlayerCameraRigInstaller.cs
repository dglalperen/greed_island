using GreedIsland.Input;
using Unity.Cinemachine;
using UnityEngine;

namespace GreedIsland.Camera
{
    public sealed class PlayerCameraRigInstaller : MonoBehaviour
    {
        [SerializeField] private CameraTargetProvider targetProvider;
        [SerializeField] private ThirdPersonCameraController fallbackCameraController;
        [SerializeField] private bool autoCreateMainCamera = true;
        [SerializeField] private bool preferCinemachine = true;

        private void Awake()
        {
            if (targetProvider == null)
            {
                targetProvider = GetComponent<CameraTargetProvider>();
            }

            var mainCamera = ResolveMainCamera();
            if (mainCamera == null)
            {
                return;
            }

            if (preferCinemachine && TrySetupCinemachine(mainCamera))
            {
                if (fallbackCameraController != null)
                {
                    fallbackCameraController.enabled = false;
                }

                return;
            }

            SetupFallback(mainCamera);
        }

        private UnityEngine.Camera ResolveMainCamera()
        {
            var mainCamera = UnityEngine.Camera.main;
            if (mainCamera != null)
            {
                return mainCamera;
            }

            if (!autoCreateMainCamera)
            {
                return null;
            }

            var cameraObject = new GameObject("Main Camera", typeof(UnityEngine.Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            return cameraObject.GetComponent<UnityEngine.Camera>();
        }

        private bool TrySetupCinemachine(UnityEngine.Camera mainCamera)
        {
            if (targetProvider == null)
            {
                return false;
            }

            var brain = mainCamera.GetComponent<CinemachineBrain>();
            if (brain == null)
            {
                brain = mainCamera.gameObject.AddComponent<CinemachineBrain>();
            }

            var cinemachineRig = GameObject.Find("CM_PlayerCamera");
            if (cinemachineRig == null)
            {
                cinemachineRig = new GameObject("CM_PlayerCamera");
            }

            var cmCamera = cinemachineRig.GetComponent<CinemachineCamera>();
            if (cmCamera == null)
            {
                cmCamera = cinemachineRig.AddComponent<CinemachineCamera>();
            }

            cmCamera.Target = new CameraTarget
            {
                TrackingTarget = targetProvider.CameraRoot,
                LookAtTarget = targetProvider.CameraLookTarget,
                CustomLookAtTarget = true
            };
            cmCamera.Priority = 100;

            var fallbackControllers = mainCamera.GetComponents<ThirdPersonCameraController>();
            for (var i = 0; i < fallbackControllers.Length; i++)
            {
                fallbackControllers[i].enabled = false;
            }

            var orbitalFollow = cinemachineRig.GetComponent<CinemachineOrbitalFollow>();
            if (orbitalFollow == null)
            {
                orbitalFollow = cinemachineRig.AddComponent<CinemachineOrbitalFollow>();
            }
            orbitalFollow.Radius = 5.15f;
            orbitalFollow.TargetOffset = new Vector3(0.55f, 1.35f, 0f);
            orbitalFollow.VerticalAxis.Range = new Vector2(-25f, 68f);
            orbitalFollow.VerticalAxis.Center = 18f;
            orbitalFollow.HorizontalAxis.Value = targetProvider.transform.eulerAngles.y;
            orbitalFollow.VerticalAxis.Value = 18f;

            var rotationComposer = cinemachineRig.GetComponent<CinemachineRotationComposer>();
            if (rotationComposer == null)
            {
                rotationComposer = cinemachineRig.AddComponent<CinemachineRotationComposer>();
            }
            rotationComposer.Damping = new Vector2(0.1f, 0.12f);

            var inputBridge = cinemachineRig.GetComponent<CinemachineOrbitBridge>();
            if (inputBridge == null)
            {
                inputBridge = cinemachineRig.AddComponent<CinemachineOrbitBridge>();
            }

            var inputReader = GetComponent<PlayerInputReader>();
            inputBridge.Configure(orbitalFollow, inputReader);

            var deoccluder = cinemachineRig.GetComponent<CinemachineDeoccluder>();
            if (deoccluder == null)
            {
                deoccluder = cinemachineRig.AddComponent<CinemachineDeoccluder>();
            }

            return true;
        }

        private void SetupFallback(UnityEngine.Camera mainCamera)
        {
            if (fallbackCameraController == null)
            {
                fallbackCameraController = mainCamera.GetComponent<ThirdPersonCameraController>();
                if (fallbackCameraController == null)
                {
                    fallbackCameraController = mainCamera.gameObject.AddComponent<ThirdPersonCameraController>();
                }
            }

            fallbackCameraController.enabled = true;
            fallbackCameraController.SetTarget(targetProvider);
        }

        [DisallowMultipleComponent]
        private sealed class CinemachineOrbitBridge : MonoBehaviour
        {
            [SerializeField] private PlayerInputReader inputReader;
            [SerializeField] private CinemachineOrbitalFollow orbitalFollow;
            [SerializeField, Min(0f)] private float mouseYawSensitivity = 0.12f;
            [SerializeField, Min(0f)] private float mousePitchSensitivity = 0.1f;
            [SerializeField, Min(0f)] private float gamepadYawSpeed = 150f;
            [SerializeField, Min(0f)] private float gamepadPitchSpeed = 130f;
            [SerializeField, Min(0f)] private float lookSmoothing = 18f;

            private Vector2 smoothedLook;

            private void LateUpdate()
            {
                if (orbitalFollow == null || inputReader == null)
                {
                    return;
                }

                var look = inputReader.Look;
                var deltaTime = Mathf.Max(Time.deltaTime, 0.0001f);
                var smoothing = 1f - Mathf.Exp(-lookSmoothing * deltaTime);
                smoothedLook = Vector2.Lerp(smoothedLook, look, smoothing);

                if (smoothedLook.sqrMagnitude <= 0.000001f)
                {
                    return;
                }

                if (inputReader.IsLookInputFromGamepad)
                {
                    orbitalFollow.HorizontalAxis.Value += smoothedLook.x * gamepadYawSpeed * deltaTime;
                    orbitalFollow.VerticalAxis.Value -= smoothedLook.y * gamepadPitchSpeed * deltaTime;
                }
                else
                {
                    orbitalFollow.HorizontalAxis.Value += smoothedLook.x * mouseYawSensitivity;
                    orbitalFollow.VerticalAxis.Value -= smoothedLook.y * mousePitchSensitivity;
                }

                var verticalRange = orbitalFollow.VerticalAxis.Range;
                orbitalFollow.VerticalAxis.Value = Mathf.Clamp(orbitalFollow.VerticalAxis.Value, verticalRange.x, verticalRange.y);
            }

            public void Configure(CinemachineOrbitalFollow follow, PlayerInputReader reader)
            {
                orbitalFollow = follow;
                inputReader = reader;
            }
        }
    }
}
