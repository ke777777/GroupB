using Photon.Pun;
using UnityEngine;

public class CustomPrefabPool : IPunPrefabPool
{
    // Instantiate: プレハブを動的にロードしてインスタンス化
    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabId); // Resources フォルダからPrefabをロード
        if (prefab != null)
        {
            GameObject obj = Object.Instantiate(prefab, position, rotation);
            obj.SetActive(false); // Photon は初期状態で非アクティブを期待する
            return obj;
        }

        Debug.LogError($"Prefab '{prefabId}' not found in Resources.");
        return null;
    }

    // Destroy: オブジェクトを破棄
    public void Destroy(GameObject gameObject)
    {
        Object.Destroy(gameObject);
    }
}
