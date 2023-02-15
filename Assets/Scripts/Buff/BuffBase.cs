using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "ScriptableObject/Buff", order = 0)]
public class BuffBase : ScriptableObject, ISerializationCallbackReceiver
{
    // TODO: this
    // maybe create a dictionary for all buff ID?

    public bool IsBuffApplied { get; protected set; }
    [SerializeField] protected int _ID;
    [SerializeField] protected float _Duration;
    [SerializeField] protected bool _IsStackable;
    [SerializeField] protected bool _IsPermanent;
    [SerializeField] protected List<string> _ValueModifierKeys = new();
    [SerializeField] protected List<float> _ValueModifierValues = new();
    [SerializeField] protected List<string> _StatusModifiers = new();

    protected float _DurationLeft;
    protected Dictionary<string, float> _ValueModifiers = new();

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        _ValueModifiers = new Dictionary<string, float>();

        for (int i = 0; i != Math.Min(_ValueModifierKeys.Count, _ValueModifierValues.Count); i++)
        {
            _ValueModifiers.Add(_ValueModifierKeys[i], _ValueModifierValues[i]);
        }
    }

    public void Update()
    {
        if (!_IsPermanent)
        {
            _DurationLeft -= Time.deltaTime;

            if (_DurationLeft <= 0f)
            {
                RemoveBuff();
            }
        }
    }

    public void ApplyBuff()
    {
        if (IsBuffApplied && !_IsStackable)
        {
            return;
        }

        foreach (var valueMod in _ValueModifiers)
        {
            GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable(valueMod.Key, valueMod.Value, "+");
        }

        // if stackable, add stacks
        _DurationLeft = _Duration;
        IsBuffApplied = true;
        Debug.Log("buff applied");
    }

    public void RemoveBuff()
    {
        if (!IsBuffApplied)
        {
            return;
        }

        foreach (var valueMod in _ValueModifiers)
        {
            GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable(valueMod.Key, -valueMod.Value, "+");
        }

        // if no stacks and IsStackable as well, else remove stacks only
        _DurationLeft = 0f;
        IsBuffApplied = false;
        Debug.Log("buff removed");
    }
}
