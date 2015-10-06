﻿///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
///     All rights reserved.
///
///     Redistribution and use in source and binary forms, with or without
///     modification, are permitted provided that the following conditions are met:
///         * Redistributions of source code must retain the above copyright
///           notice, this list of conditions and the following disclaimer.
///         * Redistributions in binary form must reproduce the above copyright
///           notice, this list of conditions and the following disclaimer in the
///           documentation and/or other materials provided with the distribution.
///         * Neither the name of the Anthilla S.r.l. nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
///
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
///     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
///     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
///     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
///     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
///     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
///     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
///     20141110
///-------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using static System.Console;

namespace antdlib.Antdsh {
    public class UpdateAntdsh {

        /// <summary>
        /// ok
        /// </summary>
        public static void UpdateFromPublicRepo() {
            WriteLine("Update From Public Repo ...");
            WriteLine("   Stopping services");
            execute.StopServices();
            WriteLine("   Cleaning directories and mounts");
            execute.Umount(Folder.Root);
            execute.Umount(Folder.Database);
            execute.Umount("/framework/antdsh");
            execute.CleanTmp();
            WriteLine("   Mounting tmp ram");
            execute.MountTmpRam();
            var antdshRepoUrl = $"{Update.remoteRepo}/{Update.remoteAntdshDir}";
            var updateFileUrl = $"{antdshRepoUrl}/{Update.remoteUpdateInfo}";
            var updateFile = $"{Folder.AntdTmpDir}/{Update.remoteUpdateInfo}";
            WriteLine($"   Downloading from: {updateFileUrl}");
            WriteLine($"                 to: {updateFile}");
            execute.DownloadFromUrl(updateFileUrl, updateFile);
            if (!File.Exists(updateFile)) {
                WriteLine($"   Download failed!");
                return;
            }
            else {
                WriteLine($"   Download complete!");
            }
            var updateText = FileSystem.ReadFile(updateFile);
            var squashName = updateText.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault(v => !v.Contains(Update.remoteUpdateInfo));
            WriteLine($"   Version found: {squashName}");
            var squashUrl = $"{antdshRepoUrl}/{squashName}";
            var squashFile = $"{Folder.AntdshVersionsDir}/{squashName}";
            WriteLine($"   Downloading from: {squashUrl}");
            WriteLine($"                 to: {squashFile}");
            execute.DownloadFromUrl(squashUrl, squashFile);
            if (!File.Exists(squashFile)) {
                WriteLine($"   Download failed!");
                return;
            }
            else {
                WriteLine($"   Download complete!");
            }
            execute.RemoveLink();
            execute.LinkVersionToRunning(squashName);
            execute.CleanTmp();
            execute.UmountTmpRam();
            WriteLine($"   Update complete!");
        }
    }
}