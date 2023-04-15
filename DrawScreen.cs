// Decompiled with JetBrains decompiler
// Type: TVSharp.DrawScreen
// Assembly: TVSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 67BFC2D9-64F8-452D-BCAC-E227BAD9AE68
// Assembly location: C:\Users\Mark\Desktop\TVSharp\TVSharp.exe

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TVSharp
{
  internal class DrawScreen : UserControl
  {
    private const PixelFormat _pxf = PixelFormat.Format8bppIndexed;
    private Bitmap _buffer;
    private Rectangle _rect;

    public DrawScreen()
    {
      this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      this.SetStyle(ControlStyles.DoubleBuffer, true);
      this.SetStyle(ControlStyles.UserPaint, true);
      this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      this.UpdateStyles();
    }

    public void ArrayToBitmap(byte[] buf, int width, int height)
    {
      if (this._rect.Width != width || this._rect.Height != height)
        this._rect = new Rectangle(0, 0, width, height);
      if (this._buffer == null || this._buffer.Width != width || this._buffer.Height != height)
      {
        this._buffer = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
        this._buffer.Palette = this.GetGrayScalePalette();
      }
      BitmapData bitmapdata = this._buffer.LockBits(this._rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
      IntPtr scan0 = bitmapdata.Scan0;
      Marshal.Copy(buf, 0, scan0, buf.Length);
      this._buffer.UnlockBits(bitmapdata);
    }

    private ColorPalette GetGrayScalePalette()
    {
      ColorPalette palette = this._buffer.Palette;
      Color[] entries = palette.Entries;
      for (int index = 0; index < 256; ++index)
        entries[index] = Color.FromArgb(index, index, index);
      return palette;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      if (this._buffer == null)
        return;
      int width = this._rect.Width;
      int height = this._rect.Height;
      int srcX = (int) ((double) width * 0.15);
      int srcY = (int) ((double) height * 0.05);
      int srcWidth = width - srcX;
      int srcHeight = height - srcY;
      e.Graphics.DrawImage((Image) this._buffer, e.ClipRectangle, srcX, srcY, srcWidth, srcHeight, GraphicsUnit.Pixel);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
    }
  }
}
