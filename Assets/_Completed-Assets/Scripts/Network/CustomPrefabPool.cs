using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class CustomPrefabPool : IPunPrefabPool
{
    private readonly Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

    // プレハブを登録する
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

    // プレハブを生成する際に呼ばれる
    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        if (prefabDictionary.TryGetValue(prefabId, out GameObject prefab))
        {
            return Object.Instantiate(prefab, position, rotation);
        }

        Debug.LogError($"Prefab with name {prefabId} not found in CustomPrefabPool.");
        return null;
    }

    // プレハブを削除する際に呼ばれる
    public void Destroy(GameObject gameObject)
    {
        Object.Destroy(gameObject);
    }
}
