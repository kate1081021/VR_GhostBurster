using UnityEngine;
using UnityEngine.UI;
using OVR;

public class GuidanceManager : MonoBehaviour
{
    private bool isGuiding = false;
    private float timer = 0f;

    [Header("必須アサイン")]
    public GameObject Milestone;
    public Transform playerCamera;

    [Header("UI設定")]
    public Image leftGlowImage;
    public Image rightGlowImage;
    [SerializeField] private float glowSpeed = 5f;
    [SerializeField] private float maxGlowAlpha = 0.6f;

    [Header("パラメータ")]
    public float autoStopDuration = 5.0f;
    [SerializeField] private float thresholdAngle = 8.0f;
    [SerializeField] private float behindAngle = 150.0f;

    void Update()
    {
        if (!isGuiding)
        {
            StopVibration();
            UpdateAllGlows(0f, 0f); // 非稼働時は消灯
            return;
        }

        timer += Time.deltaTime;
        if (timer >= autoStopDuration || Milestone == null || playerCamera == null)
        {
            StopGuidance();
            return;
        }

        Vector3 targetDir = Vector3.ProjectOnPlane(Milestone.transform.position - playerCamera.position, Vector3.up);
        Vector3 forward = Vector3.ProjectOnPlane(playerCamera.forward, Vector3.up);

        float angle = Vector3.SignedAngle(forward, targetDir, Vector3.up);
        float absAngle = Mathf.Abs(angle);

        // 目標とする透明度を定義
        float targetAlphaL = 0f;
        float targetAlphaR = 0f;

        // --- 判定ロジック ---
        if (absAngle > behindAngle)
        {
            // 真後ろ：両手振動 ＋ 両サイド発光
            VibrateBoth();
            targetAlphaL = maxGlowAlpha;
            targetAlphaR = maxGlowAlpha;
        }
        else if (angle > thresholdAngle)
        {
            // 右：右手振動 ＋ 右サイド発光
            VibrateSingle(OVRInput.Controller.RTouch);
            targetAlphaR = maxGlowAlpha;
        }
        else if (angle < -thresholdAngle)
        {
            // 左：左手振動 ＋ 左サイド発光
            VibrateSingle(OVRInput.Controller.LTouch);
            targetAlphaL = maxGlowAlpha;
        }
        else
        {
            // 正面：停止
            StopGuidance();
            return;
        }

        // UIの更新（滑らかなフェード）
        UpdateAllGlows(targetAlphaL, targetAlphaR);
    }

    void UpdateAllGlows(float targetL, float targetR)
    {
        ApplyGlow(leftGlowImage, targetL);
        ApplyGlow(rightGlowImage, targetR);
    }

    void ApplyGlow(Image img, float targetAlpha)
    {
        if (img == null) return;
        Color c = img.color;
        // 線形補間: A_new = Lerp(A_current, A_target, dt * speed)
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * glowSpeed);
        img.color = c;
    }

    // --- 振動・制御ロジック ---
    void VibrateBoth()
    {
        OVRInput.SetControllerVibration(1.0f, 1.0f, OVRInput.Controller.LTouch);
        OVRInput.SetControllerVibration(1.0f, 1.0f, OVRInput.Controller.RTouch);
    }

    void VibrateSingle(OVRInput.Controller activeController)
    {
        OVRInput.SetControllerVibration(0.6f, 0.8f, activeController);
        var other = (activeController == OVRInput.Controller.RTouch) ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;
        OVRInput.SetControllerVibration(0, 0, other);
    }

    public void StartGuidance()
    {
        if (isGuiding) return;
        isGuiding = true;
        timer = 0f;
        OVRInput.SetControllerVibration(1f, 1f, OVRInput.Controller.LTouch);
        OVRInput.SetControllerVibration(1f, 1f, OVRInput.Controller.RTouch);
        Invoke(nameof(StopVibration), 0.1f);
    }

    public void StopGuidance()
    {
        isGuiding = false;
        StopVibration();
    }

    void StopVibration()
    {
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
    } 
}