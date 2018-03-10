//  <copyright file="TransformExtensions.cs" company="Xinity">
//  Copyright (c) 2013 All Right Reserved, http://xinity.com/
// 
//  </copyright>
//  <author>Pavel</author>
//  <email>pavel.fadrhonc@xinity.com</email>
//  <date>11/12/2013 11:53:14 AM</date>
//  <summary>Extension methods for Transform</summary>

using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    private static List<Component> _descendantsComponents = new List<Component>();

    #region PUBLIC METHODS

    /// <summary>
    /// Traverse the transform hierarchy down the tree using depth first search and finds and returns all descendant components of Type T
    /// </summary>
    public static List<T> GetComponentsInDescendants<T>(this Transform transform) where T : Component
    {
        _descendantsComponents.Clear();
        AddAllDescendantsComponentsRecursively<T>(transform);

        List<T> retList = new List<T>();
        for (int index = 0; index < _descendantsComponents.Count; index++)
        {
            var comp = _descendantsComponents[index];
            retList.Add(comp as T);
        }

        return retList;
    }

    #endregion

    #region PRIVATE METHODS

    private static void AddAllDescendantsComponentsRecursively<T>(Transform transform) where T : Component
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform chilTrans = transform.GetChild(i);
            T comp = chilTrans.GetComponent<T>();
            if (comp != null)
                _descendantsComponents.Add(comp);
            AddAllDescendantsComponentsRecursively<T>(chilTrans);
        }
    }

    #endregion

}

