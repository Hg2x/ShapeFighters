using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

[RequireComponent(typeof(Button))]
public class CommonItemIcon : MonoBehaviour
{
    public Button Button { get; protected set; }
    [SerializeField] protected Image _Icon;
    [SerializeField] protected Image _Frame;
    [SerializeField] protected Image _SelectedMask;

    private void Awake()
    {
        Button = gameObject.GetComponent<Button>();
    }

    public void Selected()
    {
        _SelectedMask.gameObject.SetActive(true);
    }

    public void Unselected()
    {
        _SelectedMask.gameObject.SetActive(false);
    }

    public void LoadWeaponImage(int weaponID)
    {
        var filePath = "Assets/Sprite/ItemIcon/" + FunctionLibrary.GetWeaponIDString(weaponID) + ".png";
        var request = Addressables.LoadAssetAsync<Sprite>(filePath);
        request.Completed += op =>
        {
            var sprite = op.Result;
            _Icon.sprite = sprite;
        };
    }
}
