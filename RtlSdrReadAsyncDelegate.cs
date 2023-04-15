// Decompiled with JetBrains decompiler
// Type: TVSharp.RtlSdrReadAsyncDelegate
// Assembly: TVSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 67BFC2D9-64F8-452D-BCAC-E227BAD9AE68
// Assembly location: C:\Users\Mark\Desktop\TVSharp\TVSharp.exe

using System;
using System.Runtime.InteropServices;

namespace TVSharp
{
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public unsafe delegate void RtlSdrReadAsyncDelegate(byte* buf, uint len, IntPtr ctx);
}
