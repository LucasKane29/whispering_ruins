using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TiltShiftFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Material material;
    }

    public Settings settings = new();
    private TiltShiftPass _pass;

    public override void Create()
    {
        _pass = new TiltShiftPass(settings.material, settings.renderPassEvent);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material == null) return;
        if (renderingData.cameraData.cameraType == CameraType.Preview) return;
        renderer.EnqueuePass(_pass);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        _pass.Setup(renderer.cameraColorTargetHandle);
    }

    protected override void Dispose(bool disposing)
    {
        _pass.Dispose();
    }

    class TiltShiftPass : ScriptableRenderPass
    {
        private readonly Material _material;
        private RTHandle _source;
        private RTHandle _tempRT;

        private static readonly int CenterID     = Shader.PropertyToID("_Center");
        private static readonly int AreaSizeID   = Shader.PropertyToID("_AreaSize");
        private static readonly int BlurAmountID = Shader.PropertyToID("_BlurAmount");
        private static readonly int TexelSizeID  = Shader.PropertyToID("_TexelSize");

        public TiltShiftPass(Material material, RenderPassEvent passEvent)
        {
            _material = material;
            renderPassEvent = passEvent;
        }

        public void Setup(RTHandle source) => _source = source;

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref _tempRT, desc, FilterMode.Bilinear,
                TextureWrapMode.Clamp, name: "_TiltShiftTemp");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_material == null) return;

            var stack  = VolumeManager.instance.stack;
            var volume = stack.GetComponent<TiltShiftVolume>();
            if (volume == null || !volume.IsActive()) return;

            var desc = renderingData.cameraData.cameraTargetDescriptor;
            _material.SetFloat(CenterID,     volume.center.value);
            _material.SetFloat(AreaSizeID,   volume.areaSize.value);
            _material.SetFloat(BlurAmountID, volume.blurAmount.value);
            _material.SetVector(TexelSizeID, new Vector4(
                1f / desc.width, 1f / desc.height, desc.width, desc.height));

            var cmd = CommandBufferPool.Get("TiltShift");
            Blitter.BlitCameraTexture(cmd, _source, _tempRT, _material, 0);
            Blitter.BlitCameraTexture(cmd, _tempRT, _source);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Dispose() => _tempRT?.Release();
    }
}
