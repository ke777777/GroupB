using UnityEngine;

public class ClearPlayerPrefs : MonoBehaviour
{
    void Start()
    {
        PlayerPrefs.DeleteAll(); // ‚·‚×‚Ä‚ÌPlayerPrefs‚ğíœ
        PlayerPrefs.Save(); // •ÏX‚ğ•Û‘¶
        Debug.Log("PlayerPrefs have been cleared!");
    }
}
