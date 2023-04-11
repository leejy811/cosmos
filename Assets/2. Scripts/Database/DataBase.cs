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
	public List<PartsDBEntity> Missile;
	public List<PartsDBEntity> Barrier;
	public List<PartsDBEntity> Laser;
	public List<PartsDBEntity> Emp;
}
