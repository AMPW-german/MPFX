using Brutal.Numerics;
using KittenExtensions;
using KSA;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace MPFX
{
    [KxAsset("MPFXProfile")]
    public class MPFXProfile : SerializedId, IKeyed, IComparer<MPFXProfile>
    {
        public static string ProfileSaveDirectory => Path.Combine(ModLibrary.LocalModsFolderPath, "MPFX", "Profiles");

        private static MPFXProfile defaultProfile;
        public static MPFXProfile DefaultProfile
        {
            get
            {
                if (defaultProfile == null)
                {
                    defaultProfile = new MPFXProfile("Default");
                }
                return defaultProfile;
            }
        }

        public static HashSet<MPFXProfile> Profiles { get; set; } = new HashSet<MPFXProfile>() { };

        #region variables
        [DefaultValue(false)]
        public bool ColorBalancePreImgui = false;
        [DefaultValue(false)]
        public bool ColorBalancePreImguiSeparateRGB = false;
        public float4x4 ColorBalanceMatPreImgui = new float4x4(
            0, 0, 0, 0, // Lift
            1, 1, 1, 0, // Gamma
            1, 1, 1, 0, // Gain
            0, 0, 0, 1  // last value used as on/off toggle, 3rd value used in xml for separate RGB
        );
        public bool ShouldSerializeColorBalanceMatPreImgui() => ColorBalanceMatPreImgui != new float4x4(
            0, 0, 0, 0,
            1, 1, 1, 0,
            1, 1, 1, 0,
            0, 0, 0, 1
        );

        [DefaultValue(false)]
        public bool ColorBalancePostImgui = false;
        [DefaultValue(false)]
        public bool ColorBalancePostImguiSeparateRGB = false;
        public float4x4 ColorBalanceMatPostImgui = new float4x4(
            0, 0, 0, 0,
            1, 1, 1, 0,
            1, 1, 1, 0,
            0, 0, 0, 1
        );
        public bool ShouldSerializeColorBalanceMatPostImgui() => ColorBalanceMatPostImgui != new float4x4(
            0, 0, 0, 0,
            1, 1, 1, 0,
            1, 1, 1, 0,
            0, 0, 0, 1
        );

        [DefaultValue(false)]
        public bool ColorTempPreImgui = false;
        [DefaultValue(6500f)]
        public float ColorTempFloatPreImgui = 6500f; // neutral
        [DefaultValue(false)]
        public bool ColorTempPostImgui = false;
        [DefaultValue(6500f)]
        public float ColorTempFloatPostImgui = 6500f;

        [DefaultValue(false)]
        public bool HueShiftPreImgui = false;
        [DefaultValue(0f)]
        public float HueShiftFloatPreImgui = 0f;
        [DefaultValue(false)]
        public bool HueShiftPostImgui = false;
        [DefaultValue(0f)]
        public float HueShiftFloatPostImgui = 0f;

        [DefaultValue(false)]
        public bool BrightnessPreImgui = false;
        [DefaultValue(1f)]
        public float BrightnessFloatPreImgui = 1f;
        [DefaultValue(false)]
        public bool BrightnessPostImgui = false;
        [DefaultValue(1f)]
        public float BrightnessFloatPostImgui = 1f;

        [DefaultValue(false)]
        public bool ContrastPreImgui = false;
        [DefaultValue(1f)]
        public float ContrastFloatPreImgui = 1f;
        [DefaultValue(false)]
        public bool ContrastPostImgui = false;
        [DefaultValue(1f)]
        public float ContrastFloatPostImgui = 1f;

        [DefaultValue(false)]
        public bool SaturationPreImgui = false;
        [DefaultValue(1f)]
        public float SaturationFloatPreImgui = 1f;
        [DefaultValue(false)]
        public bool SaturationPostImgui = false;
        [DefaultValue(1f)]
        public float SaturationFloatPostImgui = 1f;

        [DefaultValue(false)]
        public bool ColorOverlayPreImgui = false;
        public float4 ColorOverlayFloatPreImgui = float4.Zero;
        public bool ShouldSerializeColorOverlayFloatPreImgui() => ColorOverlayFloatPreImgui != float4.Zero;

        [DefaultValue(false)]
        public bool ColorOverlayPostImgui = false;
        public float4 ColorOverlayFloatPostImgui = float4.Zero;
        public bool ShouldSerializeColorOverlayFloatPostImgui() => ColorOverlayFloatPostImgui != float4.Zero;

        // Vignette needs aspect ratio (also uses screen aspect ratio), inner radius, outer radius, and color
        [DefaultValue(false)]
        public bool VignettePreImgui = false;
        [DefaultValue(1f)]
        public float VignettePreAspectRatio = 1;
        public float4 VignetteFloatPreImgui = float4.Zero;
        public bool ShouldSerializeVignetteFloatPreImgui() => VignetteFloatPreImgui != float4.Zero;
        public float4 VignetteColorPreImgui = float4.Zero;
        public bool ShouldSerializeVignetteColorPreImgui() => VignetteColorPreImgui != float4.Zero;
        [DefaultValue(false)]
        public bool VignettePostImgui = false;
        [DefaultValue(1f)]
        public float VignettePostAspectRatio = 1;
        public float4 VignetteFloatPostImgui = float4.Zero;
        public bool ShouldSerializeVignetteFloatPostImgui() => VignetteFloatPostImgui != float4.Zero;
        public float4 VignetteColorPostImgui = float4.Zero;
        public bool ShouldSerializeVignetteColorPostImgui() => VignetteColorPostImgui != float4.Zero;

        #region funStuff
        [DefaultValue(false)]
        public bool RGB2HSVPreImgui = false;
        [DefaultValue(0f)]
        public float RGB2HSVFloatPreImgui = 0;
        [DefaultValue(false)]
        public bool RGB2HSVPostImgui = false;
        [DefaultValue(0f)]
        public float RGB2HSVFloatPostImgui = 0;

        [DefaultValue(false)]
        public bool HSV2RGBPreImgui = false;
        [DefaultValue(0f)]
        public float HSV2RGBFloatPreImgui = 0;
        [DefaultValue(false)]
        public bool HSV2RGBPostImgui = false;
        [DefaultValue(0f)]
        public float HSV2RGBFloatPostImgui = 0;

        [DefaultValue(false)]
        public bool SingleColorPreImgui = false;
        [DefaultValue(0f)]
        public float SingleColorFloatPreImgui = 0;
        [DefaultValue(false)]
        public bool SingleColorPostImgui = false;
        [DefaultValue(0f)]
        public float SingleColorFloatPostImgui = 0;

        [DefaultValue(false)]
        public bool SingleColorSmallestPreImgui = false;
        [DefaultValue(0f)]
        public float SingleColorSmallestFloatPreImgui = 0;
        [DefaultValue(false)]
        public bool SingleColorSmallestPostImgui = false;
        [DefaultValue(0f)]
        public float SingleColorSmallestFloatPostImgui = 0;

        [DefaultValue(false)]
        public bool SingleColorSolidPreImgui = false;
        [DefaultValue(0f)]
        public float SingleColorSolidFloatPreImgui = 0;
        [DefaultValue(false)]
        public bool SingleColorSolidPostImgui = false;
        [DefaultValue(0f)]
        public float SingleColorSolidFloatPostImgui = 0;
        #endregion
        #endregion

        public override void OnDataLoad(Mod mod)
        {
            base.OnDataLoad(mod);
            if (!Profiles.Any(p => p.Id == this.Id)) Profiles.Add(this);
        }

        public override SerializedId Populate()
        {
            throw new System.NotImplementedException();
        }

        public override TableString.Row ToRow()
        {
            throw new System.NotImplementedException();
        }

        public int Compare(MPFXProfile x, MPFXProfile y) => x.Id.CompareTo(y.Id);

        public override int GetHashCode() => Id.GetHashCode();

        public override bool Equals(object obj) => obj is MPFXProfile profile && profile.Id == this.Id;

        public static MPFXProfile GetOrCreateProfile(string id) => Profiles.FirstOrDefault(p => p.Id == id) ?? new MPFXProfile(id);

        public MPFXProfile() { }
        private MPFXProfile(string id)
        {
            this.Id = id;
            if (!Profiles.Any(p => p.Id == this.Id)) Profiles.Add(this);
        }
        public MPFXProfile(MPFXProfile profile, string id)
        {
            if (Profiles.Any(p => p.Id == this.Id))
            {
                Brutal.Logging.DefaultCategory.Log.Warning($"Profile with id {id} already exists, not creating new profile", "MPFX");
                while (true)
                {
                    id += "1";
                    if (!Profiles.Any(p => p.Id == id)) break;
                }
            }

            this.Id = id;
            Profiles.Add(this);

            this.ColorBalancePreImgui = profile.ColorBalancePreImgui;
            this.ColorBalancePreImguiSeparateRGB = profile.ColorBalancePreImguiSeparateRGB;
            this.ColorBalanceMatPreImgui = profile.ColorBalanceMatPreImgui;
            this.ColorBalancePostImgui = profile.ColorBalancePostImgui;
            this.ColorBalancePostImguiSeparateRGB = profile.ColorBalancePostImguiSeparateRGB;
            this.ColorBalanceMatPostImgui = profile.ColorBalanceMatPostImgui;

            this.ColorTempPreImgui = profile.ColorTempPreImgui;
            this.ColorTempFloatPreImgui = profile.ColorTempFloatPreImgui;
            this.ColorTempPostImgui = profile.ColorTempPostImgui;
            this.ColorTempFloatPostImgui = profile.ColorTempFloatPostImgui;

            this.HueShiftPreImgui = profile.HueShiftPreImgui;
            this.HueShiftFloatPreImgui = profile.HueShiftFloatPreImgui;
            this.HueShiftPostImgui = profile.HueShiftPostImgui;
            this.HueShiftFloatPostImgui = profile.HueShiftFloatPostImgui;

            this.BrightnessPreImgui = profile.BrightnessPreImgui;
            this.BrightnessFloatPreImgui = profile.BrightnessFloatPreImgui;
            this.BrightnessPostImgui = profile.BrightnessPostImgui;
            this.BrightnessFloatPostImgui = profile.BrightnessFloatPostImgui;

            this.ContrastPreImgui = profile.ContrastPreImgui;
            this.ContrastFloatPreImgui = profile.ContrastFloatPreImgui;
            this.ContrastPostImgui = profile.ContrastPostImgui;
            this.ContrastFloatPostImgui = profile.ContrastFloatPostImgui;

            this.SaturationPreImgui = profile.SaturationPreImgui;
            this.SaturationFloatPreImgui = profile.SaturationFloatPreImgui;
            this.SaturationPostImgui = profile.SaturationPostImgui;
            this.SaturationFloatPostImgui = profile.SaturationFloatPostImgui;

            this.ColorOverlayPreImgui = profile.ColorOverlayPreImgui;
            this.ColorOverlayFloatPreImgui = profile.ColorOverlayFloatPreImgui;
            this.ColorOverlayPostImgui = profile.ColorOverlayPostImgui;
            this.ColorOverlayFloatPostImgui = profile.ColorOverlayFloatPostImgui;

            this.VignettePreImgui = profile.VignettePreImgui;
            this.VignettePreAspectRatio = profile.VignettePreAspectRatio;
            this.VignetteFloatPreImgui = profile.VignetteFloatPreImgui;
            this.VignetteColorPreImgui = profile.VignetteColorPreImgui;
            this.VignettePostImgui = profile.VignettePostImgui;
            this.VignettePostAspectRatio = profile.VignettePostAspectRatio;
            this.VignetteFloatPostImgui = profile.VignetteFloatPostImgui;
            this.VignetteColorPostImgui = profile.VignetteColorPostImgui;

            this.RGB2HSVPreImgui = profile.RGB2HSVPreImgui;
            this.RGB2HSVFloatPreImgui = profile.RGB2HSVFloatPreImgui;
            this.RGB2HSVPostImgui = profile.RGB2HSVPostImgui;
            this.RGB2HSVFloatPostImgui = profile.RGB2HSVFloatPostImgui;
            
            this.HSV2RGBPreImgui = profile.HSV2RGBPreImgui;
            this.HSV2RGBFloatPreImgui = profile.HSV2RGBFloatPreImgui;
            this.HSV2RGBPostImgui = profile.HSV2RGBPostImgui;
            this.HSV2RGBFloatPostImgui = profile.HSV2RGBFloatPostImgui;

            this.SingleColorPreImgui = profile.SingleColorPreImgui;
            this.SingleColorFloatPreImgui = profile.SingleColorFloatPreImgui;
            this.SingleColorPostImgui = profile.SingleColorPostImgui;
            this.SingleColorFloatPostImgui = profile.SingleColorFloatPostImgui;

            this.SingleColorSmallestPreImgui = profile.SingleColorSmallestPreImgui;
            this.SingleColorSmallestFloatPreImgui = profile.SingleColorSmallestFloatPreImgui;
            this.SingleColorSmallestPostImgui = profile.SingleColorSmallestPostImgui;
            this.SingleColorSmallestFloatPostImgui = profile.SingleColorSmallestFloatPostImgui;

            this.SingleColorSolidPreImgui = profile.SingleColorSolidPreImgui;
            this.SingleColorSolidFloatPreImgui = profile.SingleColorSolidFloatPreImgui;
            this.SingleColorSolidPostImgui = profile.SingleColorSolidPostImgui;
            this.SingleColorSolidFloatPostImgui = profile.SingleColorSolidFloatPostImgui;
        }
    }
}
