//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.SiteTransfer;
using Kooboo.Api.ApiResponse;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Api;


namespace Kooboo.Web.Api.Implementation
{
    public class TransferApi : IApi
    {
        public TransferApi()
        {
            this.ModelName = "Transfer";
        }

        public string ModelName { get; set; }

        public bool RequireSite
        {
            get
            {
                return false;
            }
        }

        public bool RequireUser
        {
            get
            {
                return true;
            }
        }

        public  SingleResponse Single(string pageUrl, string name, ApiCall call)
        {
            SingleResponse response = new SingleResponse();
            var sitedb = call.WebSite.SiteDb();
                                                   
            if (Kooboo.Sites.SiteTransfer.TransferManager.IsUrlBanned(pageUrl))
            {
                string error = Data.Language.Hardcoded.GetValue("Target Url is Protected", call.Context);

                throw new Exception(error);
            }

            if (!pageUrl.ToLower().StartsWith("http"))
            {
                pageUrl = "http://" + pageUrl;
            }

            if (!Lib.Helper.UrlHelper.IsValidUrl(pageUrl, true))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Invalid Url", call.Context));
            }

            if (string.IsNullOrEmpty(name))
            {
                name = UrlHelper.GetPageName(pageUrl);
            }

            if (!sitedb.Routes.Validate(name, default(Guid)))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Url occupied", call.Context));
            }

