using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject arCamera; // Ton ARCamera Vuforia

    void Start()
    {
        // Au départ, le menu est visible, la caméra AR inactive
        menuPanel.SetActive(true);
        arCamera.SetActive(false);
    }

    public void StartAR()
    {
        // Quand on clique sur le bouton
        menuPanel.SetActive(false);
        arCamera.SetActive(true);
    }
}
