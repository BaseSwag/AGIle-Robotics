using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AGIle_Robotics;
using AGIle_Robotics.Extension;
using AGIle_Robotics.Interfaces;
using SuperTuple;
using Newtonsoft.Json;
using TicTacToe_Game;

namespace TestConsole
{
    class Program
    {
        static Trainer Trainer;
        static Task Task;
        static bool load = true;

        static void Main(string[] args)
        {
            var trainer = Trainer.Deserialize(System.IO.File.ReadAllText(@"D:\trainer.json"));
            var json = trainer.Best.Serialize();
            System.IO.File.WriteAllText(@"D:\network.json", json);
        }

        static async Task ReportStatus(Task task, bool anyway = false)
        {
            while (!task.IsCompleted || anyway)
            {
                Console.Clear();
                Console.WriteLine($"Transition ratio: {Trainer.StatusUpdater.TransitionRatio}");
                Console.WriteLine($"Random ratio: {Trainer.StatusUpdater.RandomRatio}");
                Console.WriteLine($"Mutation ratio: {Trainer.StatusUpdater.MutationRatio}");
                Console.WriteLine($"Creation ratio: {Trainer.StatusUpdater.CreationRatio}");
                Console.WriteLine($"Population count: {Trainer.StatusUpdater.PopulationCount}");
                Console.WriteLine($"Network count: {Trainer.StatusUpdater.NetworkCount}");
                Console.WriteLine($"Level: {Trainer.StatusUpdater.GenerationLevel}");
                Console.WriteLine($"Activity: {Trainer.StatusUpdater.Activity}");
                Console.WriteLine($"Best fitness: {Trainer.StatusUpdater.BestFitness}");
                Console.WriteLine($"Evaluations running: {Trainer.StatusUpdater.EvaluationsRunning}");
                Console.WriteLine($"Evaluations left: {Trainer.StatusUpdater.EvaluationsLeft}");
                Console.WriteLine($"Networks evolved: {Trainer.StatusUpdater.NetworksEvolved}");
                await Task.Delay(500);
            }
        }


        static Random random = new Random();
        static Task<STuple<double, double>> FitnessFunction(INeuralNetwork n1, INeuralNetwork n2)
        {
            #region sequence
            /*
            for (int n = 0; n < 10; n++)
            {
                Random random = new Random();
                var options = Enumerable.Range(0, Amount).ToList();
                var test = new double[Amount];
                for (int i = 0; i < Amount; i++)
                {
                    int r = random.Next(options.Count);
                    var next = (double)options[r];
                    next = Extensions.Map(next, 0, (double)Amount, -1, 1);
                    test[i] = next;
                    options.RemoveAt(r);
                }
                if (test.Length != Amount)
                {
                    Console.WriteLine("wtf!");
                }
                var result = network.Activate(test);
                for (int i = 0; i < Amount; i++)
                {
                    double loss = Math.Abs(result[i] - test[i]);
                    fitness -= loss;
                    if (debug && n == 0)
                        Console.WriteLine($"{test[i]} | {result[i]}");
                }
            }
            */
            #endregion
            #region XOR
            /*
            var tests = new Dictionary<double[], double>
            {
                {
                    new double[] { 0, 0},
                    0
                },
                {
                    new double[] { 1, 0},
                    1
                },
                {
                    new double[] { 0, 1},
                    1
                },
                {
                    new double[] { 1, 1},
                    0
                },
            };

            foreach(var kv in tests)
            {
                var result = network.Activate(kv.Key);
                double difference = result[0] - kv.Value;
                difference = -Math.Abs(difference);
                fitness += difference;
                if (debug)
                {
                    Console.WriteLine($"{kv.Key[0]} {kv.Key[1]} | {result[0]}");
                }
            }
            */
            #endregion XOR
            #region linear
            /*
            for (int i = 0; i < 10; i++)
            {
                double a, b, x;
                lock (random)
                {
                    a = Math.Round(random.NextDouble() / 2, 2);
                    b = Math.Round(random.NextDouble() / 2, 2);
                    x = Math.Round(random.NextDouble(), 2);
                }
                var res = network.Activate(new double[] { a, b, x });
                var correct = (a * x + b);

                fitness -= Math.Abs(res[0] - correct);

                if (debug)
                {
                    Console.WriteLine($"{a} * {x} + {b} = {correct} | {res[0]}");
                }
            }
            */
            #endregion
            #region TicTacToe
            Game game = new Game();
            Game.GameState gameState = Game.GameState.Undetermined;

            bool n1turn = true;

            while (gameState == Game.GameState.Undetermined)
            {
                INeuralNetwork network = n1turn ? n1 : n2;
                Game.BoardStates side = n1turn ? Game.BoardStates.O : Game.BoardStates.X;
                gameState = DoAITurn(ref game, network, side);

                n1turn = !n1turn;
            }

            (double, double) fitness = (0, 0);
            if (gameState == Game.GameState.Win)
            {
                if (n1turn)
                {
                    fitness = (-1, 1);
                }
                else
                {
                    fitness = (1, -1);
                }
            }

            #endregion

            var tcs = new TaskCompletionSource<STuple<double,double>>();
            tcs.SetResult(fitness);
            return tcs.Task;
        }

