using UnityEngine;

[CreateAssetMenu(fileName = "UIManagerData", menuName = "ScriptableObject/UIManagerData", order = 0)]
public class UIManagerData : ScriptableObject
{
    [SerializeField] public UIBase StartingUI;
    [SerializeField] public UIBase[] UICollection;
}
