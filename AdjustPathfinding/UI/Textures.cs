using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace AdjustPathfinding.UI
{
    // Icons:
    // Detour by Pierre-Luc Auclair from the Noun Project

    // Copied from TMPE. Thanks!
    public class Textures
    {
        public static readonly Texture2D AdjustPathfindSign;
        public static readonly Texture2D AdjustPathfindSignDisabled;

        static Textures()
        {
            AdjustPathfindSign = LoadDllResource("AdjustParhfindEnabled.png", 200, 200);
            AdjustPathfindSignDisabled = LoadDllResource("AdjustPathfindDisabled.png", 200, 200);
        }

        private static Texture2D LoadDllResource(string resourceName, int width, int height)
        {
            try
            {
                var myAssembly = Assembly.GetExecutingAssembly();
                var myStream = myAssembly.GetManifestResourceStream("AdjustPathfinding.Resources." + resourceName);

                var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);

                texture.LoadImage(ReadToEnd(myStream));

                return texture;
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace.ToString());
                return null;
            }
        }

        private static byte[] ReadToEnd(Stream stream)
        {
            var originalPosition = stream.Position;
            stream.Position = 0;

            try
            {
                var readBuffer = new byte[4096];

                var totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead != readBuffer.Length)
                        continue;

                    var nextByte = stream.ReadByte();
                    if (nextByte == -1)
                        continue;

                    var temp = new byte[readBuffer.Length * 2];
                    Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                    Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                    readBuffer = temp;
                    totalBytesRead++;
                }

                var buffer = readBuffer;
                if (readBuffer.Length == totalBytesRead)
                    return buffer;

                buffer = new byte[totalBytesRead];
                Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                return buffer;
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace.ToString());
                return null;
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }
    }
}