            if (!string.IsNullOrEmpty(pageUrl))
            {
                var task = TransferManager.AddTask(sitedb, pageUrl, name, call.Context.User.Id);

                TransferManager.ExecuteTask(sitedb, task).Wait();
                    
                response.TaskId = task.Id;
                response.Finish = true;

                var transferpages = sitedb.TransferPages.Query.Where(o => o.taskid == task.Id).SelectAll();
                if (transferpages == null || transferpages.Count() == 0)
                {
                    response.Success = false;
                    response.Messages.Add("no page downloaded");
                }
                else
                {
                    var transferpage = transferpages.Find(o => o.done == true);
                    if (transferpage != null && transferpage.PageId != default(Guid))
                    {
                        response.Success = true;
                        var sitepage = sitedb.Pages.Get(transferpage.PageId);
                        if (sitepage != null)
                        {
                            response.Model = PageApi.ToPageViewModel(sitedb, sitepage);
                        }
                    }

                }

            }
            else
            {
                response.Finish = true;
                response.Success = false;
                response.Messages.Add("page url not found");
            }
            return response;
        }

        public virtual TransferResponse ByPage(ApiCall call)
        {           
            string fulldomain = call.GetValue("FullDomain");
            if (string.IsNullOrEmpty(fulldomain))
            {
                string RootDomain = call.GetValue("RootDomain");
                string SubDomain = call.GetValue("SubDomain");
                fulldomain = SubDomain + "." + RootDomain;
            }
            string sitename = call.GetValue("SiteName");

            if (string.IsNullOrEmpty(sitename) || string.IsNullOrEmpty(fulldomain))
            {
                return null;
            }

            string urlstring = call.GetValue("Urls");
            List<string> urls = Lib.Helper.JsonHelper.Deserialize<List<string>>(urlstring);

            if (urls != null && urls.Count() > 0)
            {
                var first = urls.First();

                if (Kooboo.Sites.SiteTransfer.TransferManager.IsUrlBanned(first))
                {
                    string error = Data.Language.Hardcoded.GetValue("Target Url is Protected", call.Context);

                    throw new Exception(error);
                }

                WebSite newsite = Kooboo.Sites.Service.WebSiteService.AddNewSite(call.Context.User.CurrentOrgId, sitename, fulldomain, call.Context.User.Id);

                var transferTask = TransferManager.AddTask(newsite.SiteDb(), urls, call.Context.User.Id);

                TransferManager.ExecuteTask(newsite.SiteDb(), transferTask);

                return new TransferResponse
                {
                    SiteId = newsite.Id,
                    TaskId = transferTask.Id,
                    Success = true
                };  
            }   
            return null;   
        }

        [Kooboo.Attributes.RequireParameters("RootDomain", "SubDomain", "SiteName", "url")]
        public virtual TransferResponse ByLevel(ApiCall call)
        {
            string RootDomain = call.GetValue("RootDomain");
            string SubDomain = call.GetValue("SubDomain");
            string fulldomain = SubDomain + "." + RootDomain;

            string sitename = call.GetValue("SiteName");

            if (string.IsNullOrEmpty(sitename) || string.IsNullOrEmpty(fulldomain))
            {
                return null;
            }
            // model.Url, model.TotalPages, model.Depth
            string url = call.GetValue("url");
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            if (Kooboo.Sites.SiteTransfer.TransferManager.IsUrlBanned(url))
            {
                string error = Data.Language.Hardcoded.GetValue("Target Url is Protected", call.Context);

                throw new Exception(error);
            }


            url = url.Trim();
            if (!url.ToLower().StartsWith("http"))
            {
                url = "http://" + url;
            }

            if (!Lib.Helper.UrlHelper.IsValidUrl(url, true))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Invalid Url", call.Context));
            }

            string strTotalPages = call.GetValue("TotalPages");
            string strDepth = call.GetValue("Depth");

            int totalpages = 0;
            int depth = 0;

            if (string.IsNullOrEmpty(strTotalPages) || !int.TryParse(strTotalPages, out totalpages))
            {
                totalpages = 10;
            }

            if (string.IsNullOrEmpty(strDepth) || !int.TryParse(strDepth, out depth))
            {
                depth = 2;
            }

            //if (Data.AppSettings.IsOnlineServer)
            //{
            //    if (totalpages > 3)
            //    {
            //        totalpages = 3;
            //    }
            //}

            WebSite newsite = Kooboo.Sites.Service.WebSiteService.AddNewSite(call.Context.User.CurrentOrgId, sitename, fulldomain, call.Context.User.Id);

            var transferTask = TransferManager.AddTask(newsite.SiteDb(), url, totalpages, depth, call.Context.User.Id);

            TransferManager.ExecuteTask(newsite.SiteDb(), transferTask);

            return new TransferResponse
            {
                SiteId = newsite.Id,
                TaskId = transferTask.Id,
                Success = true
            };
        }

        [Kooboo.Attributes.RequireParameters("url", "pages")]
        public HashSet<string> GetSubUrl(ApiCall call)
        {
            HashSet<string> result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string url = call.GetValue("url");
            string strpage = call.GetValue("pages");
            if (string.IsNullOrEmpty(url) && string.IsNullOrEmpty(strpage))
            {
                return null;
            }

            int pagenumber = 10;
            int setpage = 0;
            if (int.TryParse(strpage, out setpage))
            {
                pagenumber = setpage;
            }

            url = System.Net.WebUtility.UrlDecode(url);
            result.Add(url);

            if (result.Count >= pagenumber)
            {
                return result;
            }

            var download = Lib.Helper.DownloadHelper.DownloadUrl(url);

            if (download != null && download.StatusCode == 200 && download.isString)
            {
                string content = download.GetString();
                var dom = Kooboo.Dom.DomParser.CreateDom(content);

                foreach (var item in dom.Links.item)
                {
                    string itemsrc = Sites.Service.DomUrlService.GetLinkOrSrc(item);

                    if (string.IsNullOrEmpty(itemsrc))
                    { continue; }

                    string absoluteurl = UrlHelper.Combine(url, itemsrc);

                    bool issamehost = Kooboo.Lib.Helper.UrlHelper.isSameHost(url, absoluteurl);
                    if (issamehost)
                    {
                        result.Add(absoluteurl);
                        if (result.Count >= pagenumber)
                        {
                            return result;
                        }
                    }
                }
            }

            if (result.Count >= pagenumber)
            {
                return result;
            }

            return result;
        }

        [Kooboo.Attributes.RequireParameters("id", "siteid")]
        public List<TransferStatusItemViewModel> GetStatus(ApiCall call)
        {
            if (call.WebSite == null)
            {
                return null;
            }
            var allpages = call.WebSite.SiteDb().TransferPages.Query.Where(o => o.taskid == call.ObjectId).SelectAll();

            List<TransferStatusItemViewModel> result = new List<TransferStatusItemViewModel>();

            foreach (var item in allpages)
            {
                result.Add(new TransferStatusItemViewModel() { Url = item.absoluteUrl, Done = item.done });
            }
            return result;
        }

        [Kooboo.Attributes.RequireParameters("siteid")]
        public TaskResponse GetTaskStatus(ApiCall call)
        {
            TaskResponse response = new TaskResponse();
            var siteDb = call.WebSite.SiteDb();

            var tasks = siteDb.TransferTasks.Store.FullScan(o => o.done == false && o.CreationDate > DateTime.UtcNow.AddMinutes(-2)).SelectAll();

            if (tasks != null && tasks.Count() > 0)
            {
                response.Done = false;
            }
            else
            {
                response.Done = true;
            }

            response.Pages = siteDb.Pages.Count();
            response.Images = siteDb.Images.Count();

            return response;
        }

    }

    public class TaskResponse
    {
        public bool Done { get; set; }

        public int Pages { get; set; }

        public int Images { get; set; }
    }

}
