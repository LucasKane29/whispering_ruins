using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenu("Custom/Tilt Shift")]
public class TiltShiftVolume : VolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter center     = new ClampedFloatParameter(0.5f, 0f, 1f);
    public ClampedFloatParameter areaSize   = new ClampedFloatParameter(0.3f, 0f, 1f);
    public ClampedFloatParameter blurAmount = new ClampedFloatParameter(10f, 0f, 30f);

    public bool IsActive() => blurAmount.value > 0f;
    public bool IsTileCompatible() => false;
}
