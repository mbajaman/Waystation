using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Rendering
{
    /// <summary>
    /// Draws the outline material over opaque renderers on the Outline rendering layer,
    /// after the opaque pass, using the camera's existing colour and depth.
    /// </summary>
    [DisallowMultipleRendererFeature("Outline")]
    public class OutlineFeature : ScriptableRendererFeature
    {
        [Serializable]
        public class Settings
        {
            public Material outlineMaterial;
            public RenderingLayerMask outlineLayer;
        }

        [SerializeField] private Settings _settings = new Settings();

        OutlinePass _pass;

        public override void Create()
        {
            _pass = new OutlinePass(_settings)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (_settings.outlineMaterial == null || _settings.outlineLayer.value == 0)
                return;

            renderer.EnqueuePass(_pass);
        }

        protected override void Dispose(bool disposing)
        {
            _pass = null;
        }

        sealed class OutlinePass : ScriptableRenderPass
        {
            readonly Settings _settings;
            readonly List<ShaderTagId> _shaderTags = new List<ShaderTagId>
            {
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("SRPDefaultUnlit"),
                new ShaderTagId("LightweightForward"),
            };

            public OutlinePass(Settings settings)
            {
                _settings = settings;
                profilingSampler = new ProfilingSampler("Outline");
            }

            class PassData
            {
                public RendererListHandle rendererList;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                if (cameraData.cameraType == CameraType.Preview)
                    return;

                UniversalResourceData resources = frameData.Get<UniversalResourceData>();
                if (!resources.activeColorTexture.IsValid() || !resources.activeDepthTexture.IsValid())
                    return;

                UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
                UniversalLightData lightData = frameData.Get<UniversalLightData>();

                DrawingSettings drawSettings = RenderingUtils.CreateDrawingSettings(
                    _shaderTags, renderingData, cameraData, lightData, cameraData.defaultOpaqueSortFlags);
                drawSettings.overrideMaterial = _settings.outlineMaterial;
                drawSettings.overrideMaterialPassIndex = 0;

                // Every GameObject layer, only the Outline rendering layer.
                var filter = new FilteringSettings(RenderQueueRange.opaque, -1, _settings.outlineLayer.value);
                var listParams = new RendererListParams(renderingData.cullResults, drawSettings, filter);
                RendererListHandle rendererList = renderGraph.CreateRendererList(listParams);
                if (!rendererList.IsValid())
                    return;

                using (var builder = renderGraph.AddRasterRenderPass<PassData>("Outline", out var passData, profilingSampler))
                {
                    passData.rendererList = rendererList;
                    builder.UseRendererList(rendererList);
                    builder.SetRenderAttachment(resources.activeColorTexture, 0, AccessFlags.Write);
                    // Depth is tested (ZTest LEqual) and not written (ZWrite Off).
                    builder.SetRenderAttachmentDepth(resources.activeDepthTexture, AccessFlags.Read);

                    builder.SetRenderFunc(static (PassData data, RasterGraphContext context) =>
                    {
                        context.cmd.DrawRendererList(data.rendererList);
                    });
                }
            }
        }
    }
}
