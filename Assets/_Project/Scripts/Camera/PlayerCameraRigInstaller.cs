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

            var orbitalFollow = cinemachineRig.GetComponent<CinemachineOrbitalFollow>();
            if (orbitalFollow == null)
            {
                orbitalFollow = cinemachineRig.AddComponent<CinemachineOrbitalFollow>();
            }
            orbitalFollow.Radius = 4.8f;
            orbitalFollow.TargetOffset = new Vector3(0.2f, 1.2f, 0f);
            orbitalFollow.VerticalAxis.Range = new Vector2(-30f, 70f);
            orbitalFollow.VerticalAxis.Center = 15f;

            var rotationComposer = cinemachineRig.GetComponent<CinemachineRotationComposer>();
            if (rotationComposer == null)
            {
                rotationComposer = cinemachineRig.AddComponent<CinemachineRotationComposer>();
            }
            rotationComposer.Damping = new Vector2(0.2f, 0.25f);

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
            [SerializeField, Min(0f)] private float yawSpeed = 115f;
            [SerializeField, Min(0f)] private float pitchSpeed = 95f;

            private void LateUpdate()
            {
                if (orbitalFollow == null || inputReader == null)
                {
                    return;
                }

                var look = inputReader.Look;
                if (look.sqrMagnitude <= 0.000001f)
                {
                    return;
                }

                var deltaTime = Mathf.Max(Time.deltaTime, 0.0001f);
                orbitalFollow.HorizontalAxis.Value += look.x * yawSpeed * deltaTime;
                orbitalFollow.VerticalAxis.Value -= look.y * pitchSpeed * deltaTime;

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
