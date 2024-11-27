using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class CustomPrefabPool : IPunPrefabPool
{
    private readonly Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

    // �v���n�u��o�^���郁�\�b�h
    public void RegisterPrefab(string prefabName, GameObject prefab)
    {
        if (!prefabDictionary.ContainsKey(prefabName))
        {
            prefabDictionary.Add(prefabName, prefab);
            Debug.Log($"Prefab '{prefabName}' registered successfully.");
        }
        else
        {
            Debug.LogWarning($"Prefab with name '{prefabName}' is already registered.");
        }
    }

    // �v���n�u���C���X�^���X�����郁�\�b�h
    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        if (prefabDictionary.TryGetValue(prefabId, out GameObject prefab))
        {
            GameObject obj = Object.Instantiate(prefab, position, rotation);
            obj.SetActive(false); // ��A�N�e�B�u�ɐݒ�
            Debug.Log($"CustomPrefabPool instantiated '{prefabId}' at {position}, set inactive: {obj.activeSelf}");
            return obj;
        }

        Debug.LogError($"Prefab with name '{prefabId}' not found in CustomPrefabPool.");
        return null;
    }

    // �v���n�u��j�����郁�\�b�h
    public void Destroy(GameObject gameObject)
    {
        Object.Destroy(gameObject);
    }
}
