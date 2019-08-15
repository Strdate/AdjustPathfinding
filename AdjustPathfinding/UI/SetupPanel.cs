using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AdjustPathfinding.UI
{
    public class SetupPanel : UIPanel
    {
        private UILabel idLabel;
        private UITextField nameTF;
        private UITextField factorTF;
        private UITextField probabilityTF;
        private UICheckBox randBox;
        private UICheckBox activeBox;
        // 0.2.0
        private UICheckBox vehicleBox;
        private UICheckBox pedestrianBox;
        //private UISlider slider;

        private UIButton removeButton;
        private UIButton saveButton;

        private bool isSegmentSelected;
        private bool changesMade;
        private bool isNewSegment;

        public void LoadSegment(ushort id)
        {
            if(id == 0)
            {
                isSegmentSelected = false;
                idLabel.text = "Segment ID: < Select segment >";
                nameTF.text = "";
                factorTF.text = "5";
                randBox.isChecked = false;
                activeBox.isChecked = false;
                vehicleBox.isChecked = false;
                pedestrianBox.isChecked = false;
                //slider.value = data.probability;
                probabilityTF.text = "0.7";
                saveButton.isEnabled = false;
                removeButton.isEnabled = false;
                return;
            }

            isSegmentSelected = true;
            AdjustedSegment data;
            if(!APManager.Instance.Dictionary.TryGetValue(id, out data))
            {
                data = new AdjustedSegment(id);
                isNewSegment = true;
                saveButton.text = "Save new";
                saveButton.isEnabled = true;
                removeButton.isEnabled = false;

            } else
            {
                isNewSegment = false;
                saveButton.text = "Save";
                saveButton.isEnabled = true;
                removeButton.isEnabled = true;
            }

            idLabel.text = "Segment: " + id;
            nameTF.text = data.name;
            factorTF.text = data.factor.ToString();
            randBox.isChecked = data.randomize;
            activeBox.isChecked = data.active;
            vehicleBox.isChecked = (data.flags & AdjustedSegment.Flags.AffectVehicles) != AdjustedSegment.Flags.None;
            pedestrianBox.isChecked = (data.flags & AdjustedSegment.Flags.AffectPedestrians) != AdjustedSegment.Flags.None;
            //slider.value = data.probability;
            probabilityTF.text = data.probability.ToString();
        }

        public override void Start()
        {
            width = parent.width;

            float cumulativeHeight = 8;

            idLabel = AddUIComponent<UILabel>();
            idLabel.textScale = 0.9f;
            idLabel.text = "Segment ID: <Select segment>";
            idLabel.tooltip = "Internal segment ID";
            idLabel.relativePosition = new Vector2(8, 8);
            cumulativeHeight += idLabel.height + 8;

            UIPanel nameLine = AddUIComponent<UIPanel>();
            nameLine.relativePosition = new Vector2(8, cumulativeHeight);
            nameLine.width = parent.width;

            UILabel labelName = nameLine.AddUIComponent<UILabel>();
            labelName.textScale = 0.9f;
            labelName.text = "Custom name:";
            labelName.relativePosition = Vector2.zero;

            nameTF = UIUtil.CreateTextField(nameLine);
            nameTF.width = width - labelName.width - 24;
            nameTF.eventTextChanged += (e, v) =>
            {
                changesMade = true;
            };
            nameTF.relativePosition = new Vector2(labelName.width + 8,0);

            nameLine.height = nameTF.height;
            cumulativeHeight += nameLine.height + 8;


            UIPanel factorLine = AddUIComponent<UIPanel>();
            factorLine.relativePosition = new Vector2(8, cumulativeHeight);
            factorLine.width = parent.width;

            UILabel labelFactor = factorLine.AddUIComponent<UILabel>();
            labelFactor.textScale = 0.9f;
            labelFactor.text = "Factor:";
            labelFactor.tooltip = "Example: if factor is 3 the segment will appear 3 times longer to the pathfinding algorithm";
            labelFactor.relativePosition = Vector2.zero;

            factorTF = UIUtil.CreateTextField(factorLine);
            factorTF.relativePosition = new Vector2(labelFactor.width + 8, 0);
            factorTF.tooltip = "Example: if factor is 3 the segment will appear 3 times longer to the pathfinding algorithm";
            factorTF.eventTextChanged += (e, v) =>
            {
                changesMade = true;
            };

            factorLine.height = factorTF.height;
            cumulativeHeight += factorLine.height + 8;

            vehicleBox = UIUtil.CreateCheckBox(this);
            vehicleBox.relativePosition = new Vector2(8, cumulativeHeight);
            vehicleBox.text = "Vehicles";
            vehicleBox.tooltip = "Apply to vehicles";
            vehicleBox.width = 130;

            pedestrianBox = UIUtil.CreateCheckBox(this);
            pedestrianBox.relativePosition = new Vector2(16 + vehicleBox.width, cumulativeHeight);
            pedestrianBox.text = "Pedestrians";
            pedestrianBox.tooltip = "Apply to pedestrians";
            pedestrianBox.width = 130;

            cumulativeHeight += vehicleBox.height + 8;

            UIPanel randomizeLine = AddUIComponent<UIPanel>();
            randomizeLine.relativePosition = new Vector2(8, cumulativeHeight);
            randomizeLine.width = parent.width;

            randBox = UIUtil.CreateCheckBox(randomizeLine);
            randBox.relativePosition = new Vector2(0, 0);
            randBox.text = "Event probability:";
            randBox.tooltip = "Between 0 and 1. Affects how many vehicles will follow the costum pathfind settings. If turned off, all vehicles will follow";
            randBox.width = 180;
            randBox.eventCheckChanged += (e, v) =>
            {
                changesMade = true;
            };

            probabilityTF = UIUtil.CreateTextField(randomizeLine);
            probabilityTF.relativePosition = new Vector2(randBox.width + 8, 0);
            probabilityTF.tooltip = "Between 0 and 1. Affects how many vehicles will follow the costum pathfind settings. If turned off, all vehicles will follow";
            probabilityTF.eventTextChanged += (e, v) =>
            {
                changesMade = true;
            };

            /*slider = randomizeLine.AddUIComponent<UISlider>();
            slider.atlas = ResourceLoader.GetAtlas("Ingame");
            slider.height = randBox.height;
            slider.width = width - randBox.width - 24;
            slider.minValue = 0;
            slider.maxValue = 1;
            slider.stepSize = 0.01F;
            slider.relativePosition = new Vector2(randBox.width + 8, 0);
            slider.eventValueChanged += (e, v) =>
            {
                randBox.text = "Randomize (" + v.ToString("0.00") + ")";
                changesMade = true;
            };

            randomizeLine.height = slider.height;*/
            randomizeLine.height = probabilityTF.height;
            cumulativeHeight += randomizeLine.height + 8;

            activeBox = UIUtil.CreateCheckBox(this);
            activeBox.tooltip = "You can temorarily turn off this policy";
            activeBox.width = 180;
            activeBox.relativePosition = new Vector2(8, cumulativeHeight);
            activeBox.text = "Active";
            activeBox.eventCheckChanged += (e, v) =>
            {
                changesMade = true;
            };

            cumulativeHeight += activeBox.height + 8;


            UIPanel lastLine = AddUIComponent<UIPanel>();
            lastLine.relativePosition = new Vector2(8, cumulativeHeight);
            lastLine.width = parent.width;

            saveButton = UIUtil.CreateButton(lastLine);
            saveButton.relativePosition = Vector2.zero;
            saveButton.text = "Save";
            saveButton.eventClicked += (e, p) => Save();

            removeButton = UIUtil.CreateButton(lastLine);
            removeButton.relativePosition = new Vector2(saveButton.width + 8, 0);
            removeButton.text = "Remove";
            removeButton.eventClicked += (e, p) =>
            {
                if (!SimulationManager.instance.SimulationPaused)
                {
                    UIWindow.ThrowErrorMsg("Simulation must be paused!");
                    return;
                }

                if (!isSegmentSelected)
                {
                    UIWindow.ThrowErrorMsg("No selected segment!");
                }

                APManager.Instance.Dictionary.Remove(AdjustPathfindingTool.Instance.SelectedSegment);

                UIWindow.Instance.dropDown.Populate();
                AdjustPathfindingTool.Instance.SelectedSegment = 0;
            };

            UIButton closeButton = UIUtil.CreateButton(lastLine);
            closeButton.relativePosition = new Vector2(saveButton.width + removeButton.width + 16, 0);
            closeButton.text = "Close";
            closeButton.eventClicked += (e, p) =>
            {
                //parent.enabled = false;
                AdjustPathfindingTool.Instance.enabled = false;
            };


            lastLine.height = saveButton.height;
            cumulativeHeight += lastLine.height + 8;

            height = cumulativeHeight;
            UIWindow.Instance.RecalculateHeight();
        }

        private void Save()
        {
            if (AdjustPathfindingTool.Instance.SelectedSegment == 0)
            {
                return;
            }

            if (!SimulationManager.instance.SimulationPaused)
            {
                UIWindow.ThrowErrorMsg("Simulation must be paused!");
                return;
            }

            if (!isSegmentSelected)
            {
                UIWindow.ThrowErrorMsg("No selected segment!");
                return;
            }

            float newFactor;
            float newProbability;
            if (!float.TryParse(factorTF.text, out newFactor) || !float.TryParse(probabilityTF.text, out newProbability) || newFactor <= 0 || newProbability > 1 || newProbability < 0)
            {
                UIWindow.ThrowErrorMsg("Invalid fields!");
                return;
            }

            AdjustedSegment data;
            if (isNewSegment)
            {
                data = new AdjustedSegment(AdjustPathfindingTool.Instance.SelectedSegment);
            }
            else
            {
                if (!APManager.Instance.Dictionary.TryGetValue(AdjustPathfindingTool.Instance.SelectedSegment, out data))
                    Debug.Log("Failed to save data in existing AdjustedSegment.");
            }

            data.name = nameTF.text;
            data.factor = newFactor;
            data.randomize = randBox.isChecked;
            data.active = activeBox.isChecked;
            //data.probability = slider.value;
            data.probability = newProbability;

            if (vehicleBox.isChecked) data.flags |= AdjustedSegment.Flags.AffectVehicles; else data.flags &= ~AdjustedSegment.Flags.AffectVehicles;
            if (pedestrianBox.isChecked) data.flags |= AdjustedSegment.Flags.AffectPedestrians; else data.flags &= ~AdjustedSegment.Flags.AffectPedestrians;

            if (isNewSegment)
            {
                APManager.Instance.Dictionary.Add(AdjustPathfindingTool.Instance.SelectedSegment, data);
                isNewSegment = false;
            }

            UIWindow.Instance.dropDown.Populate();
            UIWindow.Instance.SelectSegment(AdjustPathfindingTool.Instance.SelectedSegment);
        }
    }
}
