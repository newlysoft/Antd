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

using System.Collections.Generic;
using Antd.Database;
using Nancy;
using Nancy.Security;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class FirewallModule : CoreModule {

        private readonly FirewallListRepository _firewallRepo = new FirewallListRepository();

        public FirewallModule() {
            this.RequiresAuthentication();

            Get["/firewall/getrule/{table}/{type}/{hook}"] = x => JsonConvert.SerializeObject(_firewallRepo.GetForRule((string)x.table, (string)x.type, (string)x.hook));

            Get["/firewall/getruleset/{table}/{type}/{hook}"] = x => JsonConvert.SerializeObject(_firewallRepo.GetForRule((string)x.table, (string)x.type, (string)x.hook));

            Post["/firewall/add/list"] = x => {
                var table = (string)Request.Form.Table;
                var type = (string)Request.Form.Type;
                var hook = (string)Request.Form.Hook;
                var label = (string)Request.Form.Label;
                var values = (string)Request.Form.Elements;
                _firewallRepo.Create(new Dictionary<string, string> {
                    { "TableId", table },
                    { "TypeId", type },
                    { "HookId", hook },
                    { "Rule", "" },
                    { "Label", label },
                    { "Values", values }
                });
                return Response.AsRedirect("/");
            };

            Post["/firewall/add/value"] = x => {
                var guid = (string)Request.Form.Guid;
                var values = (string)Request.Form.Elements;
                _firewallRepo.Edit(new Dictionary<string, string> {
                    { "Id", guid },
                    { "Values", values }
                });
                return HttpStatusCode.OK;
            };
        }
    }
}