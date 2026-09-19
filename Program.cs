using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Dynamic;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using static Display;
using static Players;
using static PlayGame;
using static Program;
using static System.Console;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

//Program to play Numerical Tic Tac Toe. 2 Player game with 2 modes, Human v Human or Human v Computer
//User selects the mode of play and chooses the Board size from within 3x3 and 45x45 rows x columns
//In Computer player mode program makes a winning move if available, otherwise randomly chooses a number from it's numbers and a random empty square
//First to fill a row, column or diagonal with the winning calculation of winningNum = n(n**2 +1)/2 wins the game
class Program
{
    public static int boardSize;
    public static int winningNum;
    public static int?[,] board;
    public static int totalPlays;
    public static List<int> player1Nums = new List<int>();
    public static List<int> player2Nums = new List<int>();
    public static int currentPlayer = 2;
    public static List<int> availableNumbers = player1Nums;
    public static int mode;
    public static int turns = 0;
    public static bool gameWon = false;

    // One Class method that starts the game through a menu, proceeds to later functions to set up the game, then play the game to conclusion - a winner or full board and draw.
    static void Main(string[] args)
    {
        ChoiceofGame.Menu();
    }
}
public class ChoiceofGame
{
    public static void Menu()
    {   //While loop until a correct option to start game has been selected or user exists game.
        turns = 0;
        while (mode != 1 && mode != 2)
        {
            Console.Clear();
            Console.WriteLine("Welcome to Numerical Tic Tac Toe\nHow would you like to play?");
            Console.WriteLine("1. Human v Human");
            Console.WriteLine("2. Human v Computer");
            Console.WriteLine("3. for Help/Instructions");
            Console.WriteLine("4. Exit");
            Console.WriteLine("Choose and option from 1 to 4");
            bool tryParse = int.TryParse(Console.ReadLine(), out int Choose);
            //validation of user input against options after first validating a int of not error message displayed and loop continues
            if (tryParse == true)
            {
                if (Choose == 1) { mode = 1; SizeOfBoard.Size(); }
                else if (Choose == 2) { mode = 2; SizeOfBoard.Size(); }
                else if (Choose == 3) { Help.Help_Menu(); }
                else if (Choose == 4) { Environment.Exit(0); }
                else { Console.WriteLine("Incorrect choice please try again\nPlease press <Enter> to try again");
                    Console.ReadLine(); Console.Clear(); Menu(); }
            }
            else
            {
                Console.WriteLine("Incorrect choice please try again\nPlease press <Enter> to try again");
                Console.ReadLine();
                Menu();
            }
        }
    }
}
public class Help
{   //Simple help display user can access in game to review how to play.
    public static void Help_Menu()
    {
        Console.WriteLine("\nWelcome to help for Numerical Tic-Tac-Toe");
        Console.WriteLine("-----------------------------------");
        Console.WriteLine("Your objective is to select one of your numbers to win\n");
        Console.WriteLine("  1 | 2 | 3 ");
        Console.WriteLine(" -----------");
        Console.WriteLine("  4 | 5 | 6 ");
        Console.WriteLine(" -----------");
        Console.WriteLine("  7 | 8 | 9 ");
        Console.WriteLine("\nIn this example first to fill a row, column, or diagonal adding up to 15 wins");
        Console.WriteLine("Winning rows in this example are both diagonals and the middle row and middle column (the crosses)");
        Console.WriteLine("Press any key to return to game or menu");
        Console.ReadKey();
    }
}
public class PlayGame
{
    // Game continues until the board is complete or a row, column or diagnoal are filled with a winning amount. Players switch each loop between Player 1 and Player2/Computer dependent on mode

