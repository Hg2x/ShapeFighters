using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "ScriptableObject/StageData", order = 0)]
public class StageData : ScriptableObject
{
    public PlayerUnit PlayerUnitReference; // TODO: check this later
    public UnitStatusData PlayerStatusData;

    [Header("Spawner data")]
    public EnemyUnit _EnemyRef;
    public Vector3 _SpawnArea = new(100, 1, 100);
}
