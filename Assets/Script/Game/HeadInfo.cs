﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using cocosocket4unity;

public class HeadInfo{

    private GComponent maininfo;
    private int id;
    private GList Bufs;
    public HeadInfo(GComponent info)
    {
        maininfo = info;
        //id = unitid;
        Init();
        //FreshData(unitid);

        Bufs = maininfo.GetChild("buflist").asList;

    }


    public Dictionary<int, GComponent> BufsRes = new Dictionary<int, GComponent>();
    //刷新buf
    void FreshBuf()
    {
        UnityEntity mainunit = UnityEntityManager.Instance.GetUnityEntity(id);
        if (mainunit == null || mainunit.BuffDatas == null || mainunit.BuffDatas.Length <= 0)
        {
            BufsRes.Clear();
            Bufs.RemoveChildren();
            return;
        }

        foreach (var item in mainunit.BuffDatas)
        {
            if (BufsRes.ContainsKey(item.TypeID))
            {
                SetBufData(BufsRes[item.TypeID], item);
            }
            else
            {
                AddBuf(item);
            }
        }

        //删除多余的
        foreach (int key in new List<int>(BufsRes.Keys))
        {
            var isfind = false;
            foreach (var item1 in mainunit.BuffDatas)
            {
                if (key == item1.TypeID)
                {
                    isfind = true;
                    break;
                }
            }
            if (isfind == false)
            {
                RemoveBuf(key);
            }
        }
    }
    void RemoveBuf(int key)
    {
        if (BufsRes.ContainsKey(key))
        {
            Bufs.RemoveChild(BufsRes[key]);
            BufsRes.Remove(key);
        }

    }
    void AddBuf(Protomsg.BuffDatas data)
    {
        var clientskill = ExcelManager.Instance.GetBuffIM().GetBIByID(data.TypeID);
        if (clientskill == null || clientskill.IconPath.Length <= 0)
        {
            return;
        }

        GComponent view = UIPackage.CreateObject("GameUI", "buf_icon").asCom;
        view.scale = new Vector2(0.5f, 0.5f);
        Bufs.AddChild(view);
        BufsRes[data.TypeID] = view;
        SetBufData(view, data);
    }
    void SetBufData(GComponent obj, Protomsg.BuffDatas data)
    {
        //图标
        var clientskill = ExcelManager.Instance.GetBuffIM().GetBIByID(data.TypeID);
        if (clientskill != null)
        {
            if (clientskill.IconPath.Length > 0)
            {
                obj.GetChild("icon").asLoader.url = clientskill.IconPath;
            }

            if (clientskill.IconTimeEnable == 0)
            {
                obj.GetChild("pro").asProgress.value = 100;
            }
            else
            {
                obj.GetChild("pro").asProgress.value = (data.RemainTime / data.Time) * 100;
            }
        }


        if (data.TagNum >= 2)
        {
            obj.GetChild("count").asTextField.text = "" + data.TagNum;
        }
        else
        {
            obj.GetChild("count").asTextField.text = "";
        }


    }

    public void Init()
    {
        
        maininfo.GetChild("headbtn").asButton.onClick.Add(() =>
        {
            //玩家自己
            if (GameScene.Singleton.GetMyMainUnit().ID == id)
            {
                UnityEntity unit = UnityEntityManager.Instance.GetUnityEntity(id);
                if (unit == null)
                {
                    return;
                }
                MyInfo myinfo = new MyInfo(unit);
                
            }
            else
            {
                UnityEntity unit = UnityEntityManager.Instance.GetUnityEntity(id);
                if (unit == null)
                {
                    return;
                }
                MyInfo myinfo = new MyInfo(unit);
            }
        });

        //切换攻击模式
        maininfo.GetChild("attackmodebtn").asButton.onClick.Add(() => 
        {
            if (GameScene.Singleton.GetMyMainUnit().ID == id)
            {
                //CS_ChangeAttackMode
                Protomsg.CS_ChangeAttackMode msg1 = new Protomsg.CS_ChangeAttackMode();
                //攻击模式(1:和平模式 2:组队模式 3:全体模式 4:阵营模式(玩家,NPC) 
                if (GameScene.Singleton.GetMyMainUnit().AttackMode == 1)
                {
                    msg1.AttackMode = 3;
                }
                else
                {
                    msg1.AttackMode = 1;
                }
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_ChangeAttackMode", msg1);
            }
        });



    }

