﻿////-------------------------------------------------------------------------------------
////     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
////     All rights reserved.
////
////     Redistribution and use in source and binary forms, with or without
////     modification, are permitted provided that the following conditions are met:
////         * Redistributions of source code must retain the above copyright
////           notice, this list of conditions and the following disclaimer.
////         * Redistributions in binary form must reproduce the above copyright
////           notice, this list of conditions and the following disclaimer in the
////           documentation and/or other materials provided with the distribution.
////         * Neither the name of the Anthilla S.r.l. nor the
////           names of its contributors may be used to endorse or promote products
////           derived from this software without specific prior written permission.
////
////     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
////     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
////     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
////     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
////     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
////     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
////     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
////     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
////     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
////     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
////
////     20141110
////-------------------------------------------------------------------------------------

//using System.Collections.Generic;
//using System.Dynamic;
//using System.Threading;
//using antdlib.CCTable;
//using antdlib.Ssh;
//using antdlib.Svcs.Bind;
//using antdlib.Svcs.Dhcp;
//using antdlib.Svcs.Samba;
//using antdlib.ViewBinds;
//using Nancy;
//using Nancy.ModelBinding;
//using Nancy.Security;

//namespace Antd.Modules {
//    public class ServicesModule : CoreModule {
//        public ServicesModule() {
//            this.RequiresAuthentication();

//            Get["/services"] = x => {
//                dynamic vmod = new ExpandoObject();
//                vmod.CurrentContext = Request.Path;
//                vmod.CCTable = CCTableRepository.GetAllByContext(Request.Path);
//                vmod.Count = CCTableRepository.GetAllByContext(Request.Path).ToArray().Length;
//                return View["_page-services", vmod];
//            };

//            #region SAMBA
//            Post["/services/activate/samba"] = x => {
//                SambaConfig.SetReady();
//                SambaConfig.MapFile.Render();
//                return HttpStatusCode.OK;
//            };

//            Post["/services/refresh/samba"] = x => {
//                SambaConfig.MapFile.Render();
//                return HttpStatusCode.OK;
//            };

//            Post["/services/reloadconfig/samba"] = x => {
//                SambaConfig.ReloadConfig();
//                return HttpStatusCode.OK;
//            };

//            Post["/services/update/samba"] = x => {
//                var parameters = this.Bind<List<ServiceSamba>>();
//                SambaConfig.WriteFile.SaveGlobalConfig(parameters);
//                Thread.Sleep(1000);
//                SambaConfig.WriteFile.DumpGlobalConfig();
//                Thread.Sleep(1000);
//                SambaConfig.WriteFile.RewriteSmbconf();
//                return Response.AsRedirect("/");
//            };

//            Post["/services/update/sambashares"] = x => {
//                var parameters = this.Bind<List<ServiceSamba>>();
//                string file = Request.Form.ShareFile;
//                string name = Request.Form.ShareName;
//                string query = Request.Form.ShareQueryName;
//                SambaConfig.WriteFile.SaveShareConfig(file, name, query, parameters);
//                Thread.Sleep(1000);
//                SambaConfig.WriteFile.DumpShare(name);
//                Thread.Sleep(1000);
//                SambaConfig.WriteFile.RewriteSmbconf();
//                return Response.AsRedirect("/");
//            };

//            Post["/services/samba/addparam"] = x => {
//                string key = Request.Form.NewParameterKey;
//                string value = Request.Form.NewParameterValue;
//                SambaConfig.WriteFile.AddParameterToGlobal(key, value);
//                Thread.Sleep(1000);
//                SambaConfig.WriteFile.RewriteSmbconf();
//                return Response.AsRedirect("/");
//            };

//            Post["/services/samba/addshare"] = x => {
//                string name = Request.Form.NewShareName;
//                string directory = Request.Form.NewShareDirectory;
//                SambaConfig.WriteFile.AddShare(name, directory);
//                Thread.Sleep(1000);
//                SambaConfig.WriteFile.RewriteSmbconf();
//                return Response.AsRedirect("/");
//            };
//            #endregion SAMBA

//            #region BIND
//            Post["/services/activate/bind"] = x => {
//                BindConfig.SetReady();
//                BindConfig.MapFile.Render();
//                return HttpStatusCode.OK;
//            };

//            Post["/services/refresh/bind"] = x => {
//                BindConfig.MapFile.Render();
//                return HttpStatusCode.OK;
//            };

