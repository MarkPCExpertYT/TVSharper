// Decompiled with JetBrains decompiler
// Type: TVSharp.UnsafeBuffer
// Assembly: TVSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 67BFC2D9-64F8-452D-BCAC-E227BAD9AE68
// Assembly location: C:\Users\Mark\Desktop\TVSharp\TVSharp.exe

using System;
using System.Runtime.InteropServices;

namespace TVSharp
{
  public sealed class UnsafeBuffer : IDisposable
  {
    private readonly GCHandle _handle;
    private unsafe void* _ptr;
    private int _length;
    private Array _buffer;

    private unsafe UnsafeBuffer(Array buffer, int realLength, bool aligned)
    {
      this._buffer = buffer;
      this._handle = GCHandle.Alloc((object) this._buffer, GCHandleType.Pinned);
      this._ptr = (void*) this._handle.AddrOfPinnedObject();
      if (aligned)
        this._ptr = (void*) ((ulong) this._ptr + 15UL & 18446744073709551600UL);
      this._length = realLength;
    }

    ~UnsafeBuffer() => this.Dispose();

    public unsafe void Dispose()
    {
      if (this._handle.IsAllocated)
        this._handle.Free();
      this._buffer = (Array) null;
      this._ptr = (void*) null;
      this._length = 0;
      GC.SuppressFinalize((object) this);
    }

    public unsafe void* Address => this._ptr;

    public int Length => this._length;

    public static unsafe implicit operator void*(UnsafeBuffer unsafeBuffer) => unsafeBuffer.Address;

    public static UnsafeBuffer Create(int size) => UnsafeBuffer.Create(1, size, true);

    public static UnsafeBuffer Create(int length, int sizeOfElement) => UnsafeBuffer.Create(length, sizeOfElement, true);

    public static UnsafeBuffer Create(int length, int sizeOfElement, bool aligned) => new UnsafeBuffer((Array) new byte[length * sizeOfElement + (aligned ? 16 : 0)], length, aligned);

    public static UnsafeBuffer Create(Array buffer) => new UnsafeBuffer(buffer, buffer.Length, false);
  }
}
