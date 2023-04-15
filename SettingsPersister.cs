// Decompiled with JetBrains decompiler
// Type: TVSharp.SettingsPersister
// Assembly: TVSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 67BFC2D9-64F8-452D-BCAC-E227BAD9AE68
// Assembly location: C:\Users\Mark\Desktop\TVSharp\TVSharp.exe

using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace TVSharp
{
  public class SettingsPersister
  {
    private const string FreqMgrFilename = "settings.xml";
    private readonly string _settingsFolder;

    public SettingsPersister() => this._settingsFolder = Path.GetDirectoryName(Application.ExecutablePath);

    public SettingsMemoryEntry ReadSettings() => this.ReadObject<SettingsMemoryEntry>("settings.xml") ?? new SettingsMemoryEntry();

    public void PersistSettings(SettingsMemoryEntry entries) => this.WriteObject<SettingsMemoryEntry>(entries, "settings.xml");

    private T ReadObject<T>(string fileName)
    {
      string path = Path.Combine(this._settingsFolder, fileName);
      if (!File.Exists(path))
        return default (T);
      using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
        return (T) new XmlSerializer(typeof (T)).Deserialize((Stream) fileStream);
    }

    private void WriteObject<T>(T obj, string fileName)
    {
      using (FileStream fileStream = new FileStream(Path.Combine(this._settingsFolder, fileName), FileMode.Create))
        new XmlSerializer(obj.GetType()).Serialize((Stream) fileStream, (object) obj);
    }
  }
}