        private static double[] BoardToArray(Game.BoardStates[,] boardStates, Game.BoardStates side)
        {
            double[] result = new double[9];
            int counter = 0;
            foreach(var field in boardStates)
            {
                double f = 0;
                if (field == side)
                    f = 1;
                if (field == Game.BoardStates.Unassigned)
                    f = 0;
                else
                    f = -1;
                result[counter++] = f;
            }
            return result;
        }

        static void HumanTest(INeuralNetwork enemy)
        {
            Console.Clear();
            Game game = new Game();
            Game.GameState gameState = Game.GameState.Undetermined;

            bool playerturn = true;

            while (gameState == Game.GameState.Undetermined)
            {
                if (!playerturn)
                {
                    gameState = DoAITurn(ref game, enemy, Game.BoardStates.X);
                }
                else
                {
                    var newGameState = Game.GameState.Error;
                    while (newGameState == Game.GameState.Error)
                    {
                        int tile = -1;
                        int.TryParse(Console.ReadLine(), out tile);
                        newGameState = game.NewMove((Game.TileNumber)tile);
                    }
                    gameState = newGameState;
                }
                playerturn = !playerturn;
                Console.WriteLine();
                VisualizeBoard(game.GetBoard());
                Console.WriteLine();
            }

            if(gameState == Game.GameState.Win)
            {
                string winner = playerturn ? "Bot" : "Player";
                Console.WriteLine();
                Console.WriteLine(winner + " wins!");
            }
            else if(gameState == Game.GameState.Tie)
            {
                Console.WriteLine();
                Console.WriteLine("Tie!");
            }
            Console.ReadLine();
        }

        static void AITest(INeuralNetwork n1, INeuralNetwork n2)
        {
            Console.Clear();
            Game game = new Game();
            Game.GameState gameState = Game.GameState.Undetermined;

            bool turn = true;

            while (gameState == Game.GameState.Undetermined)
            {
                Thread.Sleep(250);
                if (turn)
                {
                    gameState = DoAITurn(ref game, n1, Game.BoardStates.O);
                }
                else
                {
                    gameState = DoAITurn(ref game, n2, Game.BoardStates.X);
                }
                turn = !turn;
                Console.WriteLine();
                VisualizeBoard(game.GetBoard());
                Console.WriteLine();
            }

            if(gameState == Game.GameState.Win)
            {
                string winner = turn ? "n2" : "n1";
                Console.WriteLine();
                Console.WriteLine(winner + " wins!");
            }
            else if(gameState == Game.GameState.Tie)
            {
                Console.WriteLine();
                Console.WriteLine("Tie!");
            }
            Console.ReadLine();
        }

        static Game.GameState DoAITurn(ref Game game, INeuralNetwork network, Game.BoardStates side)
        {
            var input = BoardToArray(game.GetBoard(), side);
            var output = network.Activate(input);

            var newGameState = Game.GameState.Error;
            List<int> chosenTiles = new List<int>();
            while (newGameState == Game.GameState.Error)
            {
                int tile = -1;
                double highest = double.MinValue;
                for (int i = 0; i < output.Length; i++)
                {
                    if (output[i] > highest && !chosenTiles.Contains(i))
                    {
                        highest = output[i];
                        tile = i;
                    }
                }
                chosenTiles.Add(tile);
                tile++;

                newGameState = game.NewMove((Game.TileNumber)tile);
            }
            return newGameState;
        }

        static void VisualizeBoard(Game.BoardStates[,] boardStates)
        {
            for (int x = 0; x < boardStates.GetLength(0); x++)
            {
                for (int y = 0; y < boardStates.GetLength(1); y++)
                {
                    string field = "-";
                    switch(boardStates[x, y])
                    {
                        case Game.BoardStates.O:
                            field = "O";
                            break;
                        case Game.BoardStates.X:
                            field = "X";
                            break;
                    }
                    Console.Write(" " + field);
                }
                Console.WriteLine();
            }
        }
    }
}
