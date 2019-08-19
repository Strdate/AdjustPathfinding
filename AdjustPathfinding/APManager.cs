using AdjustPathfinding.Util;
using System.Runtime.Serialization;
using System.Xml;
using ColossalFramework.IO;
using ICities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace AdjustPathfinding
{
    public class APManager : SerializableDataExtensionBase
    {
        public static readonly string DATA_ID = "strad.AdjustPathfinding";
        private static APManager _inst;
        public static APManager Instance
        {
            get
            {
                return _inst;
            }
        }

        public Dictionary<ushort, AdjustedSegment> Dictionary = new Dictionary<ushort, AdjustedSegment>();

        public APManager()
        {
            _inst = this;
        }

        public override void OnSaveData()
        {
            var array = Dictionary.Values.ToArray();
            serializableDataManager.SaveData(DATA_ID,  Serialization.SaveToBytes(array) );
        }

        public override void OnLoadData()
        {
            Dictionary.Clear();
            try
            {
                byte[] bytes = serializableDataManager.LoadData(DATA_ID);
                var array = Serialization.LoadFromBytes(bytes);

                if (array != null)
                {
                    foreach (var e in array)
                    {
                        if (NetUtil.ExistsSegment(e.id))
                        {
                            Dictionary.Add(e.id, e);
                        }
                    }
                }
            }
            catch
            {
                Debug.LogError("Error when loading mod data!");
                Dictionary.Clear();
            }
        }
    }

    [Serializable]
    public class AdjustedSegment
    {
        // 0.1.0
        public ushort id;

        public string name;

        public bool active = true;
        public bool randomize = false;

        public float factor = 5;
        public float probability = 0.7F;

        // 0.2.0
        public Flags flags = (Flags) 3;

        // --

        public AdjustedSegment() { }
        public AdjustedSegment(ushort segment)
        {
            id = segment;
            name = segment.ToString();
        }

        [Flags]
        public enum Flags
        {
            None = 0,
            AffectPedestrians = 1,
            AffectVehicles = 2
        }
    }

    [Serializable]
    public class ModData
    {
        public string version;
        public byte[] data;

        public ModData(byte[] data)
        {
            this.data = data;
            version = ModInfo.VERSION;
        }
    }

}
