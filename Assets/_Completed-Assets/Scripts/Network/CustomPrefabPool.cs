using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class CustomPrefabPool : IPunPrefabPool
{
    private readonly Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

    // プレハブを登録するメソッド
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

    // プレハブをインスタンス化するメソッド
    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        if (prefabDictionary.TryGetValue(prefabId, out GameObject prefab))
        {
            GameObject obj = Object.Instantiate(prefab, position, rotation);
            Debug.Log($"CustomPrefabPool instantiated '{prefabId}' at {position}, set inactive: {obj.activeSelf}");
            return obj;
        }

        Debug.LogError($"Prefab with name '{prefabId}' not found in CustomPrefabPool.");
        return null;
    }

    // プレハブを破棄するメソッド
    public void Destroy(GameObject gameObject)
    {
        Object.Destroy(gameObject);
    }

    // プレハブが登録されているか確認するメソッド
    public bool ContainsPrefab(string prefabName)
    {
        return prefabDictionary.ContainsKey(prefabName);
    }
}
