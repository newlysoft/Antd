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

using System.Linq;
using antdlib.Systemd;
using Nancy.Security;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class UnitsModule : CoreModule {
        public UnitsModule() {
            this.RequiresAuthentication();

            Get["/units"] = x => {
                var units = Units.All;
                return View["page-units", units];
            };

            Get["/units/list"] = x => {
                return JsonConvert.SerializeObject(Units.All.OrderBy(u => u.name));
            };

            Post["/units/mgmt/enable/{unit}"] = x => {
                string unit = x.unit;
                var r = Systemctl.Enable(unit);
                return JsonConvert.SerializeObject(r.outputTable);
            };

            Post["/units/mgmt/disable/{unit}"] = x => {
                string unit = x.unit;
                var r = Systemctl.Disable(unit);
                return JsonConvert.SerializeObject(r.outputTable);
            };

            Post["/units/mgmt/start/{unit}"] = x => {
                string unit = x.unit;
                var r = Systemctl.Start(unit);
                return JsonConvert.SerializeObject(r.outputTable);
            };

            Post["/units/mgmt/stop/{unit}"] = x => {
                string unit = x.unit;
                var r = Systemctl.Stop(unit);
                return JsonConvert.SerializeObject(r.outputTable);
            };

            Post["/units/mgmt/restart/{unit}"] = x => {
                string unit = x.unit;
                var r = Systemctl.Restart(unit);
                return JsonConvert.SerializeObject(r.outputTable);
            };

            Post["/units/mgmt/reload/{unit}"] = x => {
                string unit = x.unit;
                var r = Systemctl.Reload(unit);
                return JsonConvert.SerializeObject(r.outputTable);
            };

            Get["/units/mgmt/status/{unit}"] = x => {
                string unit = x.unit;
                var r = Systemctl.Status(unit);
                return JsonConvert.SerializeObject(r.outputTable);
            };
        }
    }
}