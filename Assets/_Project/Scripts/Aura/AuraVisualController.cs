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

        [Header("Shell")]
        [SerializeField, Min(1f)] private float shellScaleMultiplier = 1.03f;
        [SerializeField, Min(0f)] private float shellPulseFrequency = 6f;
        [SerializeField, Min(0f)] private float shellBaseAlpha = 0.04f;
        [SerializeField, Min(0f)] private float shellMaxAlpha = 0.26f;

        [Header("Smoke Particles")]
        [SerializeField, Min(0f)] private float particleBaseRate = 10f;
        [SerializeField, Min(0f)] private float particleMaxRate = 56f;
        [SerializeField, Min(0f)] private float particleBaseSpeed = 0.02f;
        [SerializeField, Min(0f)] private float particleMaxSpeed = 0.22f;
        [SerializeField, Min(0f)] private float particleBaseSize = 0.015f;
        [SerializeField, Min(0f)] private float particleMaxSize = 0.05f;

        private readonly List<ShellEntry> shellEntries = new(16);
        private readonly List<ParticleSystem> auraParticles = new(16);
        private Material sharedShellMaterial;
        private Material sharedParticleMaterial;

        private void Awake()
        {
            auraController ??= GetComponent<AuraController>();
            auraPool ??= GetComponent<AuraPool>();
            CleanupLegacyAuraPrimitives();
            BuildAuraVisuals();
            RefreshVisualState();
        }

        private void OnEnable()
        {
            if (auraController != null)
            {
                auraController.ModeChanged += OnAuraModeChanged;
                auraController.TechniqueChanged += OnTechniqueChanged;
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
                auraController.TechniqueChanged -= OnTechniqueChanged;
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
            var visualIntensity = auraController != null ? auraController.AuraVisualIntensity : 1f;
            var visible = auraController == null || auraController.IsAuraVisible;
            var active = visible && auraRatio > 0.01f;

            AnimateShell(auraRatio, visualIntensity, active);
            AnimateParticles(auraRatio, visualIntensity, active);
        }

        private void OnAuraModeChanged(AuraMode mode)
        {
            RefreshVisualState();
        }

        private void OnTechniqueChanged(NenTechnique technique)
        {
            RefreshVisualState();
        }

        private void OnAuraValueChanged(float current, float max)
        {
            RefreshVisualState();
        }

        private void BuildAuraVisuals()
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

            EnsureMaterials();
            shellEntries.Clear();
            auraParticles.Clear();

            var sourceRoot = transform.Find("VisualRig");
            if (sourceRoot == null)
            {
                sourceRoot = transform;
            }

            var sourceFilters = sourceRoot.GetComponentsInChildren<MeshFilter>(true);
            for (var i = 0; i < sourceFilters.Length; i++)
            {
                var sourceFilter = sourceFilters[i];
                if (sourceFilter == null || sourceFilter.sharedMesh == null || sourceFilter.name.Contains("AuraShell"))
                {
                    continue;
                }

                var sourceRenderer = sourceFilter.GetComponent<Renderer>();
                if (sourceRenderer == null || !sourceRenderer.enabled)
                {
                    continue;
                }

                var meshRenderer = sourceRenderer as MeshRenderer;
                if (meshRenderer == null)
                {
                    continue;
                }

                var shellTransform = EnsureShellForSource(sourceFilter.transform, sourceFilter.sharedMesh);
                if (shellTransform != null)
                {
                    shellEntries.Add(new ShellEntry(shellTransform, shellTransform.GetComponent<MeshRenderer>(), shellTransform.localScale));
                }

                var particle = EnsureParticleForSource(sourceFilter.transform, meshRenderer);
                if (particle != null)
                {
                    auraParticles.Add(particle);
                }
            }
        }

        private void AnimateShell(float auraRatio, float intensity, bool active)
        {
            var pulse = 1f + Mathf.Sin(Time.time * shellPulseFrequency) * 0.04f;
            var scale = shellScaleMultiplier * pulse;

            for (var i = 0; i < shellEntries.Count; i++)
            {
                var entry = shellEntries[i];
                if (entry.Transform == null || entry.Renderer == null)
                {
                    continue;
                }

                entry.Transform.localScale = entry.BaseScale * scale;
                entry.Renderer.enabled = active;
            }

            if (sharedShellMaterial == null)
            {
                return;
            }

            var alpha = Mathf.Lerp(shellBaseAlpha, shellMaxAlpha, auraRatio) * intensity;
            SetMaterialColor(sharedShellMaterial, new Color(1f, 1f, 1f, Mathf.Clamp01(alpha)));
        }

        private void AnimateParticles(float auraRatio, float intensity, bool active)
        {
            var emissionRate = Mathf.Lerp(particleBaseRate, particleMaxRate, auraRatio) * intensity;
            var speed = Mathf.Lerp(particleBaseSpeed, particleMaxSpeed, auraRatio) * intensity;
            var size = Mathf.Lerp(particleBaseSize, particleMaxSize, auraRatio);

            for (var i = 0; i < auraParticles.Count; i++)
            {
                var particle = auraParticles[i];
                if (particle == null)
                {
                    continue;
                }

                var main = particle.main;
                main.startSpeed = speed;
                main.startSize = size;

                var emission = particle.emission;
                emission.rateOverTime = emissionRate;

                if (active)
                {
                    if (!particle.isPlaying)
                    {
                        particle.Play();
                    }
                }
                else if (particle.isPlaying)
                {
                    particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
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
            renderer.sharedMaterial = sharedShellMaterial;
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

        private ParticleSystem EnsureParticleForSource(Transform source, MeshRenderer sourceRenderer)
        {
            var particleName = $"{source.name}_AuraSmoke";
            var existing = source.Find(particleName);
            if (existing == null)
            {
                existing = new GameObject(particleName).transform;
                existing.SetParent(source, false);
                existing.localPosition = Vector3.zero;
                existing.localRotation = Quaternion.identity;
                existing.localScale = Vector3.one;
            }

            var particle = existing.GetComponent<ParticleSystem>();
            if (particle == null)
            {
                particle = existing.gameObject.AddComponent<ParticleSystem>();
            }

            ConfigureParticleSystem(particle, sourceRenderer);
            return particle;
        }

        private void ConfigureParticleSystem(ParticleSystem particle, MeshRenderer sourceRenderer)
        {
            var main = particle.main;
            main.playOnAwake = false;
            main.loop = true;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.45f, 1.1f);
            main.startSpeed = particleBaseSpeed;
            main.startSize = particleBaseSize;
            main.maxParticles = 520;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            main.gravityModifier = 0f;

            var emission = particle.emission;
            emission.enabled = true;
            emission.rateOverTime = particleBaseRate;

            var shape = particle.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.MeshRenderer;
            shape.meshRenderer = sourceRenderer;
            shape.meshShapeType = ParticleSystemMeshShapeType.Vertex;
            shape.normalOffset = 0f;
            shape.randomDirectionAmount = 0.18f;
            shape.sphericalDirectionAmount = 0.12f;

            var noise = particle.noise;
            noise.enabled = true;
            noise.strength = 0.16f;
            noise.frequency = 0.55f;
            noise.scrollSpeed = 0.22f;
            noise.octaveCount = 1;

            var colorOverLifetime = particle.colorOverLifetime;
            colorOverLifetime.enabled = true;
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(
                new Gradient
                {
                    alphaKeys = new[]
                    {
                        new GradientAlphaKey(0f, 0f),
                        new GradientAlphaKey(0.25f, 0.2f),
                        new GradientAlphaKey(0.15f, 0.7f),
                        new GradientAlphaKey(0f, 1f)
                    },
                    colorKeys = new[]
                    {
                        new GradientColorKey(Color.white, 0f),
                        new GradientColorKey(Color.white, 1f)
                    }
                });

            var renderer = particle.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.material = sharedParticleMaterial;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.allowRoll = true;
            renderer.sortingFudge = 7f;
        }

        private void EnsureMaterials()
        {
            if (sharedShellMaterial == null)
            {
                var shellShader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
                if (shellShader == null)
                {
                    shellShader = Shader.Find("Universal Render Pipeline/Unlit");
                }

                if (shellShader != null)
                {
                    sharedShellMaterial = new Material(shellShader) { renderQueue = 3000 };
                    SetMaterialColor(sharedShellMaterial, new Color(1f, 1f, 1f, shellBaseAlpha));
                }
            }

            if (sharedParticleMaterial != null)
            {
                return;
            }

            var particleShader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (particleShader == null)
            {
                particleShader = Shader.Find("Particles/Standard Unlit");
            }

            if (particleShader == null)
            {
                particleShader = Shader.Find("Unlit/Color");
            }

            if (particleShader == null)
            {
                return;
            }

            sharedParticleMaterial = new Material(particleShader) { renderQueue = 3000 };
            SetMaterialColor(sharedParticleMaterial, new Color(1f, 1f, 1f, 0.35f));
        }

        private void RefreshVisualState()
        {
            if (sharedShellMaterial == null || auraPool == null)
            {
                return;
            }

            var auraRatio = auraPool.Max <= 0f ? 0f : Mathf.Clamp01(auraPool.Current / auraPool.Max);
            var intensity = auraController != null ? auraController.AuraVisualIntensity : 1f;
            var alpha = Mathf.Lerp(shellBaseAlpha, shellMaxAlpha, auraRatio) * intensity;
            SetMaterialColor(sharedShellMaterial, new Color(1f, 1f, 1f, Mathf.Clamp01(alpha)));
        }

        private void CleanupLegacyAuraPrimitives()
        {
            var legacyRoot = transform.Find("AuraVisualRoot");
            if (legacyRoot != null)
            {
                Destroy(legacyRoot.gameObject);
            }
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
                material.SetColor("_EmissionColor", color * 0.9f);
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
