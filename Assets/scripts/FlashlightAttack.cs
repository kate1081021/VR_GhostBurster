using UnityEngine;
using OVR;

public class FlashlightAttack : MonoBehaviour
{
    [Header("1. 基本設定")]
    public OVRInput.Controller controllerType = OVRInput.Controller.RTouch;

    [Header("2. ライト設定 (Spot Light)")]
    public Light mainLight;
    public float attackIntensity = 10.0f;
    public float attackSpotAngle = 15.0f;
    public float attackRange = 20.0f;

    [Header("3. ビームVFX設定 (見た目)")]
    [Tooltip("攻撃時に表示する『攻撃ビーム』(パーティクルなど)")]
    public GameObject attackBeamVFX;
    [Tooltip("攻撃時に表示する『攻撃用メッシュ』(ビームの形)")]
    public GameObject attackConeVisual;
    [Tooltip("通常時に表示する『サーチ光』")]
    public GameObject searchBeamVisual;

    [Header("4. オーディオ設定")]
    [Tooltip("攻撃開始時に1回だけ鳴らす『起動音』")]
    public AudioSource attackStartAudio;

    [Header("5. 振動（ハプティクス）設定")]
    [Range(0, 1)]
    public float vibrationFrequency = 0.5f;
    [Range(0, 1)]
    public float vibrationAmplitude = 0.2f;

    [Header("6. 当たり判定")]
    [Tooltip("ゴーストを倒すための当たり判定オブジェクト（lightcollider）")]
    public GameObject attackHitboxObject;

    // --- 内部変数 ---
    private float normalIntensity;
    private float normalSpotAngle;
    private float normalRange;
    private bool isAttacking = false;
    private ParticleSystem attackParticles;

    void Start()
    {
        // 1. 通常時のライト設定を記憶
        if (mainLight != null)
        {
            normalIntensity = mainLight.intensity;
            normalSpotAngle = mainLight.spotAngle;
            normalRange = mainLight.range;
        }

        // 2. VFXの初期状態を設定
        if (attackBeamVFX != null)
        {
            attackParticles = attackBeamVFX.GetComponentInChildren<ParticleSystem>();
            attackBeamVFX.SetActive(true); // 親はオンのまま
        }
        if (attackConeVisual != null) attackConeVisual.SetActive(false);
        if (searchBeamVisual != null) searchBeamVisual.SetActive(true);

        // 3. オーディオ設定
        if (attackStartAudio != null)
        {
            attackStartAudio.loop = false;
            attackStartAudio.playOnAwake = false;
        }

        // 4. (★重要) 当たり判定を最初はオフにしておく
        if (attackHitboxObject != null)
        {
            attackHitboxObject.SetActive(false); // enabled = false ではなく SetActive(false)
        }
    }

    void Update()
    {
        // トリガーが「押された瞬間」
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controllerType))
        {
            StartAttack();
        }
        // トリガーが「離された瞬間」
        else if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, controllerType))
        {
            StopAttack();
        }
    }

    private void StartAttack()
    {
        if (isAttacking) return;
        isAttacking = true;

        // 1. ライト変更
        if (mainLight != null)
        {
            mainLight.intensity = attackIntensity;
            mainLight.spotAngle = attackSpotAngle;
            mainLight.range = attackRange;
        }

        // 2. 見た目変更
        if (attackParticles != null)
        {
            attackParticles.Stop(); // リセット
            attackParticles.Play(); // 再生
        }
        if (attackConeVisual != null) attackConeVisual.SetActive(true);
        if (searchBeamVisual != null) searchBeamVisual.SetActive(false);

        // 3. オーディオ再生
        if (attackStartAudio != null) attackStartAudio.Play();

        // 4. 振動
        OVRInput.SetControllerVibration(vibrationFrequency, vibrationAmplitude, controllerType);

        // 5. (★重要) 当たり判定をオンにする
        if (attackHitboxObject != null)
        {
            attackHitboxObject.SetActive(true); // enabled = true ではなく SetActive(true)
        }
    }

    private void StopAttack()
    {
        if (!isAttacking) return;
        isAttacking = false;

        // 1. ライト戻す
        if (mainLight != null)
        {
            mainLight.intensity = normalIntensity;
            mainLight.spotAngle = normalSpotAngle;
            mainLight.range = normalRange;
        }

        // 2. 見た目戻す
        if (attackConeVisual != null) attackConeVisual.SetActive(false);
        if (searchBeamVisual != null) searchBeamVisual.SetActive(true);

        // 4. 振動停止
        OVRInput.SetControllerVibration(0, 0, controllerType);

        // 5. (★重要) 当たり判定をオフにする
        if (attackHitboxObject != null)
        {
            attackHitboxObject.SetActive(false); // enabled = false ではなく SetActive(false)
        }
    }

    void OnDestroy()
    {
        OVRInput.SetControllerVibration(0, 0, controllerType);
    }

    // (FlashlightHit.cs が使う可能性のある関数 - 削除しないでください)
    public bool IsAttacking()
    {
        return isAttacking;
    }
}