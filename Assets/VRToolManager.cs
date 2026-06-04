using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class VRToolManager : MonoBehaviour
{
    [Header("🛠️ 무기 스위칭 세팅")]
    public InputActionReference switchButton; 
    public GameObject pickaxe; 
    public GameObject detector; 

    [Header("📡 탐지기 세팅")]
    public InputActionReference scanButton; 
    public Transform redArrow; 
    public AudioSource beepAudio; 
    public float maxDetectDistance = 15f; 

    [Header("👀 AR 화살표 시야(HUD) 위치 세팅")]
    [Tooltip("내 눈에서 얼마나 앞으로 띄울지")]
    public float forwardOffset = 1.0f; 
    [Tooltip("내 눈에서 위/아래로 얼마나 내릴지 (-0.3이면 살짝 아래)")]
    public float upOffset = -0.3f; 
    [Tooltip("내 눈에서 좌/우로 얼마나 치우치게 할지 (0이면 정중앙)")]
    public float rightOffset = 0.0f; 

    private bool isScanning = false;
    private Transform targetFossil;

    void Start()
    {
        pickaxe.SetActive(true);
        detector.SetActive(false);
        redArrow.gameObject.SetActive(false);
    }

    void Update()
    {
        // 1. 무기 교체 로직
        if (switchButton != null && switchButton.action.WasPressedThisFrame())
        {
            bool isPickaxeActive = pickaxe.activeSelf;
            
            pickaxe.SetActive(!isPickaxeActive);
            detector.SetActive(isPickaxeActive);

            if (isPickaxeActive) StopScanning();
        }

        // 2. 탐지기 켜고 끄기 로직
        if (detector.activeInHierarchy && scanButton != null && scanButton.action.WasPressedThisFrame())
        {
            isScanning = !isScanning;
            if (isScanning) 
            {
                redArrow.gameObject.SetActive(true);
                StartCoroutine(BeepRoutine());
            }
            else 
            {
                StopScanning();
            }
        }

        // 💡 3. [시야에 완벽 박제!] 상하좌우 거리를 계산한 AR HUD 화살표
        if (isScanning && targetFossil != null && Camera.main != null)
        {
            Vector3 realCenter = targetFossil.position;
            Collider fossilCollider = targetFossil.GetComponent<Collider>();
            
            if (fossilCollider != null)
            {
                realCenter = fossilCollider.bounds.center;
            }

            // 🚨 [상하좌우 완벽 계산] 카메라(내 시선)의 로컬 3D 축을 기준으로 거리를 더함!
            Vector3 hudPosition = Camera.main.transform.position 
                                + (Camera.main.transform.forward * forwardOffset) // 앞으로
                                + (Camera.main.transform.up * upOffset)           // 위/아래로
                                + (Camera.main.transform.right * rightOffset);    // 좌/우로
            
            // 1. 위치: 내 시야에 계산된 위치로 화살표 순간이동 (고개 돌려도 완벽하게 따라옴)
            redArrow.position = hudPosition;

            // 2. 방향: 그 허공에 뜬 상태에서 화석만 정확히 쳐다보기
            redArrow.LookAt(realCenter);
        }
    }

    private void StopScanning()
    {
        isScanning = false;
        redArrow.gameObject.SetActive(false);
        StopAllCoroutines(); 
    }

    IEnumerator BeepRoutine()
    {
        while (isScanning)
        {
            FindClosestFossil(); 

            if (targetFossil != null)
            {
                float distance = Vector3.Distance(detector.transform.position, targetFossil.position);

                if (distance <= maxDetectDistance)
                {
                    if (beepAudio != null && !beepAudio.isPlaying) beepAudio.Play();

                    float waitTime = Mathf.Lerp(0.1f, 1.0f, distance / maxDetectDistance);
                    yield return new WaitForSeconds(waitTime);
                }
                else
                {
                    yield return null; 
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    private void FindClosestFossil()
    {
        GameObject[] fossils = GameObject.FindGameObjectsWithTag("Fossil");
        float closestDist = Mathf.Infinity;
        targetFossil = null;

        foreach (GameObject f in fossils)
        {
            float dist = Vector3.Distance(detector.transform.position, f.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                targetFossil = f.transform;
            }
        }
    }
}