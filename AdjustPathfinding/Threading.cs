using AdjustPathfinding.UI;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AdjustPathfinding
{
    public class Threading : ThreadingExtensionBase
    {
        private bool _processed = false;

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            //if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKey(KeyCode.P))
            if (ModInfo.ModShortcut.IsPressed())
            {

                if (_processed)
                    return;

                _processed = true;

                if(UIWindow.Instance != null && LoadingExt.patched)
                {
                    //UIWindow.Instance.enabled = !UIWindow.Instance.enabled;
                    AdjustPathfindingTool.Instance.enabled = !AdjustPathfindingTool.Instance.enabled;
                }

            }
            else
            {
                _processed = false;
            }
        }
    }
}
