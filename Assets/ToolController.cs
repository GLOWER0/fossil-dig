using UnityEngine;

public class ToolController : MonoBehaviour
{
    [Header("Tool Settings")]
    public int baseDamage = 1;
    public float minSwingVelocity = 0.5f; 
    public float damageMultiplier = 2.0f;

    [Header("Hit Area (AoE)")]
    public float attackRadius = 0.5f; // 타격 반경

    private Vector3 lastPosition;
    private float currentVelocity;

    void Update()
    {
        currentVelocity = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = transform.position;
    }

    // VR 환경에서 가장 안정적인 OnTriggerEnter 사용
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Soil"))
        {
            if (currentVelocity >= minSwingVelocity)
            {
                int finalDamage = baseDamage + Mathf.RoundToInt(currentVelocity * damageMultiplier);
                
                // 곡괭이의 위치를 중심으로 attackRadius 반경 내의 모든 콜라이더를 찾음
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);

                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.CompareTag("Soil"))
                    {
                        SoilChunk chunk = hitCollider.GetComponent<SoilChunk>();
                        if (chunk != null)
                        {
                            chunk.ApplyHit(finalDamage);
                        }
                    }
                }
                Debug.Log($"<color=green>범위 Hit! 데미지: {finalDamage} / 현재 속도: {currentVelocity:F1}</color>");
            }
            else
            {
                Debug.Log($"스윙이 너무 약합니다. (현재 속도: {currentVelocity:F1})");
            }
        }
    }
}