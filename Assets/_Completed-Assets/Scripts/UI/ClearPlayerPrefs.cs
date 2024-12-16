using UnityEngine;

public class ClearPlayerPrefs : MonoBehaviour
{
    void Start()
    {
        PlayerPrefs.DeleteAll(); // すべてのPlayerPrefsを削除
        PlayerPrefs.Save(); // 変更を保存
        Debug.Log("PlayerPrefs have been cleared!");
    }
}
