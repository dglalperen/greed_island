using UnityEngine;

namespace GreedIsland.Camera
{
    public sealed class CameraTargetProvider : MonoBehaviour
    {
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private Transform cameraLookTarget;
        [SerializeField] private Transform abilityAimTarget;

        public Transform CameraRoot => cameraRoot;
        public Transform CameraLookTarget => cameraLookTarget;
        public Transform AbilityAimTarget => abilityAimTarget;

        private void Awake()
        {
            if (cameraRoot == null)
            {
                cameraRoot = CreateChild("CameraRoot", new Vector3(0f, 1.4f, 0f));
            }

            if (cameraLookTarget == null)
            {
                cameraLookTarget = CreateChild("CameraLookTarget", new Vector3(0f, 1.6f, 0f));
            }

            if (abilityAimTarget == null)
            {
                abilityAimTarget = CreateChild("AbilityAimTarget", new Vector3(0f, 1.5f, 1.25f));
            }
        }

        private Transform CreateChild(string name, Vector3 localPosition)
        {
            var child = new GameObject(name).transform;
            child.SetParent(transform, false);
            child.localPosition = localPosition;
            child.localRotation = Quaternion.identity;
            return child;
        }
    }
}
