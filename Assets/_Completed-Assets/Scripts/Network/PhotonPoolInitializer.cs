using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class PhotonPoolInitializer : MonoBehaviour
{
    [System.Serializable]
    public class PrefabEntry
    {
        public string prefabName; // PhotonNetwork.Instantiate で使用する名前
        public GameObject prefab; // 実際のプレハブ
    }

    [Header("Prefab Entries")]
    public List<PrefabEntry> prefabEntries = new List<PrefabEntry>();

    private void Awake()
    {
        // カスタムプールの初期化
        CustomPrefabPool customPrefabPool = new CustomPrefabPool();
        PhotonNetwork.PrefabPool = customPrefabPool;

        // 各プレハブをカスタムプールに登録
        foreach (var entry in prefabEntries)
        {
            if (entry.prefab != null && !string.IsNullOrEmpty(entry.prefabName))
            {
                customPrefabPool.RegisterPrefab(entry.prefabName, entry.prefab);
            }
        }
    }
}
