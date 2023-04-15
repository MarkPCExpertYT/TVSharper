// Decompiled with JetBrains decompiler
// Type: TVSharp.SamplesAvailableEventArgs
// Assembly: TVSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 67BFC2D9-64F8-452D-BCAC-E227BAD9AE68
// Assembly location: C:\Users\Mark\Desktop\TVSharp\TVSharp.exe

using System;

namespace TVSharp
{
  public sealed class SamplesAvailableEventArgs : EventArgs
  {
    public int Length { get; set; }

    public unsafe Complex* Buffer { get; set; }
  }
}