    public void FreshData(int unitid)
    {
        UnityEntity unit = UnityEntityManager.Instance.GetUnityEntity(unitid);
        if(unit == null)
        {
            maininfo.visible = false;
            return;
        }
        else
        {
            maininfo.visible = true;
        }

        if (id != unitid)
        {
            id = unitid;
            ////模型
            //var modeeffect = (GameObject)(GameObject.Instantiate(Resources.Load(unit.ModeType)));
            //modeeffect.transform.localPosition = new Vector3(0, 10, 1000);
            //modeeffect.transform.localScale = new Vector3(50, 50, 50);
            //Vector3 rotation = modeeffect.transform.localEulerAngles;
            //rotation.x = 10; // 在这里修改坐标轴的值
            //rotation.y = 180;
            //rotation.z = 0;
            ////将旋转的角度赋值给预制出来需要打出去的麻将
            //modeeffect.transform.localEulerAngles = rotation;
            //GGraph holder = maininfo.GetChild("head").asCom.GetChild("n1").asGraph;
            //GoWrapper wrapper = new GoWrapper();
            //wrapper.supportStencil = true;
            //wrapper.SetWrapTarget(modeeffect, true);
            //holder.SetNativeObject(wrapper);

            Debug.Log("---------unittype:" + unit.TypeID);

            var unitinfo = ExcelManager.Instance.GetUnitInfoManager().GetUnitInfoByID(unit.TypeID);
            if(unitinfo != null && unitinfo.IconPath.Length > 0)
            {
                Debug.Log("---------iconpath:" + unitinfo.IconPath);
                maininfo.GetChild("head").asCom.GetChild("head_icon").asLoader.url = unitinfo.IconPath;
            }
            else
            {
                maininfo.GetChild("head").asCom.GetChild("head_icon").asLoader.url = "";
            }

            
            //head_icon
        }
        //名字
        maininfo.GetChild("name").asTextField.text = unit.Name;
        //血量数字
        maininfo.GetChild("hp_num").asTextField.text = unit.HP+"/"+unit.MaxHP;
        //蓝量数字
        maininfo.GetChild("mp_num").asTextField.text = unit.MP + "/" + unit.MaxMP;
        //血量进度条
        maininfo.GetChild("hp").asProgress.value = (int)((float)unit.HP / unit.MaxHP * 100);
        //蓝量进度条
        maininfo.GetChild("mp").asProgress.value = (int)((float)unit.MP / unit.MaxMP * 100);
        //等级数字
        maininfo.GetChild("level").asTextField.text = unit.Level+"";

        //经验条
        maininfo.GetChild("experience").asProgress.value = (int)((float)unit.Experience / (float)unit.MaxExperience * 100);

        //攻击模式 //攻击模式(1:和平模式 2:组队模式 3:全体模式 4:阵营模式(玩家,NPC) 
        if (unit.AttackMode == 1)//防御
        {
            maininfo.GetChild("attackmodebtn").asButton.GetChild("word").asTextField.text = "防御";
            maininfo.GetChild("attackmodebtn").asButton.GetChild("word").asTextField.color = new Color(0,1,0.2f);
        }
        else if(unit.AttackMode == 3)//进攻
        {
            maininfo.GetChild("attackmodebtn").asButton.GetChild("word").asTextField.text = "进攻";
            maininfo.GetChild("attackmodebtn").asButton.GetChild("word").asTextField.color = new Color(1, 0, 0.2f);
        }

        FreshBuf();

    }
}
