using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using Debug = UnityEngine.Debug;

namespace Magiclab.PackageManager
{
    public class MagiclabPackage
    {
        public MagiclabPackage(string packageName, string packageDisplayName, string packageLink)
        {
            PackageName = packageName;
            PackageDisplayName = packageDisplayName;
            PackageLink = packageLink;
        }

        public string PackageName { get; }
        public string PackageDisplayName { get; }
        public string PackageLink { get; }
        public Dictionary<Version, string> Versions { get; private set; }
        public List<string> Hashes { get; private set; }
        public bool IsEmbedded { get; private set; }
        public bool IsInstalled { get; private set; }
        public int InstalledVersionIndex { get; private set; } = -1;
        public int SelectedVersionIndex { get; set; }
        public string InstalledVersionLabel { get; private set; }
        public string[] VersionLabels { get; private set; }
        public MagiclabPackageDependency[] MagiclabPackageDependencies { get; private set; }
        public Version InstalledVersion => Versions.ElementAt(InstalledVersionIndex).Key;

        public async Task Init()
        {
            var task = ExecuteTerminalProcessAsync("git", $"ls-remote --tags {PackageLink}", 
                isGlobalPath:true);
            var output = await task;
            SetVersionsAndHashes(output);
        }

        public void CheckInstalledPackage(PackageInfo packageInfo)
        {
            if (packageInfo == null)
            {
                IsEmbedded = false;
                IsInstalled = false;
                
                UpdateInstalledVersionLabel();
            }
            else
            {
                IsEmbedded = packageInfo.source == PackageSource.Embedded;
                IsInstalled = !IsEmbedded;
                
                var installedHash = PackageManifestManager.GetHashInstalledVersion(this);
                SetInstalledVersion(installedHash);
            }
        }
        
        public static async Task<string> ExecuteTerminalProcessAsync(string command, string arguments, 
            string workingDirectory = null, bool isGlobalPath = false)
        {
            var task = Task.Run(() => ExecuteTerminalProcess(command, arguments, workingDirectory, isGlobalPath));
            return await task;
        }
        
        public static string ExecuteTerminalProcess(string command, string arguments, string workingDirectory = null, 
            bool isGlobalPath = false)
        {
            try
            {
#if UNITY_EDITOR_WIN
                arguments = arguments.Replace("'", "\"");
#else
                if (!isGlobalPath)
                {
                    command = command.Insert(0, "./");
                }
                arguments = arguments.Replace("\"", "\\\"");
#endif
                
                var startInfo = new ProcessStartInfo
                {
#if UNITY_EDITOR_WIN
                    FileName = "cmd.exe",
                    Arguments = $"/C {command} {arguments}",
#else
                    FileName = "/bin/bash",
                    Arguments = $@" -c ""{command} {arguments}""",
#endif
                    WorkingDirectory = workingDirectory == null ? string.Empty : workingDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                };
                var myProcess = new Process
                {
                    StartInfo = startInfo
                };
                myProcess.Start();
                var output = myProcess.StandardOutput.ReadToEnd();
                var error = myProcess.StandardError.ReadToEnd();
                
                if (!string.IsNullOrEmpty(error))
                    Debug.LogWarning(error);

                myProcess.ErrorDataReceived += (sendingProcess, errLine) =>
                {
                    Debug.LogError($"Sending Process : {sendingProcess}\n" +
                                   $"Error : {errLine.Data}");
                };
                myProcess.WaitForExit();
                return output;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning(e);
                return null;
            }
        }

        private void SetVersionsAndHashes(string commandOutput)
        {
            Versions = new Dictionary<Version, string>();
            Hashes = new List<string>();
            
            var lines = commandOutput.Split('\n');
            lines = lines.OrderByDescending(line => line.EndsWith("^{}")).ToArray();
            foreach (var line in lines)
            {
                if (!TryParseVersionAndHash(line, out var version, out var hash)) continue;

                if (!Versions.ContainsKey(version))
                {
                    Versions.Add(version, hash);
                }
            }

            Versions = Versions.OrderByDescending(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
            
            VersionLabels = new string[Versions.Count];
            for (int i = 0; i < Versions.Count; i++)
            {
                VersionLabels[i] = Versions.ElementAt(i).Key.ToString();
            }
            
            foreach (var hash in Versions.Values)
            {
                Hashes.Add(hash);
            }
        }

        private bool TryParseVersionAndHash(string line, out Version version, out string hash)
        {
            version = new Version(0, 0, 0, 0);
            var info = line.Split(null);
            hash = info[0];
            
            if (string.IsNullOrEmpty(hash)) return false;
            
            var refInfo = info[1].Split('/');
            var versionInfo = refInfo[refInfo.Length - 1];

            var regex = new Regex(@"^version_(\d+(\.\d+)+)\^\{\}");
            var match = regex.Match(versionInfo);

            if (!match.Success)
            {
                regex = new Regex(@"^version_(\d+(\.\d+)+)");
                match = regex.Match(versionInfo);

                if (!match.Success) return false;
            }
            
            var versionString = match.Groups[1].Value;
            if (!Version.TryParse(versionString, out version))
            {
                Debug.LogErrorFormat("Invalid version tag for \"{0}\" package at \"{1}\" commit.", PackageDisplayName, hash);
                return false;
            }

            return true;
        }

        public void SetMagiclabPackageDependencies(MagiclabPackageDependency[] dependencies)
        {
            MagiclabPackageDependencies = dependencies;
        }

        public void SetInstalledVersion(string hash)
        {
            int installedVersionIndex = -1;
            for (int i = 0; i < Hashes.Count; i++)
            {
                if (Hashes[i] != hash) continue;
                
                installedVersionIndex = i;
                break;
            }
            
            InstalledVersionIndex = installedVersionIndex;
            UpdateInstalledVersionLabel();
        }

        private void UpdateInstalledVersionLabel()
        {
            if (IsEmbedded)
            {
                InstalledVersionLabel = "Embedded";
            }
            else 
            {
                if (IsInstalled)
                {
                    if (InstalledVersionIndex == -1)
                    {
                        InstalledVersionLabel = "UndefinedVersion";
                        UnityEngine.Debug.LogErrorFormat("Undefined version installed for \"{0}\" package.", PackageDisplayName);
                    }
                    else
                    {
                        InstalledVersionLabel = Versions.Keys.ToList()[InstalledVersionIndex].ToString();
                    }
                }
                else
                {
                    InstalledVersionLabel = "---";
                }
            }
        }

        public void RemovedPackage()
        {
            IsInstalled = false;
            InstalledVersionIndex = -1;
            UpdateInstalledVersionLabel();
        }
    }
}
