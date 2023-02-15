using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffBase : MonoBehaviour
{
    // modify stats
    // does stack?
    // duration
    // per row -> modifierString, float modifierValue
    // extra effect string
    
    // TODO: this

    protected void On()
    {
        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("AttackModifier", 0.2f, "+");
        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("DefenseModifier", 0.2f, "+");
        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("AttackSpeedModifier", 0.2f, "+");
        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("MoveSpeed", 1f, "+");
    }

    protected void Off()
    {
        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("AttackModifier", -0.2f, "+");
        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("DefenseModifier", -0.2f, "+");
        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("AttackSpeedModifier", -0.2f, "+");
        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("MoveSpeed", -1f, "+");
    }
}
