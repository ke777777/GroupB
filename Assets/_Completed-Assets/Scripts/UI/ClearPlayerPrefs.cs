using UnityEngine;

public class ClearPlayerPrefs : MonoBehaviour
{
    void Start()
    {
        PlayerPrefs.DeleteAll(); // ���ׂĂ�PlayerPrefs���폜
        PlayerPrefs.Save(); // �ύX��ۑ�
        Debug.Log("PlayerPrefs have been cleared!");
    }
}
