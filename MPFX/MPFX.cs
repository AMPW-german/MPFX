using Brutal.ImGuiApi;
using Brutal.Numerics;
using KSA;
using ModMenu;
using MPFX.Buffers;
using StarMap.API;
using System;
using System.Numerics;

namespace MPFX
{
    [StarMapMod]
    public class MPFX
    {
        #region variables
        private ImGuiTreeNodeFlags TreeFlags = ImGuiTreeNodeFlags.None;
        private static bool ShowWindow = true;

        private static bool ColorBalancePreImgui = false;
        private static bool ColorBalancePreImguiSeparateRGB = false;
        private static float4x4 ColorBalanceMatPreImgui = new float4x4(
            0, 0, 0, 0, // Lift
            1, 1, 1, 0, // Gamma
            1, 1, 1, 0, // Gain
            0, 0, 0, 1  // last value used as on/off toggle
        );
        private static bool ColorBalancePostImgui = false;
        private static bool ColorBalancePostImguiSeparateRGB = false;
        private static float4x4 ColorBalanceMatPostImgui = new float4x4(
            0, 0, 0, 0,
            1, 1, 1, 0,
            1, 1, 1, 0,
            0, 0, 0, 1
        );

        private static bool ColorTempPreImgui = false;
        private static float ColorTempFloatPreImgui = 6500f; // neutral
        private static bool ColorTempPostImgui = false;
        private static float ColorTempFloatPostImgui = 6500f;

        private static bool HueShiftPreImgui = false;
        private static float HueShiftFloatPreImgui = 0f;
        private static bool HueShiftPostImgui = false;
        private static float HueShiftFloatPostImgui = 0f;

        private static bool BrightnessPreImgui = false;
        private static float BrightnessFloatPreImgui = 1f;
        private static bool BrightnessPostImgui = false;
        private static float BrightnessFloatPostImgui = 1f;

        private static bool ContrastPreImgui = false;
        private static float ContrastFloatPreImgui = 1f;
        private static bool ContrastPostImgui = false;
        private static float ContrastFloatPostImgui = 1f;

        private static bool SaturationPreImgui = false;
        private static float SaturationFloatPreImgui = 1f;
        private static bool SaturationPostImgui = false;
        private static float SaturationFloatPostImgui = 1f;

        private static bool ColorOverlayPreImgui = false;
        private static float4 ColorOverlayFloatPreImgui = float4.Zero;
        private static bool ColorOverlayPostImgui = false;
        private static float4 ColorOverlayFloatPostImgui = float4.Zero;

        // Vignette needs aspect ratio (also uses screen aspect ratio), inner radius, outer radius, and color
        private static bool VignettePreImgui = false;
        private static float VignettePreAspectRatio = 1;
        private static float4 VignetteFloatPreImgui = float4.Zero;
        private static float4 VignetteColorPreImgui = float4.Zero;
        private static bool VignettePostImgui = false;
        private static float VignettePostAspectRatio = 1;
        private static float4 VignetteFloatPostImgui = float4.Zero;
        private static float4 VignetteColorPostImgui = float4.Zero;

        #region funStuff
        private static bool RGB2HSVPreImgui = false;
        private static float RGB2HSVFloatPreImgui = 0;
        private static bool RGB2HSVPostImgui = false;
        private static float RGB2HSVFloatPostImgui = 0;

        private static bool HSV2RGBPreImgui = false;
        private static float HSV2RGBFloatPreImgui = 0;
        private static bool HSV2RGBPostImgui = false;
        private static float HSV2RGBFloatPostImgui = 0;

        private static bool SingleColorPreImgui = false;
        private static float SingleColorFloatPreImgui = 0;
        private static bool SingleColorPostImgui = false;
        private static float SingleColorFloatPostImgui = 0;

        private static bool SingleColorSmallestPreImgui = false;
        private static float SingleColorSmallestFloatPreImgui = 0;
        private static bool SingleColorSmallestPostImgui = false;
        private static float SingleColorSmallestFloatPostImgui = 0;

        private static bool SingleColorSolidPreImgui = false;
        private static float SingleColorSolidFloatPreImgui = 0;
        private static bool SingleColorSolidPostImgui = false;
        private static float SingleColorSolidFloatPostImgui = 0;
        #endregion
        #endregion

        [ModMenuEntry("MPFX")]
        public static void DrawMenu()
        {
            ImGui.Text("MPFX Mod Menu");
            if (ImGui.Button("Open")) ShowWindow = true;
        }

        private static unsafe void ImguiSliderRow(string text, string id, float min, float max, ref float value)
        {
            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.PushID($"{id}RowLabel");
            ImGui.Text(text);
            ImGui.PopID();
            ImGui.TableNextColumn();
            ImGui.PushID($"{id}RowSlider");
            float liftPre = ColorBalanceMatPreImgui[0, 0];
            ImGui.SetNextItemWidth(-1f);
            ImGui.SliderFloat("", ref value, min, max, flags: ImGuiSliderFlags.AlwaysClamp);
            ImGui.PopID();
            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth(-1f);
            ImGui.PushID($"{id}Input");
            ImGui.InputFloat("", ref value);
            ImGui.PopID();
        }

        public static Vector4 float2Vector(float4 input) => new Vector4(input.R, input.G, input.B, input.A);

