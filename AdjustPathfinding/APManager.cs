using AdjustPathfinding.Util;
using ColossalFramework.IO;
using ICities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
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
            var data = SerializeData(array);
            var containerObj = new ModData(data);
            var bytes = SerializeData(containerObj);
            serializableDataManager.SaveData(DATA_ID, bytes);
        }

        public override void OnLoadData()
        {
            Dictionary.Clear();
            byte[] bytes = serializableDataManager.LoadData(DATA_ID);

            if (bytes != null)
            {
                var containerObj = DeserializeData<ModData>(bytes);
                var deserializedArray = DeserializeData<AdjustedSegment[]>(containerObj.data);
                Debug.Log("Deserialized byte[] data. Version: " + containerObj.version);

                foreach (var e in deserializedArray)
                {
                    if (NetUtil.ExistsSegment(e.id))
                    {
                        Dictionary.Add(e.id, e);
                    }
                }
            }
        }

        private static byte[] SerializeData<T>(T obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                return stream.ToArray();
            }
        }

        private static T DeserializeData<T>(byte[] rawData)
        {
            using (MemoryStream ms = new MemoryStream(rawData))
            {
                IFormatter br = new BinaryFormatter();
                return (T)br.Deserialize(ms);
            }
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

    [Serializable]
    public class AdjustedSegment
    {
        public ushort id;

        public string name;

        public bool active = true;
        public bool randomize = false;

        public float factor = 5;
        public float probability = 0.7F;

        public AdjustedSegment() { }
        public AdjustedSegment(ushort segment)
        {
            id = segment;
            name = segment.ToString();
        }
    }
}
