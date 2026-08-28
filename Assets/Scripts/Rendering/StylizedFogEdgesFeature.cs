using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

[DisallowMultipleRendererFeature("Stylized Fog Edges")]
public class StylizedFogEdgesFeature : ScriptableRendererFeature
{
    [Serializable]
    public class Settings
    {
        public RenderPassEvent injectionPoint = RenderPassEvent.AfterRenderingPostProcessing;

        [Header("Posterized fog")]
        [Range(1f, 32f)] public float posterizeSteps = 4f;
        [ColorUsage(false)] public Color fogNearColor = new Color(0.60f, 0.65f, 0.72f);
        [ColorUsage(false)] public Color fogFarColor = new Color(0.10f, 0.12f, 0.17f);
        [Range(0f, 1f)] public float fogNear = 0f;
        [Range(0f, 1f)] public float fogFar = 0.6f;
        [Range(0f, 1f)] public float fogContribution = 0.5f;

        [Header("Edge detection")]
        [Range(0.0001f, 0.5f)] public float colorEdgeThreshold = 0.08f;
        [Range(0.0001f, 0.5f)] public float depthEdgeThreshold = 0.03f;
        [Range(0f, 4f)] public float edgeIntensity = 1f;
        [ColorUsage(false)] public Color edgeColor = Color.black;
        [Range(0.25f, 5f)] public float edgeWidth = 1f;

        [Header("Colour adjust (optional)")]
        [Range(0f, 2f)] public float saturation = 1f;
        [Range(0f, 2f)] public float contrast = 1f;
        [Range(0.1f, 4f)] public float gamma = 1f;
    }

    public Settings settings = new Settings();

    private const string k_ShaderName = "Hidden/StylizedFogEdges";
    private Material m_Material;
    private FogEdgesPass m_Pass;

