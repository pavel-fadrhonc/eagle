//  <copyright file="WorldSpaceColliderClick.cs" company="Xinity">
//  Copyright (c) 2015 All Right Reserved, http://xinity.com/
// 
//  </copyright>
//  <author>Pavel</author>
//  <email>pavel.fadrhonc@xinity.com</email>
//  <date>6/2/2015 12:29:34 PM</date>
//  <summary></summary>
using UnityEngine;
using UnityEngine.EventSystems;

public delegate void WorldSpaceColliderClickHandler();

public class WorldSpaceColliderClick : OakMonoBehaviour
{
    public event WorldSpaceColliderClickHandler WorldSpaceColliderClickEvent;
    public event WorldSpaceColliderClickHandler WorldSpaceColliderClickEventRight;

    #region PROPERTIES

    #endregion

    #region UNITY METHODS

    protected override void Awake()
    {
        base.Awake();
    }

    public void Start()
    {

    }

    public void Update()
    {

    }

    public void OnMouseOver()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (Input.GetMouseButtonDown(0))
        {
            if (WorldSpaceColliderClickEvent != null)
                WorldSpaceColliderClickEvent();
        } 
        if (Input.GetMouseButtonDown(1))
        {
            if (WorldSpaceColliderClickEventRight != null)
                WorldSpaceColliderClickEventRight();
        }

    }

    #endregion

    #region PUBLIC METHODS

    #endregion

    #region PRIVATE METHODS

    #endregion

}

