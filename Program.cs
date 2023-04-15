// Decompiled with JetBrains decompiler
// Type: TVSharp.Program
// Assembly: TVSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 67BFC2D9-64F8-452D-BCAC-E227BAD9AE68
// Assembly location: C:\Users\Mark\Desktop\TVSharp\TVSharp.exe

using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace TVSharp
{
  internal static class Program
  {
    [STAThread]
    private static void Main()
    {
      if (Environment.OSVersion.Platform == PlatformID.Win32Windows || Environment.OSVersion.Platform == PlatformID.Win32NT)
      {
        Process currentProcess = Process.GetCurrentProcess();
        currentProcess.PriorityBoostEnabled = true;
        currentProcess.PriorityClass = ProcessPriorityClass.RealTime;
      }
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run((Form) new MainForm());
    }
  }
}
