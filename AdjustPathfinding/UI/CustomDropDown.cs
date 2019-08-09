using AdjustPathfinding.Util;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AdjustPathfinding.UI
{
    public class CustomDropDown : UIDropDown
    {
        private ushort[] ids;
        private bool shifted;

        private bool isEvent;

        public override void Start()
        {
            base.Start();
            atlas = ResourceLoader.GetAtlas("Ingame");
            size = new Vector2(284f, 30f);
            listBackground = "GenericPanelLight";
            itemHeight = 25;
            itemHover = "ListItemHover";
            itemHighlight = "ListItemHighlight";
            normalBgSprite = "ButtonMenu";
            disabledBgSprite = "ButtonMenuDisabled";
            hoveredBgSprite = "ButtonMenuHovered";
            focusedBgSprite = "ButtonMenu";
            listWidth = 284;
            listHeight = 1000;
            listPosition = UIDropDown.PopupListPosition.Below;
            clampListToScreen = true;
            builtinKeyNavigation = true;
            foregroundSpriteMode = UIForegroundSpriteMode.Stretch;
            popupColor = new Color32(45, 52, 61, 255);
            popupTextColor = new Color32(170, 170, 170, 255);
            zOrder = 1;
            textScale = 0.8f;
            verticalAlignment = UIVerticalAlignment.Middle;
            horizontalAlignment = UIHorizontalAlignment.Left;
            selectedIndex = 0;
            textFieldPadding = new RectOffset(8, 0, 8, 0);
            itemPadding = new RectOffset(14, 0, 8, 0);

            UIButton button = AddUIComponent<UIButton>();
            triggerButton = button;
            button.atlas = ResourceLoader.GetAtlas("Ingame");
            button.text = "";
            button.size = size;
            button.relativePosition = new Vector3(0f, 0f);
            button.textVerticalAlignment = UIVerticalAlignment.Middle;
            button.textHorizontalAlignment = UIHorizontalAlignment.Left;
            button.normalFgSprite = "IconDownArrow";
            button.hoveredFgSprite = "IconDownArrowHovered";
            button.pressedFgSprite = "IconDownArrowPressed";
            button.focusedFgSprite = "IconDownArrowFocused";
            button.disabledFgSprite = "IconDownArrowDisabled";
            button.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
            button.horizontalAlignment = UIHorizontalAlignment.Right;
            button.verticalAlignment = UIVerticalAlignment.Middle;
            button.zOrder = 0;
            button.textScale = 0.8f;

            eventSizeChanged += new PropertyChangedEventHandler<Vector2>((c, t) =>
            {
                button.size = t; listWidth = (int)t.x;
            });

            Populate();
            SelectSegmentIndex(0);
        }

        protected override void OnSelectedIndexChanged()
        {
            base.OnSelectedIndexChanged();
            if(!isEvent)
            {
                AdjustPathfindingTool.Instance.SelectedSegment = ids[selectedIndex];
                if(ids[selectedIndex] != 0)
                {
                    InstanceID id = default(InstanceID);
                    id.NetSegment = ids[selectedIndex];
                    Vector3 center = NetUtil.Segment(ids[selectedIndex]).m_bounds.center;
                    center.y = Camera.main.transform.position.y;
                    ToolsModifierControl.cameraController.SetTarget(id, center, true);
                }
            }
        }

        public void Populate()
        {
            shifted = false;
            ids = APManager.Instance.Dictionary.Values.ToArray().Select(data => data.id).ToArray();
            items = APManager.Instance.Dictionary.Values.ToArray().Select(data => data.name).ToArray();
        }

        public void SelectSegmentIndex(ushort segment)
        {
            if(segment == 0)
            {
                ShiftArrays("< Segment >");
            } else if(ids.Contains(segment))
            {
                UnshiftArrays();
                isEvent = true;
                selectedIndex = Array.IndexOf(ids,segment);
                isEvent = false;
            } else
            {
                ShiftArrays(/*"< Segment " + segment + " >"*/"< Segment >");
            }
        }

        private void ShiftArrays(string label)
        {
            if(shifted)
            {
                items[0] = label;
            }
            else
            {
                shifted = true;
                ushort[] newIds = new ushort[ids.Length + 1];
                ids.CopyTo(newIds, 1);
                ids = newIds;
                string[] newItems = new string[items.Length + 1];
                newItems[0] = label;
                items.CopyTo(newItems, 1);
                items = newItems;
            }
            isEvent = true; // hack
            selectedIndex = 0;
            isEvent = false;
        }

        private void UnshiftArrays()
        {
            if(shifted)
            {
                ushort[] newIds = new ushort[ids.Length - 1];
                Array.Copy(ids, 1, newIds, 0, ids.Length-1);
                ids = newIds;
                string[] newItems = new string[items.Length - 1];
                Array.Copy(items, 1, newItems, 0, items.Length - 1);
                items = newItems;
            }
            shifted = false;
        }
    }
}
