// Decompiled with JetBrains decompiler
// Type: TVSharp.VideoWindow
// Assembly: TVSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 67BFC2D9-64F8-452D-BCAC-E227BAD9AE68
// Assembly location: C:\Users\Mark\Desktop\TVSharp\TVSharp.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TVSharp
{
  public class VideoWindow : Form
  {
    private IContainer components;
    private DrawScreen videoPanel;
    private int _oldVideoWindowWidth;
    private int _oldVideoWindowHeight;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.videoPanel = new DrawScreen();
      this.SuspendLayout();
      this.videoPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.videoPanel.Location = new Point(2, 3);
      this.videoPanel.Name = "videoPanel";
      this.videoPanel.Size = new Size(421, 323);
      this.videoPanel.TabIndex = 0;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.AutoValidate = AutoValidate.EnableAllowFocusChange;
      this.ClientSize = new Size(424, 327);
      this.ControlBox = false;
      this.Controls.Add((Control) this.videoPanel);
      this.FormBorderStyle = FormBorderStyle.Sizable;
      this.Name = nameof (VideoWindow);
      this.ShowIcon = true;
      this.ShowInTaskbar = true;
      this.Text = "Video";
      this.ResizeBegin += new EventHandler(this.VideoWindows_ResizeBegin);
      this.Resize += new EventHandler(this.VideoWindows_Resize);
      this.ResumeLayout(false);
    }

    public VideoWindow() => this.InitializeComponent();

    public void DrawPictures(byte[] buf, int pictureWidth, int pictureHeight)
    {
      this.videoPanel.ArrayToBitmap(buf, pictureWidth, pictureHeight);
      this.videoPanel.Refresh();
    }

    private void VideoWindows_ResizeBegin(object sender, EventArgs e)
    {
      Control control = (Control) sender;
      this._oldVideoWindowWidth = control.Size.Width;
      this._oldVideoWindowHeight = control.Size.Height;
    }

    private void VideoWindows_Resize(object sender, EventArgs e)
    {
      Control control = (Control) sender;
      int width = control.Size.Width;
      int height = control.Size.Height;
      if (this._oldVideoWindowWidth != width && this._oldVideoWindowHeight == height)
        height = width / 4 * 3;
      else if (this._oldVideoWindowWidth == width && this._oldVideoWindowHeight != height)
        width = height / 3 * 4;
      else
        height = width / 4 * 3;
      control.Size = new Size(width, height);
    }
  }
}
