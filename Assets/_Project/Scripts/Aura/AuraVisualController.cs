using System.Collections.Generic;
using GreedIsland.Stats;
using UnityEngine;
using UnityEngine.Rendering;

namespace GreedIsland.Aura
{
    [DisallowMultipleComponent]
    public sealed class AuraVisualController : MonoBehaviour
    {
        [SerializeField] private AuraController auraController;
        [SerializeField] private AuraPool auraPool;
        [SerializeField] private Transform auraRoot;
        [SerializeField, Min(1f)] private float shellScaleMultiplier = 1.06f;
        [SerializeField, Min(0f)] private float pulseFrequency = 6f;
        [SerializeField, Min(0f)] private float baseAlpha = 0.06f;
        [SerializeField, Min(0f)] private float maxAlpha = 0.33f;

        private readonly List<ShellEntry> shellEntries = new(16);
        private Material sharedAuraMaterial;

        private void Awake()
        {
            auraController ??= GetComponent<AuraController>();
            auraPool ??= GetComponent<AuraPool>();
            CleanupLegacyAuraPrimitives();
            BuildAuraShells();
            RefreshColor();
        }

        private void OnEnable()
        {
            if (auraController != null)
            {
                auraController.ModeChanged += OnAuraModeChanged;
            }

            if (auraPool != null)
            {
                auraPool.ValueChanged += OnAuraValueChanged;
            }
        }

        private void OnDisable()
        {
            if (auraController != null)
            {
                auraController.ModeChanged -= OnAuraModeChanged;
            }

            if (auraPool != null)
            {
                auraPool.ValueChanged -= OnAuraValueChanged;
            }
        }

        private void LateUpdate()
        {
            if (auraPool == null)
            {
                return;
            }

            var auraRatio = auraPool.Max <= 0f ? 0f : Mathf.Clamp01(auraPool.Current / auraPool.Max);
            var modeMultiplier = ResolveModeIntensity(auraController != null ? auraController.CurrentMode : AuraMode.Neutral);
            var pulse = 1f + Mathf.Sin(Time.time * pulseFrequency) * 0.045f;
            var shellScale = shellScaleMultiplier * pulse;

            for (var i = 0; i < shellEntries.Count; i++)
            {
                var entry = shellEntries[i];
                if (entry.Transform == null || entry.Renderer == null)
                {
                    continue;
                }

                entry.Transform.localScale = entry.BaseScale * shellScale;
                entry.Renderer.enabled = auraRatio > 0.01f;
            }

            if (sharedAuraMaterial == null)
            {
                return;
            }

            var alpha = Mathf.Lerp(baseAlpha, maxAlpha, auraRatio) * modeMultiplier;
            var white = new Color(1f, 1f, 1f, Mathf.Clamp01(alpha));
            SetMaterialColor(sharedAuraMaterial, white);
        }

        private void OnAuraModeChanged(AuraMode mode)
        {
            RefreshColor();
        }

        private void OnAuraValueChanged(float current, float max)
        {
            RefreshColor();
        }

        private void BuildAuraShells()
        {
            if (auraRoot == null)
            {
                var existing = transform.Find("AuraShellRoot");
                if (existing != null)
                {
                    auraRoot = existing;
                }
                else
                {
                    auraRoot = new GameObject("AuraShellRoot").transform;
                    auraRoot.SetParent(transform, false);
                    auraRoot.localPosition = Vector3.zero;
                    auraRoot.localRotation = Quaternion.identity;
                    auraRoot.localScale = Vector3.one;
                }
            }

            EnsureMaterial();
            shellEntries.Clear();

            var sourceRoot = transform.Find("VisualRig");
            if (sourceRoot == null)
            {
                sourceRoot = transform;
            }

            var sourceFilters = sourceRoot.GetComponentsInChildren<MeshFilter>(true);
            for (var i = 0; i < sourceFilters.Length; i++)
            {
                var sourceFilter = sourceFilters[i];
                if (sourceFilter == null || sourceFilter.sharedMesh == null)
                {
                    continue;
                }

                if (sourceFilter.name.Contains("AuraShell"))
                {
                    continue;
                }

                var sourceRenderer = sourceFilter.GetComponent<Renderer>();
                if (sourceRenderer == null || !sourceRenderer.enabled)
                {
                    continue;
                }

                var shellTransform = EnsureShellForSource(sourceFilter.transform, sourceFilter.sharedMesh);
                if (shellTransform == null)
                {
                    continue;
                }

                shellEntries.Add(new ShellEntry(
                    shellTransform,
                    shellTransform.GetComponent<MeshRenderer>(),
                    shellTransform.localScale));
            }
        }

