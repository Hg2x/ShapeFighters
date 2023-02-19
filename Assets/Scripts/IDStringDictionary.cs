using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IDStringDictionary", menuName = "ScriptableObject/Dictionary/IDStringDictionary", order = 0)]
public class IDStringDictionary : ScriptableObject, ISerializationCallbackReceiver, IEnumerable<KeyValuePair<int, string>>, IEnumerable
{
    [SerializeField] protected List<int> _ID = new();
    [SerializeField] protected List<string> _String = new();

    protected Dictionary<int, string> _Dictionary = new();

    public IReadOnlyDictionary<int, string> GetDictionary()
    {
        return _Dictionary;
    }

    public IEnumerator<KeyValuePair<int, string>> GetEnumerator()
    {
        return _Dictionary.GetEnumerator();
    }

    public void OnAfterDeserialize()
    {
        _Dictionary = new Dictionary<int, string>();

        for (int i = 0; i != Math.Min(_ID.Count, _String.Count); i++)
        {
            _Dictionary.Add(_ID[i], _String[i]);
        }
    }

    public void OnBeforeSerialize() { }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
