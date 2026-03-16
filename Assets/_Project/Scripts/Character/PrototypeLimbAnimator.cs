using UnityEngine;

namespace GreedIsland.Character
{
    [DisallowMultipleComponent]
    public sealed class PrototypeLimbAnimator : MonoBehaviour
    {
        [SerializeField] private PlayerBrain playerBrain;
        [SerializeField] private bool hideBaseCapsuleRenderer = true;
        [SerializeField, Min(0f)] private float strideAngle = 34f;
        [SerializeField, Min(0f)] private float armSwingMultiplier = 0.8f;
        [SerializeField, Min(0f)] private float gaitFrequency = 7f;
        [SerializeField, Min(0f)] private float idleSwayFrequency = 1.8f;
        [SerializeField, Min(0f)] private float bobAmplitude = 0.045f;

        private Transform visualRoot;
        private Transform torso;
        private Transform head;
        private Transform leftArm;
        private Transform rightArm;
        private Transform leftLeg;
        private Transform rightLeg;

        private Vector3 visualRootBaseLocalPos;
        private Quaternion torsoBaseLocalRotation;
        private Quaternion headBaseLocalRotation;
        private Quaternion leftArmBaseLocalRotation;
        private Quaternion rightArmBaseLocalRotation;
        private Quaternion leftLegBaseLocalRotation;
        private Quaternion rightLegBaseLocalRotation;

        private void Awake()
        {
            playerBrain ??= GetComponent<PlayerBrain>();
            BuildRigIfMissing();
            CacheBasePose();
            HideBaseMeshIfConfigured();
        }

        private void Update()
        {
            if (visualRoot == null)
            {
                return;
            }

            if (playerBrain == null)
            {
                playerBrain = GetComponent<PlayerBrain>();
            }

            var speed = playerBrain != null ? playerBrain.CurrentSpeed : 0f;
            var grounded = playerBrain == null || playerBrain.IsGrounded;
            var speed01 = Mathf.Clamp01(speed / 7.5f);
            var cycleRate = Mathf.Lerp(idleSwayFrequency, gaitFrequency, speed01);
            var cycle = Mathf.Sin(Time.time * cycleRate);

            var legSwing = cycle * strideAngle * speed01;
            var armSwing = legSwing * armSwingMultiplier;
            var torsoLean = Mathf.Lerp(0f, 8f, speed01);
            var bob = Mathf.Sin(Time.time * cycleRate * 2f) * bobAmplitude * speed01;

            if (!grounded)
            {
                leftLeg.localRotation = leftLegBaseLocalRotation * Quaternion.Euler(-15f, 0f, 0f);
                rightLeg.localRotation = rightLegBaseLocalRotation * Quaternion.Euler(20f, 0f, 0f);
                leftArm.localRotation = leftArmBaseLocalRotation * Quaternion.Euler(-8f, 0f, -4f);
                rightArm.localRotation = rightArmBaseLocalRotation * Quaternion.Euler(-8f, 0f, 4f);
                torso.localRotation = torsoBaseLocalRotation * Quaternion.Euler(6f, 0f, 0f);
                head.localRotation = headBaseLocalRotation * Quaternion.Euler(-4f, 0f, 0f);
                visualRoot.localPosition = visualRootBaseLocalPos + new Vector3(0f, bobAmplitude * 0.25f, 0f);
                return;
            }

            leftLeg.localRotation = leftLegBaseLocalRotation * Quaternion.Euler(legSwing, 0f, 0f);
            rightLeg.localRotation = rightLegBaseLocalRotation * Quaternion.Euler(-legSwing, 0f, 0f);
            leftArm.localRotation = leftArmBaseLocalRotation * Quaternion.Euler(-armSwing, 0f, -6f);
            rightArm.localRotation = rightArmBaseLocalRotation * Quaternion.Euler(armSwing, 0f, 6f);
            torso.localRotation = torsoBaseLocalRotation * Quaternion.Euler(torsoLean, cycle * 3.5f * speed01, 0f);
            head.localRotation = headBaseLocalRotation * Quaternion.Euler(-torsoLean * 0.55f, 0f, 0f);
            visualRoot.localPosition = visualRootBaseLocalPos + new Vector3(0f, bob, 0f);
        }

