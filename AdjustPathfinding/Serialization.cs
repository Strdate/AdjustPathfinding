using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace AdjustPathfinding
{
    public class Serialization
    {
        public static AdjustedSegment[] LoadFromBytes(byte[] bytes)
        {
            if (bytes != null)
            {
                var containerObj = DeserializeData<ModData>(bytes);
                var deserializedArray = DeserializeData<AdjustedSegment[]>(containerObj.data);
                //Debug.Log("Deserialized byte[] data. Version: " + containerObj.version);

                // 0.2.0 backward compatibility
                if(CompareModVersions(containerObj.version,"0.2.0") == -1)
                {
                    Debug.LogWarning("Loaded data version: " + containerObj.version + " Fixing data to match current mod version.");
                    foreach(var item in deserializedArray)
                    {
                        item.flags |= AdjustedSegment.Flags.AffectPedestrians | AdjustedSegment.Flags.AffectVehicles;
                    }
                }

                return deserializedArray;
            }
            return null;
        }

        public static byte[] SaveToBytes(AdjustedSegment[] array)
        {
            var data = SerializeData(array);
            var containerObj = new ModData(data);
            return SerializeData(containerObj);
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

        public static int CompareModVersions(string a, string b)
        {
            var arr1 = a.Split('.');
            var arr2 = b.Split('.');
            int num1 = 0;
            int num2 = 0;
            num1 = int.Parse(arr1[0]) * 1000000 + int.Parse(arr1[1]) * 1000 + int.Parse(arr1[2]);
            num2 = int.Parse(arr2[0]) * 1000000 + int.Parse(arr2[1]) * 1000 + int.Parse(arr2[2]);
            return num1.CompareTo(num2);
        }
    }
}
