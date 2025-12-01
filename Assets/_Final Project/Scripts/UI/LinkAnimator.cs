using UnityEngine;
using TMPro;

//อันนี้ก็อปมาอยากเล่นเฉยๆ 
[RequireComponent(typeof(TMP_Text))]
public class LinkAnimator : MonoBehaviour
{
    private TMP_Text textComponent;

    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    void Update()
    {
        textComponent.ForceMeshUpdate();
        var textInfo = textComponent.textInfo;

        for (int i = 0; i < textInfo.linkCount; i++)
        {
            TMP_LinkInfo link = textInfo.linkInfo[i];

            for (int j = 0; j < link.linkTextLength; j++)
            {
                int charIndex = link.linkTextfirstCharacterIndex + j;
                var charInfo = textInfo.characterInfo[charIndex];

                if (charInfo.isVisible)
                {
                    int vertexIndex = charInfo.vertexIndex;
                    int materialIndex = charInfo.materialReferenceIndex;
                    Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                    float waveOffset = Mathf.Sin(Time.time * 10f + charIndex) * 3f;
                    Vector3 offset = new Vector3(0, waveOffset, 0);

                    vertices[vertexIndex + 0] += offset;
                    vertices[vertexIndex + 1] += offset;
                    vertices[vertexIndex + 2] += offset;
                    vertices[vertexIndex + 3] += offset;
                }
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}