        [StarMapAfterGui]
        public void AfterGui(double dt)
        {
            //Vehicle? rocket = Program.ControlledVehicle;
            //float t = rocket.GetManualThrottle();

            if (MPFXDefaultBuffer.LookupSpan != null)
            {
                Span<MPFXDefaultBuffer> BrightnessData = MPFXDefaultBuffer.LookupSpan(KeyHash.Make("MPFXBrightnessBuffer"));
                BrightnessData[0].a = BrightnessPreImgui ? BrightnessFloatPreImgui : 1f;
                BrightnessData[0].b = BrightnessPostImgui ? BrightnessFloatPostImgui : 1f;

                Span<MPFXDefaultBuffer> ContrastData = MPFXDefaultBuffer.LookupSpan(KeyHash.Make("MPFXContrastBuffer"));
                ContrastData[0].a = ContrastPreImgui ? ContrastFloatPreImgui : 1f;
                ContrastData[0].b = ContrastPostImgui ? ContrastFloatPostImgui : 1f;

                Span<MPFXDefaultBuffer> SaturationData = MPFXDefaultBuffer.LookupSpan(KeyHash.Make("MPFXSaturationBuffer"));
                SaturationData[0].a = SaturationPreImgui ? SaturationFloatPreImgui : 1f;
                SaturationData[0].b = SaturationPostImgui ? SaturationFloatPostImgui : 1f;

                Span<MPFXDefaultBuffer> ColorTempData = MPFXDefaultBuffer.LookupSpan(KeyHash.Make("MPFXColorTempBuffer"));
                ColorTempData[0].a = ColorTempPreImgui ? ColorTempFloatPreImgui : 0f;
                ColorTempData[0].b = ColorTempPostImgui ? ColorTempFloatPostImgui : 0f;

                Span<MPFXDefaultBuffer> HueShiftData = MPFXDefaultBuffer.LookupSpan(KeyHash.Make("MPFXHueShiftBuffer"));
                HueShiftData[0].a = HueShiftPreImgui ? HueShiftFloatPreImgui / 180f * (float) Math.PI : 0f;
                HueShiftData[0].b = HueShiftPostImgui ? HueShiftFloatPostImgui / 180f * (float)Math.PI : 0f;


                Span<MPFXDefaultBuffer> Rgb2hsvData = MPFXDefaultBuffer.LookupSpan(KeyHash.Make("MPFXRGB2HSVBuffer"));
                Rgb2hsvData[0].a = RGB2HSVPreImgui ? RGB2HSVFloatPreImgui : 0;
                Rgb2hsvData[0].b = RGB2HSVPostImgui ? RGB2HSVFloatPostImgui : 0;

                Span<MPFXDefaultBuffer> Hsv2rgbData = MPFXDefaultBuffer.LookupSpan(KeyHash.Make("MPFXHSV2RGBBuffer"));
                Hsv2rgbData[0].a = HSV2RGBPreImgui ? HSV2RGBFloatPreImgui : 0;
                Hsv2rgbData[0].b = HSV2RGBPostImgui ? HSV2RGBFloatPostImgui : 0;

                Span<MPFXDefaultBuffer> SingleColorData = MPFXDefaultBuffer.LookupSpan(KeyHash.Make("MPFXSingleColorBuffer"));
                SingleColorData[0].a = SingleColorPreImgui ? SingleColorFloatPreImgui : 0;
                SingleColorData[0].b = SingleColorPostImgui ? SingleColorFloatPostImgui : 0;

                Span<MPFXDefaultBuffer> SingleColorSmallestData = MPFXDefaultBuffer.LookupSpan(KeyHash.Make("MPFXSingleColorSmallestBuffer"));
                SingleColorSmallestData[0].a = SingleColorSmallestPreImgui ? SingleColorSmallestFloatPreImgui : 0;
                SingleColorSmallestData[0].b = SingleColorSmallestPostImgui ? SingleColorSmallestFloatPostImgui : 0;

                Span<MPFXDefaultBuffer> SingleColorSolidData = MPFXDefaultBuffer.LookupSpan(KeyHash.Make("MPFXSingleColorSolidBuffer"));
                SingleColorSolidData[0].a = SingleColorSolidPreImgui ? SingleColorSolidFloatPreImgui : 0;
                SingleColorSolidData[0].b = SingleColorSolidPostImgui ? SingleColorSolidFloatPostImgui : 0;
            }

            if (MPFXVec4Buffer.LookupSpan != null)
            {
                Span<MPFXVec4Buffer> colorOverlayData = MPFXVec4Buffer.LookupSpan(KeyHash.Make("MPFXColorOverlayBuffer"));
                colorOverlayData[0].a = ColorOverlayPreImgui ? ColorOverlayFloatPreImgui : float4.Zero;
                colorOverlayData[0].b = ColorOverlayPostImgui ? ColorOverlayFloatPostImgui : float4.Zero;

                Span<MPFXVec4Buffer> vignettePreData = MPFXVec4Buffer.LookupSpan(KeyHash.Make("MPFXVignettePreBuffer"));
                vignettePreData[0].a = VignettePreImgui ? VignetteFloatPreImgui : float4.Zero;
                vignettePreData[0].b = VignettePreImgui ? VignetteColorPreImgui : float4.Zero;

                Span<MPFXVec4Buffer> vignettePostData = MPFXVec4Buffer.LookupSpan(KeyHash.Make("MPFXVignettePostBuffer"));
                vignettePostData[0].a = VignettePostImgui ? VignetteFloatPostImgui : float4.Zero;
                vignettePostData[0].b = VignettePostImgui ? VignetteColorPostImgui : float4.Zero;
            }

            if (MPFXMat4Buffer.LookupSpan != null)
            {
                Span<MPFXMat4Buffer> ColorBalanceData = MPFXMat4Buffer.LookupSpan(KeyHash.Make("MPFXColorBalanceBuffer"));
                ColorBalanceData[0].a = ColorBalancePreImgui ? ColorBalanceMatPreImgui : new float4x4();
                ColorBalanceData[0].b = ColorBalancePostImgui ? ColorBalanceMatPostImgui : new float4x4();
            }


            #region MainWindow
            if (ShowWindow)
            {
                ImGui.SetNextWindowSizeConstraints(
                    new float2(600f, 500f),
                    new float2(float.MaxValue)
                );
                if (ImGui.Begin("MPFX shader menu", ref ShowWindow))
                {
                    if (ImGui.Button("Rebuild renderers"))
                    {
                        Program.ScheduleRendererRebuild();
                    }

                    ImGui.BeginTable("Shaders", 3, ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable);
                    ImGui.TableSetupColumn("Enabled", ImGuiTableColumnFlags.WidthFixed, initWidthOrWeight: 150f);
                    ImGui.TableSetupColumn("Config");
                    ImGui.TableSetupColumn("InputConfig", ImGuiTableColumnFlags.WidthFixed, initWidthOrWeight: 150f);
                    ImGui.TableHeadersRow();

                    #region ColorBalanceImgui
                    ImGui.TableNextRow();
                    //ImGui.TableNextColumn();
                    ImGui.TableSetColumnIndex(0);
                    if (ImGui.CollapsingHeader("ColorBalance", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                    {
                        ImGui.Indent(20f);
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.PushID("ColorBalancePreCheckbox");
                        ImGui.Checkbox("pre imgui", ref ColorBalancePreImgui);
                        ImGui.PopID();
                        ImGui.Indent(20f);

                        ImGui.BeginDisabled(!ColorBalancePreImgui);
                        ImGui.TableNextColumn();
                        ImGui.PushID("ColorBalancePreSeparateRGB");
                        ImGui.Checkbox("Separate RGB", ref ColorBalancePreImguiSeparateRGB);
                        ImGui.PopID();

                        ImGui.TableNextColumn();
                        ImGui.PushID("ColorBalancePreReset");
                        if (ImGui.Button("Reset"))
                        {
                            ColorBalancePreImguiSeparateRGB = false;
                            ColorBalanceMatPreImgui = new float4x4(
                                0, 0, 0, 0, // Lift
                                1, 1, 1, 0, // Gamma
                                1, 1, 1, 0, // Gain
                                0, 0, 0, 1  // last value used as on/off toggle
                            );
                        }
                        ImGui.PopID();

                        if (!ColorBalancePreImguiSeparateRGB)
                        {
                            float LiftPre = ColorBalanceMatPreImgui[0, 0];
                            ImguiSliderRow("Lift", "ColorBalanceCombinedPreLift", -1f, 1f, ref LiftPre);
                            ColorBalanceMatPreImgui[0, 0] = LiftPre;
                            ColorBalanceMatPreImgui[0, 1] = LiftPre;
                            ColorBalanceMatPreImgui[0, 2] = LiftPre;

                            float GammaPre = ColorBalanceMatPreImgui[1, 0];
                            ImguiSliderRow("Gamma", "ColorBalanceCombinedPreGamma", 0.01f, 2f, ref GammaPre);
                            ColorBalanceMatPreImgui[1, 0] = GammaPre;
                            ColorBalanceMatPreImgui[1, 1] = GammaPre;
                            ColorBalanceMatPreImgui[1, 2] = GammaPre;

                            float GainPre = ColorBalanceMatPreImgui[2, 0];
                            ImguiSliderRow("Gain", "ColorBalanceCombinedPreGain", 0.01f, 2f, ref GainPre);
                            ColorBalanceMatPreImgui[2, 0] = GainPre;
                            ColorBalanceMatPreImgui[2, 1] = GainPre;
                            ColorBalanceMatPreImgui[2, 2] = GainPre;
                        }
                        else
                        {
                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.PushID("RedAdjustmentPre");
                            if (ImGui.CollapsingHeader("Red", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                            {
                                ImGui.Indent(20f);
                                float LiftRedPre = ColorBalanceMatPreImgui[0, 0];
                                ImguiSliderRow("Lift", "ColorBalanceCombinedRedPreLift", -1f, 1f, ref LiftRedPre);
                                ColorBalanceMatPreImgui[0, 0] = LiftRedPre;

                                float GammaRedPre = ColorBalanceMatPreImgui[1, 0];
                                ImguiSliderRow("Gamma", "ColorBalanceCombinedRedPreGamma", 0.01f, 2f, ref GammaRedPre);
                                ColorBalanceMatPreImgui[1, 0] = GammaRedPre;

                                float GainRedPre = ColorBalanceMatPreImgui[2, 0];
                                ImguiSliderRow("Gain", "ColorBalanceCombinedRedPreGain", 0.01f, 2f, ref GainRedPre);
                                ColorBalanceMatPreImgui[2, 0] = GainRedPre;
                                ImGui.Unindent();
                            }
                            ImGui.PopID();

                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.PushID("GreenAdjustmentPre");
                            if (ImGui.CollapsingHeader("Green", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                            {
                                ImGui.Indent(20f);
                                float LiftGreenPre = ColorBalanceMatPreImgui[0, 1];
                                ImguiSliderRow("Lift", "ColorBalanceCombinedGreenPreLift", -1f, 1f, ref LiftGreenPre);
                                ColorBalanceMatPreImgui[0, 1] = LiftGreenPre;

                                float GammaGreenPre = ColorBalanceMatPreImgui[1, 1];
                                ImguiSliderRow("Gamma", "ColorBalanceCombinedGreenPreGamma", 0.01f, 2f, ref GammaGreenPre);
                                ColorBalanceMatPreImgui[1, 1] = GammaGreenPre;

                                float GainGreenPre = ColorBalanceMatPreImgui[2, 1];
                                ImguiSliderRow("Gain", "ColorBalanceCombinedGreenPreGain", 0.01f, 2f, ref GainGreenPre);
                                ColorBalanceMatPreImgui[2, 1] = GainGreenPre;
                                ImGui.Unindent();
                            }
                            ImGui.PopID();

                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.PushID("BlueAdjustmentPre");
                            if (ImGui.CollapsingHeader("Blue", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                            {
                                ImGui.Indent(20f);
                                float LiftBluePre = ColorBalanceMatPreImgui[0, 2];
                                ImguiSliderRow("Lift", "ColorBalanceCombinedBluePreLift", -1f, 1f, ref LiftBluePre);
                                ColorBalanceMatPreImgui[0, 2] = LiftBluePre;

                                float GammaBluePre = ColorBalanceMatPreImgui[1, 2];
                                ImguiSliderRow("Gamma", "ColorBalanceCombinedBluePreGamma", 0.01f, 2f, ref GammaBluePre);
                                ColorBalanceMatPreImgui[1, 2] = GammaBluePre;

                                float GainBluePre = ColorBalanceMatPreImgui[2, 2];
                                ImguiSliderRow("Gain", "ColorBalanceCombinedBluePreGain", 0.01f, 2f, ref GainBluePre);
                                ColorBalanceMatPreImgui[2, 2] = GainBluePre;
                                ImGui.Unindent();
                            }
                            ImGui.PopID();
                        }
                        ImGui.Unindent();
                        ImGui.EndDisabled();


                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.PushID("ColorBalancePostCheckbox");
                        ImGui.Checkbox("Post imgui", ref ColorBalancePostImgui);
                        ImGui.PopID();
                        ImGui.Indent(20f);

                        ImGui.BeginDisabled(!ColorBalancePostImgui);
                        ImGui.TableNextColumn();
                        ImGui.PushID("ColorBalancePostSeparateRGB");
                        ImGui.Checkbox("Separate RGB", ref ColorBalancePostImguiSeparateRGB);
                        ImGui.PopID();

                        ImGui.TableNextColumn();
                        ImGui.PushID("ColorBalancePostReset");
                        if (ImGui.Button("Reset"))
                        {
                            ColorBalancePostImguiSeparateRGB = false;
                            ColorBalanceMatPostImgui = new float4x4(
                                0, 0, 0, 0, // Lift
                                1, 1, 1, 0, // Gamma
                                1, 1, 1, 0, // Gain
                                0, 0, 0, 1  // last value used as on/off toggle
                            );
                        }
                        ImGui.PopID();

                        if (!ColorBalancePostImguiSeparateRGB)
                        {
                            float LiftPost = ColorBalanceMatPostImgui[0, 0];
                            ImguiSliderRow("Lift", "ColorBalanceCombinedPostLift", -1f, 1f, ref LiftPost);
                            ColorBalanceMatPostImgui[0, 0] = LiftPost;
                            ColorBalanceMatPostImgui[0, 1] = LiftPost;
                            ColorBalanceMatPostImgui[0, 2] = LiftPost;

                            float GammaPost = ColorBalanceMatPostImgui[1, 0];
                            ImguiSliderRow("Gamma", "ColorBalanceCombinedPostGamma", 0.01f, 2f, ref GammaPost);
                            ColorBalanceMatPostImgui[1, 0] = GammaPost;
                            ColorBalanceMatPostImgui[1, 1] = GammaPost;
                            ColorBalanceMatPostImgui[1, 2] = GammaPost;

                            float GainPost = ColorBalanceMatPostImgui[2, 0];
                            ImguiSliderRow("Gain", "ColorBalanceCombinedPostGain", 0.01f, 2f, ref GainPost);
                            ColorBalanceMatPostImgui[2, 0] = GainPost;
                            ColorBalanceMatPostImgui[2, 1] = GainPost;
                            ColorBalanceMatPostImgui[2, 2] = GainPost;
                        }
                        else
                        {
                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.PushID("RedAdjustmentPost");
                            if (ImGui.CollapsingHeader("Red", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                            {
                                ImGui.Indent(20f);
                                float LiftRedPost = ColorBalanceMatPostImgui[0, 0];
                                ImguiSliderRow("Lift", "ColorBalanceCombinedRedPostLift", -1f, 1f, ref LiftRedPost);
                                ColorBalanceMatPostImgui[0, 0] = LiftRedPost;

                                float GammaRedPost = ColorBalanceMatPostImgui[1, 0];
                                ImguiSliderRow("Gamma", "ColorBalanceCombinedRedPostGamma", 0.01f, 2f, ref GammaRedPost);
                                ColorBalanceMatPostImgui[1, 0] = GammaRedPost;

                                float GainRedPost = ColorBalanceMatPostImgui[2, 0];
                                ImguiSliderRow("Gain", "ColorBalanceCombinedRedPostGain", 0.01f, 2f, ref GainRedPost);
                                ColorBalanceMatPostImgui[2, 0] = GainRedPost;
                                ImGui.Unindent();
                            }
                            ImGui.PopID();

                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.PushID("GreenAdjustmentPost");
                            if (ImGui.CollapsingHeader("Green", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                            {
                                ImGui.Indent(20f);
                                float LiftGreenPost = ColorBalanceMatPostImgui[0, 1];
                                ImguiSliderRow("Lift", "ColorBalanceCombinedGreenPostLift", -1f, 1f, ref LiftGreenPost);
                                ColorBalanceMatPostImgui[0, 1] = LiftGreenPost;

                                float GammaGreenPost = ColorBalanceMatPostImgui[1, 1];
                                ImguiSliderRow("Gamma", "ColorBalanceCombinedGreenPostGamma", 0.01f, 2f, ref GammaGreenPost);
                                ColorBalanceMatPostImgui[1, 1] = GammaGreenPost;

                                float GainGreenPost = ColorBalanceMatPostImgui[2, 1];
                                ImguiSliderRow("Gain", "ColorBalanceCombinedGreenPostGain", 0.01f, 2f, ref GainGreenPost);
                                ColorBalanceMatPostImgui[2, 1] = GainGreenPost;
                                ImGui.Unindent();
                            }
                            ImGui.PopID();

                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.PushID("BlueAdjustmentPost");
                            if (ImGui.CollapsingHeader("Blue", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                            {
                                ImGui.Indent(20f);
                                float LiftBluePost = ColorBalanceMatPostImgui[0, 2];
                                ImguiSliderRow("Lift", "ColorBalanceCombinedBluePostLift", -1f, 1f, ref LiftBluePost);
                                ColorBalanceMatPostImgui[0, 2] = LiftBluePost;

                                float GammaBluePost = ColorBalanceMatPostImgui[1, 2];
                                ImguiSliderRow("Gamma", "ColorBalanceCombinedBluePostGamma", 0.01f, 2f, ref GammaBluePost);
                                ColorBalanceMatPostImgui[1, 2] = GammaBluePost;

                                float GainBluePost = ColorBalanceMatPostImgui[2, 2];
                                ImguiSliderRow("Gain", "ColorBalanceCombinedBluePostGain", 0.01f, 2f, ref GainBluePost);
                                ColorBalanceMatPostImgui[2, 2] = GainBluePost;
                                ImGui.Unindent();
                            }
                            ImGui.PopID();
                        }
                        ImGui.EndDisabled();
                        ImGui.Unindent();
                        ImGui.Unindent();
                    }

                    #endregion

                    #region ColorTempImgui
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TableSetColumnIndex(0);
                    if (ImGui.CollapsingHeader("ColorTemp", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                    {
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.Indent(20f);
                        ImGui.PushID("ColorTempPreCheckbox");
                        ImGui.Checkbox("pre imgui", ref ColorTempPreImgui);
                        ImGui.PopID();

                        ImGui.TableNextColumn();
                        ImGui.TableNextColumn();
                        ImGui.PushID("ColorTempPreReset");
                        if (ImGui.Button("Reset"))
                        {
                            ColorTempFloatPreImgui = 6500f;
                        }
                        ImGui.PopID();

                        ImGui.BeginDisabled(!ColorTempPreImgui);
                        ImGui.Indent(20f);
                        ImguiSliderRow("Kelvin", "ColorTempPre", 1000f, 40000f, ref ColorTempFloatPreImgui);
                        ImGui.Unindent();
                        ImGui.EndDisabled();

                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.PushID("ColorTempPostCheckbox");
                        ImGui.Checkbox("Post imgui", ref ColorTempPostImgui);
                        ImGui.PopID();

                        ImGui.TableNextColumn();
                        ImGui.TableNextColumn();
                        ImGui.PushID("ColorTempPostReset");
                        if (ImGui.Button("Reset"))
                        {
                            ColorTempFloatPostImgui = 6500f;
                        }
                        ImGui.PopID();

                        ImGui.BeginDisabled(!ColorTempPostImgui);
                        ImGui.Indent(20f);
                        ImguiSliderRow("Kelvin", "ColorTempPost", 1000f, 40000f, ref ColorTempFloatPostImgui);
                        ImGui.Unindent();
                        ImGui.EndDisabled();

                        ImGui.Unindent();
                    }
                    #endregion

                    #region HueShiftImgui
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TableSetColumnIndex(0);
                    if (ImGui.CollapsingHeader("HueShift", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                    {
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.Indent(20f);
                        ImGui.PushID("HueShiftPreCheckbox");
                        ImGui.Checkbox("pre imgui", ref HueShiftPreImgui);
                        ImGui.PopID();

                        ImGui.TableNextColumn();
                        ImGui.TableNextColumn();
                        ImGui.PushID("HueShiftPreReset");
                        if (ImGui.Button("Reset"))
                        {
                            HueShiftFloatPreImgui = 0f;
                        }
                        ImGui.PopID();

                        ImGui.BeginDisabled(!HueShiftPreImgui);
                        ImGui.Indent(20f);
                        ImguiSliderRow("Degree", "HueShiftPre", 0f, 360f, ref HueShiftFloatPreImgui);
                        ImGui.Unindent();
                        ImGui.EndDisabled();

                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.PushID("HueShiftPostCheckbox");
                        ImGui.Checkbox("Post imgui", ref HueShiftPostImgui);
                        ImGui.PopID();

                        ImGui.TableNextColumn();
                        ImGui.TableNextColumn();
                        ImGui.PushID("HueShiftPostReset");
                        if (ImGui.Button("Reset"))
                        {
                            HueShiftFloatPostImgui = 0f;
                        }
                        ImGui.PopID();

                        ImGui.BeginDisabled(!HueShiftPostImgui);
                        ImGui.Indent(20f);
                        ImguiSliderRow("Degree", "HueShiftPost", 0f, 360f, ref HueShiftFloatPostImgui);
                        ImGui.Unindent();
                        ImGui.EndDisabled();

                        ImGui.Unindent();
                    }
                    #endregion

                    #region BrightnessImgui
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TableSetColumnIndex(0);
                    if (ImGui.CollapsingHeader("Brightness", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                    {
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.Indent(20f);
                        ImGui.PushID("BrightnessPreCheckbox");
                        ImGui.Checkbox("pre imgui", ref BrightnessPreImgui);
                        ImGui.PopID();

                        ImGui.TableNextColumn();
                        ImGui.TableNextColumn();
                        ImGui.PushID("BrightnessPreReset");
                        if (ImGui.Button("Reset"))
                        {
                            BrightnessFloatPreImgui = 1f;
                        }
                        ImGui.PopID();

                        ImGui.BeginDisabled(!BrightnessPreImgui);
                        ImGui.Indent(20f);
                        ImguiSliderRow("Multiplier", "BrightnessPre", 0.01f, 4f, ref BrightnessFloatPreImgui);
                        ImGui.Unindent();
                        ImGui.EndDisabled();

                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.PushID("BrightnessPostCheckbox");
                        ImGui.Checkbox("Post imgui", ref BrightnessPostImgui);
                        ImGui.PopID();

                        ImGui.TableNextColumn();
                        ImGui.TableNextColumn();
                        ImGui.PushID("BrightnessPostReset");
                        if (ImGui.Button("Reset"))
                        {
                            BrightnessFloatPostImgui = 1f;
                        }
                        ImGui.PopID();

                        ImGui.BeginDisabled(!BrightnessPostImgui);
                        ImGui.Indent(20f);
                        ImguiSliderRow("Multiplier", "BrightnessPost", 0.01f, 4f, ref BrightnessFloatPostImgui);
                        ImGui.Unindent();
                        ImGui.EndDisabled();

                        ImGui.Unindent();
                    }
                    #endregion

                    #region ContrastImgui
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TableSetColumnIndex(0);
                    if (ImGui.CollapsingHeader("Contrast", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                    {
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.Indent(20f);
                        ImGui.PushID("ContrastPreCheckbox");
                        ImGui.Checkbox("pre imgui", ref ContrastPreImgui);
                        ImGui.PopID();

                        ImGui.TableNextColumn();
                        ImGui.TableNextColumn();
                        ImGui.PushID("ContrastPreReset");
                        if (ImGui.Button("Reset"))
                        {
                            ContrastFloatPreImgui = 1f;
                        }
                        ImGui.PopID();

                        ImGui.BeginDisabled(!ContrastPreImgui);
                        ImGui.Indent(20f);
                        ImguiSliderRow("Adjustment", "ContrastPre", 0.01f, 4f, ref ContrastFloatPreImgui);
                        ImGui.Unindent();
                        ImGui.EndDisabled();

                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.PushID("ContrastPostCheckbox");
                        ImGui.Checkbox("Post imgui", ref ContrastPostImgui);
                        ImGui.PopID();

                        ImGui.TableNextColumn();
                        ImGui.TableNextColumn();
                        ImGui.PushID("ContrastPostReset");
                        if (ImGui.Button("Reset"))
                        {
                            ContrastFloatPostImgui = 1f;
                        }
                        ImGui.PopID();

                        ImGui.BeginDisabled(!ContrastPostImgui);
                        ImGui.Indent(20f);
                        ImguiSliderRow("Adjustment", "ContrastPost", 0.01f, 4f, ref ContrastFloatPostImgui);
                        ImGui.Unindent();
                        ImGui.EndDisabled();

                        ImGui.Unindent();
                    }
                    #endregion

                    #region SaturationImgui
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TableSetColumnIndex(0);
                    if (ImGui.CollapsingHeader("Saturation", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                    {
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.Indent(20f);
                        ImGui.PushID("SaturationPreCheckbox");
                        ImGui.Checkbox("pre imgui", ref SaturationPreImgui);
                        ImGui.PopID();

                        ImGui.TableNextColumn();
                        ImGui.TableNextColumn();
                        ImGui.PushID("SaturationPreReset");
                        if (ImGui.Button("Reset"))
                        {
                            SaturationFloatPreImgui = 1f;
                        }
                        ImGui.PopID();

                        ImGui.BeginDisabled(!SaturationPreImgui);
                        ImGui.Indent(20f);
                        ImguiSliderRow("Multiplier", "SaturationPre", 0.01f, 4f, ref SaturationFloatPreImgui);
                        ImGui.Unindent();
                        ImGui.EndDisabled();

                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.PushID("SaturationPostCheckbox");
                        ImGui.Checkbox("Post imgui", ref SaturationPostImgui);
                        ImGui.PopID();

                        ImGui.TableNextColumn();
                        ImGui.TableNextColumn();
                        ImGui.PushID("SaturationPostReset");
                        if (ImGui.Button("Reset"))
                        {
                            SaturationFloatPostImgui = 1f;
                        }
                        ImGui.PopID();

                        ImGui.BeginDisabled(!SaturationPostImgui);
                        ImGui.Indent(20f);
                        ImguiSliderRow("Multiplier", "SaturationPost", 0.01f, 4f, ref SaturationFloatPostImgui);
                        ImGui.Unindent();
                        ImGui.EndDisabled();

                        ImGui.Unindent();
                    }
                    #endregion

                    #region ColorOverlayImgui
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TableSetColumnIndex(0);
                    if (ImGui.CollapsingHeader("ColorOverlay", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                    {
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.PushID("ColorOverlayPreCheckbox");
                        ImGui.Checkbox("pre imgui", ref ColorOverlayPreImgui);
                        ImGui.PopID();

                        ImGui.BeginDisabled(!ColorOverlayPreImgui);
                        ImGui.TableNextColumn();
                        ImGui.PushID("ColorOverlayPreStrength");
                        ImGui.SetNextItemWidth(275f);
                        ImGui.ColorPicker4("Color", ref ColorOverlayFloatPreImgui, ImGuiColorEditFlags.DisplayRGB |ImGuiColorEditFlags.DisplayHSV | ImGuiColorEditFlags.DisplayRGB);
                        ImGui.PopID();
                        ImGui.EndDisabled();

                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.PushID("ColorOverlayPostCheckbox");
                        ImGui.Checkbox("post imgui", ref ColorOverlayPostImgui);
                        ImGui.PopID();

                        ImGui.BeginDisabled(!ColorOverlayPostImgui);
                        ImGui.TableNextColumn();
                        ImGui.PushID("ColorOverlayPostStrength");
                        ImGui.SetNextItemWidth(275f);
                        ImGui.ColorPicker4("Color", ref ColorOverlayFloatPostImgui, ImGuiColorEditFlags.DisplayRGB |ImGuiColorEditFlags.DisplayHSV | ImGuiColorEditFlags.DisplayRGB);
                        ImGui.PopID();
                        ImGui.EndDisabled();
                    }
                    #endregion

                    #region VignetteImgui
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TableSetColumnIndex(0);
                    if (ImGui.CollapsingHeader("Vignette", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                    {
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.Indent(20f);

                        ImGui.PushID("VignettePreCheckbox");
                        ImGui.Checkbox("pre imgui", ref VignettePreImgui);
                        ImGui.Indent(20);
                        ImGui.PopID();
                        ImGui.BeginDisabled(!VignettePreImgui);
                        ImGui.TableNextColumn();
                        ImGui.PushID("VignettePreColor");
                        ImGui.SetNextItemWidth(275f);
                        ImGui.ColorPicker4("Color", ref VignetteColorPreImgui, ImGuiColorEditFlags.DisplayRGB | ImGuiColorEditFlags.DisplayHSV | ImGuiColorEditFlags.DisplayRGB);
                        ImGui.PopID();

                        float rPre = VignetteFloatPreImgui.X;
                        ImguiSliderRow("Outer radius", "VignettePreOuterRadius", 0f, (float)Program.MainViewport.Size.X / Program.MainViewport.Size.Y, ref rPre);
                        VignetteFloatPreImgui.X = rPre;
                        float gPre = VignetteFloatPreImgui.Y;
                        ImguiSliderRow("Inner radius", "VignettePreInnerRadius", 0f, rPre - 0.001f, ref gPre);
                        VignetteFloatPreImgui.Y = Math.Min(gPre, rPre - 0.001f);
                        ImguiSliderRow("Aspectratio", "VignettePreAspectratio", 0.0001f, 5f, ref VignettePreAspectRatio);
                        VignetteFloatPreImgui.Z = VignettePreAspectRatio * Program.MainViewport.Size.Y / Program.MainViewport.Size.X;

                        ImGui.Unindent();
                        ImGui.EndDisabled();

                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();

                        ImGui.PushID("VignettePostCheckbox");
                        ImGui.Checkbox("Post imgui", ref VignettePostImgui);
                        ImGui.Indent(20);
                        ImGui.PopID();
                        ImGui.BeginDisabled(!VignettePostImgui);
                        ImGui.TableNextColumn();
                        ImGui.PushID("VignettePostColor");
                        ImGui.SetNextItemWidth(275f);
                        ImGui.ColorPicker4("Color", ref VignetteColorPostImgui, ImGuiColorEditFlags.DisplayRGB | ImGuiColorEditFlags.DisplayHSV | ImGuiColorEditFlags.DisplayRGB);
                        ImGui.PopID();

                        float rPost = VignetteFloatPostImgui.X;
                        ImguiSliderRow("Outer radius", "VignettePostOuterRadius", 0f, (float)Program.MainViewport.Size.X / Program.MainViewport.Size.Y, ref rPost);
                        VignetteFloatPostImgui.X = rPost;
                        float gPost = VignetteFloatPostImgui.Y;
                        ImguiSliderRow("Inner radius", "VignettePostInnerRadius", 0f, rPost - 0.001f, ref gPost);
                        VignetteFloatPostImgui.Y = Math.Min(gPost, rPost - 0.001f);
                        ImguiSliderRow("Aspectratio", "VignettePostAspectratio", 0.0001f, 5f, ref VignettePostAspectRatio);
                        VignetteFloatPostImgui.Z = VignettePostAspectRatio * Program.MainViewport.Size.Y / Program.MainViewport.Size.X;

                        ImGui.Unindent();
                        ImGui.EndDisabled();
                        ImGui.Unindent();
                    }
                    #endregion

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("");
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TableSetColumnIndex(0);
                    if (ImGui.CollapsingHeader("Fun stuff", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                    {
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.Indent(20f);
                        if (ImGui.CollapsingHeader("RGB2HSV", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                        {
                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.PushID("RGB2HSVPreCheckbox");
                            ImGui.Checkbox("pre imgui", ref RGB2HSVPreImgui);
                            ImGui.PopID();

                            ImGui.BeginDisabled(!RGB2HSVPreImgui);
                            ImGui.TableNextColumn();
                            ImGui.PushID("RGB2HSVPreStrength");
                            ImGui.SliderFloat("Percentage", ref RGB2HSVFloatPreImgui, 0f, 1f, flags: ImGuiSliderFlags.AlwaysClamp);
                            ImGui.PopID();
                            ImGui.EndDisabled();

                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.PushID("RGB2HSVPostCheckbox");
                            ImGui.Checkbox("post imgui", ref RGB2HSVPostImgui);
                            ImGui.PopID();

                            ImGui.BeginDisabled(!RGB2HSVPostImgui);
                            ImGui.TableNextColumn();
                            ImGui.PushID("RGB2HSVPostStrength");
                            ImGui.SliderFloat("Percentage", ref RGB2HSVFloatPostImgui, 0f, 1f, flags: ImGuiSliderFlags.AlwaysClamp);
                            ImGui.PopID();
                            ImGui.EndDisabled();
                        }

                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.TableSetColumnIndex(0);
                        if (ImGui.CollapsingHeader("HSV2RGB", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                        {
                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.PushID("HSV2RGBPreCheckbox");
                            ImGui.Checkbox("pre imgui", ref HSV2RGBPreImgui);
                            ImGui.PopID();

                            ImGui.BeginDisabled(!HSV2RGBPreImgui);
                            ImGui.TableNextColumn();
                            ImGui.PushID("HSV2RGBPreStrength");
                            ImGui.SliderFloat("Percentage", ref HSV2RGBFloatPreImgui, 0f, 1f, flags: ImGuiSliderFlags.AlwaysClamp);
                            ImGui.PopID();
                            ImGui.EndDisabled();

                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.PushID("HSV2RGBPostCheckbox");
                            ImGui.Checkbox("post imgui", ref HSV2RGBPostImgui);
                            ImGui.PopID();

                            ImGui.BeginDisabled(!HSV2RGBPostImgui);
                            ImGui.TableNextColumn();
                            ImGui.PushID("HSV2RGBPostStrength");
                            ImGui.SliderFloat("Percentage", ref HSV2RGBFloatPostImgui, 0f, 1f, flags: ImGuiSliderFlags.AlwaysClamp);
                            ImGui.PopID();
                            ImGui.EndDisabled();
                        }

                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.TableSetColumnIndex(0);
                        if (ImGui.CollapsingHeader("SingleColor", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                        {
                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.PushID("SingleColorPreCheckbox");
                            ImGui.Checkbox("pre imgui", ref SingleColorPreImgui);
                            ImGui.PopID();

                            ImGui.BeginDisabled(!SingleColorPreImgui);
                            ImGui.TableNextColumn();
                            ImGui.PushID("SingleColorPreStrength");
                            ImGui.SliderFloat("Percentage", ref SingleColorFloatPreImgui, 0f, 1f, flags: ImGuiSliderFlags.AlwaysClamp);
                            ImGui.PopID();
                            ImGui.EndDisabled();

                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.PushID("SingleColorPostCheckbox");
                            ImGui.Checkbox("post imgui", ref SingleColorPostImgui);
                            ImGui.PopID();

                            ImGui.BeginDisabled(!SingleColorPostImgui);
                            ImGui.TableNextColumn();
                            ImGui.PushID("SingleColorPostStrength");
                            ImGui.SliderFloat("Percentage", ref SingleColorFloatPostImgui, 0f, 1f, flags: ImGuiSliderFlags.AlwaysClamp);
                            ImGui.PopID();
                            ImGui.EndDisabled();
                        }

                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.TableSetColumnIndex(0);
                        if (ImGui.CollapsingHeader("SingleColorSmallest", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                        {
                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.PushID("SingleColorSmallestPreCheckbox");
                            ImGui.Checkbox("pre imgui", ref SingleColorSmallestPreImgui);
                            ImGui.PopID();

                            ImGui.BeginDisabled(!SingleColorSmallestPreImgui);
                            ImGui.TableNextColumn();
                            ImGui.PushID("SingleColorSmallestPreStrength");
                            ImGui.SliderFloat("Percentage", ref SingleColorSmallestFloatPreImgui, 0f, 1f, flags: ImGuiSliderFlags.AlwaysClamp);
                            ImGui.PopID();
                            ImGui.EndDisabled();

                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.PushID("SingleColorSmallestPostCheckbox");
                            ImGui.Checkbox("post imgui", ref SingleColorSmallestPostImgui);
                            ImGui.PopID();

                            ImGui.BeginDisabled(!SingleColorSmallestPostImgui);
                            ImGui.TableNextColumn();
                            ImGui.PushID("SingleColorSmallestPostStrength");
                            ImGui.SliderFloat("Percentage", ref SingleColorSmallestFloatPostImgui, 0f, 1f, flags: ImGuiSliderFlags.AlwaysClamp);
                            ImGui.PopID();
                            ImGui.EndDisabled();
                        }

                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.TableSetColumnIndex(0);
                        if (ImGui.CollapsingHeader("SingleColorSolid", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                        {
                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.PushID("SingleColorSolidPreCheckbox");
                            ImGui.Checkbox("pre imgui", ref SingleColorSolidPreImgui);
                            ImGui.PopID();

                            ImGui.BeginDisabled(!SingleColorSolidPreImgui);
                            ImGui.TableNextColumn();
                            ImGui.PushID("SingleColorSolidPreStrength");
                            ImGui.SliderFloat("Percentage", ref SingleColorSolidFloatPreImgui, 0f, 1f, flags: ImGuiSliderFlags.AlwaysClamp);
                            ImGui.PopID();
                            ImGui.EndDisabled();

                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.PushID("SingleColorSolidPostCheckbox");
                            ImGui.Checkbox("post imgui", ref SingleColorSolidPostImgui);
                            ImGui.PopID();

                            ImGui.BeginDisabled(!SingleColorSolidPostImgui);
                            ImGui.TableNextColumn();
                            ImGui.PushID("SingleColorSolidPostStrength");
                            ImGui.SliderFloat("Percentage", ref SingleColorSolidFloatPostImgui, 0f, 1f, flags: ImGuiSliderFlags.AlwaysClamp);
                            ImGui.PopID();
                            ImGui.EndDisabled();
                        }

                        ImGui.Unindent();
                    }

                    ImGui.EndTable();
                }
                ImGui.End();
            }
            #endregion
        }
    }
}
