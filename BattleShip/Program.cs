using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BattleShipLibrary;
using BattleShipLibrary.Models;
namespace BattleShip
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WelcomeMessage();
            PlayerInfoModel activePlayer = CreatePlayer("Player 1");
            PlayerInfoModel opponent = CreatePlayer("Player 2");
            PlayerInfoModel winner = null;


            do
            {
                DisplayShotGrid(activePlayer);

                RecordPlayerShot(activePlayer, opponent);


                bool doesGameContinue = GameLogic.PlayerStillActive(opponent);


                if (doesGameContinue == true)
                {


                    (activePlayer, opponent) = (opponent, activePlayer);
                }
                else
                {
                    winner = activePlayer;

                }

            } while (winner == null);

            IdentifyWinner(winner);

            Console.ReadLine();
        }

        private static void IdentifyWinner(PlayerInfoModel winner)
        {
            Console.WriteLine($"Congratualion to {winner.UsersName} for winning");
            Console.WriteLine($"{winner.UsersName} took {GameLogic.GetShotCount(winner)} shots.");
        }

        private static void RecordPlayerShot(PlayerInfoModel activePlayer, PlayerInfoModel opponent)
        {
            bool isValidShot = false;
            string row = "";
            int column = 0;
            do
            {
                string shot = AskForShot(activePlayer);
                try
                {
                    (row, column) = GameLogic.SplitShotIntoRowAndColumn(shot);
                    isValidShot = GameLogic.ValidateShot(activePlayer, row, column);
                }
                catch (Exception ex)
                {
                    

                    isValidShot=false;

                }



                if (isValidShot == false)
                {
                    Console.WriteLine("Invalid shot location. Please try again.");
                }


            } while (isValidShot == false);

            // Determine shot result

            bool isAHit = GameLogic.IdentifyShotResult(opponent, row, column);

            GameLogic.MarkShotResult(activePlayer, row, column, isAHit);

            DisplayShotResult(row, column, isAHit);

        }

        private static void DisplayShotResult(string row, int column, bool isAHit)
        {
            if (isAHit)
            {
                Console.WriteLine($"{row}  {column} is a Hit");
            }
            else
            { Console.WriteLine($"{row}  {column} is a miss");

            }
            Console.WriteLine();
            }

        private static string AskForShot(PlayerInfoModel activePlayer)
        {
            Console.Write($"Pleae enter your shoot selection {activePlayer.UsersName}:");
            string output = Console.ReadLine();
            return output;
        }

        private static void DisplayShotGrid(PlayerInfoModel activePlayer)
        {

            string currentRow = activePlayer.ShotGrid[0].SpotLetter;

            foreach (var gridspot in activePlayer.ShotGrid)
            {
                if (gridspot.SpotLetter != currentRow)
                {
                    Console.WriteLine();
                    currentRow = gridspot.SpotLetter;
                }


                if (gridspot.Status == GridSpotStatus.Empty)
                {
                    Console.Write($" { gridspot.SpotLetter }{ gridspot.SpotNumber } ");
                }
                else if (gridspot.Status == GridSpotStatus.Hit)
                {
                    Console.Write(" X  ");
                }
                else if (gridspot.Status == GridSpotStatus.Miss)
                {
                    Console.Write(" O  ");
                }
                else
                {
                    Console.Write(" ?  "); 
                }

            }
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void WelcomeMessage()
        {
            Console.WriteLine("Welcome to Battleship");
            Console.WriteLine("Created by: Denis");
            Console.WriteLine();
        }

        private static PlayerInfoModel CreatePlayer(string playerTitle)
        {
            PlayerInfoModel output = new PlayerInfoModel();
            Console.WriteLine($"Player information for {playerTitle}");

            //ASK THE USER FOR THEIR NAME
            output.UsersName = AskForUersName();
            // CREATE THE SHIP GRID
            GameLogic.InitializeGrid(output); // This is a public static class which take one argument from PlayerInfoModel  
            //ASK THE USER FOR THEIR 5 SHIPS
            PlaceShips(output);

            Console.Clear();

            return output;
        }

        private static string AskForUersName()
        {
            Console.Write("Please enter your name: ");
            string output = Console.ReadLine();
            return output;
        }

        private static void PlaceShips(PlayerInfoModel model)
        {
            do
            {
                Console.Write($"Where do you want to place ship number {model.ShipLocations.Count + 1}:");
                string location = Console.ReadLine();


                bool isValidLocation = false;

                try
                {
                    isValidLocation = GameLogic.PlaceShip(model, location);

                }
                catch (Exception ex)
                {

                    Console.WriteLine("Error:" + ex.Message);
                }
                if (isValidLocation == false)
                {
                    Console.WriteLine("That is not a valid location");
                }



            } while (model.ShipLocations.Count < 5);
        }
    }
}
