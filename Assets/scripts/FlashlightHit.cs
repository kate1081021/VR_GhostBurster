using UnityEngine;

// このスクリプトは「当たり判定」オブジェクト（lightcollider）にアタッチします
public class FlashlightHit : MonoBehaviour
{
    // 当たり判定に「入り始めた」時
    void OnTriggerEnter(Collider other)
    {
        // 相手が "Enemy" タグなら
        if (other.CompareTag("Enemy"))
        {
            // まず、Enemy.cs (本編の敵) を探す
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Enemy.cs があれば、OnLightEnter を呼ぶ
                enemy.OnLightEnter();
            }
            else
            {
                // もし Enemy.cs がなければ、TrialGhost.cs (お試し用) を探す
                TrialGhost trialGhost = other.GetComponent<TrialGhost>();
                if (trialGhost != null)
                {
                    // TrialGhost.cs があれば、OnLightEnter を呼ぶ
                    trialGhost.OnLightEnter();
                }
            }
        }
    }

    // 当たり判定から「離れた」時
    void OnTriggerExit(Collider other)
    {
        // 相手が "Enemy" タグなら
        if (other.CompareTag("Enemy"))
        {
            // Enemy.cs を探す
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Enemy.cs があれば、OnLightExit を呼ぶ
                enemy.OnLightExit();
            }
            else
            {
                // もし Enemy.cs がなければ、TrialGhost.cs (お試し用) を探す
                TrialGhost trialGhost = other.GetComponent<TrialGhost>();
                if (trialGhost != null)
                {
                    // TrialGhost.cs があれば、OnLightExit を呼ぶ
                    trialGhost.OnLightExit();
                }
            }
        }
    }
}