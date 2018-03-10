//  <copyright file="OakUtils.cs" company="Xinity">
//  Copyright (c) 2014 All Right Reserved, http://xinity.com/
// 
//  </copyright>
//  <author>Pavel</author>
//  <email>pavel.fadrhonc@xinity.com</email>
//  <date>11/1/2014 11:53:51 AM</date>
//  <summary></summary>

using System;
using UnityEngine;

static class OakUtils
{
    #region PROPERTIES

    #endregion

    #region PUBLIC METHODS

    public static float AbsAngleDifference(float angle1, float angle2)
    {
        return Mathf.Min(Mathf.Min(Math.Abs(angle1 - angle2), angle1 + 360 - angle2), 360 - angle1 + angle2);
    }
	
	public static String ConvertToString(this Enum eff)
	{
		return Enum.GetName(eff.GetType(), eff);
	}	

    #endregion

    #region PRIVATE METHODS

    #endregion

}

