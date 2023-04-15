// Decompiled with JetBrains decompiler
// Type: TVSharp.MainForm
// Assembly: TVSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 67BFC2D9-64F8-452D-BCAC-E227BAD9AE68
// Assembly location: C:\Users\Mark\Desktop\TVSharp\TVSharp.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace TVSharp
{
  public unsafe class MainForm : Form
  {
    private unsafe readonly RtlSdrIO _rtlDevice = new RtlSdrIO();
    private bool _isDecoding;
    private bool _initialized;
    private byte[] _grayScaleValues;
    private byte[] _videoWindowArray;
    private int _detectLevel;
    private float _detectorLevelCoef;
    private int _detectImpulsePeriod;
    private int _x;
    private int _y;
    private int _pictureWidth;
    private int _pictureHeight;
    private int _correctX;
    private int _correctY;
    private int _autoCorrectX;
    private int _autoCorrectY;
    private int _counterAutoFreqCorrect;
    private bool _autoPositionCorrect;
    private bool _autoFrequencyCorrection;
    private int _fineFrequencyCorrection;
    private int _fineTick;
    private int _pictureCentr;
    private float _countryCoeff;
    private int _lineInFrame;
    private int _pixelCounter;
    private int _maxSignalLevel;
    private int _agcSignalLevel;
    private int _blackLevel;
    private int _bright;
    private float _contrast;
    private float _coeff;
    private double _sampleRate;
    private int _frequencyCorrection;
    private SettingsMemoryEntry _settings;
    private readonly SettingsPersister _settingsPersister = new SettingsPersister();
    private VideoWindow videoWindow;
    private bool _inverseVideo;
    private bool _bufferIsFull;
    private static object locker = new object();
    private IContainer components;
    private Button startBtn;
    private Timer fpsTimer;
    private Label gainLabel;
    private NumericUpDown frequencyCorrectionNumericUpDown;
    private Label label2;
    private TrackBar tunerGainTrackBar;
    private Label tunerTypeLabel;
    private ComboBox deviceComboBox;
    private Label label1;
    private NumericUpDown frequencyNumericUpDown;
    private Label label5;
    private ComboBox samplerateComboBox;
    private Label label3;
    private TrackBar brightnesTrackBar;
    private TrackBar contrastTrackBar;
    private Label label6;
    private Label label7;
    private CheckBox tunerAgcCheckBox;
    private CheckBox rtlAgcCheckBox;
    private CheckBox programAgcCheckBox;
    private Timer frequencyCorrectionTimer;
    private TrackBar frequencyCorrectionTrackBar;
    private Label label4;
    private TrackBar fineFrequencyCorrectTrackBar;
    private TrackBar xCorrectionTrackBar;
    private TrackBar yCorrectionTrackBar;
    private Label label9;
    private Label label10;
    private Label label11;
    private GroupBox frequencyCorrectionGroupBox;
    private GroupBox positionGroupBox;
    private CheckBox autoSincCheckBox;
    private GroupBox tunerGroupBox;
    private Label label8;
    private Button autoFrequencyCorrectionButton;
    private CheckBox inverseCheckBox;
    private Label label12;

    public MainForm()
    {
      this.InitializeComponent();
      this.videoWindow = new VideoWindow();
      this._settings = this._settingsPersister.ReadSettings();
      this.frequencyNumericUpDown_ValueChanged((object) null, (EventArgs) null);
      try
      {
        this._rtlDevice.Open();
        DeviceDisplay[] activeDevices = DeviceDisplay.GetActiveDevices();
        this.deviceComboBox.Items.Clear();
        this.deviceComboBox.Items.AddRange((object[]) activeDevices);
        this.deviceComboBox.SelectedIndex = 0;
        this.deviceComboBox_SelectedIndexChanged((object) null, (EventArgs) null);
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(ex.Message);
      }
    }

    private void startBtn_Click(object sender, EventArgs e)
    {
      if (!this._isDecoding)
        this.StartDecoding();
      else
        this.StopDecoding();
      this.startBtn.Text = this._isDecoding ? "Stop" : "Start";
      this.deviceComboBox.Enabled = !this._rtlDevice.Device.IsStreaming;
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (this._isDecoding)
        this.StopDecoding();
      this._settings.Contrast = this.contrastTrackBar.Value;
      this._settings.Brightnes = this.brightnesTrackBar.Value;
      this._settings.Frequency = this.frequencyNumericUpDown.Value;
      this._settings.FrequencyCorrection = (int) this.frequencyCorrectionNumericUpDown.Value;
      this._settings.Samplerate = this.samplerateComboBox.SelectedIndex;
      this._settings.ProgramAgc = this.programAgcCheckBox.Checked;
      this._settings.TunerAgc = this.tunerAgcCheckBox.Checked;
      this._settings.RtlAgc = this.rtlAgcCheckBox.Checked;
      this._settings.DetectorLevel = this._detectorLevelCoef;
      this._settings.AutoPositionCorrecion = this._autoPositionCorrect;
      this._settings.InverseVideo = this._inverseVideo;
      this._settingsPersister.PersistSettings(this._settings);
    }

    private void deviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      DeviceDisplay selectedItem = (DeviceDisplay) this.deviceComboBox.SelectedItem;
      if (selectedItem == null)
        return;
      try
      {
        this._rtlDevice.SelectDevice(selectedItem.Index);
        this._rtlDevice.Frequency = 100000000L;
        this._rtlDevice.Device.Samplerate = 2000000U;
        this._rtlDevice.Device.UseRtlAGC = false;
        this._rtlDevice.Device.UseTunerAGC = false;
        this._rtlDevice.Device.SamplingMode = SamplingMode.DirectSamplingQ;
        this._initialized = true;
      }
      catch (Exception ex)
      {
        this.deviceComboBox.SelectedIndex = -1;
        this._initialized = false;
        int num = (int) MessageBox.Show((IWin32Window) this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }
      this.ConfigureDevice();
      this.ConfigureGUI();
    }

    private void tunerGainTrackBar_Scroll(object sender, EventArgs e)
    {
      if (!this._initialized)
        return;
      int supportedGain = this._rtlDevice.Device.SupportedGains[this.tunerGainTrackBar.Value];
      this._rtlDevice.Device.TunerGain = supportedGain;
      this.gainLabel.Text = ((double) supportedGain / 10.0).ToString() + " dB";
    }

    private void frequencyCorrectionNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
      if (!this._initialized)
        return;
      this._rtlDevice.Device.FrequencyCorrection = (int) this.frequencyCorrectionNumericUpDown.Value;
      this.frequencyCorrectionTrackBar.Value = (int) this.frequencyCorrectionNumericUpDown.Value;
    }

    private void ConfigureGUI()
    {
      this.startBtn.Enabled = this._initialized;
      if (!this._initialized)
        return;
      this.videoWindow.Visible = true;
      this.tunerTypeLabel.Text = this._rtlDevice.Device.TunerType.ToString();
      this.tunerGainTrackBar.Maximum = this._rtlDevice.Device.SupportedGains.Length - 1;
      this.tunerGainTrackBar.Value = this.tunerGainTrackBar.Maximum;
      if (this._settings.Samplerate < 0)
        this._settings.Samplerate = 2;
      this.samplerateComboBox.SelectedIndex = this._settings.Samplerate;
      this.samplerateComboBox_SelectedIndexChanged((object) null, (EventArgs) null);
      this.brightnesTrackBar.Value = this._settings.Brightnes;
      this.brightnesTrackBar_Scroll((object) null, (EventArgs) null);
      this.contrastTrackBar.Value = this._settings.Contrast;
      this.contrastTrackBar_Scroll((object) null, (EventArgs) null);
      this.frequencyCorrectionNumericUpDown.Value = (Decimal) this._settings.FrequencyCorrection;
      this.frequencyCorrectionNumericUpDown_ValueChanged((object) null, (EventArgs) null);
      this.frequencyNumericUpDown.Value = this._settings.Frequency;
      this.frequencyNumericUpDown_ValueChanged((object) null, (EventArgs) null);
      this.rtlAgcCheckBox.Checked = this._settings.RtlAgc;
      this.rtlAgcCheckBox_CheckedChanged((object) null, (EventArgs) null);
      this.tunerAgcCheckBox.Checked = this._settings.TunerAgc;
      this.tunerAgcCheckBox_CheckedChanged((object) null, (EventArgs) null);
      this.programAgcCheckBox.Checked = this._settings.ProgramAgc;
      this.programAgcCheckBox_CheckedChanged((object) null, (EventArgs) null);
      this.autoSincCheckBox.Checked = this._settings.AutoPositionCorrecion;
      this.autoSincCheckBox_CheckedChanged((object) null, (EventArgs) null);
      this.inverseCheckBox.Checked = this._settings.InverseVideo;
      this.inverseCheckBox_CheckedChanged((object) null, (EventArgs) null);
      for (int index = 0; index < this.deviceComboBox.Items.Count; ++index)
      {
        if ((int) ((DeviceDisplay) this.deviceComboBox.Items[index]).Index == (int) this._rtlDevice.Device.Index)
        {
          this.deviceComboBox.SelectedIndex = index;
          break;
        }
      }
    }

    private void ConfigureDevice()
    {
      this.frequencyCorrectionNumericUpDown_ValueChanged((object) null, (EventArgs) null);
      this.tunerGainTrackBar_Scroll((object) null, (EventArgs) null);
    }

    private void StartDecoding()
    {
      this._detectorLevelCoef = this._settings.DetectorLevel;
      if ((double) this._detectorLevelCoef == 0.0)
        this._detectorLevelCoef = 0.77f;
      this._pictureHeight = this._lineInFrame;
      this._pictureWidth = (int) (this._sampleRate / (double) this._countryCoeff);
      this._pictureCentr = this._pictureWidth / 2;
      this._detectImpulsePeriod = this._pictureWidth / 4;
      int length = this._pictureWidth * this._pictureHeight;
      this._grayScaleValues = new byte[length];
      this._videoWindowArray = new byte[length];
      try
      {
        this._rtlDevice.Start(new SamplesReadyDelegate(this.rtl_SamplesAvailable));
      }
      catch (Exception ex)
      {
        this.StopDecoding();
        int num = (int) MessageBox.Show("Unable to start RTL device\n" + ex.Message);
        return;
      }
      this._isDecoding = true;
    }

    private void StopDecoding()
    {
      this._rtlDevice.Stop();
      this._isDecoding = false;
      this._grayScaleValues = (byte[]) null;
    }

    private unsafe void rtl_SamplesAvailable(object sender, Complex* buf, int length)
    {
      int val2_1 = 0;
      int val2_2 = 0;
      for (int index = 0; index < length; ++index)
      {
        sbyte real = buf[index].Real;
        sbyte imag = buf[index].Imag;
        val2_1 = Math.Max((int) real, val2_1);
        int num = (int) Math.Sqrt((double) ((int) real * (int) real + (int) imag * (int) imag));
        val2_2 = Math.Max(num, val2_2);
        if (this._inverseVideo)
          num = this._maxSignalLevel - num;
        this.DrawPixel(num);
      }
      this._maxSignalLevel = (int) ((double) this._maxSignalLevel * 0.9 + (double) val2_2 * 0.1);
      this._detectLevel = Convert.ToInt32((float) this._maxSignalLevel * this._detectorLevelCoef);
      this._blackLevel = Convert.ToInt32((float) this._maxSignalLevel * 0.7f);
      this._coeff = (float) byte.MaxValue / (float) this._blackLevel;
      this._agcSignalLevel = val2_1;
      if (!this._bufferIsFull)
        return;
      lock (MainForm.locker)
      {
        Array.Copy((Array) this._grayScaleValues, (Array) this._videoWindowArray, this._grayScaleValues.Length);
        this._bufferIsFull = false;
      }
    }

    private void DrawPixel(int mag)
    {
      if (mag > this._detectLevel)
      {
        ++this._pixelCounter;
      }
      else
      {
        if (this._pixelCounter > 5)
        {
          if (this._pixelCounter > this._detectImpulsePeriod)
            this._autoCorrectY = this._y;
          else if (this._pixelCounter < this._detectImpulsePeriod)
            this._autoCorrectX = this._x - this._pixelCounter;
        }
        this._pixelCounter = 0;
      }
      if (this._x >= this._pictureWidth)
      {
        this._y += 2;
        this._x = 0;
      }
      if (this._y == this._pictureHeight)
      {
        this._y = 0;
        this.FineFrequencyCorrection_Tick();
      }
      if (this._y > this._pictureHeight)
      {
        this._y = 1;
        this.FineFrequencyCorrection_Tick();
        this._bufferIsFull = true;
      }
      int index = (this._y + this._correctY) % this._pictureHeight * this._pictureWidth + (this._x + this._correctX) % this._pictureWidth;
      float num = (float) (this._blackLevel - mag) * this._coeff * this._contrast + (float) this._bright;
      if ((double) num > (double) byte.MaxValue)
        num = (float) byte.MaxValue;
      if ((double) num < 0.0)
        num = 0.0f;
      this._grayScaleValues[index] = (byte) num;
      ++this._x;
    }

    private void FineFrequencyCorrection_Tick()
    {
      this._fineTick += this._fineFrequencyCorrection;
      if (this._fineTick > 200)
      {
        this._correctX = (this._correctX + 1) % this._pictureWidth;
        this._fineTick = 0;
      }
      else
      {
        if (this._fineTick >= -200)
          return;
        --this._correctX;
        if (this._correctX < 0)
          this._correctX = this._pictureWidth;
        this._fineTick = 0;
      }
    }

    private void fpsTimer_Tick(object sender, EventArgs e)
    {
      if (!this._isDecoding)
        return;
      lock (MainForm.locker)
        this.videoWindow.DrawPictures(this._videoWindowArray, this._pictureWidth, this._pictureHeight);
    }

    private void frequencyNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
      if (!this._initialized)
        return;
      this._rtlDevice.Device.Frequency = (uint) (this.frequencyNumericUpDown.Value * 1000000M);
      this.videoWindow.Text = Convert.ToString(this.frequencyNumericUpDown.Value);
    }

    private void brightnesTrackBar_Scroll(object sender, EventArgs e) => this._bright = this.brightnesTrackBar.Value;

    private void contrastTrackBar_Scroll(object sender, EventArgs e) => this._contrast = (float) this.contrastTrackBar.Value / 10f;

    private void samplerateComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (!this._initialized)
        return;
      bool isDecoding = this._isDecoding;
      if (isDecoding)
        this.StopDecoding();
      this._sampleRate = double.Parse(this.samplerateComboBox.Items[this.samplerateComboBox.SelectedIndex].ToString().Split(' ')[0], (IFormatProvider) CultureInfo.InvariantCulture) * 1000000.0;
      this._rtlDevice.Device.Samplerate = (uint) this._sampleRate;
      if (this.samplerateComboBox.SelectedIndex > 5)
      {
        this._countryCoeff = 15734.25f;
        this._lineInFrame = 525;
      }
      else
      {
        this._countryCoeff = 15625f;
        this._lineInFrame = 625;
      }
      if (!isDecoding)
        return;
      this.StartDecoding();
    }

    private void tunerAgcCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      this._rtlDevice.Device.UseTunerAGC = this.tunerAgcCheckBox.Checked;
      this.tunerGainTrackBar.Enabled = !this.tunerAgcCheckBox.Checked;
    }

    private void rtlAgcCheckBox_CheckedChanged(object sender, EventArgs e) => this._rtlDevice.Device.UseRtlAGC = this.rtlAgcCheckBox.Checked;

    private void programAgcCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      if (this.programAgcCheckBox.Checked)
      {
        this.tunerAgcCheckBox.Enabled = false;
        this.tunerGainTrackBar.Enabled = false;
        this._rtlDevice.Device.UseTunerAGC = false;
        this._rtlDevice.Device.SamplingMode = SamplingMode.DirectSamplingQ;
        }
      else
      {
        this._rtlDevice.Device.UseTunerAGC = this.tunerAgcCheckBox.Checked;
        this.tunerGainTrackBar.Enabled = true;
        this.tunerAgcCheckBox.Enabled = true;
        this._rtlDevice.Device.SamplingMode = SamplingMode.Quadrature;
        }
      }

    private void frequencyCorrectionTimer_Tick(object sender, EventArgs e)
    {
      if (!this._isDecoding)
        return;
      if (this._autoFrequencyCorrection)
      {
        if (this._frequencyCorrection < this._autoCorrectX)
        {
          this.frequencyCorrectionNumericUpDown.Value += this.frequencyCorrectionNumericUpDown.Value % 200M;
          this._counterAutoFreqCorrect = 0;
        }
        else if (this._frequencyCorrection > this._autoCorrectX)
        {
          this.frequencyCorrectionNumericUpDown.Value -= this.frequencyCorrectionNumericUpDown.Value % 200M;
          this._counterAutoFreqCorrect = 0;
        }
        else if (this._frequencyCorrection == this._autoCorrectX)
        {
          ++this._counterAutoFreqCorrect;
          if (this._counterAutoFreqCorrect == 3)
            this.autoFrequencyCorrectionButton_Click((object) null, (EventArgs) null);
        }
        this._frequencyCorrection = this._autoCorrectX;
      }
      if (this._autoPositionCorrect)
      {
        this._correctX = this._pictureWidth - this._autoCorrectX;
        this._correctY = this._pictureHeight - this._autoCorrectY;
      }
      if (this._agcSignalLevel > 125)
        this.label2.Text = "Gain Overload";
      else
        this.label2.Text = "Gain";
    }

    private void xCorrectionTrackBar_Scroll(object sender, EventArgs e) => this._correctX = this.xCorrectionTrackBar.Value;

    private void yCorrectionTrackBar_Scroll(object sender, EventArgs e) => this._correctY = this.yCorrectionTrackBar.Value * 2;

    private void frequencyCorrectionTrackBar_Scroll(object sender, EventArgs e)
    {
      this.frequencyCorrectionNumericUpDown.Value = (Decimal) this.frequencyCorrectionTrackBar.Value;
      this.frequencyCorrectionNumericUpDown_ValueChanged((object) null, (EventArgs) null);
    }

    private void autoSincCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      this._autoPositionCorrect = this.autoSincCheckBox.Checked;
      this.xCorrectionTrackBar.Enabled = !this._autoPositionCorrect;
      this.yCorrectionTrackBar.Enabled = !this._autoPositionCorrect;
      if (this._autoPositionCorrect)
        return;
      this.xCorrectionTrackBar.Value = this._correctX;
      this.yCorrectionTrackBar.Value = this._correctY / 2;
    }

    private void autoFrequencyCorrectionButton_Click(object sender, EventArgs e)
    {
      this._autoFrequencyCorrection = !this._autoFrequencyCorrection;
      if (this._autoFrequencyCorrection)
        this.autoFrequencyCorrectionButton.Text = "Stop correction";
      else
        this.autoFrequencyCorrectionButton.Text = "Auto correction";
    }

    private void fineFrequencyCorrectTrackBar_Scroll(object sender, EventArgs e) => this._fineFrequencyCorrection = this.fineFrequencyCorrectTrackBar.Value;

    private void inverseCheckBox_CheckedChanged(object sender, EventArgs e) => this._inverseVideo = this.inverseCheckBox.Checked;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (MainForm));
      this.startBtn = new Button();
      this.fpsTimer = new Timer(this.components);
      this.gainLabel = new Label();
      this.frequencyCorrectionNumericUpDown = new NumericUpDown();
      this.label2 = new Label();
      this.tunerGainTrackBar = new TrackBar();
      this.tunerTypeLabel = new Label();
      this.deviceComboBox = new ComboBox();
      this.label1 = new Label();
      this.frequencyNumericUpDown = new NumericUpDown();
      this.label5 = new Label();
      this.samplerateComboBox = new ComboBox();
      this.label3 = new Label();
      this.brightnesTrackBar = new TrackBar();
      this.contrastTrackBar = new TrackBar();
      this.label6 = new Label();
      this.label7 = new Label();
      this.tunerAgcCheckBox = new CheckBox();
      this.rtlAgcCheckBox = new CheckBox();
      this.programAgcCheckBox = new CheckBox();
      this.frequencyCorrectionTimer = new Timer(this.components);
      this.frequencyCorrectionTrackBar = new TrackBar();
      this.label4 = new Label();
      this.fineFrequencyCorrectTrackBar = new TrackBar();
      this.xCorrectionTrackBar = new TrackBar();
      this.yCorrectionTrackBar = new TrackBar();
      this.label9 = new Label();
      this.label10 = new Label();
      this.label11 = new Label();
      this.frequencyCorrectionGroupBox = new GroupBox();
      this.autoFrequencyCorrectionButton = new Button();
      this.positionGroupBox = new GroupBox();
      this.autoSincCheckBox = new CheckBox();
      this.tunerGroupBox = new GroupBox();
      this.label8 = new Label();
      this.inverseCheckBox = new CheckBox();
      this.label12 = new Label();
      this.frequencyCorrectionNumericUpDown.BeginInit();
      this.tunerGainTrackBar.BeginInit();
      this.frequencyNumericUpDown.BeginInit();
      this.brightnesTrackBar.BeginInit();
      this.contrastTrackBar.BeginInit();
      this.frequencyCorrectionTrackBar.BeginInit();
      this.fineFrequencyCorrectTrackBar.BeginInit();
      this.xCorrectionTrackBar.BeginInit();
      this.yCorrectionTrackBar.BeginInit();
      this.frequencyCorrectionGroupBox.SuspendLayout();
      this.positionGroupBox.SuspendLayout();
      this.tunerGroupBox.SuspendLayout();
      this.SuspendLayout();
      this.startBtn.Enabled = false;
      this.startBtn.Location = new Point(276, 372);
      this.startBtn.Name = "startBtn";
      this.startBtn.Size = new Size(37, 82);
      this.startBtn.TabIndex = 0;
      this.startBtn.Text = "Start";
      this.startBtn.UseVisualStyleBackColor = true;
      this.startBtn.Click += new EventHandler(this.startBtn_Click);
      this.fpsTimer.Enabled = true;
      this.fpsTimer.Interval = 50;
      this.fpsTimer.Tick += new EventHandler(this.fpsTimer_Tick);
      this.gainLabel.Location = new Point(181, 106);
      this.gainLabel.Name = "gainLabel";
      this.gainLabel.Size = new Size(68, 13);
      this.gainLabel.TabIndex = 26;
      this.gainLabel.Text = "1000dB";
      this.gainLabel.TextAlign = ContentAlignment.MiddleRight;
      this.frequencyCorrectionNumericUpDown.Location = new Point(190, 18);
      this.frequencyCorrectionNumericUpDown.Maximum = new Decimal(new int[4]
      {
        200,
        0,
        0,
        0
      });
      this.frequencyCorrectionNumericUpDown.Minimum = new Decimal(new int[4]
      {
        200,
        0,
        0,
        int.MinValue
      });
      this.frequencyCorrectionNumericUpDown.Name = "frequencyCorrectionNumericUpDown";
      this.frequencyCorrectionNumericUpDown.Size = new Size(55, 20);
      this.frequencyCorrectionNumericUpDown.TabIndex = 4;
      this.frequencyCorrectionNumericUpDown.TextAlign = HorizontalAlignment.Right;
      this.frequencyCorrectionNumericUpDown.ValueChanged += new EventHandler(this.frequencyCorrectionNumericUpDown_ValueChanged);
      this.label2.AutoSize = true;
      this.label2.Location = new Point(15, 106);
      this.label2.Name = "label2";
      this.label2.Size = new Size(29, 13);
      this.label2.TabIndex = 22;
      this.label2.Text = "Gain";
      this.tunerGainTrackBar.AutoSize = false;
      this.tunerGainTrackBar.Location = new Point(18, 122);
      this.tunerGainTrackBar.Maximum = 10000;
      this.tunerGainTrackBar.Name = "tunerGainTrackBar";
      this.tunerGainTrackBar.Size = new Size(231, 30);
      this.tunerGainTrackBar.TabIndex = 3;
      this.tunerGainTrackBar.Scroll += new EventHandler(this.tunerGainTrackBar_Scroll);
      this.tunerTypeLabel.Location = new Point(156, 17);
      this.tunerTypeLabel.Name = "tunerTypeLabel";
      this.tunerTypeLabel.Size = new Size(93, 13);
      this.tunerTypeLabel.TabIndex = 29;
      this.tunerTypeLabel.Text = "E4000";
      this.tunerTypeLabel.TextAlign = ContentAlignment.MiddleRight;
      this.deviceComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
      this.deviceComboBox.FormattingEnabled = true;
      this.deviceComboBox.Location = new Point(6, 33);
      this.deviceComboBox.Name = "deviceComboBox";
      this.deviceComboBox.Size = new Size(243, 21);
      this.deviceComboBox.TabIndex = 0;
      this.deviceComboBox.SelectedIndexChanged += new EventHandler(this.deviceComboBox_SelectedIndexChanged);
      this.label1.AutoSize = true;
      this.label1.Location = new Point(6, 16);
      this.label1.Name = "label1";
      this.label1.Size = new Size(41, 13);
      this.label1.TabIndex = 20;
      this.label1.Text = "Device";
      this.frequencyNumericUpDown.DecimalPlaces = 3;
      this.frequencyNumericUpDown.Font = new Font("Microsoft Sans Serif", 14.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 204);
      this.frequencyNumericUpDown.Location = new Point(124, 425);
      this.frequencyNumericUpDown.Maximum = new Decimal(new int[4]
      {
        1700,
        0,
        0,
        0
      });
      this.frequencyNumericUpDown.Minimum = new Decimal(new int[4]
      {
        0,
        0,
        0,
        0
      });
      this.frequencyNumericUpDown.Name = "frequencyNumericUpDown";
      this.frequencyNumericUpDown.Size = new Size(137, 29);
      this.frequencyNumericUpDown.TabIndex = 30;
      this.frequencyNumericUpDown.Value = new Decimal(new int[4]
      {
        224,
        0,
        0,
        0
      });
      this.frequencyNumericUpDown.ValueChanged += new EventHandler(this.frequencyNumericUpDown_ValueChanged);
      this.label5.AutoSize = true;
      this.label5.Font = new Font("Microsoft Sans Serif", 14.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 204);
      this.label5.Location = new Point(15, 427);
      this.label5.Name = "label5";
      this.label5.Size = new Size(102, 24);
      this.label5.TabIndex = 31;
      this.label5.Text = "Frequency";
      this.samplerateComboBox.FormattingEnabled = true;
      this.samplerateComboBox.Items.AddRange(new object[10]
      {
        (object) "3.0 MSPS",
        (object) "2.5 MSPS pal secam",
        (object) "2.4 MSPS pal secam",
        (object) "2.0 MSPS pal secam",
        (object) "1.5 MSPS pal secam",
        (object) "1.0 MSPS pal secam",
        (object) "2.517480 MSPS ntsc",
        (object) "1.951047 MSPS ntsc",
        (object) "1.573425 MSPS ntsc",
        (object) "1.006992 MSPS ntsc"
      });
      this.samplerateComboBox.Location = new Point(86, 59);
      this.samplerateComboBox.Name = "samplerateComboBox";
      this.samplerateComboBox.Size = new Size(163, 21);
      this.samplerateComboBox.TabIndex = 32;
      this.samplerateComboBox.SelectedIndexChanged += new EventHandler(this.samplerateComboBox_SelectedIndexChanged);
      this.label3.AutoSize = true;
      this.label3.Location = new Point(15, 62);
      this.label3.Name = "label3";
      this.label3.Size = new Size(65, 13);
      this.label3.TabIndex = 33;
      this.label3.Text = "SampleRate";
      this.brightnesTrackBar.Location = new Point(274, 29);
      this.brightnesTrackBar.Maximum = 100;
      this.brightnesTrackBar.Minimum = -100;
      this.brightnesTrackBar.Name = "brightnesTrackBar";
      this.brightnesTrackBar.Orientation = Orientation.Vertical;
      this.brightnesTrackBar.Size = new Size(45, 123);
      this.brightnesTrackBar.TabIndex = 36;
      this.brightnesTrackBar.TickFrequency = 20;
      this.brightnesTrackBar.TickStyle = TickStyle.Both;
      this.brightnesTrackBar.Scroll += new EventHandler(this.brightnesTrackBar_Scroll);
      this.contrastTrackBar.Location = new Point(274, 174);
      this.contrastTrackBar.Maximum = 20;
      this.contrastTrackBar.Minimum = 1;
      this.contrastTrackBar.Name = "contrastTrackBar";
      this.contrastTrackBar.Orientation = Orientation.Vertical;
      this.contrastTrackBar.Size = new Size(45, 138);
      this.contrastTrackBar.TabIndex = 37;
      this.contrastTrackBar.TickFrequency = 2;
      this.contrastTrackBar.TickStyle = TickStyle.Both;
      this.contrastTrackBar.Value = 10;
      this.contrastTrackBar.Scroll += new EventHandler(this.contrastTrackBar_Scroll);
      this.label6.AutoSize = true;
      this.label6.Location = new Point(271, 9);
      this.label6.Name = "label6";
      this.label6.Size = new Size(51, 13);
      this.label6.TabIndex = 38;
      this.label6.Text = "Brightnes";
      this.label7.AutoSize = true;
      this.label7.Location = new Point(273, 155);
      this.label7.Name = "label7";
      this.label7.Size = new Size(46, 13);
      this.label7.TabIndex = 39;
      this.label7.Text = "Contrast";
      this.tunerAgcCheckBox.AutoSize = true;
      this.tunerAgcCheckBox.Location = new Point(53, 87);
      this.tunerAgcCheckBox.Name = "tunerAgcCheckBox";
      this.tunerAgcCheckBox.Size = new Size(54, 17);
      this.tunerAgcCheckBox.TabIndex = 40;
      this.tunerAgcCheckBox.Text = "Tuner";
      this.tunerAgcCheckBox.UseVisualStyleBackColor = true;
      this.tunerAgcCheckBox.CheckedChanged += new EventHandler(this.tunerAgcCheckBox_CheckedChanged);
      this.rtlAgcCheckBox.AutoSize = true;
      this.rtlAgcCheckBox.Location = new Point(113, 87);
      this.rtlAgcCheckBox.Name = "rtlAgcCheckBox";
      this.rtlAgcCheckBox.Size = new Size(47, 17);
      this.rtlAgcCheckBox.TabIndex = 41;
      this.rtlAgcCheckBox.Text = "RTL";
      this.rtlAgcCheckBox.UseVisualStyleBackColor = true;
      this.rtlAgcCheckBox.CheckedChanged += new EventHandler(this.rtlAgcCheckBox_CheckedChanged);
      this.programAgcCheckBox.AutoSize = true;
      this.programAgcCheckBox.Location = new Point(166, 87);
      this.programAgcCheckBox.Name = "programAgcCheckBox";
      this.programAgcCheckBox.Size = new Size(65, 17);
      this.programAgcCheckBox.TabIndex = 42;
      this.programAgcCheckBox.Text = "Direct sampling";
      this.programAgcCheckBox.UseVisualStyleBackColor = true;
      this.programAgcCheckBox.CheckedChanged += new EventHandler(this.programAgcCheckBox_CheckedChanged);
      this.frequencyCorrectionTimer.Enabled = true;
      this.frequencyCorrectionTimer.Interval = 200;
      this.frequencyCorrectionTimer.Tick += new EventHandler(this.frequencyCorrectionTimer_Tick);
      this.frequencyCorrectionTrackBar.AutoSize = false;
      this.frequencyCorrectionTrackBar.Location = new Point(70, 44);
      this.frequencyCorrectionTrackBar.Maximum = 200;
      this.frequencyCorrectionTrackBar.Minimum = -200;
      this.frequencyCorrectionTrackBar.Name = "frequencyCorrectionTrackBar";
      this.frequencyCorrectionTrackBar.Size = new Size(175, 30);
      this.frequencyCorrectionTrackBar.TabIndex = 45;
      this.frequencyCorrectionTrackBar.TickFrequency = 20;
      this.frequencyCorrectionTrackBar.Scroll += new EventHandler(this.frequencyCorrectionTrackBar_Scroll);
      this.label4.AutoSize = true;
      this.label4.Location = new Point(22, 44);
      this.label4.Name = "label4";
      this.label4.Size = new Size(46, 13);
      this.label4.TabIndex = 46;
      this.label4.Text = "Roughly";
      this.fineFrequencyCorrectTrackBar.AutoSize = false;
      this.fineFrequencyCorrectTrackBar.Location = new Point(70, 80);
      this.fineFrequencyCorrectTrackBar.Maximum = 20;
      this.fineFrequencyCorrectTrackBar.Minimum = -20;
      this.fineFrequencyCorrectTrackBar.Name = "fineFrequencyCorrectTrackBar";
      this.fineFrequencyCorrectTrackBar.Size = new Size(175, 30);
      this.fineFrequencyCorrectTrackBar.TabIndex = 47;
      this.fineFrequencyCorrectTrackBar.TickFrequency = 2;
      this.fineFrequencyCorrectTrackBar.Scroll += new EventHandler(this.fineFrequencyCorrectTrackBar_Scroll);
      this.xCorrectionTrackBar.AutoSize = false;
      this.xCorrectionTrackBar.Location = new Point(70, 42);
      this.xCorrectionTrackBar.Maximum = 200;
      this.xCorrectionTrackBar.Name = "xCorrectionTrackBar";
      this.xCorrectionTrackBar.Size = new Size(175, 30);
      this.xCorrectionTrackBar.TabIndex = 48;
      this.xCorrectionTrackBar.TickStyle = TickStyle.None;
      this.xCorrectionTrackBar.Scroll += new EventHandler(this.xCorrectionTrackBar_Scroll);
      this.yCorrectionTrackBar.AutoSize = false;
      this.yCorrectionTrackBar.Location = new Point(70, 78);
      this.yCorrectionTrackBar.Maximum = 400;
      this.yCorrectionTrackBar.Name = "yCorrectionTrackBar";
      this.yCorrectionTrackBar.Size = new Size(175, 30);
      this.yCorrectionTrackBar.TabIndex = 49;
      this.yCorrectionTrackBar.TickStyle = TickStyle.None;
      this.yCorrectionTrackBar.Scroll += new EventHandler(this.yCorrectionTrackBar_Scroll);
      this.label9.AutoSize = true;
      this.label9.Location = new Point(22, 80);
      this.label9.Name = "label9";
      this.label9.Size = new Size(27, 13);
      this.label9.TabIndex = 46;
      this.label9.Text = "Fine";
      this.label10.AutoSize = true;
      this.label10.Location = new Point(22, 42);
      this.label10.Name = "label10";
      this.label10.Size = new Size(14, 13);
      this.label10.TabIndex = 46;
      this.label10.Text = "X";
      this.label11.AutoSize = true;
      this.label11.Location = new Point(22, 78);
      this.label11.Name = "label11";
      this.label11.Size = new Size(14, 13);
      this.label11.TabIndex = 46;
      this.label11.Text = "Y";
      this.frequencyCorrectionGroupBox.Controls.Add((Control) this.autoFrequencyCorrectionButton);
      this.frequencyCorrectionGroupBox.Controls.Add((Control) this.label4);
      this.frequencyCorrectionGroupBox.Controls.Add((Control) this.frequencyCorrectionNumericUpDown);
      this.frequencyCorrectionGroupBox.Controls.Add((Control) this.fineFrequencyCorrectTrackBar);
      this.frequencyCorrectionGroupBox.Controls.Add((Control) this.frequencyCorrectionTrackBar);
      this.frequencyCorrectionGroupBox.Controls.Add((Control) this.label9);
      this.frequencyCorrectionGroupBox.Location = new Point(12, 174);
      this.frequencyCorrectionGroupBox.Name = "frequencyCorrectionGroupBox";
      this.frequencyCorrectionGroupBox.Size = new Size(258, 122);
      this.frequencyCorrectionGroupBox.TabIndex = 50;
      this.frequencyCorrectionGroupBox.TabStop = false;
      this.frequencyCorrectionGroupBox.Text = "Frequency correction";
      this.autoFrequencyCorrectionButton.Location = new Point(18, 15);
      this.autoFrequencyCorrectionButton.Name = "autoFrequencyCorrectionButton";
      this.autoFrequencyCorrectionButton.Size = new Size(107, 23);
      this.autoFrequencyCorrectionButton.TabIndex = 48;
      this.autoFrequencyCorrectionButton.Text = "Auto correction";
      this.autoFrequencyCorrectionButton.UseVisualStyleBackColor = true;
      this.autoFrequencyCorrectionButton.Click += new EventHandler(this.autoFrequencyCorrectionButton_Click);
      this.positionGroupBox.Controls.Add((Control) this.autoSincCheckBox);
      this.positionGroupBox.Controls.Add((Control) this.label10);
      this.positionGroupBox.Controls.Add((Control) this.label11);
      this.positionGroupBox.Controls.Add((Control) this.yCorrectionTrackBar);
      this.positionGroupBox.Controls.Add((Control) this.xCorrectionTrackBar);
      this.positionGroupBox.Location = new Point(12, 302);
      this.positionGroupBox.Name = "positionGroupBox";
      this.positionGroupBox.Size = new Size(258, 117);
      this.positionGroupBox.TabIndex = 48;
      this.positionGroupBox.TabStop = false;
      this.positionGroupBox.Text = "Position correction";
      this.autoSincCheckBox.AutoSize = true;
      this.autoSincCheckBox.Location = new Point(25, 19);
      this.autoSincCheckBox.Name = "autoSincCheckBox";
      this.autoSincCheckBox.Size = new Size(48, 17);
      this.autoSincCheckBox.TabIndex = 48;
      this.autoSincCheckBox.Text = "Auto";
      this.autoSincCheckBox.UseVisualStyleBackColor = true;
      this.autoSincCheckBox.CheckedChanged += new EventHandler(this.autoSincCheckBox_CheckedChanged);
      this.tunerGroupBox.Controls.Add((Control) this.label8);
      this.tunerGroupBox.Controls.Add((Control) this.label1);
      this.tunerGroupBox.Controls.Add((Control) this.tunerTypeLabel);
      this.tunerGroupBox.Controls.Add((Control) this.deviceComboBox);
      this.tunerGroupBox.Controls.Add((Control) this.programAgcCheckBox);
      this.tunerGroupBox.Controls.Add((Control) this.gainLabel);
      this.tunerGroupBox.Controls.Add((Control) this.rtlAgcCheckBox);
      this.tunerGroupBox.Controls.Add((Control) this.samplerateComboBox);
      this.tunerGroupBox.Controls.Add((Control) this.tunerAgcCheckBox);
      this.tunerGroupBox.Controls.Add((Control) this.label2);
      this.tunerGroupBox.Controls.Add((Control) this.label3);
      this.tunerGroupBox.Controls.Add((Control) this.tunerGainTrackBar);
      this.tunerGroupBox.Location = new Point(12, 0);
      this.tunerGroupBox.Name = "tunerGroupBox";
      this.tunerGroupBox.Size = new Size(258, 167);
      this.tunerGroupBox.TabIndex = 51;
      this.tunerGroupBox.TabStop = false;
      this.tunerGroupBox.Text = "Tuner";
      this.label8.AutoSize = true;
      this.label8.Location = new Point(15, 87);
      this.label8.Name = "label8";
      this.label8.Size = new Size(29, 13);
      this.label8.TabIndex = 43;
      this.label8.Text = "AGC";
      this.inverseCheckBox.AutoSize = true;
      this.inverseCheckBox.Location = new Point(288, 344);
      this.inverseCheckBox.Name = "inverseCheckBox";
      this.inverseCheckBox.Size = new Size(15, 14);
      this.inverseCheckBox.TabIndex = 52;
      this.inverseCheckBox.UseVisualStyleBackColor = true;
      this.inverseCheckBox.CheckedChanged += new EventHandler(this.inverseCheckBox_CheckedChanged);
      this.label12.AutoSize = true;
      this.label12.Location = new Point(278, 322);
      this.label12.Name = "label12";
      this.label12.Size = new Size(42, 13);
      this.label12.TabIndex = 53;
      this.label12.Text = "Inverse";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(323, 460);
      this.Controls.Add((Control) this.label12);
      this.Controls.Add((Control) this.inverseCheckBox);
      this.Controls.Add((Control) this.tunerGroupBox);
      this.Controls.Add((Control) this.positionGroupBox);
      this.Controls.Add((Control) this.frequencyCorrectionGroupBox);
      this.Controls.Add((Control) this.label7);
      this.Controls.Add((Control) this.label6);
      this.Controls.Add((Control) this.contrastTrackBar);
      this.Controls.Add((Control) this.brightnesTrackBar);
      this.Controls.Add((Control) this.label5);
      this.Controls.Add((Control) this.frequencyNumericUpDown);
      this.Controls.Add((Control) this.startBtn);
      this.DoubleBuffered = true;
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.MaximizeBox = false;
      this.Name = nameof (MainForm);
      this.ShowIcon = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "TVSharper";
      this.FormClosing += new FormClosingEventHandler(this.MainForm_FormClosing);
      this.frequencyCorrectionNumericUpDown.EndInit();
      this.tunerGainTrackBar.EndInit();
      this.frequencyNumericUpDown.EndInit();
      this.brightnesTrackBar.EndInit();
      this.contrastTrackBar.EndInit();
      this.frequencyCorrectionTrackBar.EndInit();
      this.fineFrequencyCorrectTrackBar.EndInit();
      this.xCorrectionTrackBar.EndInit();
      this.yCorrectionTrackBar.EndInit();
      this.frequencyCorrectionGroupBox.ResumeLayout(false);
      this.frequencyCorrectionGroupBox.PerformLayout();
      this.positionGroupBox.ResumeLayout(false);
      this.positionGroupBox.PerformLayout();
      this.tunerGroupBox.ResumeLayout(false);
      this.tunerGroupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
