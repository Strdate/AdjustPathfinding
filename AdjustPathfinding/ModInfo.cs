using AdjustPathfinding.UI;
using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using System;
using UnityEngine;

/* By Strad, 2019 */

namespace AdjustPathfinding
{
    public class ModInfo : IUserMod
    {
        public static readonly string VERSION = "0.2.0";
        public static readonly string SETTINGS_FILENAME = "AdjustPathfinding";

        public static readonly SavedInputKey ModShortcut = new SavedInputKey("modShortcut", SETTINGS_FILENAME, SavedInputKey.Encode(KeyCode.P, true, false, false), true);
        public static readonly SavedBool ShowUIButton = new SavedBool("showUIButton", SETTINGS_FILENAME, true, true);

        public static readonly Vector2 defWindowPosition = new Vector2(200, 200);
        public static readonly SavedInt savedWindowX = new SavedInt("windowX", SETTINGS_FILENAME, (int)defWindowPosition.x, true);
        public static readonly SavedInt savedWindowY = new SavedInt("windowY", SETTINGS_FILENAME, (int)defWindowPosition.y, true);

        public string Name => "Adjust Pathfinding";

        public string Description => "[" + VERSION + "]";

        public ModInfo()
        {
            try
            {
                // Creating setting file - from SamsamTS
                if (GameSettings.FindSettingsFileByName(SETTINGS_FILENAME) == null)
                {
                    GameSettings.AddSettingsFile(new SettingsFile[] { new SettingsFile() { fileName = SETTINGS_FILENAME } });
                }
            }
            catch (Exception e)
            {
                Debug.Log("Couldn't load/create the setting file.");
                Debug.LogException(e);
            }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            try
            {
                UIHelper group = helper.AddGroup(Name) as UIHelper;

                UIPanel uipanel = group.self as UIPanel;
                group.AddSpace(10);
                uipanel.gameObject.AddComponent<OptionsKeymapping>();
                group.AddSpace(10);

                UICheckBox checkBox = (UICheckBox)group.AddCheckbox("Show mod icon on toolbar (needs reload)", ShowUIButton.value, (b) =>
                {
                    ShowUIButton.value = b;
                });
                checkBox.tooltip = "Show Adjust Pathfinding icon in road tools panel (You can always use the shortcut to open mod menu)";

                group.AddSpace(10);

                group.AddButton("Reset tool window position", () =>
                {
                    savedWindowX.Delete();
                    savedWindowY.Delete();

                    if (UIWindow.Instance != null)
                        UIWindow.Instance.absolutePosition = defWindowPosition;
                });

                group.AddSpace(10);

                group.AddButton("Acknowledgements", () =>
                {
                    UIWindow.ThrowErrorMsg("Detour icon by Pierre-Luc Auclair from the Noun Project");
                });
            }
            catch (Exception e)
            {
                Debug.LogError("OnSettingsUI failed");
                Debug.Log(e);
            }
        }
    }
}
