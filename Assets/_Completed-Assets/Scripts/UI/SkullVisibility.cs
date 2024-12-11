using Photon.Pun;
using UnityEngine;

public class SkullVisibility : MonoBehaviour
{
    [SerializeField] private GameObject skull; // Skullオブジェクトをアサイン

    void Start()
    {
        // 自分の所有物でなければSkullを非表示
        if (!PhotonView.Get(this).IsMine)
        {
            skull.SetActive(false);
        }
    }
}
