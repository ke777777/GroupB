using Photon.Pun;
using UnityEngine;

public class CustomPrefabPool : IPunPrefabPool
{
    // Instantiate: �v���n�u�𓮓I�Ƀ��[�h���ăC���X�^���X��
    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabId); // Resources �t�H���_����Prefab�����[�h
        if (prefab != null)
        {
            GameObject obj = Object.Instantiate(prefab, position, rotation);
            obj.SetActive(false); // Photon �͏�����ԂŔ�A�N�e�B�u�����҂���
            return obj;
        }

        Debug.LogError($"Prefab '{prefabId}' not found in Resources.");
        return null;
    }

    // Destroy: �I�u�W�F�N�g��j��
    public void Destroy(GameObject gameObject)
    {
        Object.Destroy(gameObject);
    }
}
