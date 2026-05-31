using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// 화석 한 마리의 데이터를 담는 구조체
[System.Serializable]
public class FossilEntry
{
    public string fossilName;
    public string period;
    [TextArea(3, 8)]
    public string description;
    public Sprite fossilImage;  // 나중에 이미지 들어갈 자리 (지금은 null OK)
}

public class FossilEncyclopedia : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI fossilNumberText;
    public TextMeshProUGUI fossilNameText;
    public TextMeshProUGUI fossilPeriodText;
    public TextMeshProUGUI fossilDescriptionText;
    public Image fossilImage;            // 왼쪽 페이지의 이미지
    public GameObject imagePlaceholder;  // 이미지 없을 때 보일 텍스트
    public TextMeshProUGUI pageCounterText;
    
    public Button prevButton;
    public Button nextButton;
    public Button closeButton;
    
    [Header("Fossil Data (가짜 데이터)")]
    public List<FossilEntry> fossils = new List<FossilEntry>();
    
    [Header("Behavior")]
    public bool faceCamera = true;
    public float openDuration = 0.8f;
    
    private CanvasGroup canvasGroup;
    private Camera mainCam;
    private int currentIndex = 0;
    private Vector3 originalScale;
    private Transform bookContainer;
    
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        originalScale = transform.localScale;
        
        // BookContainer 찾기 (회전 애니메이션용)
        bookContainer = transform.Find("BookContainer");
        
        // 버튼 이벤트 연결
        if (prevButton != null) prevButton.onClick.AddListener(PreviousFossil);
        if (nextButton != null) nextButton.onClick.AddListener(NextFossil);
        if (closeButton != null) closeButton.onClick.AddListener(Close);
    }
    
    void Start()
    {
        mainCam = Camera.main;
        
        // 가짜 데이터 자동 채우기 (Inspector에 안 채웠을 경우)
        if (fossils.Count == 0)
        {
            FillDefaultData();
        }
        
        DisplayFossil(0);
    }
    
    void LateUpdate()
    {
        if (faceCamera && mainCam != null && bookContainer != null)
        {
            Vector3 dir = transform.position - mainCam.transform.position;
            dir.y = 0;
            if (dir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(dir);
        }
    }
    
    // 가짜 화석 데이터 (Inspector에 데이터 없으면 자동 사용)
    void FillDefaultData()
    {
        fossils.Add(new FossilEntry {
            fossilName = "티라노사우루스 이빨",
            period = "백악기 후기 · 약 6,800만 년 전",
            description = "육식 공룡의 이빨로, 강력한 턱 힘으로 사냥감의 뼈까지 부술 수 있었습니다. 발견된 이빨의 길이는 약 15cm이며, 표면에 톱니 형태의 미세한 돌기가 보입니다.\n\n서식지: 북아메리카\n분류: 수각류 공룡\n크기: 약 12미터"
        });
        fossils.Add(new FossilEntry {
            fossilName = "암모나이트",
            period = "쥐라기 · 약 1억 5천만 년 전",
            description = "나선형 껍데기를 가진 두족류로, 현재의 오징어와 유사한 생물이었습니다. 껍데기 내부의 가스 조절로 부력을 조절하며 살았습니다.\n\n서식지: 전 세계 바다\n분류: 두족류\n크기: 약 5~30cm"
        });
        fossils.Add(new FossilEntry {
            fossilName = "삼엽충",
            period = "고생대 캄브리아기 · 약 5억 년 전",
            description = "초기 절지동물의 대표 화석으로, 단단한 외골격이 화석으로 잘 보존됩니다. 세 갈래로 나뉜 몸 구조가 이름의 유래입니다.\n\n서식지: 고대 바다 바닥\n분류: 절지동물\n크기: 약 3~10cm"
        });
        fossils.Add(new FossilEntry {
            fossilName = "프테라노돈 뼈",
            period = "백악기 후기 · 약 8,500만 년 전",
            description = "거대 익룡의 골격 화석입니다. 가벼운 속이 빈 뼈 구조로 거대한 날개를 지탱했습니다. 부리에는 이빨이 없었습니다.\n\n서식지: 북아메리카 해안\n분류: 익룡류\n날개폭: 약 7미터"
        });
        fossils.Add(new FossilEntry {
            fossilName = "벨로키랍토르 발톱",
            period = "백악기 후기 · 약 7,500만 년 전",
            description = "작지만 영리한 육식 공룡의 갈고리 발톱입니다. 사냥감을 제압하는 데 사용되었으며, 무리 사냥을 했다는 증거도 발견됩니다.\n\n서식지: 몽골\n분류: 수각류 공룡\n크기: 약 2미터"
        });
    }
    
    public void DisplayFossil(int index)
    {
        if (fossils.Count == 0) return;
        
        // 인덱스 범위 안전 처리
        currentIndex = Mathf.Clamp(index, 0, fossils.Count - 1);
        
        FossilEntry entry = fossils[currentIndex];
        
        // UI 업데이트
        if (fossilNumberText != null)
            fossilNumberText.text = $"No. {(currentIndex + 1):D3}";
        
        if (fossilNameText != null) fossilNameText.text = entry.fossilName;
        if (fossilPeriodText != null) fossilPeriodText.text = entry.period;
        if (fossilDescriptionText != null) fossilDescriptionText.text = entry.description;
        
        // 이미지 처리
        if (fossilImage != null)
        {
            if (entry.fossilImage != null)
            {
                fossilImage.sprite = entry.fossilImage;
                fossilImage.color = Color.white;
                if (imagePlaceholder != null) imagePlaceholder.SetActive(false);
            }
            else
            {
                fossilImage.sprite = null;
                fossilImage.color = new Color(200/255f, 195/255f, 180/255f, 1f);
                if (imagePlaceholder != null) imagePlaceholder.SetActive(true);
            }
        }
        
        // 페이지 카운터
        if (pageCounterText != null)
            pageCounterText.text = $"{currentIndex + 1} / {fossils.Count}";
        
        // 버튼 활성/비활성 (첫 페이지면 이전 버튼 흐리게, 마지막 페이지면 다음 버튼 흐리게)
        if (prevButton != null) prevButton.interactable = (currentIndex > 0);
        if (nextButton != null) nextButton.interactable = (currentIndex < fossils.Count - 1);
    }
    
    public void NextFossil()
    {
        if (currentIndex < fossils.Count - 1)
            DisplayFossil(currentIndex + 1);
    }
    
    public void PreviousFossil()
    {
        if (currentIndex > 0)
            DisplayFossil(currentIndex - 1);
    }
    
    /// <summary>
    /// 도감 열기 (외부에서 호출 가능)
    /// </summary>
    public void Open()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(OpenAnimation());
    }
    
    /// <summary>
    /// 도감 닫기
    /// </summary>
    public void Close()
    {
        StopAllCoroutines();
        StartCoroutine(CloseAnimation());
    }
    
    IEnumerator OpenAnimation()
    {
        float t = 0;
        canvasGroup.alpha = 0;
        
        // 책이 펼쳐지는 느낌 — Y축 회전 90도에서 0도로
        if (bookContainer != null)
            bookContainer.localRotation = Quaternion.Euler(0, 90, 0);
        
        transform.localScale = originalScale * 0.7f;
        
        while (t < openDuration)
        {
            t += Time.deltaTime;
            float p = t / openDuration;
            float smooth = Mathf.SmoothStep(0, 1, p);
            
            canvasGroup.alpha = smooth;
            
            // 회전: 90도 → 0도 (책 펼치기)
            if (bookContainer != null)
            {
                float angle = Mathf.Lerp(90, 0, smooth);
                bookContainer.localRotation = Quaternion.Euler(0, angle, 0);
            }
            
            // 살짝 커지는 효과
            float scale = Mathf.Lerp(0.7f, 1f, smooth);
            transform.localScale = originalScale * scale;
            
            yield return null;
        }
        
        canvasGroup.alpha = 1;
        transform.localScale = originalScale;
        if (bookContainer != null)
            bookContainer.localRotation = Quaternion.identity;
    }
    
    IEnumerator CloseAnimation()
    {
        float t = 0;
        float duration = 0.5f;
        
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;
            
            canvasGroup.alpha = 1f - p;
            
            // 회전: 0 → 90도 (책 덮기)
            if (bookContainer != null)
            {
                float angle = Mathf.Lerp(0, 90, p);
                bookContainer.localRotation = Quaternion.Euler(0, angle, 0);
            }
            
            float scale = Mathf.Lerp(1f, 0.7f, p);
            transform.localScale = originalScale * scale;
            
            yield return null;
        }
        
        gameObject.SetActive(false);
        if (bookContainer != null)
            bookContainer.localRotation = Quaternion.identity;
        transform.localScale = originalScale;
    }
}