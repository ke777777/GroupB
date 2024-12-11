using Photon.Pun;
using UnityEngine;

public class SkullVisibility : MonoBehaviour
{
    [SerializeField] private GameObject skull; // Skull�I�u�W�F�N�g���A�T�C��

    void Start()
    {
        // �����̏��L���łȂ����Skull���\��
        if (!PhotonView.Get(this).IsMine)
        {
            skull.SetActive(false);
        }
    }
}
