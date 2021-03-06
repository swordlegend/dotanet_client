﻿using DG.Tweening;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Btnstick : EventDispatcher
{
    //事件的监听者
    public EventListener onMove { get; private set; }//设置了一个安全权限
    public EventListener onEnd { get; private set; }
    public EventListener onDown { get; private set; }//设置了一个安全权限

    //UI里的对象
    //private GButton joystickButton;
    private GObject thumb;
    private GObject touchArea;
    private GObject center;
    //
    private GObject m_ui;

    //摇杆的属性
    private Vector2 initPos;
    private Vector2 startStagePos;
    private Vector2 lastStagePos;
    private int touchID;
    public int radius { get; set; }
    public float m_MoveCallNeedDir; //move回调需要移动的距离
    public bool m_IsMoved;//是否已经开始移动
    private Tweener tweener;

    public Btnstick(GComponent UI)
    {
        onMove = new EventListener(this, "onMove");
        onEnd = new EventListener(this, "onEnd");
        onDown = new EventListener(this, "onDown");
        

        thumb = UI.GetChild("thumb");
        thumb.visible = false;
        touchArea = UI.GetChild("toucharea");
        center = UI.GetChild("centerpic");
        center.visible = false;

        m_ui = UI;

        initPos = new Vector2(center.x, center.y);

       
        touchID = -1;
        radius = 120;
        m_MoveCallNeedDir = 20;//
        m_IsMoved = false;

        touchArea.onTouchBegin.Add(OnTouchBegin);
        touchArea.onTouchMove.Add(OnTouchMove);
        touchArea.onTouchEnd.Add(OnTouchEnd);

        Debug.Log("touchArea:"+ touchArea);

    }

    //开始触摸
    private void OnTouchBegin(EventContext context)
    {
        Debug.Log("OnTouchBegin");
        if (touchID == -1)//第一次触摸
        {
            InputEvent inputEvent = (InputEvent)context.data;
            touchID = inputEvent.touchId;

            if (tweener != null)
            {
                tweener.Kill();//杀死上一个动画
                tweener = null;
            }
            
            
            Vector2 localPos = m_ui.GlobalToLocal(inputEvent.position);
            

            lastStagePos = localPos;
            startStagePos = localPos;
            

            center.visible = true;
            thumb.visible = true;
            center.SetXY(localPos.x, localPos.y);
            thumb.SetXY(localPos.x, localPos.y);
            context.CaptureTouch();
            onDown.Call(1);
            m_IsMoved = false;


        }
    }

    public void SetTouchSize(int width,int height)
    {
        touchArea.SetSize(width, height);
    }

    //移动触摸
    private void OnTouchMove(EventContext context)
    {
        InputEvent inputEvent = (InputEvent)context.data;
        if (touchID != -1 && inputEvent.touchId == touchID)
        {
            //Vector2 localPos = GRoot.inst.GlobalToLocal(new Vector2(inputEvent.x, inputEvent.y));
            Vector2 localPos = m_ui.GlobalToLocal(inputEvent.position);

            var dir = (localPos - startStagePos);
            var len = Vector2.Distance(dir, new Vector2(0, 0));
            if(len > radius)
            {
                len = radius;
            }
            var dest = dir.normalized * len+ startStagePos;


            thumb.SetXY(dest.x, dest.y);

            thumb.rotation = Vector2.SignedAngle(new Vector2(0,-1),dir);

            if(len >= m_MoveCallNeedDir)
            {
                m_IsMoved = true;
                
            }
            if (m_IsMoved)
            {
                onMove.Call(dir);
            }
            
        }
    }

    //结束触摸
    private void OnTouchEnd(EventContext context)
    {
        InputEvent inputEvent = (InputEvent)context.data;
        if (touchID != -1 && inputEvent.touchId == touchID)
        {
            touchID = -1;
            center.visible = false;
            thumb.visible = false;

            Vector2 localPos = m_ui.GlobalToLocal(inputEvent.position);

            var dir = (localPos - startStagePos);

            onEnd.Call(Vector2.SignedAngle(new Vector2(0, 1), new Vector2(dir.x, -dir.y)));
            //onEnd.Call();
        }
        
    }


}
