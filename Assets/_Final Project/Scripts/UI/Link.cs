using UnityEngine;
using UnityEngine.UI;

public class Link : MonoBehaviour
{
    public AudioSource myAudio;
    public AudioClip meowSound;

    public void OpenLink(string urlToOpen)
    {

      
        if (!string.IsNullOrEmpty(urlToOpen))
        {
         
            Application.OpenURL(urlToOpen);
            myAudio.PlayOneShot(meowSound);
        }
      
    }
}
