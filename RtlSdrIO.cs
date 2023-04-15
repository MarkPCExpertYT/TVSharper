// Decompiled with JetBrains decompiler
// Type: TVSharp.RtlSdrIO
// Assembly: TVSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 67BFC2D9-64F8-452D-BCAC-E227BAD9AE68
// Assembly location: C:\Users\Mark\Desktop\TVSharp\TVSharp.exe

using System;

namespace TVSharp
{
  public class RtlSdrIO : IDisposable
  {
    private RtlDevice _rtlDevice;
    private uint _frequency = 224000000;
    private SamplesReadyDelegate _callback;

    ~RtlSdrIO() => this.Dispose();

    public void Dispose() => GC.SuppressFinalize((object) this);

    public void SelectDevice(uint index)
    {
      this.Close();
      this._rtlDevice = new RtlDevice(index);
      this._rtlDevice.SamplesAvailable += new SamplesAvailableDelegate(this.rtlDevice_SamplesAvailable);
      this._rtlDevice.Frequency = this._frequency;
    }

    public RtlDevice Device => this._rtlDevice;

    public void Open()
    {
      DeviceDisplay[] activeDevices = DeviceDisplay.GetActiveDevices();
      foreach (DeviceDisplay deviceDisplay in activeDevices)
      {
        try
        {
          this.SelectDevice(deviceDisplay.Index);
          return;
        }
        catch (ApplicationException ex)
        {
        }
      }
      if (activeDevices.Length > 0)
        throw new ApplicationException(activeDevices.Length.ToString() + " compatible devices have been found but are all busy");
      throw new ApplicationException("No compatible devices found");
    }

    public void Close()
    {
      if (this._rtlDevice == null)
        return;
      this._rtlDevice.Stop();
      this._rtlDevice.SamplesAvailable -= new SamplesAvailableDelegate(this.rtlDevice_SamplesAvailable);
      this._rtlDevice.Dispose();
      this._rtlDevice = (RtlDevice) null;
    }

    public void Start(SamplesReadyDelegate callback)
    {
      if (this._rtlDevice == null)
        throw new ApplicationException("No device selected");
      this._callback = callback;
      try
      {
        this._rtlDevice.Start();
      }
      catch
      {
        this.Open();
        this._rtlDevice.Start();
      }
    }

    public void Stop() => this._rtlDevice.Stop();

    public double Samplerate => this._rtlDevice != null ? (double) this._rtlDevice.Samplerate : 0.0;

    public long Frequency
    {
      get => (long) this._frequency;
      set
      {
        this._frequency = (uint) value;
        if (this._rtlDevice == null)
          return;
        this._rtlDevice.Frequency = this._frequency;
      }
    }

    private unsafe void rtlDevice_SamplesAvailable(object sender, SamplesAvailableEventArgs e) => this._callback((object) this, e.Buffer, e.Length);
  }
}
