using System;
using System.IO;
class P { static void Main(){ var p=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"Temp","maui-createapp-exception.txt"); if(!File.Exists(p)){ p=Path.Combine(Path.GetTempPath(),"maui-win-init-exception.txt"); }
 if(File.Exists(p)) Console.WriteLine(File.ReadAllText(p)); else Console.WriteLine("No temp crash file found: " + p);
 }}