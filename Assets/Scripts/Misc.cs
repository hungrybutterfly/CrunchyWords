using UnityEngine;
using System.Collections;

public class MyMisc : MonoBehaviour 
{
    static public void HexToRGB(string hex, out float r, out float g, out float b, out float a) 
    {
        r = (byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber)) / 255.0f;
        g = (byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber)) / 255.0f;
        b = (byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber)) / 255.0f;
        a = (byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber)) / 255.0f;
    }

    static public void HexToColour(string hex, out Color _colour)
    {
        _colour = new Color();
        HexToRGB(hex, out _colour.r, out _colour.g, out _colour.b, out _colour.a);
    }
}
