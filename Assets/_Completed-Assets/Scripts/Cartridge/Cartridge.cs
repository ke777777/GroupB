using System.Collections;
using UnityEngine;

public class Cartridge : MonoBehaviour
{
    public float lifetime = 5f;           // 消滅までの時間
    public float blinkStartTime = 3f;    // 明滅を開始する時間
    public float blinkInterval = 0.2f;   // 明滅の間隔

    private Renderer m_Renderer;         // Rendererコンポーネント

    private void Start()
    {
        // Rendererコンポーネントを取得
        // m_Renderer = GetComponent<Renderer>();

        // if (m_Renderer == null)
        // {
        //     Debug.LogError("Renderer component is missing on this GameObject.");
        //     return; // 処理を中断
        // }
        m_Renderer = GetComponent<Renderer>();
        if (m_Renderer != null)
        {
            m_Renderer.enabled = true; // レンダラーを有効にする
        }
        else
        {
            Debug.LogWarning("The cartridge prefab does not have a Renderer attached.");
        }

        // 明滅と消滅の処理をコルーチンで実行
        StartCoroutine(BlinkAndDestroy());
    }

    private IEnumerator BlinkAndDestroy()
    {
        float startTime = Time.time; // 開始時の経過時間
        bool isVisible = true; // 表示状態

        // 明滅を開始するまで待機
        yield return new WaitForSeconds(blinkStartTime);

        // 明滅処理
        while (Time.time < startTime + lifetime)
        {
            if (m_Renderer != null)
            {
                isVisible = !isVisible;
                m_Renderer.enabled = isVisible; // 状態が変わる時だけ更新
            }
            yield return new WaitForSeconds(blinkInterval);
        }

        // 明滅終了後に非表示に設定
        if (m_Renderer != null)
        {
            m_Renderer.enabled = false;
        }

        // 最後にオブジェクトを消去
        Destroy(gameObject);
    }
}
