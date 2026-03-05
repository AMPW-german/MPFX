using Brutal.ImGuiApi;
using Brutal.Numerics;
using KSA;
using ModMenu;
using MPFX.Buffers;
using StarMap.API;
using System;
using System.Collections.Immutable;
using System.Numerics;
using System.Text;

namespace MPFX
{
    [StarMapMod]
    public class MPFX
    {
        private enum WindowTabs
        {
            Profiles,
            Shaders,
            ExportProfile,
        }

        private ImGuiTreeNodeFlags TreeFlags = ImGuiTreeNodeFlags.None;
        private static bool ShowWindow = true;
        private static WindowTabs CurrentTab = WindowTabs.Profiles;

        public static MPFXProfile CurrentProfile { get; private set; } = MPFXProfile.DefaultProfile;
        private static byte[] CurrentProfileID = null;

        public static void ApplyProfile(MPFXProfile profile)
        {
            CurrentProfile = profile;
            CurrentProfileID = new byte[256];
            Encoding.UTF8.GetBytes(profile.Id, CurrentProfileID);
        }

        [ModMenuEntry("MPFX")]
        public static void DrawMenu()
        {
            ImGui.Text("MPFX Mod Menu");
            if (ImGui.Button("Open")) ShowWindow = true;
        }

        private static void ImguiSliderRow(string text, string id, float min, float max, ref float value)
        {
            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.PushID($"{id}RowLabel");
            ImGui.Text(text);
            ImGui.PopID();
            ImGui.TableNextColumn();
            ImGui.PushID($"{id}RowSlider");
            float liftPre = CurrentProfile.ColorBalanceMatPreImgui[0, 0];
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
                BrightnessData[0].a = CurrentProfile.BrightnessPreImgui ? CurrentProfile.BrightnessFloatPreImgui : 1f;
                BrightnessData[0].b = CurrentProfile.BrightnessPostImgui ? CurrentProfile.BrightnessFloatPostImgui : 1f;

                Span<MPFXDefaultBuffer> ContrastData = MPFXDefaultBuffer.LookupSpan(KeyHash.Make("MPFXContrastBuffer"));
                ContrastData[0].a = CurrentProfile.ContrastPreImgui ? CurrentProfile.ContrastFloatPreImgui : 1f;
                ContrastData[0].b = CurrentProfile.ContrastPostImgui ? CurrentProfile.ContrastFloatPostImgui : 1f;

                Span<MPFXDefaultBuffer> SaturationData = MPFXDefaultBuffer.LookupSpan(KeyHash.Make("MPFXSaturationBuffer"));
                SaturationData[0].a = CurrentProfile.SaturationPreImgui ? CurrentProfile.SaturationFloatPreImgui : 1f;
                SaturationData[0].b = CurrentProfile.SaturationPostImgui ? CurrentProfile.SaturationFloatPostImgui : 1f;

                Span<MPFXDefaultBuffer> ColorTempData = MPFXDefaultBuffer.LookupSpan(KeyHash.Make("MPFXColorTempBuffer"));
                ColorTempData[0].a = CurrentProfile.ColorTempPreImgui ? CurrentProfile.ColorTempFloatPreImgui : 0f;
                ColorTempData[0].b = CurrentProfile.ColorTempPostImgui ? CurrentProfile.ColorTempFloatPostImgui : 0f;

                Span<MPFXDefaultBuffer> HueShiftData = MPFXDefaultBuffer.LookupSpan(KeyHash.Make("MPFXHueShiftBuffer"));
                HueShiftData[0].a = CurrentProfile.HueShiftPreImgui ? CurrentProfile.HueShiftFloatPreImgui / 180f * (float)Math.PI : 0f;
                HueShiftData[0].b = CurrentProfile.HueShiftPostImgui ? CurrentProfile.HueShiftFloatPostImgui / 180f * (float)Math.PI : 0f;

                Span<MPFXDefaultBuffer> Rgb2hsvData = MPFXDefaultBuffer.LookupSpan(KeyHash.Make("MPFXRGB2HSVBuffer"));
                Rgb2hsvData[0].a = CurrentProfile.RGB2HSVPreImgui ? CurrentProfile.RGB2HSVFloatPreImgui : 0;
                Rgb2hsvData[0].b = CurrentProfile.RGB2HSVPostImgui ? CurrentProfile.RGB2HSVFloatPostImgui : 0;

                Span<MPFXDefaultBuffer> Hsv2rgbData = MPFXDefaultBuffer.LookupSpan(KeyHash.Make("MPFXHSV2RGBBuffer"));
                Hsv2rgbData[0].a = CurrentProfile.HSV2RGBPreImgui ? CurrentProfile.HSV2RGBFloatPreImgui : 0;
                Hsv2rgbData[0].b = CurrentProfile.HSV2RGBPostImgui ? CurrentProfile.HSV2RGBFloatPostImgui : 0;

                Span<MPFXDefaultBuffer> SingleColorData = MPFXDefaultBuffer.LookupSpan(KeyHash.Make("MPFXSingleColorBuffer"));
                SingleColorData[0].a = CurrentProfile.SingleColorPreImgui ? CurrentProfile.SingleColorFloatPreImgui : 0;
                SingleColorData[0].b = CurrentProfile.SingleColorPostImgui ? CurrentProfile.SingleColorFloatPostImgui : 0;

                Span<MPFXDefaultBuffer> SingleColorSmallestData = MPFXDefaultBuffer.LookupSpan(KeyHash.Make("MPFXSingleColorSmallestBuffer"));
                SingleColorSmallestData[0].a = CurrentProfile.SingleColorSmallestPreImgui ? CurrentProfile.SingleColorSmallestFloatPreImgui : 0;
                SingleColorSmallestData[0].b = CurrentProfile.SingleColorSmallestPostImgui ? CurrentProfile.SingleColorSmallestFloatPostImgui : 0;

                Span<MPFXDefaultBuffer> SingleColorSolidData = MPFXDefaultBuffer.LookupSpan(KeyHash.Make("MPFXSingleColorSolidBuffer"));
                SingleColorSolidData[0].a = CurrentProfile.SingleColorSolidPreImgui ? CurrentProfile.SingleColorSolidFloatPreImgui : 0;
                SingleColorSolidData[0].b = CurrentProfile.SingleColorSolidPostImgui ? CurrentProfile.SingleColorSolidFloatPostImgui : 0;
            }

