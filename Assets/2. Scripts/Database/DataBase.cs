using DataBaseEntity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class DataBase : ScriptableObject
{
	public List<AchieveDBEntity> Achieves;
	public List<WaveDBEntity> Waves;
	public List<MissileDBEntity> Missile;
	public List<BarrierDBEntity> Barrier;
	public List<LaserDBEntity> Laser;
	public List<EmpDBEntity> Emp;
}
