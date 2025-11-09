using UnityEngine;

public class Link : MonoBehaviour
{
   
    public void OpenLink(string urlToOpen)
    {
      
        if (!string.IsNullOrEmpty(urlToOpen))
        {
         
            Application.OpenURL(urlToOpen);
        }
      
    }
}
