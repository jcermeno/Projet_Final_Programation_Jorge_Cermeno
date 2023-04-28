using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tic_Tac_Toe
{
    public partial class Form1 : Form
    {
        private char currentPlayer = 'o'; // Joueur actuel, 'o' commence le jeu
        private int movesCount = 0; // Compteur des mouvements effectués
        private Button[,] boardButtons; // Matrice des boutons pour le plateau de jeu

        // Combinaisons gagnantes possibles
        private static readonly int[,] winCombinations = new int[,]
        {
            { 0, 1, 2 }, // Lignes
            { 3, 4, 5 },
            { 6, 7, 8 },
            { 0, 3, 6 }, // Colonnes
            { 1, 4, 7 },
            { 2, 5, 8 },
            { 0, 4, 8 }, // Diagonales
            { 2, 4, 6 }
        };


        public Form1()
        {
            InitializeComponent();
            InitializeBoard(); // Initialiser le plateau de jeu
        }

        // Initialise le plateau de jeu et assigne des événements de clic aux boutons
        private void InitializeBoard()
        {
            boardButtons = new Button[,]
            {
                { b1, b2, b3 },
                { b4, b5, b6 },
                { b7, b8, b9 }
            };

            foreach (Button button in boardButtons)
            {
                button.Click += button_Click;
                button.BackColor = Color.White;
            }

        }

        // Effectuer le mouvement de l'ordinateur (IA)
        private void MakeComputerMove()
        {
            int bestScore = int.MinValue;
            int bestMoveRow = -1;
            int bestMoveCol = -1;

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (boardButtons[row, col].Text == "")
                    {
                        boardButtons[row, col].Text = "x";// Simuler le mouvement de l'ordinateur
                        int score = Minimax(boardButtons, 0, false, int.MinValue, int.MaxValue);
                        boardButtons[row, col].Text = "";// Annuler le mouvement

                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestMoveRow = row;
                            bestMoveCol = col;
                        }
                    }
                }
            }

            if (bestMoveRow != -1 && bestMoveCol != -1)
            {
                boardButtons[bestMoveRow, bestMoveCol].PerformClick(); // Effectuer le mouvement sur le bouton correspondant
            }
        }


        // Algorithme Minimax pour trouver le meilleur mouvement
        private int Minimax(Button[,] board, int depth, bool isMaximizingPlayer, int alpha, int beta)
        {
            char opponentPlayer = (currentPlayer == 'o') ? 'x' : 'o';

            if (CheckForWin(board, opponentPlayer))
            {
                return -1; // L'adversaire gagne
            }
            if (CheckForWin(board, currentPlayer))
            {
                return 1; // Le joueur actuel gagne
            }

            if (IsBoardFull(board))
            {
                return 0; // C'est une égalité, aucun mouvement supplémentaire possible
            }

            if (isMaximizingPlayer)
            {
                int bestScore = int.MinValue;

                for (int row = 0; row < 3; row++)
                {
                    for (int col = 0; col < 3; col++)
                    {
                        if (board[row, col].Text == "")
                        {
                            board[row, col].Text = "x"; // Simuler le mouvement de l'ordinateur
                            int score = Minimax(board, depth + 1, false, alpha, beta);
                            board[row, col].Text = ""; // Annuler le mouvement

                            bestScore = Math.Max(bestScore, score);
                            alpha = Math.Max(alpha, bestScore);

                            if (beta <= alpha)
                            {
                                break; // Coupure Beta
                            }
                        }
                    }
                }

                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;

                for (int row = 0; row < 3; row++)
                {
                    for (int col = 0; col < 3; col++)
                    {
                        if (board[row, col].Text == "")
                        {
                            board[row, col].Text = "o";// Simuler le mouvement du joueur actuel
                            int score = Minimax(board, depth + 1, true, alpha, beta);
                            board[row, col].Text = "";// Annuler le mouvement

                            bestScore = Math.Min(bestScore, score);
                            beta = Math.Min(beta, bestScore);

                            if (beta <= alpha)
                            {
                                break; // Coupure Alpha
                            }
                        }
                    }
                }

                return bestScore;
            }
        }

        // Gestionnaire d'événements pour le clic sur un bouton
        private void button_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button.Text != "")
                return; // Ignorer les boutons déjà marqués

            button.Text = currentPlayer.ToString();// Marquer le bouton avec le symbole du joueur actuel
            if (currentPlayer == 'o')
            {
                button.BackColor = Color.Yellow;
            }
            else if (currentPlayer == 'x')
            {
                button.BackColor = Color.Blue; // Vous pouvez choisir n'importe quelle autre couleur pour le deuxième joueur
            }

            button.Enabled = false;

            if (CheckForWin(boardButtons, currentPlayer))
            {
                MessageBox.Show($"Le gagnant est {currentPlayer.ToString().ToUpper()}!");
                DisableBoard();
                return;
            }
            else if (movesCount == 8)
            {
                MessageBox.Show("C'est une égalité");
                DisableBoard();
                return;
            }

            currentPlayer = (currentPlayer == 'o') ? 'x' : 'o'; // Passer au joueur suivant
            movesCount++;

            if (currentPlayer == 'x')
            {
                MakeComputerMove();// C'est le tour de l'ordinateur
            }
        }

        // Vérifier s'il y a une combinaison gagnante sur le plateau
        private bool CheckForWin(Button[,] board, char player)
        {
            for (int i = 0; i < winCombinations.GetLength(0); i++)
            {
                int pos1 = winCombinations[i, 0];
                int pos2 = winCombinations[i, 1];
                int pos3 = winCombinations[i, 2];

                Button button1 = boardButtons[pos1 / 3, pos1 % 3];
                Button button2 = boardButtons[pos2 / 3, pos2 % 3];
                Button button3 = boardButtons[pos3 / 3, pos3 % 3];

                if (button1.Text == player.ToString() &&
                    button2.Text == player.ToString() &&
                    button3.Text == player.ToString())
                {
                    return true; // Il y a une combinaison gagnante
                }
            }
            return false; // Aucune combinaison gagnante
        }

        // Vérifier si le plateau est plein (tous les boutons sont marqués)
        private bool IsBoardFull(Button[,] board)
        {
            foreach (Button button in board)
            {
                if (button.Text == "")
                {
                    return false; // Il reste des boutons vides sur le plateau
                }
            }
            return true; // Le plateau est plein
        }

        // Désactiver tous les boutons du plateau
        private void DisableBoard()
        {
            foreach (Button button in boardButtons)
            {
                button.Enabled = false;
            }
        }

        // Gestionnaire d'événements pour le menu "Nouvelle partie"
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPlayer = 'o'; // Réinitialiser le joueur actuel
            movesCount = 0; // Réinitialiser le compteur de mouvements

            foreach (Button button in boardButtons)
            {
                button.Enabled = true; // Activer tous les boutons du plateau
                button.Text = ""; // Effacer le texte des boutons
                button.BackColor = Color.White; // Rétablir la couleur de fond des boutons
            }
        }

        // Gestionnaire d'événements pour le menu "Informations"
        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Ce jeu est Tic Tac Toe et l'auteur de ce jeu est Jorge Cermeno");
        }

        // Gestionnaire d'événements pour l'événement Load du formulaire
        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }
}
