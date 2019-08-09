using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AdjustPathfinding.UI
{
    public class UIWindow : UIPanel
    {
        public static UIWindow Instance;

        private const float spacing = 8;

        private float cumulativeHeight;
        private SetupPanel setupPanel;

        public CustomDropDown dropDown;

        public override void Start()
        {
            name = "APF_panel";
            atlas = ResourceLoader.GetAtlas("Ingame");
            backgroundSprite = "SubcategoriesPanel";
            size = new Vector2(300, 180);

            cumulativeHeight = 8;

            absolutePosition = new Vector3(ModInfo.savedWindowX.value, ModInfo.savedWindowY.value);

            eventPositionChanged += (c, p) =>
            {
                if (absolutePosition.x < 0)
                    absolutePosition = ModInfo.defWindowPosition;

                Vector2 resolution = GetUIView().GetScreenResolution();

                absolutePosition = new Vector2(
                    Mathf.Clamp(absolutePosition.x, 0, resolution.x - width),
                    Mathf.Clamp(absolutePosition.y, 0, resolution.y - height));

                ModInfo.savedWindowX.value = (int)absolutePosition.x;
                ModInfo.savedWindowY.value = (int)absolutePosition.y;
            };

            UIPanel header = AddUIComponent<UIPanel>();
            header.relativePosition = Vector2.zero;
            header.width = width;

            UILabel label = header.AddUIComponent<UILabel>();
            label.textScale = 0.9f;
            label.text = "Adjust Pathfinding";
            label.relativePosition = new Vector2(spacing, spacing);

            UIDragHandle dragHandle = header.AddUIComponent<UIDragHandle>();
            dragHandle.width = width;
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.target = this;

            dragHandle.height = spacing + label.height;
            header.height = dragHandle.height;
            cumulativeHeight += header.height;

            dropDown = AddUIComponent<CustomDropDown>();
            dropDown.relativePosition = new Vector2(8, cumulativeHeight);
            dropDown.width = width - 16;
            cumulativeHeight += /*dropDown.height*/ + 8 + 30;

            setupPanel = AddUIComponent<SetupPanel>();
            setupPanel.relativePosition = new Vector2(0, cumulativeHeight);

            //height = cumulativeHeight + setupPanel.height + 8;
        }

        public bool SelectSegment(ushort segment)
        {
            setupPanel.LoadSegment(segment);
            dropDown.SelectSegmentIndex(segment);
            AdjustPathfindingTool.Instance.Cleanup();
            return true;
        }

        protected override void OnMouseDown(UIMouseEventParameter p)
        {
            base.OnMouseDown(p);
            AdjustPathfindingTool.Instance.enabled = true;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            //AdjustPathfindingTool.Instance.enabled = true;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            //AdjustPathfindingTool.Instance.enabled = false;
        }

        public void RecalculateHeight()
        {
            height = cumulativeHeight + setupPanel.height + 8;
        }

        public static void ThrowErrorMsg(string content, bool error = false)
        {
            ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
            panel.SetMessage("Adjust Pathfinding", content, error);
        }
    }
}
