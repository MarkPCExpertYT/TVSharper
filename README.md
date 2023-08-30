# UPDATE
### Since i usually do not program in C#, i'm looking for a decompiler that decompiles to VB.NET. If i find it i will rename this branch to "old" and will make a new "main" branch with the VB version. if not i may continue updating this.

# TVSharper
### An analog TV decoder for the RTL-SDR (but sharper).
help me find a new name.
##
TVSharper is a modification of the original TVSharp with some extra features such as: 
 - Direct sampling support (toggle-able)
 - Support for frequencies below 10 MHz
 - The video window is now a normal window visible in the taskbar

Removed features: 
 - Built-in program gain control (replaced with direct sampling option)

To-do:
 - [ ] Better layout
 - [ ] Fix bug where the screen constantly moves horizontally when on 2.5MSPS PAL SECAM mode
 - [ ] Fix bug where the program sometimes randomly throws an ArgumentException "The frequency cannot be set"
 - [ ] Seperate 3.0 MSPS PAL and NTSC modes 


## Credits: 

 - Youssef Touil and Ian Gilmour for the original code
 - JetBrains for the DotPeek decompiler
 - Me for the extra features
 - Microsoft for the icon

### If the original authors want this taken down, I will do so.