    public override void Create()
    {
        Shader shader = Shader.Find(k_ShaderName);
        if (shader == null)
        {
            Debug.LogError($"StylizedFogEdgesFeature: shader '{k_ShaderName}' not found.");
            return;
        }

        m_Material = CoreUtils.CreateEngineMaterial(shader);
        m_Pass = new FogEdgesPass(m_Material, settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (m_Material == null || m_Pass == null)
        {
            return;
        }

        CameraType cameraType = renderingData.cameraData.cameraType;
        if (cameraType != CameraType.Game && cameraType != CameraType.SceneView)
        {
            return;
        }

        m_Pass.renderPassEvent = settings.injectionPoint;
        m_Pass.ConfigureInput(ScriptableRenderPassInput.Depth);
        m_Pass.requiresIntermediateTexture = true;
        renderer.EnqueuePass(m_Pass);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(m_Material);
        m_Material = null;
    }

    private class FogEdgesPass : ScriptableRenderPass
    {
        private readonly Material m_Material;
        private readonly Settings m_Settings;

        private static readonly MaterialPropertyBlock s_Mpb = new MaterialPropertyBlock();

        private static readonly int ID_BlitTexture = Shader.PropertyToID("_BlitTexture");
        private static readonly int ID_BlitScaleBias = Shader.PropertyToID("_BlitScaleBias");
        private static readonly int ID_DepthTexture = Shader.PropertyToID("_DepthTexture");
        private static readonly int ID_PosterizeSteps = Shader.PropertyToID("_PosterizeSteps");
        private static readonly int ID_FogNearColor = Shader.PropertyToID("_FogNearColor");
        private static readonly int ID_FogFarColor = Shader.PropertyToID("_FogFarColor");
        private static readonly int ID_FogNear = Shader.PropertyToID("_FogNear");
        private static readonly int ID_FogFar = Shader.PropertyToID("_FogFar");
        private static readonly int ID_FogContribution = Shader.PropertyToID("_FogContribution");
        private static readonly int ID_ColorEdgeThreshold = Shader.PropertyToID("_ColorEdgeThreshold");
        private static readonly int ID_DepthEdgeThreshold = Shader.PropertyToID("_DepthEdgeThreshold");
        private static readonly int ID_EdgeIntensity = Shader.PropertyToID("_EdgeIntensity");
        private static readonly int ID_EdgeColor = Shader.PropertyToID("_EdgeColor");
        private static readonly int ID_EdgeWidth = Shader.PropertyToID("_EdgeWidth");
        private static readonly int ID_TexelSize = Shader.PropertyToID("_TexelSize");
        private static readonly int ID_Saturation = Shader.PropertyToID("_Saturation");
        private static readonly int ID_Contrast = Shader.PropertyToID("_Contrast");
        private static readonly int ID_Gamma = Shader.PropertyToID("_Gamma");

        public FogEdgesPass(Material material, Settings settings)
        {
            m_Material = material;
            m_Settings = settings;
            profilingSampler = new ProfilingSampler("StylizedFogEdges");
        }

        private void ApplySettings()
        {
            m_Material.SetFloat(ID_PosterizeSteps, m_Settings.posterizeSteps);
            m_Material.SetColor(ID_FogNearColor, m_Settings.fogNearColor);
            m_Material.SetColor(ID_FogFarColor, m_Settings.fogFarColor);
            m_Material.SetFloat(ID_FogNear, m_Settings.fogNear);
            m_Material.SetFloat(ID_FogFar, m_Settings.fogFar);
            m_Material.SetFloat(ID_FogContribution, m_Settings.fogContribution);
            m_Material.SetFloat(ID_ColorEdgeThreshold, m_Settings.colorEdgeThreshold);
            m_Material.SetFloat(ID_DepthEdgeThreshold, m_Settings.depthEdgeThreshold);
            m_Material.SetFloat(ID_EdgeIntensity, m_Settings.edgeIntensity);
            m_Material.SetColor(ID_EdgeColor, m_Settings.edgeColor);
            m_Material.SetFloat(ID_EdgeWidth, m_Settings.edgeWidth);
            m_Material.SetFloat(ID_Saturation, m_Settings.saturation);
            m_Material.SetFloat(ID_Contrast, m_Settings.contrast);
            m_Material.SetFloat(ID_Gamma, m_Settings.gamma);
        }

        private class PassData
        {
            public Material material;
            public TextureHandle source;
            public TextureHandle depth;
            public bool hasDepth;
            public Vector4 texelSize;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            UniversalResourceData resources = frameData.Get<UniversalResourceData>();
            if (resources.isActiveTargetBackBuffer)
            {
                return;
            }

            ApplySettings();

            TextureHandle cameraColor = resources.activeColorTexture;

            TextureDesc desc = renderGraph.GetTextureDesc(cameraColor);
            desc.name = "StylizedFogEdges_Source";
            desc.clearBuffer = false;
            desc.depthBufferBits = 0;
            desc.msaaSamples = MSAASamples.None;
            TextureHandle source = renderGraph.CreateTexture(desc);

            int w = Mathf.Max(1, desc.width);
            int h = Mathf.Max(1, desc.height);

            renderGraph.AddCopyPass(cameraColor, source, "StylizedFogEdges Copy");

            using (IRasterRenderGraphBuilder builder =
                   renderGraph.AddRasterRenderPass<PassData>("StylizedFogEdges", out PassData passData, profilingSampler))
            {
                passData.material = m_Material;
                passData.source = source;
                passData.depth = resources.cameraDepthTexture;
                passData.hasDepth = resources.cameraDepthTexture.IsValid();
                passData.texelSize = new Vector4(1f / w, 1f / h, w, h);

                builder.UseTexture(source, AccessFlags.Read);
                if (passData.hasDepth)
                {
                    builder.UseTexture(passData.depth, AccessFlags.Read);
                }
                builder.SetRenderAttachment(cameraColor, 0, AccessFlags.Write);

                builder.SetRenderFunc(static (PassData data, RasterGraphContext ctx) =>
                {
                    RTHandle sourceRT = data.source;
                    s_Mpb.Clear();
                    s_Mpb.SetTexture(ID_BlitTexture, sourceRT);
                    if (data.hasDepth)
                    {
                        RTHandle depthRT = data.depth;
                        s_Mpb.SetTexture(ID_DepthTexture, depthRT);
                    }
                    s_Mpb.SetVector(ID_BlitScaleBias, new Vector4(1f, 1f, 0f, 0f));
                    s_Mpb.SetVector(ID_TexelSize, data.texelSize);

                    ctx.cmd.DrawProcedural(Matrix4x4.identity, data.material, 0, MeshTopology.Triangles, 3, 1, s_Mpb);
                });
            }
        }
    }
}
