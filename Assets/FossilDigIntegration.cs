using UnityEngine;

/// <summary>
/// 발굴(곡괭이로 돌 깨기) → 화석 획득 → 발굴 팝업 / 도감을 잇는 "연결 다리".
///
/// FossilInteractManager.onFossilGrabbedByLeftHand 이벤트에 *코드로* 구독하므로
/// 인스펙터에서 UnityEvent를 손으로 연결할 필요가 없다.
/// (FossilInfoCard.Show 는 인자가 4개라 빈 UnityEvent로는 직접 못 부르기 때문에 이 다리가 필요함.)
///
/// 셋업: 씬에 이 컴포넌트를 가진 GameObject 1개 +
///       FossilInfoCard 프리팹 + EncyclopediaCanvas 프리팹만 넣으면 끝.
///       참조 슬롯은 비워두면 씬에서 자동으로 찾는다.
/// </summary>
public class FossilDigIntegration : MonoBehaviour
{
    [Header("참조 (비워두면 씬에서 자동 탐색)")]
    [SerializeField] FossilInteractManager fossilManager;
    [SerializeField] FossilInfoCard infoCard;
    [SerializeField] FossilEncyclopedia encyclopedia;

    [Header("발굴한 화석 정보 (팝업에 표시)")]
    [SerializeField] string fossilName = "삼엽충";
    [SerializeField] string period = "고생대 캄브리아기 · 약 5억 년 전";
    [TextArea(3, 6)]
    [SerializeField] string description =
        "초기 절지동물의 대표 화석으로, 단단한 외골격이 화석으로 잘 보존됩니다. " +
        "세 갈래로 나뉜 몸 구조가 이름의 유래입니다.";

    [Header("팝업/도감 위치 (카메라 기준)")]
    [SerializeField] float popupDistance = 1.2f;
    [SerializeField] float popupHeight = 1.5f;

    [Header("동작")]
    [Tooltip("화석을 집는 순간 도감도 자동으로 펼칠지 (끄면 팝업만)")]
    [SerializeField] bool alsoOpenEncyclopedia = true;

    void Start()
    {
        if (fossilManager == null) fossilManager = FindObjectOfType<FossilInteractManager>(true);
        if (infoCard == null)      infoCard      = FindObjectOfType<FossilInfoCard>(true);
        if (encyclopedia == null)  encyclopedia  = FindObjectOfType<FossilEncyclopedia>(true);

        // 시작 시 UI 숨김
        if (infoCard != null)     infoCard.gameObject.SetActive(false);
        if (encyclopedia != null) encyclopedia.gameObject.SetActive(false);

        if (fossilManager != null)
            fossilManager.onFossilGrabbedByLeftHand.AddListener(OnFossilCollected);
        else
            Debug.LogWarning("[FossilDigIntegration] FossilInteractManager를 찾지 못했습니다. 화석 오브젝트가 씬에 있는지 확인하세요.");
    }

    void OnDestroy()
    {
        if (fossilManager != null)
            fossilManager.onFossilGrabbedByLeftHand.RemoveListener(OnFossilCollected);
    }

    /// <summary>화석을 왼손으로 집은 순간 호출됨 (onFossilGrabbedByLeftHand 구독).</summary>
    public void OnFossilCollected()
    {
        Debug.Log("[FossilDigIntegration] 화석 획득 → 발굴 팝업 표시");
        ShowPopup();
        if (alsoOpenEncyclopedia) OpenEncyclopedia();
    }

    /// <summary>발굴 팝업을 카메라 앞에 띄운다.</summary>
    public void ShowPopup()
    {
        if (infoCard == null)
        {
            Debug.LogWarning("[FossilDigIntegration] FossilInfoCard가 없습니다. 프리팹을 씬에 넣었는지 확인하세요.");
            return;
        }
        infoCard.Show(fossilName, period, description, InFrontOfCamera(popupDistance));
    }

    /// <summary>도감을 카메라 앞에 펼친다. (버튼 등에서도 호출 가능)</summary>
    public void OpenEncyclopedia()
    {
        if (encyclopedia == null)
        {
            Debug.LogWarning("[FossilDigIntegration] FossilEncyclopedia가 없습니다. 프리팹을 씬에 넣었는지 확인하세요.");
            return;
        }
        encyclopedia.transform.position = InFrontOfCamera(popupDistance + 0.6f);
        encyclopedia.Open();
    }

    Vector3 InFrontOfCamera(float distance)
    {
        var cam = Camera.main;
        if (cam == null) return transform.position;

        Vector3 fwd = cam.transform.forward;
        fwd.y = 0f;
        if (fwd.sqrMagnitude < 0.0001f) fwd = Vector3.forward;
        fwd.Normalize();

        Vector3 pos = cam.transform.position + fwd * distance;
        pos.y = popupHeight;
        return pos;
    }
}
