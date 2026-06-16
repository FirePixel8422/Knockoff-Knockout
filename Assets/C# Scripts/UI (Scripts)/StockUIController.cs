using UnityEngine;


/// <summary>
/// Sub HUDManager system handler class that is responsible for updating the stocks UI.
/// </summary>
[System.Serializable]
public class StockUIController
{
    [SerializeField] private GameObject[] stocks;

    [EditorReadOnly, SerializeField] private int cStocks;

    private const int TOTAL_STOCKS = 3;


    public void AddStock()
    {
        if (cStocks == TOTAL_STOCKS)
        {
            DebugLogger.LogWarning("Exceeded max stocks");
            return;
        }

        stocks[cStocks++].SetActive(true);
    }
    public void ResetStocks()
    {
        for (int i = 0; i < TOTAL_STOCKS; i++)
        {
            stocks[i].SetActive(false);
        }
        cStocks = 0;
    }
}