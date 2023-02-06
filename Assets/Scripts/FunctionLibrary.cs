using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                int index = (int)(Random.value * (max - min + 1));
                result[i] = numbers[index];
            }
        }
        else
        {
            for (int i = 0; i < amount; i++)
            {
                int index = (int)(Random.value * (max - min - i + 1)) + min + i;
                result[i] = numbers[index];
                numbers[index] = numbers[min + i];
            }
        }

        return result;
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
