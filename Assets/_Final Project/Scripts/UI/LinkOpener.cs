using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class LinkOpener : MonoBehaviour, IPointerClickHandler
{
    private TMP_Text pTextMeshPro;
    private Canvas canvas;
    private Camera cam;

    void Awake()
    {
        pTextMeshPro = GetComponent<TMP_Text>();
        canvas = GetComponentInParent<Canvas>();

        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            cam = canvas.worldCamera;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, eventData.position, cam);

        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];
            string url = linkInfo.GetLinkID();

            if (!string.IsNullOrEmpty(url))
            {
                Debug.Log("Open Link: " + url); 
                Application.OpenURL(url);
            }
        }
    }
}