//            Post["/services/reloadconfig/bind"] = x => {
//                BindConfig.ReloadConfig();
//                return HttpStatusCode.OK;
//            };

//            Post["/services/update/bind/{section}"] = x => {
//                var section = (string)x.section;
//                var parameters = this.Bind<List<ServiceBind>>();
//                if (section == "acl") {
//                    BindConfig.WriteFile.SaveAcls(parameters);
//                }
//                else {
//                    BindConfig.WriteFile.SaveGlobalConfig(section, parameters);
//                }
//                Thread.Sleep(1000);
//                BindConfig.WriteFile.DumpGlobalConfig();
//                return Response.AsRedirect("/");
//            };

//            Post["/services/update/bind/zone/{zone}"] = x => {
//                var zoneName = (string)x.zone;
//                var parameters = this.Bind<List<ServiceBind>>();
//                BindConfig.WriteFile.SaveZoneConfig(zoneName, parameters);
//                Thread.Sleep(1000);
//                BindConfig.WriteFile.DumpGlobalConfig();
//                return Response.AsRedirect("/");
//            };

//            Post["/services/bind/addacl"] = x => {
//                string k = Request.Form.NewAclKey;
//                string v = Request.Form.NewAclValue;
//                BindConfig.MapFile.AddAcl(k, v);
//                return Response.AsRedirect("/");
//            };

//            Post["/services/bind/addkey"] = x => {
//                string name = Request.Form.NewKeyName;
//                BindConfig.MapFile.AddKey(name);
//                return Response.AsRedirect("/");
//            };

//            Post["/services/bind/addmasters"] = x => {
//                string name = Request.Form.NewMastersName;
//                BindConfig.MapFile.AddMasters(name);
//                return Response.AsRedirect("/");
//            };

//            Post["/services/bind/addserver"] = x => {
//                string name = Request.Form.NewServerName;
//                BindConfig.MapFile.AddServer(name);
//                return Response.AsRedirect("/");
//            };

//            Post["/services/bind/addview"] = x => {
//                string name = Request.Form.NewViewName;
//                BindConfig.MapFile.AddView(name);
//                return Response.AsRedirect("/");
//            };

//            Post["/services/bind/addzone"] = x => {
//                string name = Request.Form.NewZoneName;
//                BindConfig.MapFile.AddZone(name);
//                return Response.AsRedirect("/");
//            };
//            #endregion BIND

//            #region DHCP
//            Post["/services/activate/dhcp"] = x => {
//                DhcpConfig.SetReady();
//                DhcpConfig.MapFile.Render();
//                return HttpStatusCode.OK;
//            };

//            Post["/services/refresh/dhcp"] = x => {
//                DhcpConfig.MapFile.Render();
//                return HttpStatusCode.OK;
//            };

//            Post["/services/reloadconfig/dhcp"] = x => {
//                DhcpConfig.ReloadConfig();
//                return HttpStatusCode.OK;
//            };

//            Post["/services/update/dhcp/{section}"] = x => {
//                var parameters = this.Bind<List<ServiceDhcp>>();
//                var section = (string)x.section;
//                if (section == "global") {
//                    DhcpConfig.WriteFile.SaveGlobal(parameters);
//                }
//                if (section == "prefix6") {
//                    DhcpConfig.WriteFile.SavePrefix6(parameters);
//                }
//                if (section == "range6") {
//                    DhcpConfig.WriteFile.SaveRange6(parameters);
//                }
//                if (section == "range") {
//                    DhcpConfig.WriteFile.SaveRange(parameters);
//                }
//                else {
//                    DhcpConfig.WriteFile.SaveConfigFor(section, parameters);
//                }
//                Thread.Sleep(1000);
//                DhcpConfig.WriteFile.DumpGlobalConfig();
//                return Response.AsRedirect("/");
//            };

//            Post["/services/dhcp/addglobal"] = x => {
//                string k = Request.Form.NewKey;
//                string v = Request.Form.NewValue;
//                DhcpConfig.MapFile.AddGlobal(k, v);
//                return Response.AsRedirect("/");
//            };

//            Post["/services/dhcp/addrange"] = x => {
//                string k = Request.Form.NewKey;
//                string v = Request.Form.NewValue;
//                DhcpConfig.MapFile.AddGlobal(k, v);
//                return Response.AsRedirect("/");
//            };

//            Post["/services/dhcp/addprefix6"] = x => {
//                string k = Request.Form.NewKey;
//                string v = Request.Form.NewValue;
//                DhcpConfig.MapFile.AddPrefix6(k, v);
//                return Response.AsRedirect("/");
//            };

