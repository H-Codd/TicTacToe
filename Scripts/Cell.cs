using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cell : MonoBehaviour
{
    // Botão da célula (UI Button)
    public Button button;

    // Texto dentro do botão (TMP_Text) que mostra X ou O
    public TMP_Text cellText;

    // Guarda o símbolo atual da célula: vazio, "X" ou "O"
    private string playerSymbol = "";

    // Define o símbolo da célula (X ou O),
    // atualiza o texto e desativa o botão para não ser clicado novamente
    public void SetSymbol(string symbol)
    {
        playerSymbol = symbol;
        cellText.text = symbol;
        button.interactable = false;
    }

    // Retorna o símbolo atual da célula
    public string GetSymbol()
    {
        return playerSymbol;
    }

    // Reseta a célula para estado inicial:
    // símbolo vazio, texto limpo e botão clicável novamente
    public void ResetCell()
    {
        playerSymbol = "";
        cellText.text = "";
        button.interactable = true;
    }
}
