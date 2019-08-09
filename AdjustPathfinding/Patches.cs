using AdjustPathfinding.UI;
using AdjustPathfinding.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdjustPathfinding
{
    public class Patches
    {
        private static Random random = new Random();

        // Patch #1
        public static void CalculateLaneSpeedPostfix(ref float __result, ref NetSegment segment)
        {
            if(APManager.Instance.Dictionary.TryGetValue(FindSegmentId(ref segment), out AdjustedSegment data) && data.active)
            {
                if(!data.randomize || random.NextDouble() < data.probability )
                {
                    __result /= data.factor;
                }
            }
        }

        // Patch #2
        public static void ReleaseSegmentPostfix(ushort segment)
        {
            if( APManager.Instance.Dictionary.ContainsKey(segment) )
            {
                APManager.Instance.Dictionary.Remove(segment);
                UIWindow.Instance.dropDown.Populate();
                AdjustPathfindingTool.Instance.SelectedSegment = 0;
            }
        }

        private static ushort FindSegmentId(ref NetSegment segment)
        {

            NetNode node = NetUtil.Node(segment.m_startNode);
            for (int i = 0; i < 8; i++)
            {
                ushort id = node.GetSegment(i);
                if (NetUtil.Segment(id).m_endNode == segment.m_endNode) // lol
                {
                    return id;
                }
            }

            return 0;
        }
    }
}
