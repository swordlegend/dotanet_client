﻿using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.Threading;
using System;

public class HeroSimpleInfo
{
    private GComponent main;
    
    public HeroSimpleInfo(int characterid)
    {
        //通过角色ID 获取角色信息
        MsgManager.Instance.AddListener("SC_GetCharacterSimpleInfo", new HandleMsg(this.SC_GetCharacterSimpleInfo));
       

        main = UIPackage.CreateObject("GameUI", "HeroInfo_Simple").asCom;
        GRoot.inst.AddChild(main);
        main.xy = Tool.GetPosition(0.5f, 0.5f);
        main.GetChild("close").asButton.onClick.Add(() =>
        {
            this.Destroy();
        });

        Protomsg.CS_GetCharacterSimpleInfo msg1 = new Protomsg.CS_GetCharacterSimpleInfo();
        msg1.CharacterID = characterid;
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetCharacterSimpleInfo", msg1);

    }

    //初始化技能信息
    public void InitSkillInfo(string skillstr)
    {
        
        var skilllist = main.GetChild("skill_list").asList;
        if (skilllist == null)
        {
            return;
        }
        skilllist.RemoveChildren();

        var skillstrarr = skillstr.Split(';');
        //排序
        foreach (var item in skillstrarr)
        {
            var itemstrarr = item.Split(',');
            if (itemstrarr.Length < 2)
            {
                continue;
            }
            var typeid = int.Parse(itemstrarr[0]);
            var level = int.Parse(itemstrarr[1]);

            var clientitem = ExcelManager.Instance.GetSkillManager().GetSkillByID(typeid);
            if (clientitem != null)
            {
                var onedropitem = UIPackage.CreateObject("GameUI", "HeroInfo_Skill").asButton;
                onedropitem.icon = clientitem.IconPath;

                onedropitem.GetChild("level").asTextField.text = "Lv." + level;

                if (level <= 0)
                {
                    onedropitem.asCom.alpha = 0.2f;
                }
                else
                {
                    onedropitem.asCom.alpha = 1;
                }

                onedropitem.onClick.Add(() => {
                    //Debug.Log("onClick");
                    if (clientitem.TypeID != -1)
                    {
                        new SkillInfo(clientitem.TypeID);
                    }
                });
                skilllist.AddChild(onedropitem);
            }
        }
    }


    public bool SC_GetCharacterSimpleInfo(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetCharacterSimpleInfo:");
        IMessage IMperson = new Protomsg.SC_GetCharacterSimpleInfo();
        Protomsg.SC_GetCharacterSimpleInfo p1 = (Protomsg.SC_GetCharacterSimpleInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        main.GetChild("name").asTextField.text = p1.Name;
        main.GetChild("level").asTextField.text = "lv." + p1.Level;
        main.GetChild("lastlogindate").asTextField.text = p1.LastLoginDate;

        //道具
        for (var j = 0; j < p1.EquipItems.Count; j++)
        {
            var item = main.GetChild("item" + (j + 1)).asButton;
            if(item == null)
            {
                continue;
            }
            var itemstrarr = p1.EquipItems[j].Split(',');
            if(itemstrarr.Length < 2)
            {
                item.GetChild("level").visible = false;
                continue;
            }
            var typeid = int.Parse(itemstrarr[0]);
            var level = int.Parse(itemstrarr[1]);

            var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(typeid);
            if (clientitem == null)
            {
                item.asButton.icon = "";
                item.asButton.GetChild("level").asTextField.text = "";
                continue;
            }
            item.asButton.icon = clientitem.IconPath;
            item.asButton.GetChild("level").asTextField.text = "Lv." + level;

            item.onClick.Add(() => {
               
                if (typeid != -1)
                {
                    new ItemInfo(typeid);
                }
            });
        }

        //技能
        InitSkillInfo(p1.Skills);



        //模型
        var modeeffect = (GameObject)(GameObject.Instantiate(Resources.Load(p1.ModeType)));
        modeeffect.transform.localPosition = new Vector3(0, 0, 100);
        var box = modeeffect.GetComponent<BoxCollider>();
        modeeffect.transform.localScale = new Vector3(100, 100, 100);
        if (box != null)
        {
            var scale = box.size.y / 1.2f;
            modeeffect.transform.localScale = new Vector3(100 / scale, 100 / scale, 100 / scale);
        }
        Vector3 rotation = modeeffect.transform.localEulerAngles;
        rotation.x = 10; // 在这里修改坐标轴的值
        rotation.y = 180;
        rotation.z = 0;
        //将旋转的角度赋值给预制
        modeeffect.transform.localEulerAngles = rotation;
        //var modeeffect = (GameObject)(GameObject.Instantiate(Resources.Load(p1.ModeType)));
        //modeeffect.transform.localPosition = new Vector3(0, 0, 100);
        GGraph holder = main.GetChild("heromode").asGraph;
        GoWrapper wrapper = new GoWrapper(modeeffect);
        holder.SetNativeObject(wrapper);
        holder.z = 10;

        return true;
    }





    //
    public void Destroy()
    {
        MsgManager.Instance.RemoveListener("SC_GetCharacterSimpleInfo");
        AudioManager.Am.Play2DSound(AudioManager.Sound_CloseUI);
        if (main != null)
        {
            main.Dispose();
        }
    }

    
}
