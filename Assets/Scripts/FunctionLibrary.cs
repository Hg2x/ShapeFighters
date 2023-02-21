using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class FunctionLibrary : MonoBehaviour
{
    public static void SwapElements(MonoBehaviour[] obj, int index1, int index2)
    {
        if (index1 < 0 || index1 >= obj.Length || index2 < 0 || index2 >= obj.Length)
        {
            Debug.LogError("FunctionLibrary SwapElements Invalid indices");
            return;
        }

        if (index1 == index2)
        {
            Debug.LogError("FunctionLibrary SwapElements same indices");
            return;
        }

        if (obj[index1] == null && obj[index2] == null)
        {
            Debug.LogError("FunctionLibrary SwapElements Both elements are null");
            return;
        }

        (obj[index2], obj[index1]) = (obj[index1], obj[index2]);
    }

    public static int[] GetRandomNumbers(int amount, int min, int max, bool allowRepeats = false)
    {
        int[] numbers = new int[max - min + 1];
        for (int i = 0; i < numbers.Length; i++)
        {
            numbers[i] = min + i;
        }

        int[] result = new int[amount];
        if (allowRepeats)
        {
            for (int i = 0; i < amount; i++)
            {
                int index = (int)(UnityEngine.Random.value * (max - min + 1));
                result[i] = numbers[index];
            }
        }
        else
        {
            for (int i = 0; i < amount; i++)
            {
                int index = (int)(UnityEngine.Random.value * (max - min - i + 1)) + min + i;
                result[i] = numbers[index];
                numbers[index] = numbers[min + i];
            }
        }

        return result;
    }

    public static void FillZeroMemberFields<T>(T[] array)
    {
        T tempData = array[0];
        for (int i = 0; i < array.Length; i++)
        {
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                object currentValue = field.GetValue(array[i]);
                object tempValue = field.GetValue(tempData);
                if ((currentValue is int notZero && notZero != 0) || (currentValue is float nonZero && nonZero != 0f))
                {
                    field.SetValue(tempData, currentValue);
                }
                else if ((tempValue is int notZero2 && notZero2 != 0) || (tempValue is float nonZero2 && nonZero2 != 0f))
                {
                    field.SetValue(array[i], tempValue);
                }
            }
        }
    }

    public static T TryGetAssetSync<T>(string addressablePathString) where T : UnityEngine.Object
    {
        if (!string.IsNullOrEmpty(addressablePathString))
        {
            var request = Addressables.LoadAssetAsync<T>(addressablePathString);
            T asset = request.WaitForCompletion();
            Addressables.Release(request);
            return asset;
        }
        return null;
    }

    // Functions below are specific to this project

    public static string GetWeaponIDString(int weaponID)
    {
        return weaponID > 0 ? "W" + weaponID.ToString() : "None";
    }

    public static string GetWeaponName(int weaponID) // TODO: implement a better way to get weapon name
    {
        return weaponID switch
        {
            1 => "Sphere",
            2 => "Cube",
            3 => "Cone",
            4 => "Cylinder",
            _ => "None"
        };
    }
}


// Editor/Inspector related
[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
public class ReadOnlyFieldAttribute : PropertyAttribute
{

}

[CustomPropertyDrawer(typeof(ReadOnlyFieldAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
