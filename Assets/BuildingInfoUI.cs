using UnityEngine;
using TMPro;

public class BuildingInfoUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public TMP_InputField inputField;
    public TextMeshProUGUI resultText;

    [Header("Data Source")]
    public BuildingManager buildingManager;

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
        if (!buildingManager.IsLoaded)
        {
            resultText.text = "⏳ Base de données en cours de chargement...";
            return;
        }

        string id = inputField.text.Trim();

        if (string.IsNullOrEmpty(id))
        {
            resultText.text = "Veuillez entrer un ID bâtiment.";
            return;
        }

        Building found = buildingManager.GetBuildingByID(id);

        if (found == null)
        {
            resultText.text = "❌ Aucun bâtiment trouvé avec cet ID.";
            return;
        }

        var p = found.properties;

        resultText.text =
            "<b>BÂTIMENT TROUVÉ</b>\n\n" +
            $"ID : {p.batiment_construction_id}\n" +
            $"Usage : {p.usage_main}\n" +
            $"Année : {(p.construction_year.HasValue ? p.construction_year.Value.ToString("F0") : "Inconnue")}\n" +
            $"Chauffage : {p.heating_energy}";
    }
}
