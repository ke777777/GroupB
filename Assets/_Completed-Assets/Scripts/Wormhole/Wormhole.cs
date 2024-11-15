using UnityEngine;
using Complete;
using System.Collections.Generic;

public class Wormhole : MonoBehaviour
{
    public Wormhole linkedGate; // 対応するゲート
    public float teleportDuration = 2f; // ワームホール通過時間
    private bool isTeleporting = false; // テレポート中かを判定
    private float cooldownDuration = 4f; // ワープ後のクールダウン時間
    private HashSet<TankMovement> cooldownTanks = new HashSet<TankMovement>(); // クールダウン中の戦車を追跡
    private void OnTriggerEnter(Collider other)
    {
        if (isTeleporting || linkedGate == null) return;

        if (other.CompareTag("Player"))
        {
            TankMovement tank = other.GetComponent<TankMovement>();

            if (tank != null && !tank.isCooldown && !cooldownTanks.Contains(tank))
            {
                StartCoroutine(TeleportTank(tank));
            }
        }
        else if (other.CompareTag("Shell"))
        {
            Destroy(other.gameObject); // 砲弾は通過できない
        }
    }

    private System.Collections.IEnumerator TeleportTank(TankMovement tank)
    {
        if (tank == null) yield break;

        // テレポート中フラグを設定
        isTeleporting = true;
        linkedGate.isTeleporting = true;

        // Rigidbody を無効化
        Rigidbody tankRigidbody = tank.GetComponent<Rigidbody>();
        if (tankRigidbody != null)
        {   tankRigidbody.isKinematic = false;
            tankRigidbody.velocity = Vector3.zero;
            tankRigidbody.angularVelocity = Vector3.zero;
            tankRigidbody.isKinematic = true;
        }

        // 戦車を無敵状態にし、行動を停止
        tank.SetInvincible(true);
        tank.DisableActions();
        tank.StartBlinking();

        // ワープの待機時間
        yield return new WaitForSeconds(teleportDuration);

        // ワープ先の位置を取得
        Vector3 targetPosition = linkedGate.transform.position;

        // 地形の高さを調整
        if (Physics.Raycast(targetPosition + Vector3.up * 10f, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            targetPosition = hit.point; // 地形の表面に位置を合わせる
        }

        // 戦車の位置をワープ先に設定
        tank.transform.position = targetPosition;

        //  少し待機して位置を安定させる
        yield return new WaitForSeconds(0.1f);

        // Rigidbody を再有効化
        if (tankRigidbody != null)
        {
            tankRigidbody.isKinematic = false; // 物理演算を再開
        }

        // 無敵状態と行動制限を解除
        tank.StopBlinking();
        tank.SetInvincible(false);
        tank.EnableActions();

        // テレポート中フラグを解除
        isTeleporting = false;
        linkedGate.isTeleporting = false;yield return new WaitForSeconds(cooldownDuration);
        cooldownTanks.Remove(tank);
    }
}