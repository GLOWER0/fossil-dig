using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events; // 유니티 인스펙터에서 이벤트를 연결하기 위해 필수!

public class FossilInteractManager : MonoBehaviour
{
    [Header("컴포넌트 연결")]
    [Tooltip("화석의 XR Grab Interactable 컴포넌트를 넣으세요")]
    public XRGrabInteractable grabInteractable;
    
    [Tooltip("파란색 윤곽선을 그려주는 스크립트를 넣으세요 (예: Outline)")]
    public Outline outlineScript;

    [Header("UI 및 연출 이벤트 (선배 작업물 연결용)")]
    [Tooltip("왼손으로 잡았을 때 실행할 UI 켜기 / 애니메이션 함수들을 연결하세요")]
    public UnityEvent onFossilGrabbedByLeftHand; 

    void Start()
    {
        // 1. 처음엔 집기 불가능하게 잠그고, 파란 윤곽선도 꺼두기
        if (grabInteractable != null) grabInteractable.enabled = false;
        if (outlineScript != null) outlineScript.enabled = false;

        // 2. 화석을 집었을 때 감지하는 리스너 달아주기
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
        }
    }

    // 3. 돌을 다 캤을 때 바깥(돌 부수는 스크립트)에서 이 함수를 호출해주면 됨!
    public void RevealFossil()
    {
        // 잡기 활성화 및 윤곽선 켜기
        if (grabInteractable != null) grabInteractable.enabled = true;
        if (outlineScript != null) outlineScript.enabled = true;
        
        Debug.Log("<color=green>화석 발굴 완료! 파란 윤곽선 활성화, 이제 집을 수 있습니다.</color>");
    }

    // 4. 화석을 집는 순간 실행되는 함수
    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // 화석을 집은 주체(컨트롤러)의 태그가 "LeftHand"인지 검사
        if (args.interactorObject.transform.CompareTag("LeftHand"))
        {
            Debug.Log("<color=cyan>화석 획득 성공! 등록된 UI 및 애니메이션 실행</color>");
            
            // 이미 집었으니 윤곽선은 깔끔하게 끄기
            if (outlineScript != null) outlineScript.enabled = false;

            // 인스펙터에 연결해둔 문수 선배의 UI 켜기 및 책 애니메이션 함수들이 여기서 한 번에 쫙 실행됨!
            onFossilGrabbedByLeftHand.Invoke();
        }
        else
        {
            Debug.Log("오른손으로 잡았습니다. (이벤트는 왼손으로 잡을 때만 발동합니다)");
        }
    }
}