using AdjustPathfinding.UI;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using Harmony;
using ICities;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AdjustPathfinding
{
    public class LoadingExt : ILoadingExtension
    {
        public static readonly UInt64[] TMPE_IDs = { 583429740, 1637663252, 1806963141 };

        public static HarmonyInstance harmonyInstance;

        private MethodInfo originalLaneSpeed;
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
                }
            }

            if (tmpeDetected)
            {
                try
                {
                    originalLaneSpeed = GetTMPEMethod();
                }
                catch
                {
                    Debug.LogWarning("Failed to retrieve pointer to TMPE method although it should be loaded");
                    originalLaneSpeed = typeof(PathFind).GetMethod("CalculateLaneSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
                }
            }
            else
            {
                originalLaneSpeed = typeof(PathFind).GetMethod("CalculateLaneSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            }

            harmonyInstance.Patch(originalLaneSpeed, null, new HarmonyMethod(postfixLaneSpeed));
            harmonyInstance.Patch(originalReleaseSegment, null, new HarmonyMethod(postfixReleaseSegment));

            patched = true;
        }

        // May not be interpreted if tmpe is missing
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static MethodInfo GetTMPEMethod()
        {
            return typeof(TrafficManager.Custom.PathFinding.CustomPathFind).GetMethod("CalculateLaneSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
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
                harmonyInstance.Unpatch(originalLaneSpeed, postfixLaneSpeed);
                harmonyInstance.Unpatch(originalReleaseSegment, postfixReleaseSegment);
                patched = false;
            }
            APManager.Instance.Dictionary.Clear();
        }
    }
}
