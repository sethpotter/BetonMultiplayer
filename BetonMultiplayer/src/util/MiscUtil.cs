using UnityEngine;
using System;

namespace BetonMultiplayer
{
    public class MiscUtil
    {
        public static Color? floatStringToColor(string color)
        {
            string[] split = color.Split(',');
            if(split.Length == 3)
            {
                try
                {
                    return new Color(
                        float.Parse(split[0]),
                        float.Parse(split[1]),
                        float.Parse(split[2])
                    );
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
