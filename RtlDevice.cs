// Decompiled with JetBrains decompiler
// Type: TVSharp.RtlDevice
// Assembly: TVSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 67BFC2D9-64F8-452D-BCAC-E227BAD9AE68
// Assembly location: C:\Users\Mark\Desktop\TVSharp\TVSharp.exe

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace TVSharp
{
  public unsafe sealed class RtlDevice : IDisposable
  {
    private const uint DefaultFrequency = 183000000;
    private const int DefaultSamplerate = 2000000;
    private readonly uint _index;
    private IntPtr _dev;
    private readonly string _name;
    private readonly int[] _supportedGains;
    private bool _useTunerAGC = false;
    private bool _useRtlAGC;
    private int _tunerGain;
    private uint _centerFrequency = 183000000;
    private uint _sampleRate = 2000000;
    private int _frequencyCorrection;
    private SamplingMode _samplingMode = SamplingMode.Quadrature;
    private bool _useOffsetTuning;
    private readonly bool _supportsOffsetTuning;
    private GCHandle _gcHandle;
    private UnsafeBuffer _iqBuffer;
    private unsafe Complex* _iqPtr;
    private Thread _worker;
    private readonly SamplesAvailableEventArgs _eventArgs = new SamplesAvailableEventArgs();
    private static readonly RtlSdrReadAsyncDelegate _rtlCallback = new RtlSdrReadAsyncDelegate(RtlDevice.RtlSdrSamplesAvailable);
    private static readonly uint _readLength = 32768;

    public RtlDevice(uint index)
    {
      this._index = index;
      if (NativeMethods.rtlsdr_open(out this._dev, this._index) != 0)
        throw new ApplicationException("Cannot open RTL device. Is the device locked somewhere?");
      int length = this._dev == IntPtr.Zero ? 0 : NativeMethods.rtlsdr_get_tuner_gains(this._dev, (int[]) null);
      if (length < 0)
        length = 0;
      this._supportsOffsetTuning = NativeMethods.rtlsdr_set_offset_tuning(this._dev, 0) != -2;
      this._supportedGains = new int[length];
      if (length >= 0)
        NativeMethods.rtlsdr_get_tuner_gains(this._dev, this._supportedGains);
      this._name = NativeMethods.rtlsdr_get_device_name(this._index);
      this._gcHandle = GCHandle.Alloc((object) this);
    }

    ~RtlDevice() => this.Dispose();

    public void Dispose()
    {
      this.Stop();
      NativeMethods.rtlsdr_close(this._dev);
      if (this._gcHandle.IsAllocated)
        this._gcHandle.Free();
      this._dev = IntPtr.Zero;
      GC.SuppressFinalize((object) this);
    }

    public event SamplesAvailableDelegate SamplesAvailable;

    public void Start()
    {
      if (this._worker != null)
        throw new ApplicationException("Already running");
      if (NativeMethods.rtlsdr_set_center_freq(this._dev, this._centerFrequency) != 0)
        throw new ApplicationException("Cannot access RTL device");
      if (NativeMethods.rtlsdr_set_tuner_gain_mode(this._dev, this._useTunerAGC ? 0 : 1) != 0)
        throw new ApplicationException("Cannot access RTL device");
      if (!this._useTunerAGC && NativeMethods.rtlsdr_set_tuner_gain(this._dev, this._tunerGain) != 0)
        throw new ApplicationException("Cannot access RTL device");
      if (NativeMethods.rtlsdr_reset_buffer(this._dev) != 0)
        throw new ApplicationException("Cannot access RTL device");
      this._worker = new Thread(new ThreadStart(this.StreamProc));
      this._worker.Priority = ThreadPriority.Highest;
      this._worker.Start();
    }

    public void Stop()
    {
      if (this._worker == null)
        return;
      NativeMethods.rtlsdr_cancel_async(this._dev);
      if (this._worker.ThreadState == ThreadState.Running)
        this._worker.Join();
      this._worker = (Thread) null;
    }

    public uint Index => this._index;

    public string Name => this._name;

    public uint Samplerate
    {
      get => this._sampleRate;
      set
      {
        this._sampleRate = value;
        if (!(this._dev != IntPtr.Zero))
          return;
        NativeMethods.rtlsdr_set_sample_rate(this._dev, this._sampleRate);
      }
    }

    public uint Frequency
    {
      get => this._centerFrequency;
      set
      {
        this._centerFrequency = value;
        if (this._dev != IntPtr.Zero && NativeMethods.rtlsdr_set_center_freq(this._dev, this._centerFrequency) != 0)
          throw new ArgumentException("The frequency cannot be set: " + (object) value);
      }
    }

    public bool UseRtlAGC
    {
      get => this._useRtlAGC;
      set
      {
        this._useRtlAGC = value;
        if (!(this._dev != IntPtr.Zero))
          return;
        NativeMethods.rtlsdr_set_agc_mode(this._dev, this._useRtlAGC ? 1 : 0);
      }
    }

    public bool UseTunerAGC
    {
      get => this._useTunerAGC;
      set
      {
        this._useTunerAGC = value;
        if (!(this._dev != IntPtr.Zero))
          return;
        NativeMethods.rtlsdr_set_tuner_gain_mode(this._dev, this._useTunerAGC ? 0 : 1);
      }
    }

    public SamplingMode SamplingMode
    {
      get => this._samplingMode;
      set
      {
        this._samplingMode = value;
        if (!(this._dev != IntPtr.Zero))
          return;
        NativeMethods.rtlsdr_set_direct_sampling(this._dev, (int) this._samplingMode);
      }
    }

    public bool SupportsOffsetTuning => this._supportsOffsetTuning;

    public bool UseOffsetTuning
    {
      get => this._useOffsetTuning;
      set
      {
        this._useOffsetTuning = value;
        if (!(this._dev != IntPtr.Zero))
          return;
        NativeMethods.rtlsdr_set_offset_tuning(this._dev, this._useOffsetTuning ? 1 : 0);
      }
    }

    public int[] SupportedGains => this._supportedGains;

    public int TunerGain
    {
      get => this._tunerGain;
      set
      {
        this._tunerGain = value;
        if (!(this._dev != IntPtr.Zero))
          return;
        NativeMethods.rtlsdr_set_tuner_gain(this._dev, this._tunerGain);
      }
    }

    public int FrequencyCorrection
    {
      get => this._frequencyCorrection;
      set
      {
        this._frequencyCorrection = value;
        if (!(this._dev != IntPtr.Zero))
          return;
        NativeMethods.rtlsdr_set_freq_correction(this._dev, this._frequencyCorrection);
      }
    }

    public RtlSdrTunerType TunerType => !(this._dev == IntPtr.Zero) ? NativeMethods.rtlsdr_get_tuner_type(this._dev) : RtlSdrTunerType.Unknown;

    public bool IsStreaming => this._worker != null;

    private void StreamProc() => NativeMethods.rtlsdr_read_async(this._dev, RtlDevice._rtlCallback, (IntPtr) this._gcHandle, 0U, RtlDevice._readLength);

    private unsafe void ComplexSamplesAvailable(Complex* buffer, int length)
    {
      if (this.SamplesAvailable == null)
        return;
      this._eventArgs.Buffer = buffer;
      this._eventArgs.Length = length;
      this.SamplesAvailable((object) this, this._eventArgs);
    }

    private static unsafe void RtlSdrSamplesAvailable(byte* buf, uint len, IntPtr ctx)
    {
      GCHandle gcHandle = GCHandle.FromIntPtr(ctx);
      if (!gcHandle.IsAllocated)
        return;
      RtlDevice target = (RtlDevice) gcHandle.Target;
      int length = (int) len / 2;
      if (target._iqBuffer == null || target._iqBuffer.Length != length)
      {
        target._iqBuffer = UnsafeBuffer.Create(length, sizeof (Complex));
        target._iqPtr = (Complex*) (void*) target._iqBuffer;
      }
      Complex* iqPtr = target._iqPtr;
      for (int index = 0; index < length; ++index)
      {
        iqPtr->Imag = (sbyte) ((int) *buf++ - 128);
        iqPtr->Real = (sbyte) ((int) *buf++ - 128);
        ++iqPtr;
      }
      target.ComplexSamplesAvailable(target._iqPtr, target._iqBuffer.Length);
    }
  }
}
