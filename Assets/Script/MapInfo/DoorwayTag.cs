﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorwayTag : MonoBehaviour {
    public int NextSceneID = 1; //传送到下个场景的ID
    public int NeedLevel = 1; //传送门需要的最低等级
    public float R = 2;//传送门半径
    public Vector2 NextScenePosition = new Vector2(10, 10);//传送到下个场景的位置
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}