using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPD_First_Response.Mod.API;
using StopThePed.API;
using System.Reflection;
using System.Linq.Expressions;

namespace TiredCallouts
{
    public class Main : Plugin
    {

        public override void Initialize()
        {
            LSPD_First_Response.Mod.API.Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Game.LogTrivial("Plugin TiredCallouts" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " by Seitheach has been initialised.");
            Game.LogTrivial("Go On Duty to fully load TiredCallouts");

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(LSPDFRResolveEventHandler);
   
        }


        public override void Finally()
        {
            Game.LogTrivial("Plugin TiredCallouts has been cleaned up.");
        }

        private static void OnOnDutyStateChangedHandler(bool OnDuty)
        {
            if (OnDuty)
            {
                RegisterCallout();
                Game.DisplayNotification("TiredCallouts by Seitheach | Version 1.0.0 | Alpha - has been loaded.");
            }
        }

        private static void RegisterCallout()
        {
            LSPD_First_Response.Mod.API.Functions.RegisterCallout(typeof(Callouts.VeryFastChase));
            LSPD_First_Response.Mod.API.Functions.RegisterCallout(typeof(Callouts.ArmedDrunkBasic));
            LSPD_First_Response.Mod.API.Functions.RegisterCallout(typeof(Callouts.OfficerWelfareCheck));
        }


        public static Assembly LSPDFRResolveEventHandler(object sender, ResolveEventArgs args)
        {
            foreach (Assembly assembly in LSPD_First_Response.Mod.API.Functions.GetAllUserPlugins())
            {
                if (args.Name.ToLower().Contains(assembly.GetName().Name.ToLower()))
                {
                    return assembly;
                }
            }
            return null;

        }

        public static bool IsLSPDFRPluginRunning(string Plugin, Version minversion = null)
        {
            foreach (Assembly assembly in LSPD_First_Response.Mod.API.Functions.GetAllUserPlugins())
            {
                AssemblyName an = assembly.GetName();
                if (an.Name.ToLower() == Plugin.ToLower())
                {
                    if (minversion == null || an.Version.CompareTo(minversion) >= 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