        private void CleanupLegacyAuraPrimitives()
        {
            var legacyRoot = transform.Find("AuraVisualRoot");
            if (legacyRoot != null)
            {
                Destroy(legacyRoot.gameObject);
            }
        }

        private Transform EnsureShellForSource(Transform source, Mesh mesh)
        {
            var shellName = $"{source.name}_AuraShell";
            var existing = source.Find(shellName);
            if (existing == null)
            {
                var shellObject = new GameObject(shellName, typeof(MeshFilter), typeof(MeshRenderer));
                existing = shellObject.transform;
                existing.SetParent(source, false);
            }

            existing.localPosition = Vector3.zero;
            existing.localRotation = Quaternion.identity;
            existing.localScale = Vector3.one;

            var filter = existing.GetComponent<MeshFilter>();
            filter.sharedMesh = mesh;

            var renderer = existing.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = sharedAuraMaterial;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            renderer.allowOcclusionWhenDynamic = false;

            var sourceRenderer = source.GetComponent<Renderer>();
            if (sourceRenderer != null)
            {
                renderer.sortingLayerID = sourceRenderer.sortingLayerID;
                renderer.sortingOrder = sourceRenderer.sortingOrder + 1;
                renderer.renderingLayerMask = sourceRenderer.renderingLayerMask;
            }

            return existing;
        }

        private void EnsureMaterial()
        {
            if (sharedAuraMaterial != null)
            {
                return;
            }

            var shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (shader == null)
            {
                shader = Shader.Find("Universal Render Pipeline/Unlit");
            }

            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }

            if (shader == null)
            {
                return;
            }

            sharedAuraMaterial = new Material(shader);
            sharedAuraMaterial.renderQueue = 3000;
            SetMaterialColor(sharedAuraMaterial, new Color(1f, 1f, 1f, baseAlpha));
        }

        private void RefreshColor()
        {
            if (sharedAuraMaterial == null || auraPool == null)
            {
                return;
            }

            var auraRatio = auraPool.Max <= 0f ? 0f : Mathf.Clamp01(auraPool.Current / auraPool.Max);
            var modeIntensity = ResolveModeIntensity(auraController != null ? auraController.CurrentMode : AuraMode.Neutral);
            var alpha = Mathf.Lerp(baseAlpha, maxAlpha, auraRatio) * modeIntensity;
            SetMaterialColor(sharedAuraMaterial, new Color(1f, 1f, 1f, Mathf.Clamp01(alpha)));
        }

        private static float ResolveModeIntensity(AuraMode mode)
        {
            return mode switch
            {
                AuraMode.Concealment => 0.75f,
                AuraMode.Reinforcement => 1.2f,
                AuraMode.Expansion => 1.12f,
                AuraMode.Perception => 1.05f,
                _ => 1f
            };
        }

        private static void SetMaterialColor(Material material, Color color)
        {
            if (material == null)
            {
                return;
            }

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }
            else if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", color);
            }

            if (material.HasProperty("_EmissionColor"))
            {
                material.SetColor("_EmissionColor", color * 0.85f);
            }
        }

        private readonly struct ShellEntry
        {
            public ShellEntry(Transform transform, MeshRenderer renderer, Vector3 baseScale)
            {
                Transform = transform;
                Renderer = renderer;
                BaseScale = baseScale;
            }

            public Transform Transform { get; }
            public MeshRenderer Renderer { get; }
            public Vector3 BaseScale { get; }
        }
    }
}
