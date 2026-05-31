using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class FossilInfoCard : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI periodText;
    public TextMeshProUGUI descriptionText;
    public Button closeButton;
    
    [Header("Behavior")]
    public bool faceCamera = true;
    public float fadeInDuration = 0.5f;
    public float popScale = 1.2f;
    
    private CanvasGroup canvasGroup;
    private Camera mainCam;
    private Vector3 originalScale;
    
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        originalScale = transform.localScale;
        
        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);
    }
    
    void Start()
    {
        mainCam = Camera.main;
    }
    
    void LateUpdate()
    {
        if (faceCamera && mainCam != null)
        {
            Vector3 dir = transform.position - mainCam.transform.position;
            dir.y = 0;
            if (dir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(dir);
        }
    }
    
    /// <summary>
    /// 팀원이 호출할 핵심 API. 화석 정보를 받아 카드를 표시.
    /// </summary>
    public void Show(string fossilName, string period, string description, Vector3 worldPosition)
    {
        nameText.text = fossilName;
        periodText.text = period;
        descriptionText.text = description;
        
        transform.position = worldPosition;
        gameObject.SetActive(true);
        
        StopAllCoroutines();
        StartCoroutine(FadeInAnimation());
    }
    
    public void Hide()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutAnimation());
    }
    
    IEnumerator FadeInAnimation()
    {
        float t = 0;
        canvasGroup.alpha = 0;
        transform.localScale = originalScale * 0.5f;
        
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            float p = t / fadeInDuration;
            
            canvasGroup.alpha = Mathf.SmoothStep(0, 1, p);
            
            float scale;
            if (p < 0.7f)
                scale = Mathf.Lerp(0.5f, popScale, p / 0.7f);
            else
                scale = Mathf.Lerp(popScale, 1f, (p - 0.7f) / 0.3f);
            
            transform.localScale = originalScale * scale;
            yield return null;
        }
        
        canvasGroup.alpha = 1;
        transform.localScale = originalScale;
    }
    
    IEnumerator FadeOutAnimation()
    {
        float t = 0;
        float duration = 0.3f;
        Vector3 startScale = transform.localScale;
        
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;
            canvasGroup.alpha = 1f - p;
            transform.localScale = Vector3.Lerp(startScale, originalScale * 0.5f, p);
            yield return null;
        }
        
        gameObject.SetActive(false);
    }
}