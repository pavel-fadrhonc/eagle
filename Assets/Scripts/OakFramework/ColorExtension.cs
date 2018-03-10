//  <author>Pavel</author>
//  <email>pavel.fadrhonc@gmail.com</email>
//  <date>1/15/2014 5:47:20 PM</date>
//  <summary></summary>
using UnityEngine;

public static class ColorExtensions
{
    public static Color WithAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
}

