using AdjustPathfinding.UI;
using AdjustPathfinding.Util;
using ColossalFramework.UI;
using System.Collections.Generic;
using UnityEngine;

namespace AdjustPathfinding
{
    public class AdjustPathfindingTool : ToolBase
    {
        public static AdjustPathfindingTool Instance;

        private Quaternion? lastCamRot = null;
        private Vector3? lastCamPos = null;
        private HashSet<AdjustedSegment> currentlyVisibleSegments = new HashSet<AdjustedSegment>();

        private readonly float signSize = 80f;

        protected bool insideUI;

        protected ushort m_hoverSegment;

        private ushort m_selectedSegment;
        public ushort SelectedSegment
        {
            get => m_selectedSegment;
            set
            {
                if (value != m_selectedSegment && UIWindow.Instance != null && UIWindow.Instance.SelectSegment(value))
                {
                    m_selectedSegment = value;
                }
            }
        }

        protected override void OnToolUpdate()
        {
            base.OnToolUpdate();

            /* This last part was more or less copied from Elektrix's Segment Slope Smoother. He takes the credit. 
             * https://github.com/CosignCosine/CS-SegmentSlopeSmoother
             * https://steamcommunity.com/sharedfiles/filedetails/?id=1597198847 */

            if ((UIView.IsInsideUI() || !Cursor.visible))
            {
                m_hoverSegment = 0;
                insideUI = true;
                return;
            }

            insideUI = false;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastInput input = new RaycastInput(ray, Camera.main.farClipPlane);

            input.m_ignoreNodeFlags = NetNode.Flags.All;
            input.m_ignoreSegmentFlags = NetSegment.Flags.None;
            input.m_ignoreParkFlags = DistrictPark.Flags.All;
            input.m_ignorePropFlags = PropInstance.Flags.All;
            input.m_ignoreTreeFlags = TreeInstance.Flags.All;
            input.m_ignoreCitizenFlags = CitizenInstance.Flags.All;
            input.m_ignoreVehicleFlags = Vehicle.Flags.Created;
            input.m_ignoreBuildingFlags = Building.Flags.All;
            input.m_ignoreDisasterFlags = DisasterData.Flags.All;
            input.m_ignoreTransportFlags = TransportLine.Flags.All;
            input.m_ignoreParkedVehicleFlags = VehicleParked.Flags.All;
            input.m_ignoreTerrain = false;

            RayCast(input, out RaycastOutput output);
            m_hoverSegment = output.m_netSegment;
            //m_hoverPos = output.m_hitPos;

            if (Input.GetMouseButtonUp(0))
            {
                SelectedSegment = m_hoverSegment;
            }
        }

        protected override void OnToolGUI(Event e)
        {
            Quaternion camRot = Camera.main.transform.rotation;
            Vector3 camPos = Camera.main.transform.position;

            if (lastCamPos == null || lastCamRot == null || !lastCamRot.Equals(camRot) || !lastCamPos.Equals(camPos))
            {
                // cache visible segments
                currentlyVisibleSegments.Clear();

                foreach (KeyValuePair<ushort, AdjustedSegment> entry in APManager.Instance.Dictionary)
                {
                    ushort segmentId = entry.Key;
                    if (!NetUtil.ExistsSegment(segmentId))
                    {
                        APManager.Instance.Dictionary.Remove(segmentId);
                        continue;
                    }
                    /*if ((netManager.m_segments.m_buffer[segmentId].m_flags & NetSegment.Flags.Untouchable) != NetSegment.Flags.None)
						continue;*/

                    if ((NetUtil.Segment(segmentId).m_bounds.center - camPos).magnitude > 3000f)
                        continue; // do not draw if too distant

                    Vector3 screenPos;
                    bool visible = WorldToScreenPoint(NetUtil.Segment(segmentId).m_bounds.center, out screenPos);

                    if (!visible)
                        continue;

                    currentlyVisibleSegments.Add(APManager.Instance.Dictionary[segmentId]);
                }

                lastCamPos = camPos;
                lastCamRot = camRot;
            }

            //bool handleHovered = false;
            //bool clicked = !viewOnly && MainTool.CheckClicked();
            foreach (AdjustedSegment segment in currentlyVisibleSegments)
            {
                DrawSign(segment, ref camPos);

            }
        }

        private void DrawSign(AdjustedSegment segment, ref Vector3 camPos)
        {
            Vector3 center = NetUtil.Segment(segment.id).m_bounds.center;

            Vector3 screenPos;
            bool visible = WorldToScreenPoint(center, out screenPos);

            if (!visible)
                return;

            //float zoom = 1.0f / (center - camPos).magnitude * 100f * (Screen.height / 1200f);
            //float size = signSize * zoom * 5;
            float size = signSize * 1f;
            Color guiColor = GUI.color;
            guiColor.a = 0.6f;
            Rect boundingBox = new Rect(screenPos.x - size / 2, screenPos.y - size / 2, size, size);

            GUI.color = guiColor;
            GUI.DrawTexture(boundingBox, segment.active ? Textures.AdjustPathfindSign : Textures.AdjustPathfindSignDisabled);

            guiColor.a = 1f;
            GUI.color = guiColor;
        }

        public void Cleanup()
        {
            //segmentCenterByDir.Clear();
            currentlyVisibleSegments.Clear();
            lastCamPos = null;
            lastCamRot = null;
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            base.RenderOverlay(cameraInfo);

            if (m_hoverSegment != 0 && m_hoverSegment != SelectedSegment)
            {
                NetTool.RenderOverlay(cameraInfo, ref NetUtil.Segment(m_hoverSegment), new Color(0f, 0f, 0f, 0.5f), new Color(0f, 0f, 0f, 0.5f));
            }

            if (SelectedSegment != 0)
            {
                NetTool.RenderOverlay(cameraInfo, ref NetUtil.Segment(SelectedSegment), new Color(1f, 1f, 0.1875f, 0.75f), new Color(1f, 1f, 0.1875f, 0.75f));
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if(UIWindow.Instance != null)
            {
                UIWindow.Instance.enabled = false;
            }
            ToolsModifierControl.SetTool<DefaultTool>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (UIWindow.Instance != null)
            {
                UIWindow.Instance.enabled = true;
            }
        }

        private static bool WorldToScreenPoint(Vector3 worldPos, out Vector3 screenPos)
        {
            screenPos = Camera.main.WorldToScreenPoint(worldPos);
            screenPos.y = Screen.height - screenPos.y;

            return screenPos.z >= 0;
        }
    }
}
