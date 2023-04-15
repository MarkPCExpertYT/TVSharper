// Decompiled with JetBrains decompiler
// Type: TVSharp.SettingsMemoryEntry
// Assembly: TVSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 67BFC2D9-64F8-452D-BCAC-E227BAD9AE68
// Assembly location: C:\Users\Mark\Desktop\TVSharp\TVSharp.exe

using System;

namespace TVSharp
{
  public class SettingsMemoryEntry
  {
    private Decimal _frequency;
    private int _samplerate;
    private bool _programAgc;
    private bool _rtlAgc;
    private bool _tunerAgc;
    private bool _autoPositionCorrection;
    private float _detectorLevel;
    private int _frequencyCorrection;
    private int _contrast;
    private int _brightnes;
    private long[] _palSecamChannelFrequency;
    private long[] _ntscChannelFequency;
    private bool _inverseVideo;

    public SettingsMemoryEntry()
    {
    }

    public SettingsMemoryEntry(SettingsMemoryEntry memoryEntry)
    {
      this._programAgc = memoryEntry._programAgc;
      this._rtlAgc = memoryEntry._rtlAgc;
      this._tunerAgc = memoryEntry._tunerAgc;
      this._autoPositionCorrection = memoryEntry._autoPositionCorrection;
      this._frequencyCorrection = memoryEntry._frequencyCorrection;
      this._samplerate = memoryEntry._samplerate;
      this._frequency = memoryEntry._frequency;
      this._detectorLevel = memoryEntry._detectorLevel;
      this._brightnes = memoryEntry._brightnes;
      this._contrast = memoryEntry._contrast;
      this._palSecamChannelFrequency = memoryEntry._palSecamChannelFrequency;
      this._ntscChannelFequency = memoryEntry._ntscChannelFequency;
      this._inverseVideo = memoryEntry._inverseVideo;
    }

    public int Brightnes
    {
      get => this._brightnes;
      set => this._brightnes = value;
    }

    public bool AutoPositionCorrecion
    {
      get => this._autoPositionCorrection;
      set => this._autoPositionCorrection = value;
    }

    public long[] PalSecamChannelFrequency
    {
      get => this._palSecamChannelFrequency;
      set => this._palSecamChannelFrequency = value;
    }

    public long[] ntscChannelFrequency
    {
      get => this._ntscChannelFequency;
      set => this._ntscChannelFequency = value;
    }

    public bool ProgramAgc
    {
      get => this._programAgc;
      set => this._programAgc = value;
    }

    public bool TunerAgc
    {
      get => this._tunerAgc;
      set => this._tunerAgc = value;
    }

    public bool RtlAgc
    {
      get => this._rtlAgc;
      set => this._rtlAgc = value;
    }

    public int FrequencyCorrection
    {
      get => this._frequencyCorrection;
      set => this._frequencyCorrection = value;
    }

    public Decimal Frequency
    {
      get => this._frequency;
      set => this._frequency = value;
    }

    public int Samplerate
    {
      get => this._samplerate;
      set => this._samplerate = value;
    }

    public float DetectorLevel
    {
      get => this._detectorLevel;
      set => this._detectorLevel = value;
    }

    public int Contrast
    {
      get => this._contrast;
      set => this._contrast = value;
    }

    public bool InverseVideo
    {
      get => this._inverseVideo;
      set => this._inverseVideo = value;
    }
  }
}
