using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class PhotonPoolInitializer : MonoBehaviour
{
    [System.Serializable]
    public class PrefabEntry
    {
        public string prefabName; // PhotonNetwork.Instantiate �Ŏg�p���閼�O
        public GameObject prefab; // ���ۂ̃v���n�u
    }

    [Header("Prefab Entries")]
    public List<PrefabEntry> prefabEntries = new List<PrefabEntry>();

    private void Awake()
    {
        // �J�X�^���v�[���̏�����
        CustomPrefabPool customPrefabPool = new CustomPrefabPool();
        PhotonNetwork.PrefabPool = customPrefabPool;

        // �e�v���n�u���J�X�^���v�[���ɓo�^
        foreach (var entry in prefabEntries)
        {
            if (entry.prefab != null && !string.IsNullOrEmpty(entry.prefabName))
            {
                customPrefabPool.RegisterPrefab(entry.prefabName, entry.prefab);
            }
        }
    }
}
