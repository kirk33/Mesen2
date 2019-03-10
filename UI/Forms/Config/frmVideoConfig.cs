﻿using Mesen.GUI.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mesen.GUI.Forms.Config
{
	public partial class frmVideoConfig : BaseConfigForm
	{
		public frmVideoConfig()
		{
			InitializeComponent();
			if(DesignMode) {
				return;
			}

			Entity = ConfigManager.Config.Video.Clone();

			//AddBinding(nameof(VideoConfig.ShowFPS), chkShowFps);
			AddBinding(nameof(VideoConfig.UseBilinearInterpolation), chkBilinearInterpolation);
			AddBinding(nameof(VideoConfig.VerticalSync), chkVerticalSync);
			AddBinding(nameof(VideoConfig.IntegerFpsMode), chkIntegerFpsMode);
			AddBinding(nameof(VideoConfig.FullscreenForceIntegerScale), chkFullscreenForceIntegerScale);
			AddBinding(nameof(VideoConfig.UseExclusiveFullscreen), chkUseExclusiveFullscreen);
			AddBinding(nameof(VideoConfig.ExclusiveFullscreenRefreshRate), cboRefreshRate);

			AddBinding(nameof(VideoConfig.VideoScale), nudScale);
			AddBinding(nameof(VideoConfig.AspectRatio), cboAspectRatio);
			AddBinding(nameof(VideoConfig.CustomAspectRatio), nudCustomRatio);
			AddBinding(nameof(VideoConfig.VideoFilter), cboFilter);

			AddBinding(nameof(VideoConfig.Brightness), trkBrightness);
			AddBinding(nameof(VideoConfig.Contrast), trkContrast);
			AddBinding(nameof(VideoConfig.Hue), trkHue);
			AddBinding(nameof(VideoConfig.Saturation), trkSaturation);
			AddBinding(nameof(VideoConfig.ScanlineIntensity), trkScanlines);

			AddBinding(nameof(VideoConfig.NtscArtifacts), trkArtifacts);
			AddBinding(nameof(VideoConfig.NtscBleed), trkBleed);
			AddBinding(nameof(VideoConfig.NtscFringing), trkFringing);
			AddBinding(nameof(VideoConfig.NtscGamma), trkGamma);
			AddBinding(nameof(VideoConfig.NtscResolution), trkResolution);
			AddBinding(nameof(VideoConfig.NtscSharpness), trkSharpness);
			AddBinding(nameof(VideoConfig.NtscMergeFields), chkMergeFields);
		}

		protected override bool ValidateInput()
		{
			UpdateObject();
			UpdateCustomRatioVisibility();

			VideoFilterType filter = ((VideoConfig)Entity).VideoFilter;
			grpNtscFilter.Visible = (filter == VideoFilterType.NTSC);

			((VideoConfig)this.Entity).ApplyConfig();

			return true;
		}

		protected override void OnApply()
		{
			ConfigManager.Config.Video = (VideoConfig)this.Entity;
			ConfigManager.ApplyChanges();
		}

		private void UpdateCustomRatioVisibility()
		{
			VideoAspectRatio ratio = cboAspectRatio.GetEnumValue<VideoAspectRatio>();
			lblCustomRatio.Visible = ratio == VideoAspectRatio.Custom;
			nudCustomRatio.Visible = ratio == VideoAspectRatio.Custom;
		}

		private void btnResetPictureSettings_Click(object sender, EventArgs e)
		{
			SetNtscPreset(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, false);
			cboFilter.SetEnumValue(VideoFilterType.None);
		}

		private void btnSelectPreset_Click(object sender, EventArgs e)
		{
			ctxPicturePresets.Show(btnSelectPreset.PointToScreen(new Point(0, btnSelectPreset.Height - 1)));
		}

		private void SetNtscPreset(int hue, int saturation, int contrast, int brightness, int sharpness, int gamma, int resolution, int artifacts, int fringing, int bleed, int scanlines, bool mergeFields)
		{
			cboFilter.SetEnumValue(VideoFilterType.NTSC);
			trkHue.Value = hue;
			trkSaturation.Value = saturation;
			trkContrast.Value = contrast;
			trkBrightness.Value = brightness;
			trkSharpness.Value = sharpness;
			trkGamma.Value = gamma;
			trkResolution.Value = resolution;
			trkArtifacts.Value = artifacts;
			trkFringing.Value = fringing;
			trkBleed.Value = bleed;
			chkMergeFields.Checked = mergeFields;

			trkScanlines.Value = scanlines;
		}

		private void mnuPresetComposite_Click(object sender, EventArgs e)
		{
			SetNtscPreset(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 15, false);
		}

		private void mnuPresetSVideo_Click(object sender, EventArgs e)
		{
			SetNtscPreset(0, 0, 0, 0, 20, 0, 20, -100, -100, 0, 15, false);
		}

		private void mnuPresetRgb_Click(object sender, EventArgs e)
		{
			SetNtscPreset(0, 0, 0, 0, 20, 0, 70, -100, -100, -100, 15, false);
		}

		private void mnuPresetMonochrome_Click(object sender, EventArgs e)
		{
			SetNtscPreset(0, -100, 0, 0, 20, 0, 70, -20, -20, -10, 15, false);
		}
	}
}
