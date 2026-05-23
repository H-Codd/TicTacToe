using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    // Array com todas as células do tabuleiro (0 a 8)
    public Cell[] cells;

    // Painel e mensagem de vitória
    public GameObject victoryPanel;
    public TMP_Text victoryMessage;

    // Textos da UI (placar, modo de jogo, turno, símbolo)
    public TMP_Text scoreText;
    public TMP_Text modeButtonText;
    public TMP_Text turnButtonText;
    public TMP_Text symbolButtonText;

    // Sons de vitória e empate
    public AudioSource sfxSource;
    public AudioClip winXClip;
    public AudioClip winOClip;
    public AudioClip drawClip;

    // Enum para definir modo de jogo
    public enum GameMode { TwoPlayers, VsAI }
    public GameMode gameMode = GameMode.TwoPlayers;

    // Símbolos dos jogadores
    public string player1Symbol = "X";
    public string player2Symbol = "O";

    // Controle interno
    private string currentPlayer; // jogador da vez
    private int scoreX = 0;       // placar X
    private int scoreO = 0;       // placar O

    // Inicialização do jogo
    void Start()
    {
        AjustarSimbolos(); // garante que os símbolos não sejam iguais
        currentPlayer = player1Symbol; // Player1 sempre começa
        if (turnButtonText != null)
            turnButtonText.text = "Vez: " + currentPlayer;
        if (symbolButtonText != null)
            symbolButtonText.text = "Player1: " + player1Symbol;
    }

    // Método chamado quando uma célula é clicada
    public void CellClicked(int index)
    {
        // Só permite jogar em célula vazia
        if (cells[index].GetSymbol() == "")
        {
            // Marca jogada
            cells[index].SetSymbol(currentPlayer);

            // Verifica estado do jogo
            string state = CheckGameState();

            if (state == "win")
            {
                ShowVictory("Jogador '" + currentPlayer + "' venceu!");
                UpdateScore();
                return;
            }
            else if (state == "draw")
            {
                ShowVictory("Empate!");
                return;
            }

            // Alterna jogador
            currentPlayer = (currentPlayer == player1Symbol) ? player2Symbol : player1Symbol;

            if (turnButtonText != null)
                turnButtonText.text = "Vez: " + currentPlayer;

            // Se modo VsAI e for vez do Player2 → IA joga
            if (gameMode == GameMode.VsAI && currentPlayer == player2Symbol)
            {
                PlayAI();
            }

            // Desativa botão de trocar símbolo após primeira jogada
            if (symbolButtonText != null)
                symbolButtonText.transform.parent.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
    }

    // Verifica se houve vitória, empate ou se jogo continua
    private string CheckGameState()
    {
        int[,] winConditions = new int[,] {
            {0,1,2}, {3,4,5}, {6,7,8},   // Linhas
            {0,3,6}, {1,4,7}, {2,5,8},   // Colunas
            {0,4,8}, {2,4,6}             // Diagonais
        };

        // Percorre todas as combinações de vitória
        for (int i = 0; i < winConditions.GetLength(0); i++)
        {
            if (cells[winConditions[i,0]].GetSymbol() == currentPlayer &&
                cells[winConditions[i,1]].GetSymbol() == currentPlayer &&
                cells[winConditions[i,2]].GetSymbol() == currentPlayer)
            {
                return "win";
            }
        }

        // Verifica empate (tabuleiro cheio)
        bool allFilled = true;
        foreach (Cell cell in cells)
        {
            if (cell.GetSymbol() == "")
            {
                allFilled = false;
                break;
            }
        }

        return allFilled ? "draw" : "continue";
    }

    // Exibe painel de vitória/empate e toca som
    private void ShowVictory(string message)
    {
        victoryMessage.text = message;
        victoryPanel.SetActive(true);
        DisableAllCells();

        if (message.Contains("Empate"))
        {
            if (drawClip != null)
                sfxSource.PlayOneShot(drawClip);
        }
        else if (currentPlayer == "X" && winXClip != null)
        {
            sfxSource.PlayOneShot(winXClip);
        }
        else if (currentPlayer == "O" && winOClip != null)
        {
            sfxSource.PlayOneShot(winOClip);
        }
    }

    // Desativa interação em todas as células
    private void DisableAllCells()
    {
        foreach (Cell cell in cells)
        {
            cell.button.interactable = false;
        }
    }

    // Reinicia tabuleiro
    public void ResetBoard()
    {
        foreach (Cell cell in cells)
        {
            cell.ResetCell();
        }

        currentPlayer = player1Symbol;
        victoryPanel.SetActive(false);

        if (turnButtonText != null)
            turnButtonText.text = "Vez: " + currentPlayer;

        // Reativa botão de trocar símbolo
        if (symbolButtonText != null)
            symbolButtonText.transform.parent.GetComponent<UnityEngine.UI.Button>().interactable = true;
    }

    // Jogada da IA (aleatória)
    private void PlayAI()
    {
        List<int> emptyCells = new List<int>();
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].GetSymbol() == "")
            {
                emptyCells.Add(i);
            }
        }

        if (emptyCells.Count > 0)
        {
            int randomIndex = Random.Range(0, emptyCells.Count);
            CellClicked(emptyCells[randomIndex]);
        }
    }

    // Atualiza placar
    private void UpdateScore()
    {
        if (currentPlayer == "X") scoreX++;
        else if (currentPlayer == "O") scoreO++;

        if (scoreText != null)
            scoreText.text = "Placar - X: " + scoreX + " | O: " + scoreO;
    }

    // Alterna modo de jogo (2 jogadores ↔ IA)
    public void ToggleGameMode()
    {
        if (gameMode == GameMode.TwoPlayers)
        {
            gameMode = GameMode.VsAI;
            if (modeButtonText != null) modeButtonText.text = "Vs IA";
        }
        else
        {
            gameMode = GameMode.TwoPlayers;
            if (modeButtonText != null) modeButtonText.text = "Vs 2 Players";
        }

        ResetBoard();
    }

    // Zera placar
    public void ResetScore()
    {
        scoreX = 0;
        scoreO = 0;

        if (scoreText != null)
            scoreText.text = "Placar - X: " + scoreX + " | O: " + scoreO;

        ResetBoard();
    }

    // Troca símbolo do Player1 (X ↔ O), só se tabuleiro estiver vazio
    public void TogglePlayerSymbol()
    {
        if (IsBoardEmpty())
        {
            player1Symbol = (player1Symbol == "X") ? "O" : "X";
            player2Symbol = (player1Symbol == "X") ? "O" : "X";

            if (symbolButtonText != null)
                symbolButtonText.text = "Player1: " + player1Symbol;

            ResetBoard();
        }
    }

    // Verifica se tabuleiro está vazio
    private bool IsBoardEmpty()
    {
        foreach (Cell cell in cells)
        {
            if (cell.GetSymbol() != "")
                return false;
        }
        return true;
    }

    // Ajusta símbolos para não serem iguais
    private void AjustarSimbolos()
    {
        if (player1Symbol == player2Symbol)
            player2Symbol = (player1Symbol == "X") ? "O" : "X";
    }
}
