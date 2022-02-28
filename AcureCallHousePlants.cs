using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.CloudScriptModels;
using PlayFab.ClientModels;

public class AcureCallHousePlants : MonoBehaviour
{
    public string mKey = "inputValue";
    public string mValue = "Something";

    public string catalogName = "House Plants";
    public List<CatalogItem> catalog;
    public List<Plants> housePlants = new List<Plants>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetCatalog();
        }
    }

    public void DisplayCatalog()
    {
        foreach (Plants pl in housePlants)
        {
            string printThePlant = "___" + pl.plantName + "___"
                + "\n" + pl.description
                + "\n Nickname: " + pl.nickname
                + "\n Family: " + pl.family
                + "\n Genus: " + pl.genus
                + "\n Species: " + pl.species
                + "\n Growth Habit: " + pl.growthhabit;

            print(printThePlant);
        }
    }

    public void PrintCatalogItem(CatalogItem cI)
    {
        string info = cI.CustomData;
    }

    public void GetCatalog()
    {
        GetCatalogItemsRequest getCatalogRequest = new GetCatalogItemsRequest
        {
            CatalogVersion = catalogName
        };

        PlayFabClientAPI.GetCatalogItems(getCatalogRequest,
            result =>
            {
                catalog = result.Catalog;
            },
            error => Debug.Log(error.ErrorMessage)
        );

        Invoke("BreakApartCatalog", 3f);
    }

    public void BreakApartCatalog()
    {
        foreach (CatalogItem cI in catalog)
        {
            Plants pl = JsonUtility.FromJson<Plants>(cI.CustomData);
            pl.plantName = cI.DisplayName;
            pl.description = cI.Description;
            housePlants.Add(pl);
        }

        DisplayCatalog();
    }

    private void CallCSharpExecuteFunction()
    {
        PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                Id = PlayFabSettings.staticPlayer.EntityId, //Get this from when you logged in,
                Type = PlayFabSettings.staticPlayer.EntityType, //Get this from when you logged in
            },
            FunctionName = "HelloWorld", //This should be the name of your Azure Function that you created.
            FunctionParameter = new Dictionary<string, object>() { { mKey, mValue } }, //This is the data that you would want to pass into your function.
            GeneratePlayStreamEvent = false //Set this to true if you would like this call to show up in PlayStream
        }, (ExecuteFunctionResult result) =>
        {
            if (result.FunctionResultTooLarge ?? false)
            {
                Debug.Log("This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
                return;
            }
            Debug.Log($"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");
            Debug.Log($"Result: {result.FunctionResult.ToString()}");
        }, (PlayFabError error) =>
        {
            Debug.Log($"Oops Something went wrong: {error.GenerateErrorReport()}");
        });
    }
}

public class Plants
{
    public string plantName;
    public string description;
    public string nickname;
    public string family;
    public string genus;
    public string species;
    public string growthhabit;
}
