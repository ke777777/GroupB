using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class CustomPrefabPool : IPunPrefabPool
{
    private readonly Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

    // �v���n�u��o�^����
    public void RegisterPrefab(string prefabName, GameObject prefab)
    {
        if (!prefabDictionary.ContainsKey(prefabName))
        {
            prefabDictionary.Add(prefabName, prefab);
        }
        else
        {
            Debug.LogWarning($"Prefab with name {prefabName} is already registered.");
        }
    }

    // �v���n�u�𐶐�����ۂɌĂ΂��
    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        if (prefabDictionary.TryGetValue(prefabId, out GameObject prefab))
        {
            return Object.Instantiate(prefab, position, rotation);
        }

        Debug.LogError($"Prefab with name {prefabId} not found in CustomPrefabPool.");
        return null;
    }

    // �v���n�u���폜����ۂɌĂ΂��
    public void Destroy(GameObject gameObject)
    {
        Object.Destroy(gameObject);
    }
}
