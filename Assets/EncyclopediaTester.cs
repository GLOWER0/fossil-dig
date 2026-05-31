using UnityEngine;

public class EncyclopediaTester : MonoBehaviour
{
    public FossilEncyclopedia encyclopedia;
    
    void Start()
    {
        // 처음엔 안 보이게
        encyclopedia.gameObject.SetActive(false);
    }
    
    void Update()
    {
        // E 키로 열기
        if (Input.GetKeyDown(KeyCode.E))
        {
            encyclopedia.Open();
        }
    }
}