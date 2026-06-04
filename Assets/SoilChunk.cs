using UnityEngine;

public class SoilChunk : MonoBehaviour
{
    public int hp = 1;
    private bool isBroken = false;
    private Rigidbody rb;
    private Collider col;

    [Header("Effects")]
    public GameObject destroyEffect; // 먼지 효과를 넣을 빈칸

    [Header("Fossil Settings")]
    public FossilInteractManager fossilManager; // 💡 화석 매니저를 연결할 빈칸 추가!

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (rb != null)
        {
            rb.isKinematic = false; 
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void ApplyHit(int damage)
    {
        if (isBroken) return; 

        hp -= damage;

        if (hp <= 0)
        {
            BreakAndFall();
        }
    }

    private void BreakAndFall()
    {
        isBroken = true;
        
        Debug.Log("1. 돌 부서짐 함수 실행됨!"); // 💡 추적기 1
        
        if (fossilManager != null)
        {
            Debug.Log("2. 화석 매니저 연결 확인됨! 전화 건다!"); // 💡 추적기 2
            fossilManager.RevealFossil();
        }
        else
        {
            Debug.LogWarning("🚨 에러: 화석 매니저 빈칸이 비어있음!"); // 💡 추적기 3 (경고)
        }

        if (rb != null && col != null)
        {
            Debug.Log("3. Rigidbody와 Collider 연결 확인됨!"); // 💡 추적기 3
            if (destroyEffect != null)
            {
                Instantiate(destroyEffect, col.bounds.center, Quaternion.identity);
            }

            col.isTrigger = true; 
            rb.constraints = RigidbodyConstraints.None;
            rb.useGravity = true;
            rb.AddForce(Vector3.down * 2f, ForceMode.Impulse);
        }

        Destroy(gameObject, 2.0f); 
    }
}