//            Post["/services/dhcp/addrange6"] = x => {
//                string k = Request.Form.NewKey;
//                string v = Request.Form.NewValue;
//                DhcpConfig.MapFile.AddRange6(k, v);
//                return Response.AsRedirect("/");
//            };

//            Post["/services/dhcp/addrange"] = x => {
//                string k = Request.Form.NewKey;
//                string v = Request.Form.NewValue;
//                DhcpConfig.MapFile.AddRange(k, v);
//                return Response.AsRedirect("/");
//            };

//            Post["/services/dhcp/addkey"] = x => {
//                string name = Request.Form.NewKeyName;
//                DhcpConfig.MapFile.AddKey(name);
//                return Response.AsRedirect("/");
//            };

//            Post["/services/dhcp/addsubnet"] = x => {
//                string name = Request.Form.NewSubnet6Name;
//                DhcpConfig.MapFile.AddSubnet6(name);
//                return Response.AsRedirect("/");
//            };

//            Post["/services/dhcp/addsubnet"] = x => {
//                string name = Request.Form.NewSubnetName;
//                DhcpConfig.MapFile.AddSubnet(name);
//                return Response.AsRedirect("/");
//            };

//            Post["/services/dhcp/addhost"] = x => {
//                string name = Request.Form.NewHostName;
//                DhcpConfig.MapFile.AddHost(name);
//                return Response.AsRedirect("/");
//            };

//            Post["/services/dhcp/addclass"] = x => {
//                string name = Request.Form.NewClassName;
//                DhcpConfig.MapFile.AddClass(name);
//                return Response.AsRedirect("/");
//            };

//            Post["/services/dhcp/addsubclass"] = x => {
//                string name = Request.Form.NewSubclassName;
//                DhcpConfig.MapFile.AddSubclass(name);
//                return Response.AsRedirect("/");
//            };

//            Post["/services/dhcp/addfailover"] = x => {
//                string name = Request.Form.NewFailoverName;
//                DhcpConfig.MapFile.AddFailover(name);
//                return Response.AsRedirect("/");
//            };

//            Post["/services/dhcp/addlogging"] = x => {
//                string name = Request.Form.NewLoggingName;
//                DhcpConfig.MapFile.AddLogging(name);
//                return Response.AsRedirect("/");
//            };

//            Post["/services/dhcp/addgroup"] = x => {
//                string name = Request.Form.NewGroupName;
//                DhcpConfig.MapFile.AddGroup(name);
//                return Response.AsRedirect("/");
//            };

//            Post["/services/dhcp/addkey"] = x => {
//                string name = Request.Form.NewKeyName;
//                DhcpConfig.MapFile.AddKey(name);
//                return Response.AsRedirect("/");
//            };
//            #endregion DHCP

//            #region SSH
//            Post["/services/activate/ssh"] = x => {
//                SshConfig.SetReady();
//                SshConfig.MapFile.Render();
//                return HttpStatusCode.OK;
//            };

//            Post["/services/refresh/ssh"] = x => {
//                SshConfig.MapFile.Render();
//                return HttpStatusCode.OK;
//            };

//            Post["/services/reloadconfig/ssh"] = x => {
//                SshConfig.ReloadConfig();
//                return HttpStatusCode.OK;
//            };

//            Post["/services/update/ssh/{section}"] = x => {
//                //var parameters = this.Bind<List<ServiceSsh>>();
//                //var section = (string)x.section;
//                //if (section == "global") {
//                //    SshConfig.WriteFile.SaveGlobal(parameters);
//                //}
//                //if (section == "prefix6") {
//                //    SshConfig.WriteFile.SavePrefix6(parameters);
//                //}
//                //if (section == "range6") {
//                //    SshConfig.WriteFile.SaveRange6(parameters);
//                //}
//                //if (section == "range") {
//                //    SshConfig.WriteFile.SaveRange(parameters);
//                //}
//                //else {
//                //    SshConfig.WriteFile.SaveConfigFor(section, parameters);
//                //}
//                //Thread.Sleep(1000);
//                SshConfig.WriteFile.DumpGlobalConfig();
//                return Response.AsRedirect("/");
//            };

//            //Post["/services/ssh/addkey"] = x => {
//            //    string name = Request.Form.NewKeyName;
//            //    SshConfig.Keys.PropagateKeys(name);
//            //    return Response.AsRedirect("/");
//            //};
//            #endregion SSH
//        }
//    }
//}