            if (MPFXVec4Buffer.LookupSpan != null)
            {
                Span<MPFXVec4Buffer> colorOverlayData = MPFXVec4Buffer.LookupSpan(KeyHash.Make("MPFXColorOverlayBuffer"));
                colorOverlayData[0].a = CurrentProfile.ColorOverlayPreImgui ? CurrentProfile.ColorOverlayFloatPreImgui : float4.Zero;
                colorOverlayData[0].b = CurrentProfile.ColorOverlayPostImgui ? CurrentProfile.ColorOverlayFloatPostImgui : float4.Zero;

                Span<MPFXVec4Buffer> vignettePreData = MPFXVec4Buffer.LookupSpan(KeyHash.Make("MPFXVignettePreBuffer"));
                vignettePreData[0].a = CurrentProfile.VignettePreImgui ? CurrentProfile.VignetteFloatPreImgui : float4.Zero;
                vignettePreData[0].b = CurrentProfile.VignettePreImgui ? CurrentProfile.VignetteColorPreImgui : float4.Zero;

                Span<MPFXVec4Buffer> vignettePostData = MPFXVec4Buffer.LookupSpan(KeyHash.Make("MPFXVignettePostBuffer"));
                vignettePostData[0].a = CurrentProfile.VignettePostImgui ? CurrentProfile.VignetteFloatPostImgui : float4.Zero;
                vignettePostData[0].b = CurrentProfile.VignettePostImgui ? CurrentProfile.VignetteColorPostImgui : float4.Zero;
            }

            if (MPFXMat4Buffer.LookupSpan != null)
            {
                Span<MPFXMat4Buffer> ColorBalanceData = MPFXMat4Buffer.LookupSpan(KeyHash.Make("MPFXColorBalanceBuffer"));
                ColorBalanceData[0].a = CurrentProfile.ColorBalancePreImgui ? CurrentProfile.ColorBalanceMatPreImgui : new float4x4();
                ColorBalanceData[0].b = CurrentProfile.ColorBalancePostImgui ? CurrentProfile.ColorBalanceMatPostImgui : new float4x4();
            }


