using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AdjustPathfinding.UI
{
    // From CimTools

    public static class UIUtil
    {
        public static UIButton CreateButton(UIComponent parent)
        {
            UIButton uibutton = parent.AddUIComponent<UIButton>();
            uibutton.atlas = ResourceLoader.GetAtlas("Ingame");
            uibutton.size = new Vector2(90f, 30f);
            uibutton.textScale = 0.9f;
            uibutton.normalBgSprite = "ButtonMenu";
            uibutton.hoveredBgSprite = "ButtonMenuHovered";
            uibutton.pressedBgSprite = "ButtonMenuPressed";
            uibutton.disabledBgSprite = "ButtonMenuDisabled";
            Color32 disabledTextColor = new Color32(7, 7, 7, byte.MaxValue);
            uibutton.disabledTextColor = disabledTextColor;
            uibutton.canFocus = false;
            uibutton.playAudioEvents = true;
            return uibutton;
        }

        // Token: 0x0600001B RID: 27 RVA: 0x00002A94 File Offset: 0x00000C94
        public static UICheckBox CreateCheckBox(UIComponent parent)
        {
            UICheckBox uicheckBox = parent.AddUIComponent<UICheckBox>();
            uicheckBox.width = 300f;
            uicheckBox.height = 20f;
            uicheckBox.clipChildren = true;
            UISprite uisprite = uicheckBox.AddUIComponent<UISprite>();
            uisprite.spriteName = "ToggleBase";
            uisprite.size = new Vector2(16f, 16f);
            uisprite.relativePosition = Vector3.zero;
            uicheckBox.checkedBoxObject = uisprite.AddUIComponent<UISprite>();
            ((UISprite)uicheckBox.checkedBoxObject).atlas = ResourceLoader.GetAtlas("Ingame");
            ((UISprite)uicheckBox.checkedBoxObject).spriteName = "ToggleBaseFocused";
            uicheckBox.checkedBoxObject.size = new Vector2(16f, 16f);
            uicheckBox.checkedBoxObject.relativePosition = Vector3.zero;
            uicheckBox.label = uicheckBox.AddUIComponent<UILabel>();
            uicheckBox.label.atlas = ResourceLoader.GetAtlas("Ingame");
            uicheckBox.label.text = " ";
            uicheckBox.label.textScale = 0.9f;
            uicheckBox.label.relativePosition = new Vector3(22f, 2f);
            uicheckBox.playAudioEvents = true;
            return uicheckBox;
        }

        // Token: 0x0600001C RID: 28 RVA: 0x00002BAC File Offset: 0x00000DAC
        public static UITextField CreateTextField(UIComponent parent)
        {
            UITextField uitextField = parent.AddUIComponent<UITextField>();
            uitextField.atlas = ResourceLoader.GetAtlas("Ingame");
            uitextField.size = new Vector2(90f, 20f);
            uitextField.padding = new RectOffset(6, 6, 3, 3);
            uitextField.builtinKeyNavigation = true;
            uitextField.isInteractive = true;
            uitextField.readOnly = false;
            uitextField.horizontalAlignment = UIHorizontalAlignment.Center;
            uitextField.selectionSprite = "EmptySprite";
            uitextField.selectionBackgroundColor = new Color32(0, 172, 234, byte.MaxValue);
            uitextField.normalBgSprite = "TextFieldPanelHovered";
            uitextField.textColor = new Color32(0, 0, 0, byte.MaxValue);
            uitextField.disabledTextColor = new Color32(0, 0, 0, 128);
            uitextField.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            return uitextField;
        }

        // Token: 0x0600001E RID: 30 RVA: 0x00002F50 File Offset: 0x00001150
        public static UIColorField CreateColorField(UIComponent parent)
        {
            UIColorField component = UnityEngine.Object.Instantiate<GameObject>(UnityEngine.Object.FindObjectOfType<UIColorField>().gameObject).GetComponent<UIColorField>();
            parent.AttachUIComponent(component.gameObject);
            component.size = new Vector2(40f, 26f);
            component.normalBgSprite = "ColorPickerOutline";
            component.hoveredBgSprite = "ColorPickerOutlineHovered";
            component.selectedColor = Color.white;
            component.pickerPosition = UIColorField.ColorPickerPosition.RightAbove;
            return component;
        }
    }
}
