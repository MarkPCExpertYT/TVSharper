// Decompiled with JetBrains decompiler
// Type: TVSharp.DeviceDisplay
// Assembly: TVSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 67BFC2D9-64F8-452D-BCAC-E227BAD9AE68
// Assembly location: C:\Users\Mark\Desktop\TVSharp\TVSharp.exe

using System;

namespace TVSharp
{
  public class DeviceDisplay
  {
    public uint Index { get; private set; }

    public string Name { get; set; }

    public static DeviceDisplay[] GetActiveDevices()
    {
      uint deviceCount = NativeMethods.rtlsdr_get_device_count();
      DeviceDisplay[] activeDevices = new DeviceDisplay[(int)(IntPtr) deviceCount];
      for (uint index = 0; index < deviceCount; ++index)
      {
        string deviceName = NativeMethods.rtlsdr_get_device_name(index);
        activeDevices[(int)(IntPtr)index] = new DeviceDisplay()
        {
          Index = index,
          Name = deviceName
        };
      }
      return activeDevices;
    }

    public override string ToString() => this.Name;
  }
}
