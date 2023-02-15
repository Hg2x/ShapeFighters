using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//using UnityEngine.AddressableAssets;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ActiveSkillIcon : MonoBehaviour
{
    public Button Button { get; protected set; }
    [SerializeField] protected Image _Icon;
    [SerializeField] protected Image _CooldownMask;
    [SerializeField] protected TextMeshProUGUI _CooldownText;

    private void Awake()
    {
        Button = gameObject.GetComponent<Button>();
    }

    public void Init()
    {
        GameInstance.GetWeaponManager().OnWeaponSwitched += ChangeSkillIcon;
    }

    private void ChangeSkillIcon(int firstWeaponSlot, int secondWeaponSlot)
    {
        if (firstWeaponSlot == 0)
        {

        }
        else if (secondWeaponSlot == 0)
        {

        }

        //var filePath = "Assets/Sprite/ItemIcon/" + FunctionLibrary.GetWeaponIDString(weaponID) + ".png";
        //var request = Addressables.LoadAssetAsync<Sprite>(filePath);
        //request.Completed += op =>
        //{
        //    var sprite = op.Result;
        //    _Icon.sprite = sprite;
        //};
    }
}
