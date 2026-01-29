using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BuildingInfoUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public TMP_InputField inputField;
    public TextMeshProUGUI resultText;

    [Header("Data Source")]
    public BuildingManager buildingManager;   // ← NEW

    void Start()
    {
        panel.SetActive(false);
    }

    public void OpenPanel()
    {
        panel.SetActive(true);
        resultText.text = "";
        inputField.text = "";
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
    }

    public void SearchBuilding()
    {
        string id = inputField.text.Trim();

        if (string.IsNullOrEmpty(id))
        {
            resultText.text = "Veuillez entrer un ID bâtiment.";
            return;
        }

        // 🔥 NOW WE ASK THE MANAGER
        Building found = buildingManager.GetBuildingByID(id);

        if (found == null)
        {
            resultText.text = "❌ Aucun bâtiment trouvé avec cet ID.";
            return;
        }

        Vector2 center = GetPolygonCenter(found.polygon);

        resultText.text =
            "<b>BÂTIMENT TROUVÉ</b>\n\n" +
            $"ID : {found.properties.batiment_construction_id}\n" +
            $"Usage : {found.properties.usage_main}\n" +
            $"Année : {found.properties.construction_year}\n" +
            $"Chauffage : {found.properties.heating_energy}\n\n" +
            $"Position approx :\nX = {center.x:F2}\nY = {center.y:F2}";
    }

    Vector2 GetPolygonCenter(List<Vector2> poly)
    {
        Vector2 sum = Vector2.zero;
        foreach (var p in poly)
            sum += p;
        return sum / poly.Count;
    }
}
