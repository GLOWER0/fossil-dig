using UnityEngine;
using UnityEngine.InputSystem;

public class SnapForwardProvider : MonoBehaviour
{
    [Header("Input Settings")]
    [Tooltip("순간이동을 실행할 컨트롤러 버튼 액션")]
    public InputActionReference teleportButtonAction; 

    [Header("Movement Settings")]
    [Tooltip("한 번 누를 때마다 이동할 거리 (미터 단위)")]
    public float moveDistance = 2.0f; 
    
    [Tooltip("바라보는 방향의 기준 (보통 Main Camera)")]
    public Transform forwardSource; 

    [Header("Collision Settings")]
    [Tooltip("벽이나 장애물로 인식할 레이어 (설정하지 않으면 모든 물체에 막힘)")]
    public LayerMask obstacleLayer = ~0; // 기본값: Everything

    private void OnEnable()
    {
        if (teleportButtonAction != null)
        {
            teleportButtonAction.action.Enable();
            teleportButtonAction.action.performed += OnTeleportPressed;
        }
    }

    private void OnDisable()
    {
        if (teleportButtonAction != null)
        {
            teleportButtonAction.action.performed -= OnTeleportPressed;
        }
    }

    private void OnTeleportPressed(InputAction.CallbackContext context)
    {
        if (forwardSource == null) return;

        // 1. 시선이 향하는 방향 벡터 가져오기
        Vector3 forwardDirection = forwardSource.forward;
        
        // 2. 하늘을 보거나 땅을 볼 때 위아래로 날아가지 않도록 Y축(높이) 변화 차단
        forwardDirection.y = 0;
        forwardDirection.Normalize(); 

        // 3. 목표 위치 계산
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = currentPosition + (forwardDirection * moveDistance);

        // 4. 벽 뚫기 방지 (Raycast)
        // 발밑이 아니라 가슴 높이 정도에서 앞으로 레이저를 쏴서 장애물을 검사
        Vector3 rayStartPoint = currentPosition + Vector3.up; 

        if (Physics.Raycast(rayStartPoint, forwardDirection, out RaycastHit hit, moveDistance, obstacleLayer))
        {
            // 장애물에 부딪혔다면, 벽에 완전히 파묻히지 않도록 부딪힌 지점보다 살짝(0.3m) 앞에서 멈춤
            targetPosition = hit.point - (forwardDirection * 0.3f);
            targetPosition.y = currentPosition.y; // 높이는 원래 높이 유지
            
            Debug.Log($"<color=yellow>장애물({hit.collider.name}) 감지! 벽 앞에서 멈춤.</color>");
        }

        // 5. 최종 위치로 캐릭터 순간이동
        transform.position = targetPosition;
        Debug.Log("<color=cyan>순간이동 슉!</color>");
    }
}