            #region MainWindow
            if (ShowWindow)
            {
                if (CurrentProfileID == null)
                {
                    CurrentProfileID = new byte[256];
                    Encoding.UTF8.GetBytes(CurrentProfile.Id, CurrentProfileID);
                }

                ImGui.SetNextWindowSizeConstraints(
                    new float2(650f, 500f),
                    new float2(float.MaxValue)
                );
                if (ImGui.Begin("MPFX shader menu", ref ShowWindow, ImGuiWindowFlags.NoSavedSettings))
                {
#if DEBUG
                    if (ImGui.Button("Rebuild renderers"))
                    {
                        Program.ScheduleRendererRebuild();
                    }
                    ImGui.Separator();
#endif
                    ImGui.BeginChild("ContentChild", new float2(0f, -32f), ImGuiChildFlags.AlwaysAutoResize);
                    switch (CurrentTab)
                    {
                        case WindowTabs.Profiles:
                            ImGui.Text("Change/create profile:");
                            ImGui.BeginTable("ProfileTable", 3);
                            ImGui.TableSetupColumn("ProfileNameChangeColumn", ImGuiTableColumnFlags.WidthStretch);
                            ImGui.TableSetupColumn("ProfileNameChangeApplyColumn", ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.NoResize, 80f);
                            ImGui.TableSetupColumn("ProfileNameChangeCreateColumn", ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.NoResize, 80f);

                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.InputText("##ProfileIdInput", CurrentProfileID);

                            ImGui.TableNextColumn();
                            if (ImGui.Button("Apply##ApplyCurrentProfileButton"))
                            {
                                string profileId = Encoding.UTF8.GetString(CurrentProfileID).TrimEnd('\0');
                                CurrentProfile.Id = profileId;
                            }
                            ImGui.TableNextColumn();
                            if (ImGui.Button("Create##CreateNewProfileButton"))
                            {
                                string profileId = Encoding.UTF8.GetString(CurrentProfileID).TrimEnd('\0');
                                MPFXProfile newProfile = MPFXProfile.GetOrCreateProfile(profileId);
                                ApplyProfile(newProfile);
                            }
                            ImGui.EndTable();

                            ImGui.BeginChild("ProfileButtonsChild", new float2(0f, -64f), ImGuiChildFlags.AlwaysAutoResize);
                            ImGui.BeginTable("ProfileTable", 2);
                            ImGui.TableSetupColumn("ProfileSelectColumn", ImGuiTableColumnFlags.WidthStretch);
                            ImGui.TableSetupColumn("ProfileDuplicateColumn", ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.NoResize, 100f);
                            foreach (MPFXProfile item in MPFXProfile.Profiles.ToImmutableList())
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.BeginDisabled(CurrentProfile == item);
                                if (ImGui.Button($"{item.Id}##ProfileSelectButton")) ApplyProfile(item);
                                ImGui.EndDisabled();
                                ImGui.SameLine();

                                ImGui.TableNextColumn();
                                if (ImGui.Button($"Duplicate##{item.Id}DuplicateButton"))
                                {
                                    MPFXProfile newProfile = new MPFXProfile(item, item.Id + "1");
                                    ApplyProfile(newProfile);
                                }
                            }
                            ImGui.EndTable();
                            ImGui.EndChild();

                            ImGui.TextColored(Color.Red, "Warning: new profiles will NOT be saved.");
                            ImGui.TextColored(Color.Red, "They need to be manually added to an asset xml file.");
                            break;

                        case WindowTabs.Shaders:
                            ImGui.BeginTable("Shaders", 3, ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable);
                            ImGui.TableSetupColumn("Enabled", ImGuiTableColumnFlags.WidthFixed, initWidthOrWeight: 150f);
                            ImGui.TableSetupColumn("Config", ImGuiTableColumnFlags.WidthStretch);
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
                                ImGui.Checkbox("pre imgui", ref CurrentProfile.ColorBalancePreImgui);
                                ImGui.PopID();
                                ImGui.Indent(20f);

                                ImGui.BeginDisabled(!CurrentProfile.ColorBalancePreImgui);
                                ImGui.TableNextColumn();
                                ImGui.PushID("ColorBalancePreSeparateRGB");
                                ImGui.Checkbox("Separate RGB", ref CurrentProfile.ColorBalancePreImguiSeparateRGB);
                                ImGui.PopID();

                                ImGui.TableNextColumn();
                                ImGui.PushID("ColorBalancePreReset");
                                if (ImGui.Button("Reset"))
                                {
                                    CurrentProfile.ColorBalancePreImguiSeparateRGB = false;
                                    CurrentProfile.ColorBalanceMatPreImgui = new float4x4(
                                        0, 0, 0, 0, // Lift
                                        1, 1, 1, 0, // Gamma
                                        1, 1, 1, 0, // Gain
                                        0, 0, 0, 1  // last value used as on/off toggle
                                    );
                                }
                                ImGui.PopID();

                                if (!CurrentProfile.ColorBalancePreImguiSeparateRGB)
                                {
                                    float LiftPre = CurrentProfile.ColorBalanceMatPreImgui[0, 0];
                                    ImguiSliderRow("Lift", "ColorBalanceCombinedPreLift", -1f, 1f, ref LiftPre);
                                    CurrentProfile.ColorBalanceMatPreImgui[0, 0] = LiftPre;
                                    CurrentProfile.ColorBalanceMatPreImgui[0, 1] = LiftPre;
                                    CurrentProfile.ColorBalanceMatPreImgui[0, 2] = LiftPre;

                                    float GammaPre = CurrentProfile.ColorBalanceMatPreImgui[1, 0];
                                    ImguiSliderRow("Gamma", "ColorBalanceCombinedPreGamma", 0.01f, 2f, ref GammaPre);
                                    CurrentProfile.ColorBalanceMatPreImgui[1, 0] = GammaPre;
                                    CurrentProfile.ColorBalanceMatPreImgui[1, 1] = GammaPre;
                                    CurrentProfile.ColorBalanceMatPreImgui[1, 2] = GammaPre;

                                    float GainPre = CurrentProfile.ColorBalanceMatPreImgui[2, 0];
                                    ImguiSliderRow("Gain", "ColorBalanceCombinedPreGain", 0.01f, 2f, ref GainPre);
                                    CurrentProfile.ColorBalanceMatPreImgui[2, 0] = GainPre;
                                    CurrentProfile.ColorBalanceMatPreImgui[2, 1] = GainPre;
                                    CurrentProfile.ColorBalanceMatPreImgui[2, 2] = GainPre;
                                }
                                else
                                {
                                    ImGui.TableNextRow();
                                    ImGui.TableNextColumn();
                                    ImGui.PushID("RedAdjustmentPre");
                                    if (ImGui.CollapsingHeader("Red", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                                    {
                                        ImGui.Indent(20f);
                                        float LiftRedPre = CurrentProfile.ColorBalanceMatPreImgui[0, 0];
                                        ImguiSliderRow("Lift", "ColorBalanceCombinedRedPreLift", -1f, 1f, ref LiftRedPre);
                                        CurrentProfile.ColorBalanceMatPreImgui[0, 0] = LiftRedPre;

                                        float GammaRedPre = CurrentProfile.ColorBalanceMatPreImgui[1, 0];
                                        ImguiSliderRow("Gamma", "ColorBalanceCombinedRedPreGamma", 0.01f, 2f, ref GammaRedPre);
                                        CurrentProfile.ColorBalanceMatPreImgui[1, 0] = GammaRedPre;

                                        float GainRedPre = CurrentProfile.ColorBalanceMatPreImgui[2, 0];
                                        ImguiSliderRow("Gain", "ColorBalanceCombinedRedPreGain", 0.01f, 2f, ref GainRedPre);
                                        CurrentProfile.ColorBalanceMatPreImgui[2, 0] = GainRedPre;
                                        ImGui.Unindent();
                                    }
                                    ImGui.PopID();

                                    ImGui.TableNextRow();
                                    ImGui.TableNextColumn();
                                    ImGui.PushID("GreenAdjustmentPre");
                                    if (ImGui.CollapsingHeader("Green", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                                    {
                                        ImGui.Indent(20f);
                                        float LiftGreenPre = CurrentProfile.ColorBalanceMatPreImgui[0, 1];
                                        ImguiSliderRow("Lift", "ColorBalanceCombinedGreenPreLift", -1f, 1f, ref LiftGreenPre);
                                        CurrentProfile.ColorBalanceMatPreImgui[0, 1] = LiftGreenPre;

                                        float GammaGreenPre = CurrentProfile.ColorBalanceMatPreImgui[1, 1];
                                        ImguiSliderRow("Gamma", "ColorBalanceCombinedGreenPreGamma", 0.01f, 2f, ref GammaGreenPre);
                                        CurrentProfile.ColorBalanceMatPreImgui[1, 1] = GammaGreenPre;

                                        float GainGreenPre = CurrentProfile.ColorBalanceMatPreImgui[2, 1];
                                        ImguiSliderRow("Gain", "ColorBalanceCombinedGreenPreGain", 0.01f, 2f, ref GainGreenPre);
                                        CurrentProfile.ColorBalanceMatPreImgui[2, 1] = GainGreenPre;
                                        ImGui.Unindent();
                                    }
                                    ImGui.PopID();

                                    ImGui.TableNextRow();
                                    ImGui.TableNextColumn();
                                    ImGui.PushID("BlueAdjustmentPre");
                                    if (ImGui.CollapsingHeader("Blue", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                                    {
                                        ImGui.Indent(20f);
                                        float LiftBluePre = CurrentProfile.ColorBalanceMatPreImgui[0, 2];
                                        ImguiSliderRow("Lift", "ColorBalanceCombinedBluePreLift", -1f, 1f, ref LiftBluePre);
                                        CurrentProfile.ColorBalanceMatPreImgui[0, 2] = LiftBluePre;

                                        float GammaBluePre = CurrentProfile.ColorBalanceMatPreImgui[1, 2];
                                        ImguiSliderRow("Gamma", "ColorBalanceCombinedBluePreGamma", 0.01f, 2f, ref GammaBluePre);
                                        CurrentProfile.ColorBalanceMatPreImgui[1, 2] = GammaBluePre;

                                        float GainBluePre = CurrentProfile.ColorBalanceMatPreImgui[2, 2];
                                        ImguiSliderRow("Gain", "ColorBalanceCombinedBluePreGain", 0.01f, 2f, ref GainBluePre);
                                        CurrentProfile.ColorBalanceMatPreImgui[2, 2] = GainBluePre;
                                        ImGui.Unindent();
                                    }
                                    ImGui.PopID();
                                }
                                ImGui.Unindent();
                                ImGui.EndDisabled();


                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.PushID("ColorBalancePostCheckbox");
                                ImGui.Checkbox("Post imgui", ref CurrentProfile.ColorBalancePostImgui);
                                ImGui.PopID();
                                ImGui.Indent(20f);

                                ImGui.BeginDisabled(!CurrentProfile.ColorBalancePostImgui);
                                ImGui.TableNextColumn();
                                ImGui.PushID("ColorBalancePostSeparateRGB");
                                ImGui.Checkbox("Separate RGB", ref CurrentProfile.ColorBalancePostImguiSeparateRGB);
                                ImGui.PopID();

                                ImGui.TableNextColumn();
                                ImGui.PushID("ColorBalancePostReset");
                                if (ImGui.Button("Reset"))
                                {
                                    CurrentProfile.ColorBalancePostImguiSeparateRGB = false;
                                    CurrentProfile.ColorBalanceMatPostImgui = new float4x4(
                                        0, 0, 0, 0, // Lift
                                        1, 1, 1, 0, // Gamma
                                        1, 1, 1, 0, // Gain
                                        0, 0, 0, 1  // last value used as on/off toggle
                                    );
                                }
                                ImGui.PopID();

                                if (!CurrentProfile.ColorBalancePostImguiSeparateRGB)
                                {
                                    float LiftPost = CurrentProfile.ColorBalanceMatPostImgui[0, 0];
                                    ImguiSliderRow("Lift", "ColorBalanceCombinedPostLift", -1f, 1f, ref LiftPost);
                                    CurrentProfile.ColorBalanceMatPostImgui[0, 0] = LiftPost;
                                    CurrentProfile.ColorBalanceMatPostImgui[0, 1] = LiftPost;
                                    CurrentProfile.ColorBalanceMatPostImgui[0, 2] = LiftPost;

                                    float GammaPost = CurrentProfile.ColorBalanceMatPostImgui[1, 0];
                                    ImguiSliderRow("Gamma", "ColorBalanceCombinedPostGamma", 0.01f, 2f, ref GammaPost);
                                    CurrentProfile.ColorBalanceMatPostImgui[1, 0] = GammaPost;
                                    CurrentProfile.ColorBalanceMatPostImgui[1, 1] = GammaPost;
                                    CurrentProfile.ColorBalanceMatPostImgui[1, 2] = GammaPost;

                                    float GainPost = CurrentProfile.ColorBalanceMatPostImgui[2, 0];
                                    ImguiSliderRow("Gain", "ColorBalanceCombinedPostGain", 0.01f, 2f, ref GainPost);
                                    CurrentProfile.ColorBalanceMatPostImgui[2, 0] = GainPost;
                                    CurrentProfile.ColorBalanceMatPostImgui[2, 1] = GainPost;
                                    CurrentProfile.ColorBalanceMatPostImgui[2, 2] = GainPost;
                                }
                                else
                                {
                                    ImGui.TableNextRow();
                                    ImGui.TableNextColumn();
                                    ImGui.PushID("RedAdjustmentPost");
                                    if (ImGui.CollapsingHeader("Red", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                                    {
                                        ImGui.Indent(20f);
                                        float LiftRedPost = CurrentProfile.ColorBalanceMatPostImgui[0, 0];
                                        ImguiSliderRow("Lift", "ColorBalanceCombinedRedPostLift", -1f, 1f, ref LiftRedPost);
                                        CurrentProfile.ColorBalanceMatPostImgui[0, 0] = LiftRedPost;

                                        float GammaRedPost = CurrentProfile.ColorBalanceMatPostImgui[1, 0];
                                        ImguiSliderRow("Gamma", "ColorBalanceCombinedRedPostGamma", 0.01f, 2f, ref GammaRedPost);
                                        CurrentProfile.ColorBalanceMatPostImgui[1, 0] = GammaRedPost;

                                        float GainRedPost = CurrentProfile.ColorBalanceMatPostImgui[2, 0];
                                        ImguiSliderRow("Gain", "ColorBalanceCombinedRedPostGain", 0.01f, 2f, ref GainRedPost);
                                        CurrentProfile.ColorBalanceMatPostImgui[2, 0] = GainRedPost;
                                        ImGui.Unindent();
                                    }
                                    ImGui.PopID();

                                    ImGui.TableNextRow();
                                    ImGui.TableNextColumn();
                                    ImGui.PushID("GreenAdjustmentPost");
                                    if (ImGui.CollapsingHeader("Green", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                                    {
                                        ImGui.Indent(20f);
                                        float LiftGreenPost = CurrentProfile.ColorBalanceMatPostImgui[0, 1];
                                        ImguiSliderRow("Lift", "ColorBalanceCombinedGreenPostLift", -1f, 1f, ref LiftGreenPost);
                                        CurrentProfile.ColorBalanceMatPostImgui[0, 1] = LiftGreenPost;

                                        float GammaGreenPost = CurrentProfile.ColorBalanceMatPostImgui[1, 1];
                                        ImguiSliderRow("Gamma", "ColorBalanceCombinedGreenPostGamma", 0.01f, 2f, ref GammaGreenPost);
                                        CurrentProfile.ColorBalanceMatPostImgui[1, 1] = GammaGreenPost;

                                        float GainGreenPost = CurrentProfile.ColorBalanceMatPostImgui[2, 1];
                                        ImguiSliderRow("Gain", "ColorBalanceCombinedGreenPostGain", 0.01f, 2f, ref GainGreenPost);
                                        CurrentProfile.ColorBalanceMatPostImgui[2, 1] = GainGreenPost;
                                        ImGui.Unindent();
                                    }
                                    ImGui.PopID();

                                    ImGui.TableNextRow();
                                    ImGui.TableNextColumn();
                                    ImGui.PushID("BlueAdjustmentPost");
                                    if (ImGui.CollapsingHeader("Blue", TreeFlags | ImGuiTreeNodeFlags.SpanAllColumns))
                                    {
                                        ImGui.Indent(20f);
                                        float LiftBluePost = CurrentProfile.ColorBalanceMatPostImgui[0, 2];
                                        ImguiSliderRow("Lift", "ColorBalanceCombinedBluePostLift", -1f, 1f, ref LiftBluePost);
                                        CurrentProfile.ColorBalanceMatPostImgui[0, 2] = LiftBluePost;

                                        float GammaBluePost = CurrentProfile.ColorBalanceMatPostImgui[1, 2];
                                        ImguiSliderRow("Gamma", "ColorBalanceCombinedBluePostGamma", 0.01f, 2f, ref GammaBluePost);
                                        CurrentProfile.ColorBalanceMatPostImgui[1, 2] = GammaBluePost;

                                        float GainBluePost = CurrentProfile.ColorBalanceMatPostImgui[2, 2];
                                        ImguiSliderRow("Gain", "ColorBalanceCombinedBluePostGain", 0.01f, 2f, ref GainBluePost);
                                        CurrentProfile.ColorBalanceMatPostImgui[2, 2] = GainBluePost;
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
                                ImGui.Checkbox("pre imgui", ref CurrentProfile.ColorTempPreImgui);
                                ImGui.PopID();

                                ImGui.TableNextColumn();
                                ImGui.TableNextColumn();
                                ImGui.PushID("ColorTempPreReset");
                                if (ImGui.Button("Reset"))
                                {
                                    CurrentProfile.ColorTempFloatPreImgui = 6500f;
                                }
                                ImGui.PopID();

                                ImGui.BeginDisabled(!CurrentProfile.ColorTempPreImgui);
                                ImGui.Indent(20f);
                                ImguiSliderRow("Kelvin", "ColorTempPre", 1000f, 40000f, ref CurrentProfile.ColorTempFloatPreImgui);
                                ImGui.Unindent();
                                ImGui.EndDisabled();

                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.PushID("ColorTempPostCheckbox");
                                ImGui.Checkbox("Post imgui", ref CurrentProfile.ColorTempPostImgui);
                                ImGui.PopID();

                                ImGui.TableNextColumn();
                                ImGui.TableNextColumn();
                                ImGui.PushID("ColorTempPostReset");
                                if (ImGui.Button("Reset"))
                                {
                                    CurrentProfile.ColorTempFloatPostImgui = 6500f;
                                }
                                ImGui.PopID();

                                ImGui.BeginDisabled(!CurrentProfile.ColorTempPostImgui);
                                ImGui.Indent(20f);
                                ImguiSliderRow("Kelvin", "ColorTempPost", 1000f, 40000f, ref CurrentProfile.ColorTempFloatPostImgui);
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
                                ImGui.Checkbox("pre imgui", ref CurrentProfile.HueShiftPreImgui);
                                ImGui.PopID();

                                ImGui.TableNextColumn();
                                ImGui.TableNextColumn();
                                ImGui.PushID("HueShiftPreReset");
                                if (ImGui.Button("Reset"))
                                {
                                    CurrentProfile.HueShiftFloatPreImgui = 0f;
                                }
                                ImGui.PopID();

                                ImGui.BeginDisabled(!CurrentProfile.HueShiftPreImgui);
                                ImGui.Indent(20f);
                                ImguiSliderRow("Degree", "HueShiftPre", 0f, 360f, ref CurrentProfile.HueShiftFloatPreImgui);
                                ImGui.Unindent();
                                ImGui.EndDisabled();

                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.PushID("HueShiftPostCheckbox");
                                ImGui.Checkbox("Post imgui", ref CurrentProfile.HueShiftPostImgui);
                                ImGui.PopID();

                                ImGui.TableNextColumn();
                                ImGui.TableNextColumn();
                                ImGui.PushID("HueShiftPostReset");
                                if (ImGui.Button("Reset"))
                                {
                                    CurrentProfile.HueShiftFloatPostImgui = 0f;
                                }
                                ImGui.PopID();

                                ImGui.BeginDisabled(!CurrentProfile.HueShiftPostImgui);
                                ImGui.Indent(20f);
                                ImguiSliderRow("Degree", "HueShiftPost", 0f, 360f, ref CurrentProfile.HueShiftFloatPostImgui);
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
                                ImGui.Checkbox("pre imgui", ref CurrentProfile.BrightnessPreImgui);
                                ImGui.PopID();

                                ImGui.TableNextColumn();
                                ImGui.TableNextColumn();
                                ImGui.PushID("BrightnessPreReset");
                                if (ImGui.Button("Reset"))
                                {
                                    CurrentProfile.BrightnessFloatPreImgui = 1f;
                                }
                                ImGui.PopID();

                                ImGui.BeginDisabled(!CurrentProfile.BrightnessPreImgui);
                                ImGui.Indent(20f);
                                ImguiSliderRow("Multiplier", "BrightnessPre", 0.01f, 4f, ref CurrentProfile.BrightnessFloatPreImgui);
                                ImGui.Unindent();
                                ImGui.EndDisabled();

                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.PushID("BrightnessPostCheckbox");
                                ImGui.Checkbox("Post imgui", ref CurrentProfile.BrightnessPostImgui);
                                ImGui.PopID();

                                ImGui.TableNextColumn();
                                ImGui.TableNextColumn();
                                ImGui.PushID("BrightnessPostReset");
                                if (ImGui.Button("Reset"))
                                {
                                    CurrentProfile.BrightnessFloatPostImgui = 1f;
                                }
                                ImGui.PopID();

                                ImGui.BeginDisabled(!CurrentProfile.BrightnessPostImgui);
                                ImGui.Indent(20f);
                                ImguiSliderRow("Multiplier", "BrightnessPost", 0.01f, 4f, ref CurrentProfile.BrightnessFloatPostImgui);
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
                                ImGui.Checkbox("pre imgui", ref CurrentProfile.ContrastPreImgui);
                                ImGui.PopID();

                                ImGui.TableNextColumn();
                                ImGui.TableNextColumn();
                                ImGui.PushID("ContrastPreReset");
                                if (ImGui.Button("Reset"))
                                {
                                    CurrentProfile.ContrastFloatPreImgui = 1f;
                                }
                                ImGui.PopID();

                                ImGui.BeginDisabled(!CurrentProfile.ContrastPreImgui);
                                ImGui.Indent(20f);
                                ImguiSliderRow("Adjustment", "ContrastPre", 0.01f, 4f, ref CurrentProfile.ContrastFloatPreImgui);
                                ImGui.Unindent();
                                ImGui.EndDisabled();

                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.PushID("ContrastPostCheckbox");
                                ImGui.Checkbox("Post imgui", ref CurrentProfile.ContrastPostImgui);
                                ImGui.PopID();

                                ImGui.TableNextColumn();
                                ImGui.TableNextColumn();
                                ImGui.PushID("ContrastPostReset");
                                if (ImGui.Button("Reset"))
                                {
                                    CurrentProfile.ContrastFloatPostImgui = 1f;
                                }
                                ImGui.PopID();

                                ImGui.BeginDisabled(!CurrentProfile.ContrastPostImgui);
                                ImGui.Indent(20f);
                                ImguiSliderRow("Adjustment", "ContrastPost", 0.01f, 4f, ref CurrentProfile.ContrastFloatPostImgui);
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
                                ImGui.Checkbox("pre imgui", ref CurrentProfile.SaturationPreImgui);
                                ImGui.PopID();

                                ImGui.TableNextColumn();
                                ImGui.TableNextColumn();
                                ImGui.PushID("SaturationPreReset");
                                if (ImGui.Button("Reset"))
                                {
                                    CurrentProfile.SaturationFloatPreImgui = 1f;
                                }
                                ImGui.PopID();

                                ImGui.BeginDisabled(!CurrentProfile.SaturationPreImgui);
                                ImGui.Indent(20f);
                                ImguiSliderRow("Multiplier", "SaturationPre", 0.01f, 4f, ref CurrentProfile.SaturationFloatPreImgui);
                                ImGui.Unindent();
                                ImGui.EndDisabled();

                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.PushID("SaturationPostCheckbox");
                                ImGui.Checkbox("Post imgui", ref CurrentProfile.SaturationPostImgui);
                                ImGui.PopID();

                                ImGui.TableNextColumn();
                                ImGui.TableNextColumn();
                                ImGui.PushID("SaturationPostReset");
                                if (ImGui.Button("Reset"))
                                {
                                    CurrentProfile.SaturationFloatPostImgui = 1f;
                                }
                                ImGui.PopID();

                                ImGui.BeginDisabled(!CurrentProfile.SaturationPostImgui);
                                ImGui.Indent(20f);
                                ImguiSliderRow("Multiplier", "SaturationPost", 0.01f, 4f, ref CurrentProfile.SaturationFloatPostImgui);
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
                                ImGui.Checkbox("pre imgui", ref CurrentProfile.ColorOverlayPreImgui);
                                ImGui.PopID();

                                ImGui.BeginDisabled(!CurrentProfile.ColorOverlayPreImgui);
                                ImGui.TableNextColumn();
                                ImGui.PushID("ColorOverlayPreStrength");
                                ImGui.SetNextItemWidth(200f);
                                ImGui.ColorPicker4("Color", ref CurrentProfile.ColorOverlayFloatPreImgui, ImGuiColorEditFlags.DisplayRGB | ImGuiColorEditFlags.DisplayHSV | ImGuiColorEditFlags.DisplayRGB);
                                ImGui.PopID();
                                ImGui.EndDisabled();

                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.PushID("ColorOverlayPostCheckbox");
                                ImGui.Checkbox("post imgui", ref CurrentProfile.ColorOverlayPostImgui);
                                ImGui.PopID();

                                ImGui.BeginDisabled(!CurrentProfile.ColorOverlayPostImgui);
                                ImGui.TableNextColumn();
                                ImGui.PushID("ColorOverlayPostStrength");
                                ImGui.SetNextItemWidth(200f);
                                ImGui.ColorPicker4("Color", ref CurrentProfile.ColorOverlayFloatPostImgui, ImGuiColorEditFlags.DisplayRGB | ImGuiColorEditFlags.DisplayHSV | ImGuiColorEditFlags.DisplayRGB);
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
                                ImGui.Checkbox("pre imgui", ref CurrentProfile.VignettePreImgui);
                                ImGui.Indent(20);
                                ImGui.PopID();
                                ImGui.BeginDisabled(!CurrentProfile.VignettePreImgui);
                                ImGui.TableNextColumn();
                                ImGui.PushID("VignettePreColor");
                                ImGui.SetNextItemWidth(275f);
                                ImGui.ColorPicker4("Color", ref CurrentProfile.VignetteColorPreImgui, ImGuiColorEditFlags.DisplayRGB | ImGuiColorEditFlags.DisplayHSV | ImGuiColorEditFlags.DisplayRGB);
                                ImGui.PopID();

                                float rPre = CurrentProfile.VignetteFloatPreImgui.X;
                                ImguiSliderRow("Outer radius", "VignettePreOuterRadius", 0f, (float)Program.MainViewport.Size.X / Program.MainViewport.Size.Y, ref rPre);
                                CurrentProfile.VignetteFloatPreImgui.X = rPre;
                                float gPre = CurrentProfile.VignetteFloatPreImgui.Y;
                                ImguiSliderRow("Inner radius", "VignettePreInnerRadius", 0f, rPre - 0.001f, ref gPre);
                                CurrentProfile.VignetteFloatPreImgui.Y = Math.Min(gPre, rPre - 0.001f);
                                ImguiSliderRow("Aspectratio", "VignettePreAspectratio", 0.0001f, 5f, ref CurrentProfile.VignettePreAspectRatio);
                                CurrentProfile.VignetteFloatPreImgui.Z = CurrentProfile.VignettePreAspectRatio * Program.MainViewport.Size.Y / Program.MainViewport.Size.X;

                                ImGui.Unindent();
                                ImGui.EndDisabled();

                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();

                                ImGui.PushID("VignettePostCheckbox");
                                ImGui.Checkbox("Post imgui", ref CurrentProfile.VignettePostImgui);
                                ImGui.Indent(20);
                                ImGui.PopID();
                                ImGui.BeginDisabled(!CurrentProfile.VignettePostImgui);
                                ImGui.TableNextColumn();
                                ImGui.PushID("VignettePostColor");
                                ImGui.SetNextItemWidth(275f);
                                ImGui.ColorPicker4("Color", ref CurrentProfile.VignetteColorPostImgui, ImGuiColorEditFlags.DisplayRGB | ImGuiColorEditFlags.DisplayHSV | ImGuiColorEditFlags.DisplayRGB);
                                ImGui.PopID();

                                float rPost = CurrentProfile.VignetteFloatPostImgui.X;
                                ImguiSliderRow("Outer radius", "VignettePostOuterRadius", 0f, (float)Program.MainViewport.Size.X / Program.MainViewport.Size.Y, ref rPost);
                                CurrentProfile.VignetteFloatPostImgui.X = rPost;
                                float gPost = CurrentProfile.VignetteFloatPostImgui.Y;
                                ImguiSliderRow("Inner radius", "VignettePostInnerRadius", 0f, rPost - 0.001f, ref gPost);
                                CurrentProfile.VignetteFloatPostImgui.Y = Math.Min(gPost, rPost - 0.001f);
                                ImguiSliderRow("Aspectratio", "VignettePostAspectratio", 0.0001f, 5f, ref CurrentProfile.VignettePostAspectRatio);
                                CurrentProfile.VignetteFloatPostImgui.Z = CurrentProfile.VignettePostAspectRatio * Program.MainViewport.Size.Y / Program.MainViewport.Size.X;

                                ImGui.Unindent();
                                ImGui.EndDisabled();
                                ImGui.Unindent();
                            }
                            #endregion

                            #region FunStuffImgui
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
                                    ImGui.Checkbox("pre imgui", ref CurrentProfile.RGB2HSVPreImgui);
                                    ImGui.PopID();

                                    ImGui.BeginDisabled(!CurrentProfile.RGB2HSVPreImgui);
                                    ImGui.TableNextColumn();
                                    ImGui.PushID("RGB2HSVPreStrength");
                                    ImGui.SliderFloat("Percentage", ref CurrentProfile.RGB2HSVFloatPreImgui, 0f, 1f, flags: ImGuiSliderFlags.AlwaysClamp);
                                    ImGui.PopID();
                                    ImGui.EndDisabled();

                                    ImGui.TableNextRow();
                                    ImGui.TableNextColumn();
                                    ImGui.PushID("RGB2HSVPostCheckbox");
                                    ImGui.Checkbox("post imgui", ref CurrentProfile.RGB2HSVPostImgui);
                                    ImGui.PopID();

                                    ImGui.BeginDisabled(!CurrentProfile.RGB2HSVPostImgui);
                                    ImGui.TableNextColumn();
                                    ImGui.PushID("RGB2HSVPostStrength");
                                    ImGui.SliderFloat("Percentage", ref CurrentProfile.RGB2HSVFloatPostImgui, 0f, 1f, flags: ImGuiSliderFlags.AlwaysClamp);
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
                                    ImGui.Checkbox("pre imgui", ref CurrentProfile.HSV2RGBPreImgui);
                                    ImGui.PopID();

                                    ImGui.BeginDisabled(!CurrentProfile.HSV2RGBPreImgui);
                                    ImGui.TableNextColumn();
                                    ImGui.PushID("HSV2RGBPreStrength");
                                    ImGui.SliderFloat("Percentage", ref CurrentProfile.HSV2RGBFloatPreImgui, 0f, 1f, flags: ImGuiSliderFlags.AlwaysClamp);
                                    ImGui.PopID();
                                    ImGui.EndDisabled();

                                    ImGui.TableNextRow();
                                    ImGui.TableNextColumn();
                                    ImGui.PushID("HSV2RGBPostCheckbox");
                                    ImGui.Checkbox("post imgui", ref CurrentProfile.HSV2RGBPostImgui);
                                    ImGui.PopID();

                                    ImGui.BeginDisabled(!CurrentProfile.HSV2RGBPostImgui);
                                    ImGui.TableNextColumn();
                                    ImGui.PushID("HSV2RGBPostStrength");
                                    ImGui.SliderFloat("Percentage", ref CurrentProfile.HSV2RGBFloatPostImgui, 0f, 1f, flags: ImGuiSliderFlags.AlwaysClamp);
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
                                    ImGui.Checkbox("pre imgui", ref CurrentProfile.SingleColorPreImgui);
                                    ImGui.PopID();

                                    ImGui.BeginDisabled(!CurrentProfile.SingleColorPreImgui);
                                    ImGui.TableNextColumn();
                                    ImGui.PushID("SingleColorPreStrength");
                                    ImGui.SliderFloat("Percentage", ref CurrentProfile.SingleColorFloatPreImgui, 0f, 1f, flags: ImGuiSliderFlags.AlwaysClamp);
                                    ImGui.PopID();
                                    ImGui.EndDisabled();

                                    ImGui.TableNextRow();
                                    ImGui.TableNextColumn();
                                    ImGui.PushID("SingleColorPostCheckbox");
                                    ImGui.Checkbox("post imgui", ref CurrentProfile.SingleColorPostImgui);
                                    ImGui.PopID();

                                    ImGui.BeginDisabled(!CurrentProfile.SingleColorPostImgui);
                                    ImGui.TableNextColumn();
                                    ImGui.PushID("SingleColorPostStrength");
                                    ImGui.SliderFloat("Percentage", ref CurrentProfile.SingleColorFloatPostImgui, 0f, 1f, flags: ImGuiSliderFlags.AlwaysClamp);
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
                                    ImGui.Checkbox("pre imgui", ref CurrentProfile.SingleColorSmallestPreImgui);
                                    ImGui.PopID();

                                    ImGui.BeginDisabled(!CurrentProfile.SingleColorSmallestPreImgui);
                                    ImGui.TableNextColumn();
                                    ImGui.PushID("SingleColorSmallestPreStrength");
                                    ImGui.SliderFloat("Percentage", ref CurrentProfile.SingleColorSmallestFloatPreImgui, 0f, 1f, flags: ImGuiSliderFlags.AlwaysClamp);
                                    ImGui.PopID();
                                    ImGui.EndDisabled();

                                    ImGui.TableNextRow();
                                    ImGui.TableNextColumn();
                                    ImGui.PushID("SingleColorSmallestPostCheckbox");
                                    ImGui.Checkbox("post imgui", ref CurrentProfile.SingleColorSmallestPostImgui);
                                    ImGui.PopID();

                                    ImGui.BeginDisabled(!CurrentProfile.SingleColorSmallestPostImgui);
                                    ImGui.TableNextColumn();
                                    ImGui.PushID("SingleColorSmallestPostStrength");
                                    ImGui.SliderFloat("Percentage", ref CurrentProfile.SingleColorSmallestFloatPostImgui, 0f, 1f, flags: ImGuiSliderFlags.AlwaysClamp);
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
                                    ImGui.Checkbox("pre imgui", ref CurrentProfile.SingleColorSolidPreImgui);
                                    ImGui.PopID();

                                    ImGui.BeginDisabled(!CurrentProfile.SingleColorSolidPreImgui);
                                    ImGui.TableNextColumn();
                                    ImGui.PushID("SingleColorSolidPreStrength");
                                    ImGui.SliderFloat("Percentage", ref CurrentProfile.SingleColorSolidFloatPreImgui, 0f, 1f, flags: ImGuiSliderFlags.AlwaysClamp);
                                    ImGui.PopID();
                                    ImGui.EndDisabled();

                                    ImGui.TableNextRow();
                                    ImGui.TableNextColumn();
                                    ImGui.PushID("SingleColorSolidPostCheckbox");
                                    ImGui.Checkbox("post imgui", ref CurrentProfile.SingleColorSolidPostImgui);
                                    ImGui.PopID();

                                    ImGui.BeginDisabled(!CurrentProfile.SingleColorSolidPostImgui);
                                    ImGui.TableNextColumn();
                                    ImGui.PushID("SingleColorSolidPostStrength");
                                    ImGui.SliderFloat("Percentage", ref CurrentProfile.SingleColorSolidFloatPostImgui, 0f, 1f, flags: ImGuiSliderFlags.AlwaysClamp);
                                    ImGui.PopID();
                                    ImGui.EndDisabled();
                                }

                                ImGui.Unindent();
                            }
                            #endregion

                            ImGui.EndTable();
                            break;

                        case WindowTabs.ExportProfile:
                            ImGui.Text("Current Profile XML:");
                            ImGui.Separator();

                            string xml = Serializer.SerializeToXml(CurrentProfile);

                            ImGui.BeginChild("ExportProfileChild", new float2(0f, -32f), ImGuiChildFlags.AlwaysAutoResize);
                            ImGui.Text(xml);
                            ImGui.EndChild();

                            if (ImGui.Button("Copy to clipboard"))
                            {
                                ImGui.SetClipboardText(xml);
                            }

                            break;
                    }
                    ImGui.EndChild();

                    ImGui.Separator();

                    ImGui.BeginDisabled(CurrentTab == WindowTabs.Profiles);
                    if (ImGui.Button("Profiles")) CurrentTab = WindowTabs.Profiles;
                    ImGui.SameLine();
                    ImGui.EndDisabled();

                    ImGui.BeginDisabled(CurrentTab == WindowTabs.Shaders);
                    if (ImGui.Button("Shaders")) CurrentTab = WindowTabs.Shaders;
                    ImGui.SameLine();
                    ImGui.EndDisabled();

                    ImGui.BeginDisabled(CurrentTab == WindowTabs.ExportProfile);
                    if (ImGui.Button("Export profile")) CurrentTab = WindowTabs.ExportProfile;
                    ImGui.SameLine();
                    ImGui.EndDisabled();
                }
                ImGui.End();
            }
            #endregion
        }
    }
}