    public static void Playing()
    {

        Player1 player1 = new Player1();
        Player2 player2 = new Player2();
        Computer computer = new Computer();
        Console.Clear();
        Display_Board();
        turns += 1;
        while (totalPlays >= turns && !gameWon)
        {
            PlayerChange();
            if (mode == 1)
            {
                if (currentPlayer == 1) { player1.Player_Move(); }
                else { player2.Player_Move(); }
            }
            else if (mode == 2)
            {
                if (currentPlayer == 1) { player1.Player_Move(); }
                else { computer.Player_Move(); }
            }         
        }
        if (!gameWon)
        {
            Console.WriteLine("No more moves available. Players are tied");
            EndMessage();
        }
    }
    public static void EndMessage()
    {   //Method to reset game variables, lists, and 2d arrays and return to main menu to play again
        player1Nums.Clear();
        player2Nums.Clear();
        Array.Clear(board, 0, board.Length);
        mode = 0;
        boardSize = 0;
        winningNum = 0;
        totalPlays = 0;
        currentPlayer = 2;
        gameWon = false;
        // how to end playgame and loop to allow return to menu
        Console.WriteLine("Press any key to return to the menu");
        Console.ReadKey();
        ChoiceofGame.Menu();
    }
    //Method to produce a message is a player wins the game and to reset the game calling the EndMessage method.
    public static void Winner()
    {
            gameWon = true;
            Console.WriteLine($"\nWinner winner!! Game over the winning sum of {Program.winningNum} has been reached!");
            EndMessage();
    }
    //Method to calculate whether a player has won the game.  Also used by the computer player to identify winning moves
    public static bool CheckWin(int Testrow, int Testcol)
    {   //Checks rows for a winner and returns true
        int? rowSum = 0;
        for (int col = 0; col < boardSize; col++)
        {
            if (board[Testrow, col] == null) { rowSum = null; break; }
            rowSum += board[Testrow, col];
        }
        if (rowSum == winningNum) { return true; }
        // Checks columns for a winner and returns true
        int? colSum = 0;
        for (int row = 0; row < boardSize; row++)
        {
            if (board[row, Testcol] == null) { colSum = null; break; }
            colSum += board[row, Testcol];
        }
        if (colSum == winningNum) { return true; }
        // Checks diagonal from index 0:0 to n:n for winner and returns true
        if (Testrow == Testcol)
        {
            int? diagonalSum = 0;
            for (int i = 0; i < boardSize; i++)
            {
                if (board[i, i] == null) { diagonalSum = null; break; }
                diagonalSum += board[i, i];
            }
            if (diagonalSum == winningNum) { return true; }
        }
        // Checks the opposite diagonal from index n:0 to 0:n for winner and returns true
        if (Testrow + Testcol == boardSize - 1)
        {
            int? oppDiagonalSum = 0;
            for (int i = 0; i < boardSize; i++)
            {
                if (board[i, boardSize - 1 - i] == null) { oppDiagonalSum = null; break; }
                oppDiagonalSum += board[i, boardSize - 1 - i];
            }
            if (oppDiagonalSum == winningNum) { return true; }
        }
        return false;
    }
    //Method to switch players each loop and the list that is used each turn to align odds to Player 1 and evens to Player2/Computer
    public static void PlayerChange()
    {
            if (currentPlayer == 1) { currentPlayer = 2; availableNumbers = player2Nums; }
            else { currentPlayer = 1; availableNumbers = player1Nums; }
    }
    
}
public class Display
{   //Class/Method to display the board, null or 0 squares are rendered blank
    public static void Display_Board()
    {
        Console.Write("     ");
        for (int col = 1; col <= boardSize; col++)
            if (col <= 9) { Console.Write($"   {col} "); }
            else { Console.Write($" {col}  "); }
        Console.WriteLine("\n    " + new string('-', (boardSize * 5) + 1));

        for (int row = 0; row < boardSize; row++)
        {
            if (row <= 8) { Console.Write($" {row + 1}   | "); }
            else { Console.Write($" {row + 1}  | "); }
            for (int col = 0; col < boardSize; col++)
            {
                string squareNum = board[row, col].HasValue ? board[row, row == row ? col : col].ToString() : " ";
                if (board[row, col].HasValue && board[row, col] != 0)
                {
                    Console.Write($"{squareNum?.PadRight(2)}| ");
                }
                else
                {
                    Console.Write("   | ");
                }
            }
            Console.WriteLine("\n    " + new string('-', (boardSize * 5) + 1));
        }
    }
}
public class SizeOfBoard
{
    //Class and method to create the board size, calculate the winning amount, create the lists of odds and even numbers and calculate total squares to fill on the board.
    public static void Size()
    {
        while (true)
        {
            Console.WriteLine("\nWould you like to play the standard 3 x 3 or enter a larger number x number (45 or less) game?\n");
            bool tryParse = int.TryParse(Console.ReadLine(), out int Option);
            //Validation of input are in and within rendering available on a full screen with a minimum of 3x3 and largest 45x45 rows and columns
            if (tryParse == true)
                if (Option >= 3 && Option <= 45)
                {
                    boardSize = Option;
                    board = new int?[boardSize, boardSize];
                    winningNum = boardSize * ((boardSize * boardSize) + 1) / 2;
                    totalPlays = boardSize * boardSize;

                    int a; a = 1;
                    while (a <= totalPlays) { player1Nums.Add(a); a += 2; }

                    int b; b = 2;
                    while (b <= totalPlays) { player2Nums.Add(b); b += 2; }
                    PlayGame.Playing();
                }
                else
                {
                    Console.WriteLine("Incorrect choice please try again\nPlease press any key to try again\n");
                    Console.ReadKey();
                }
        }
    }
}
//Players class with inherited methods to be used by Player1 and Player2 with an override for Computer.
abstract public class Players
{
    public virtual void Player_Move()
    {   //Method called and data validated to be int, within range, and a current number available in the players list of numbers to use
        GetValidSquare();
        static int GetValidSquare()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine($"\nPlayer's {currentPlayer} Available numbers: {string.Join(", ", availableNumbers)}\n");
                    Console.WriteLine($"You need a row, column or diagonal to add up to {Program.winningNum} to win. Choose an empty square.\n");
                    Console.Write($"Please enter the row number: ");
                    bool tryRow = int.TryParse(Console.ReadLine(), out int row);
                    if (tryRow) { if (row > 0 && row <= boardSize) { row = row - 1; } }
                    else { Console.WriteLine("Invalid input, must be a number no letters or characters try again:\n"); }

                    Console.Write("Please enter the column number: ");
                    bool tryCol = int.TryParse(Console.ReadLine(), out int column);
                    if (tryCol) { if (column > 0 && column <= boardSize) column = column - 1; }
                    else { Console.WriteLine("Invalid input, must be a number and within range try again:\n"); }

                    //validation that the square selected does not already have data and producing and error if it does requesting new input of a valid square, otherwise the move is made.
                    if (board[row, column] == 0 || board[row, column] == null)
                    {
                        int number = GetValidNumber();
                        board[row, column] = number;
                        availableNumbers.Remove(number);
                        Console.Clear();
                        Display_Board();
                        Console.WriteLine($"Previous player move placed {number} onto the board");
                        Console.WriteLine("Press any key to continue\n"); Console.ReadKey();
                        if (CheckWin(row, column)) { Winner(); }
                        else { Playing(); }
                    }
                    else { Console.WriteLine($"Square must be empty. Please try again.\n"); }


                } //Exception if input from user is out of index range
                catch (IndexOutOfRangeException ex)
                {
                    Console.WriteLine($"Invalid input, {ex.Message}\nSquare must be empty and within the {boardSize} rows and {boardSize} columns.\n");
                }
            }
        }
        //Method to validate user input for a number to use is an int and to verify is a number available to play in users list of numbers.
        static int GetValidNumber()
        {
            int number;
            while (true)
            {
                Console.Write($"Choose an available number to play or F1 fo Help:\n");
                while (true)
                {
                    // true hides the pressed key from printing in the console
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                    if (keyInfo.Key == ConsoleKey.F1) { Help.Help_Menu(); }
                    else if (int.TryParse(Console.ReadLine(), out number) && availableNumbers.Contains(number))
                    {
                        return number;
                    }
                    else { Console.WriteLine("Invalid choice. Please select from your remaining list of available numbers:\n"); }
                }
            }
        }
    }
}
public class Player1 : Players { }
public class Player2 : Players { }
public class Computer : Players
{
    //Method to find a winning move. For loops used to iterate through squares that are empty and test if numbers in Computers list are a winner, if so the winning move it completed, otherwise the move is undone.
    public (int row, int col, int winNum)? WinningMove()
    {
        for (int row = 0; row < Program.boardSize; row++)
        {
            for (int col = 0; col < Program.boardSize; col++)
            {
                if (board[row, col] == 0 || board[row, col] == null)
                {
                    foreach (int n in availableNumbers)
                    {
                        board[row, col] = n;

                        if (CheckWin(row, col))
                        {
                            availableNumbers.Remove(n);
                            Console.Clear();
                            Display_Board();
                            Console.WriteLine($"The Computer has placed {n} onto the board\n");
                            Winner();
                        }
                        else { board[row, col] = 0; }
                    }
                }
            }
        }
        return null;
    }
    //Method override of the inherited methods for human players to enable first a winning move to be identified if not play a valid move by randomly selecting an available number then randomly an empty square.
    public override void Player_Move()
    {
        if (WinningMove() != null) { WinningMove(); }
        else
        {
            int ranIndex = Random.Shared.Next(availableNumbers.Count);
            int number = availableNumbers[ranIndex];
            var emptySquares = new List<(int X, int Y)>();
            for (int row = 0; row < (boardSize - 1); row++)
            {
                for (int col = 0; col < (boardSize - 1); col++)
                {
                    if (board[row, col] == 0 || board[row, col] == null) { emptySquares.Add((row, col)); }
                }
            }
            Random random = new Random();
            int index = random.Next(emptySquares.Count);
            var randomItem = emptySquares[index];
            int r = randomItem.X;
            int c = randomItem.Y;
            board[r, c] = number;
            Console.Clear();
            Display_Board();
            Console.WriteLine($"The Computer has placed {number} onto the board\n");
            if (CheckWin(r, c))
            {
                Winner();
                availableNumbers.Remove(number);
                emptySquares.Clear();
            }
            else
            {
                Console.WriteLine("Press any key to continue\n"); Console.ReadLine();
                Playing();
            }
        }
    }
}






        