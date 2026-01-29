using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Newtonsoft.Json;

#region JSON STRUCTURES

[System.Serializable]
public class FeatureCollection
{
    public Feature[] features;
}

[System.Serializable]
public class Feature
{
    public Properties properties;
}

[System.Serializable]
public class Properties
{
    public string batiment_construction_id;
    public string usage_main;
    public double? construction_year;
    public string heating_energy;
}

#endregion

public class Building
{
    public Properties properties;
}

public class BuildingManager : MonoBehaviour
{
    [Header("JSON FILE (StreamingAssets)")]
    public string fileName = "building_db_light.json"; // ✅ FIXED

    private Dictionary<string, Building> buildingDict = new Dictionary<string, Building>();
    public bool IsLoaded { get; private set; } = false;

    IEnumerator Start()
    {
        Debug.Log("📂 Loading building database...");

        string path = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        UnityWebRequest req = UnityWebRequest.Get(path);

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ JSON LOAD ERROR: " + req.error);
            yield break;
        }

        string json = req.downloadHandler.text;

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("❌ JSON FILE EMPTY");
            yield break;
        }

        Debug.Log("✅ JSON LOADED, size: " + json.Length);

        FeatureCollection geo = JsonConvert.DeserializeObject<FeatureCollection>(json);

        if (geo == null || geo.features == null)
        {
            Debug.LogError("❌ NO FEATURES FOUND");
            yield break;
        }

        foreach (Feature f in geo.features)
        {
            if (f.properties == null) continue;

            string id = f.properties.batiment_construction_id;

            if (string.IsNullOrEmpty(id)) continue;

            Building b = new Building { properties = f.properties };

            buildingDict[id] = b;
        }

        IsLoaded = true;

        Debug.Log("🏢 BUILDINGS LOADED: " + buildingDict.Count);
        Debug.Log("🎉 DATABASE READY");
    }

    // 🔍 SEARCH BY ID (used by UI)
    public Building GetBuildingByID(string id)
    {
        if (!IsLoaded) return null;

        buildingDict.TryGetValue(id, out Building b);
        return b;
    }
}
