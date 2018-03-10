//  <copyright file="GameSingleton.cs" company="Xinity">
//  Copyright (c) 2013 All Right Reserved, http://xinity.com/
// 
//  </copyright>
//  <author>Pavel</author>
//  <email>pavel.fadrhonc@xinity.com</email>
//  <date>12/8/2013 11:26:46 PM</date>
//  <summary>Differs from SceneSingleton in that it is not destroyed on new scene load
// and is persistent throughout the whole game. </summary>
using UnityEngine;
using System.Linq;

public class GameSingleton<T> : SceneSingleton<T> where T : OakMonoBehaviour
{
    #region UNITY METHODS

	protected override void Awake()
    {
        base.Awake();

        if (_instance != null)
        {
            var children = (from Transform child in transform select child.gameObject).ToList();
            children.ForEach(Destroy);
            Destroy(gameObject);
        }

        transform.parent = null;

        DontDestroyOnLoad(this);
    }

    #endregion
}

