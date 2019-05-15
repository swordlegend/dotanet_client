﻿
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEntity
{

    protected GameScene m_Scene;
    protected Vector3 m_StartPos;
    protected Vector3 m_Position;
    protected Vector3 m_EndPos;
    protected int m_State;////子弹状态(1:创建,2:移动,3:到达后计算结果(伤害和回血) 4:完成 可以删除了)
    protected string m_ModeType;//
    protected int m_ID;
    protected GameObject m_Mode;

    public BulletEntity(Protomsg.BulletDatas data, GameScene scene)
    {
        //数据赋值
        m_Scene = scene;
        m_ID = data.ID;
        m_Position = new Vector3(data.X, data.Z, data.Y);
        m_StartPos = new Vector3(data.StartX, data.StartZ, data.StartY);
        m_EndPos = new Vector3(data.EndX, data.EndZ, data.EndY);
        m_State = data.State;
        m_ModeType = data.ModeType;

        //数据处理
        m_Mode = (GameObject)(GameObject.Instantiate(Resources.Load(m_ModeType)));
        m_Mode.transform.parent = m_Scene.transform.parent;
        m_Mode.transform.position = m_Position;
        m_Mode.transform.LookAt(m_EndPos - m_Position);
        if (m_State == 1)
        {
            m_Mode.GetComponent<ProjectileScript>().ShowStartParticle();
        }else if(m_State == 3)
        {
            m_Mode.GetComponent<ProjectileScript>().ShowEndParticle();
        }

        Debug.Log("1111bullet pos:" + m_Mode.transform.position + "  end pos:" + m_EndPos + "   state:" + m_State);
    }
    public void Change(Protomsg.BulletDatas data)
    {
        // 更新位置
        if (m_Mode != null)
        {
            //Vector3 add = new Vector3(data.X, 0, data.Y);
            m_Position += new Vector3(data.X, data.Z, data.Y);
            m_StartPos += new Vector3(data.StartX, data.StartZ, data.StartY);
            m_EndPos += new Vector3(data.EndX, data.EndZ, data.EndY);
            m_State += data.State;

            //更新位置
            m_Mode.transform.position = m_Position;
            m_Mode.transform.LookAt(m_EndPos - m_Position);

            if(m_State == 3)
            {
                m_Mode.GetComponent<ProjectileScript>().ShowEndParticle();
            }

            Debug.Log("2222bullet pos:" + m_Mode.transform.position+"  end pos:"+ m_EndPos+"   state:"+m_State);
        }
    }
    public void ChangeShowPos(float scale,Vector3 next)
    {
        if (m_Mode != null)
        {
            m_Mode.transform.position = new Vector3(m_Position.x + (scale * next.x), m_Position.y + (scale * next.z), m_Position.z + (scale * next.y));
            //Debug.Log("3333bullet pos:" + m_Mode.transform.position + "  end pos:" + m_EndPos);
        }
    }

    public void Destroy()
    {
        if (m_Mode != null)
        {
            Debug.Log("4444bullet pos:" + m_Mode.transform.position);
            m_Mode.GetComponent<ProjectileScript>().Destroy();


        }
    }
}