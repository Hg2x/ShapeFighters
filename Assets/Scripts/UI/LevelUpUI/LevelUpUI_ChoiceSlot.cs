using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelUpUI_ChoiceSlot : MonoBehaviour
{
    public Button Button { get; private set; }
    [SerializeField] private CommonItemIcon _ItemIcon;
    [SerializeField] private TextMeshProUGUI _TitleText;
    [SerializeField] private TextMeshProUGUI _SubTitleText;
    [SerializeField] private TextMeshProUGUI _BodyText;

    private void Awake()
    {
        Button = GetComponent<Button>();
    }

    public void LoadWeapon(int weaponID)
    {
        _ItemIcon.LoadWeaponImage(weaponID);
        _TitleText.text = FunctionLibrary.GetWeaponName(weaponID);
        _SubTitleText.text = FunctionLibrary.GetWeaponName(weaponID);
        _BodyText.text = "Placeholder";
    }
}