        private void BuildRigIfMissing()
        {
            var existing = transform.Find("VisualRig");
            if (existing != null)
            {
                visualRoot = existing;
                torso = existing.Find("Torso");
                head = existing.Find("Head");
                leftArm = existing.Find("Arm_L");
                rightArm = existing.Find("Arm_R");
                leftLeg = existing.Find("Leg_L");
                rightLeg = existing.Find("Leg_R");
                return;
            }

            visualRoot = new GameObject("VisualRig").transform;
            visualRoot.SetParent(transform, false);
            visualRoot.localPosition = new Vector3(0f, 0.04f, 0f);

            torso = CreatePart(visualRoot, "Torso", PrimitiveType.Capsule, new Vector3(0f, 1.05f, 0f), new Vector3(0.6f, 0.65f, 0.42f), new Color(0.15f, 0.54f, 0.88f, 1f));
            head = CreatePart(visualRoot, "Head", PrimitiveType.Sphere, new Vector3(0f, 1.78f, 0.03f), new Vector3(0.45f, 0.45f, 0.45f), new Color(0.91f, 0.83f, 0.68f, 1f));

            leftArm = CreatePart(visualRoot, "Arm_L", PrimitiveType.Capsule, new Vector3(-0.45f, 1.2f, 0f), new Vector3(0.22f, 0.45f, 0.22f), new Color(0.12f, 0.46f, 0.77f, 1f));
            rightArm = CreatePart(visualRoot, "Arm_R", PrimitiveType.Capsule, new Vector3(0.45f, 1.2f, 0f), new Vector3(0.22f, 0.45f, 0.22f), new Color(0.12f, 0.46f, 0.77f, 1f));

            leftLeg = CreatePart(visualRoot, "Leg_L", PrimitiveType.Capsule, new Vector3(-0.18f, 0.46f, 0f), new Vector3(0.24f, 0.5f, 0.24f), new Color(0.12f, 0.24f, 0.33f, 1f));
            rightLeg = CreatePart(visualRoot, "Leg_R", PrimitiveType.Capsule, new Vector3(0.18f, 0.46f, 0f), new Vector3(0.24f, 0.5f, 0.24f), new Color(0.12f, 0.24f, 0.33f, 1f));
        }

        private void CacheBasePose()
        {
            if (visualRoot == null || torso == null || head == null || leftArm == null || rightArm == null || leftLeg == null || rightLeg == null)
            {
                return;
            }

            visualRootBaseLocalPos = visualRoot.localPosition;
            torsoBaseLocalRotation = torso.localRotation;
            headBaseLocalRotation = head.localRotation;
            leftArmBaseLocalRotation = leftArm.localRotation;
            rightArmBaseLocalRotation = rightArm.localRotation;
            leftLegBaseLocalRotation = leftLeg.localRotation;
            rightLegBaseLocalRotation = rightLeg.localRotation;
        }

        private void HideBaseMeshIfConfigured()
        {
            if (!hideBaseCapsuleRenderer)
            {
                return;
            }

            var baseRenderer = GetComponent<Renderer>();
            if (baseRenderer != null)
            {
                baseRenderer.enabled = false;
            }
        }

        private static Transform CreatePart(Transform parent, string name, PrimitiveType primitiveType, Vector3 localPosition, Vector3 localScale, Color color)
        {
            var part = GameObject.CreatePrimitive(primitiveType);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localRotation = Quaternion.identity;
            part.transform.localScale = localScale;

            var collider = part.GetComponent<Collider>();
            if (collider != null)
            {
                Object.Destroy(collider);
            }

            var renderer = part.GetComponent<Renderer>();
            if (renderer != null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null)
                {
                    shader = Shader.Find("Standard");
                }

                if (shader != null)
                {
                    var material = new Material(shader);
                    if (material.HasProperty("_BaseColor"))
                    {
                        material.SetColor("_BaseColor", color);
                    }
                    else if (material.HasProperty("_Color"))
                    {
                        material.SetColor("_Color", color);
                    }

                    renderer.sharedMaterial = material;
                }
            }

            return part.transform;
        }
    }
}
