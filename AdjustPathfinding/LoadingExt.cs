using AdjustPathfinding.UI;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using Harmony;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AdjustPathfinding
{
    public class LoadingExt : ILoadingExtension
    {
        public static readonly UInt64[] TMPE_IDs = { 583429740, 1637663252, 1806963141 };

        public static HarmonyInstance harmonyInstance;

        List<MethodInfo> pathFinds = new List<MethodInfo>();
        private MethodInfo postfixLaneSpeed = typeof(Patches).GetMethod("CalculateLaneSpeedPostfix");

        private MethodInfo originalReleaseSegment = typeof(NetManager).GetMethod("ReleaseSegment", BindingFlags.Public | BindingFlags.Instance);
        private MethodInfo postfixReleaseSegment = typeof(Patches).GetMethod("ReleaseSegmentPostfix");

        public static bool patched;

        public void OnCreated(ILoading loading)
        {
            if (loading.currentMode != AppMode.Game)
            {
                return;
            }

            if (harmonyInstance == null)
            {
                harmonyInstance = HarmonyInstance.Create("strad.AdjustPathFinding");
            }

            bool tmpeDetected = false;
            foreach (PluginManager.PluginInfo current in PluginManager.instance.GetPluginsInfo())
            {
                if ((current.name.Contains("TrafficManager") || TMPE_IDs.Contains(current.publishedFileID.AsUInt64)) && current.isEnabled)
                {
                    tmpeDetected = true;
                    ModInfo.DeveloperInfo += "Traffic Manager detected\nWorkshop ID: " + current.publishedFileID.AsUInt64 + "\nLocal: " + current.name + "\n";
                }
            }

            if (tmpeDetected)
            {
                try
                {
                    Type tmpePathfind1 = Type.GetType("TrafficManager.Custom.PathFinding.CustomPathFind, TrafficManager");
                    pathFinds.Add( tmpePathfind1.GetMethod("CalculateLaneSpeed", BindingFlags.NonPublic | BindingFlags.Instance) );
                    try
                    {
                        Type tmpePathfind2 = Type.GetType("TrafficManager.Custom.PathFinding.CustomPathFind2, TrafficManager");
                        pathFinds.Add(tmpePathfind2.GetMethod("CalculateLaneSpeed", BindingFlags.NonPublic | BindingFlags.Instance));
                    } catch { }
                    string tmpeVersion = GetTMPEVersion();
                    ModInfo.DeveloperInfo += "TMPE version: " + tmpeVersion + "\n";
                }
                catch
                {
                    string text = "Failed to retrieve TMPE version although it should be loaded. Pathfind methods might not be bound";
                    Debug.LogError(text);
                    ModInfo.DeveloperInfo += text + "\n";
                }
            }

            pathFinds.Add( typeof(PathFind).GetMethod("CalculateLaneSpeed", BindingFlags.NonPublic | BindingFlags.Instance) );

            pathFinds = pathFinds.Where((pf) => pf != null).ToList();
            ModInfo.DeveloperInfo += "Detoured CalculateLaneSpeed methods: " + pathFinds.Count + "\n";
            if (pathFinds.Count == 0 || originalReleaseSegment == null)
            {
                Debug.LogError("Failed to detour methods");
                ModInfo.DeveloperInfo += "FAILED to detour methods - mod is not working!\n";
            }
            else
            {
                foreach(var originalLaneSpeed in pathFinds)
                {
                    harmonyInstance.Patch(originalLaneSpeed, null, new HarmonyMethod(postfixLaneSpeed));
                }
                harmonyInstance.Patch(originalReleaseSegment, null, new HarmonyMethod(postfixReleaseSegment));
                patched = true;
            }
        }

        private static string GetTMPEVersion()
        {
            Version TMPE_Version = Assembly.Load("TrafficManager").GetName().Version;
            string version = null;
            try
            {
                Type tmpeMod = Type.GetType("TrafficManager.TrafficManagerMod, TrafficManager");
                version = tmpeMod.GetField("Version").GetValue(null) as string;
            }
            catch { version = TMPE_Version.ToString(); }
            return version;
        }

        public void OnLevelLoaded(LoadMode mode)
        {
            if (AdjustPathfindingTool.Instance == null)
            {
                ToolController toolController = GameObject.FindObjectOfType<ToolController>();
                AdjustPathfindingTool.Instance = toolController.gameObject.AddComponent<AdjustPathfindingTool>();
                AdjustPathfindingTool.Instance.enabled = false;
            }

            if (ModInfo.ShowUIButton.value && mode != LoadMode.NewGame && mode != LoadMode.LoadGame)
            {
                UIPanelButton.instance.enabled = false;
            }
            else
            {
                UIPanelButton.instance.enabled = true;
            }

            if (UIWindow.Instance == null)
            {
                UIWindow.Instance = (UIWindow)UIView.GetAView().AddUIComponent(typeof(UIWindow));
                UIWindow.Instance.enabled = false;
            }
        }

        public void OnLevelUnloading()
        {
            AdjustPathfindingTool.Instance.SelectedSegment = 0;
        }

        public void OnReleased()
        {
            if (patched)
            {
                foreach (var originalLaneSpeed in pathFinds)
                {
                    harmonyInstance.Unpatch(originalLaneSpeed, postfixLaneSpeed);
                }
                harmonyInstance.Unpatch(originalReleaseSegment, postfixReleaseSegment);
                patched = false;
            }
            APManager.Instance.Dictionary.Clear();
        }
    }
}
