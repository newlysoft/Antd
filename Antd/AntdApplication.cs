﻿//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using antdlib;
using antdlib.Boot;
using antdlib.Common;
using antdlib.Log;
using Microsoft.Owin.Builder;
using Nowin;
using Owin;

namespace Antd {
    internal static class AntdApplication {
        private static void Main() {
            try {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                var startTime = DateTime.Now;
                Console.Title = "ANTD";


                if (AssemblyInfo.IsUnix == false) {
                    Directory.CreateDirectory("/cfg/antd");
                    Directory.CreateDirectory("/cfg/antd/database");
                    Directory.CreateDirectory("/mnt/cdrom/DIRS");
                    ConsoleLogger.Warn("This application is not running on an Anthilla OS Linux, some functions may be disabled");
                }

                var stop = new ManualResetEvent(false);
                Console.CancelKeyPress +=
                    (sender, e) => {
                        Console.WriteLine("^C");
                        Environment.Exit(1);
                        stop.Set();
                        e.Cancel = true;
                    };

                Configuration();

                var owinbuilder = new AppBuilder();
                OwinServerFactory.Initialize(owinbuilder.Properties);
                new Startup().Configuration(owinbuilder);

                var httpPort = Convert.ToInt32(CoreParametersConfig.GetHttpPort());
                var httpBuilder = ServerBuilder.New()
                    //.SetAddress(IPAddress.Parse("0.0.0.0"))
                    .SetPort(httpPort)
                    .SetOwinApp(owinbuilder.Build())
                    .SetOwinCapabilities((IDictionary<string, object>)owinbuilder.Properties[OwinKeys.ServerCapabilitiesKey])
                    .SetExecutionContextFlow(ExecutionContextFlow.SuppressAlways)
                    .SetCertificate(new X509Certificate2(CoreParametersConfig.GetCertificatePath()));

                var httpsPort = Convert.ToInt32(CoreParametersConfig.GetHttpsPort());
                var httpsBuilder = ServerBuilder.New()
                    .SetPort(httpsPort)
                    .SetOwinApp(owinbuilder.Build())
                    .SetOwinCapabilities((IDictionary<string, object>)owinbuilder.Properties[OwinKeys.ServerCapabilitiesKey])
                    .SetExecutionContextFlow(ExecutionContextFlow.SuppressAlways)
                    .SetCertificate(new X509Certificate2(CoreParametersConfig.GetCertificatePath()));

                using (var httpsServer = httpsBuilder.Build()) {
                    if (httpsServer == null)
                        return;
                    using (var httpServer = httpBuilder.Build()) {
                        if (httpServer == null)
                            return;
                        Task.Run(() => httpsServer.Start());
                        Task.Run(() => httpServer.Start());
                        ConsoleLogger.Log("loading service");
                        ConsoleLogger.Log($"http port: {httpPort}");
                        ConsoleLogger.Log($"https port: {httpsPort}");
                        ConsoleLogger.Log("antd is running");
                        ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");
                        stop.WaitOne();
                    }
                }
            }
            catch (Exception ex) {
                Directory.CreateDirectory($"{Parameter.AntdCfgReport}");
                File.WriteAllText($"{Parameter.AntdCfgReport}/{Timestamp.Now}-crash-report.txt", ex.ToString());
                DeNSo.Session.ShutDown();
            }
        }

        private static void Configuration() {
            AntdBoot.CheckOsIsRw();
            AntdBoot.SetWorkingDirectories();
            AntdBoot.SetCoreParameters();
            AntdBoot.StartDatabase();
            AntdBoot.CheckCertificate();
            AntdBoot.ReloadUsers();
            AntdBoot.ReloadSsh();
            AntdBoot.SetOverlayDirectories();
            //AntdBoot.SetSystemdJournald();
            AntdBoot.SetMounts();
            AntdBoot.SetOsMount();
            AntdBoot.LaunchDefaultOsConfiguration();
            //AntdBoot.SetWebsocketd();
            AntdBoot.CheckResolv();
            AntdBoot.SetFirewall();
            AntdBoot.ImportSystemInformation();
            //AntdBoot.StartScheduler(true);
            AntdBoot.StartDirectoryWatcher();
            AntdBoot.LaunchApps();
            //AntdBoot.DownloadDefaultRepoFiles();
        }
    }

    internal class Startup {
        public void Configuration(IAppBuilder app) {
            ConsoleLogger.Log("loading app configuration");
            AntdBoot.StartSignalR(app, true, true);
            AntdBoot.StartNancy(app);
            ConsoleLogger.Log("app configuration ready");
        }
    }
}