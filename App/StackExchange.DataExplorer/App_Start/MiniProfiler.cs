using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using MvcMiniProfiler;
using MvcMiniProfiler.MVCHelpers;
using Microsoft.Web.Infrastructure;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using StackExchange.DataExplorer.Controllers;

[assembly: WebActivator.PreApplicationStartMethod(
	typeof(StackExchange.DataExplorer.App_Start.MiniProfilerPackage), "PreStart")]

[assembly: WebActivator.PostApplicationStartMethod(
	typeof(StackExchange.DataExplorer.App_Start.MiniProfilerPackage), "PostStart")]


namespace StackExchange.DataExplorer.App_Start 
{
    public static class MiniProfilerPackage
    {
        class ProxySafeUserProvider : IUserProvider
        {
            public string GetUser(HttpRequest request)
            {
                return StackOverflowController.GetRemoteIP(request.ServerVariables);
            }
        }

        public static void PreStart()
        {
            WebRequestProfilerProvider.Settings.UserProvider = new ProxySafeUserProvider();
            MiniProfiler.Settings.SqlFormatter = new MvcMiniProfiler.SqlFormatters.SqlServerFormatter();

            //Make sure the MiniProfiler handles BeginRequest and EndRequest
            DynamicModuleUtility.RegisterModule(typeof(MiniProfilerStartupModule));

            //Setup profiler for Controllers via a Global ActionFilter
            GlobalFilters.Filters.Add(new ProfilingActionFilter());
        }

        public static void PostStart()
        {
            // Intercept ViewEngines to profile all partial views and regular views.
            // If you prefer to insert your profiling blocks manually you can comment this out
            var copy = ViewEngines.Engines.ToList();
            ViewEngines.Engines.Clear();
            foreach (var item in copy)
            {
                ViewEngines.Engines.Add(new ProfilingViewEngine(item));
            }
        }
    }

    public class MiniProfilerStartupModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.BeginRequest += (sender, e) =>
            {
                MiniProfiler.Start();
            };

            context.EndRequest += (sender, e) =>
            {
                MiniProfiler.Stop();
            };
        }

        public void Dispose() { }
    }
}

