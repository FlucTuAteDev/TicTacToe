﻿using TicTacToe.ConsoleHelpers;
using Utilities;

namespace TicTacToe {
    interface IRenderer {
        void DrawGreeting(string message);
        void DrawWin(Player winner);

        void DrawBoard();
        void DrawCell(Cell cell);

        void DrawError(string message);
        void DrawPrompt(string message);
    }

    class ConsoleRenderer : IRenderer {
        public ConsoleRenderer() {
            InitConsoleWindow();
            offset = new Vector(
                0,
                (Console.WindowWidth - (Board.size * cellDimensions.Col + Board.size + 1)) / 2
            );
        }

        public void DrawCell(Cell cell) {
            ErrorCleanup();
            Vector start = cell.Position * (cellDimensions + 1) + offset;
            Console.SetCursorPosition(start.Col, start.Row);

            SetBorderColors(cell);
            Console.Write(horizontalBorder);

            var shape = shapes[cell.State];
            for (int i = 0; i < cellDimensions.Row; i++) {
                Console.SetCursorPosition(start.Col, start.Row + i + 1);
                Console.Write("|"); // Vertical border

                Console.ForegroundColor = shape.Color;
                Console.Write(shape.Rows[i]);

                SetBorderColors(cell);
                Console.Write("|");
            }
            Console.SetCursorPosition(start.Col, start.Row + cellDimensions.Row + 1);
            Console.Write(horizontalBorder);
        }

        public void DrawBoard() {
            ErrorCleanup();
            Console.Clear();
            foreach (Cell cell in GameController.board.Cells) {
                DrawCell(cell);
            }
        }

        public void DrawPrompt(string message) {
            ErrorCleanup();
            promptRegion.Clear();
            promptRegion.WriteLine($"{message}: ");
            promptRegion.Flush();
            promptRegion.ClearBuffer();
        }

        public void DrawError(string message) {
            ErrorCleanup();
            SaveConsoleState();
            errorRegion.Write(message, ConsoleColor.Red);
            errorRegion.Flush();
            errorRegion.ClearBuffer();
            RestoreConsoleState();
        }

        public void DrawGreeting(string message) {
            SaveConsoleState();

            ConsoleRegion greetingRegion = new(0, 0, 120, 30, Justify.CenterMiddle);
            foreach (char character in message) {
                greetingRegion.Write(character.ToString(), ConsoleColor.Green);
                greetingRegion.Flush(false);
                //Console.Write(character);
                Thread.Sleep(20);
            }

            RestoreConsoleState();
        }

        public void DrawWin(Player winner) {
            ConsoleRegion winRegion = new(0, 0, 120, 30, Justify.CenterMiddle);
            winRegion.Write($"{(winner.Team == CellState.X ? 'X' : 'O')} won!", ConsoleColor.White);
            winRegion.Flush();
        }

        private static (int Left, int Top) cursorPosition;
        private static ConsoleColor textColor;

        private static void SaveConsoleState() {
            cursorPosition = Console.GetCursorPosition();
            textColor = Console.ForegroundColor;
        }

        private static void RestoreConsoleState() {
            Console.SetCursorPosition(cursorPosition.Left, cursorPosition.Top);
            Console.ForegroundColor = textColor;
        }

        private static void SetBorderColors(Cell cell) {
            Console.ForegroundColor = (GameController.board.SelectedCell == cell) ? ConsoleColor.Green : ConsoleColor.White;
        }

        private static void ErrorCleanup() {
            SaveConsoleState();
            errorRegion.Clear();
            RestoreConsoleState();
        }

        static ConsoleRenderer() {
            // Add paddings to the shapes with respect to the cell's dimensions
            // Assuming same length for all rows
            int padColsLen = (cellDimensions.Col - shapes[CellState.Empty].Rows[0].Length) / 2;
            string padCols = new(' ', padColsLen);

            foreach (var (_, shape) in shapes) {
                for (int i = 0; i < shape.Rows.Length; i++) {
                    shape.Rows[i] = padCols + shape.Rows[i] + padCols;
                }
            }
        }

        private readonly Vector offset;
        private static readonly Vector cellDimensions = new(4, 8);

        private static readonly ConsoleRegion errorRegion = new(29, 0, 120, 1, Justify.TopRight);
        private static readonly ConsoleRegion promptRegion = new(0, 0, 120, 28, Justify.CenterMiddle);

        private readonly static Dictionary<CellState, Shape> shapes = new() {
            { CellState.X, new Shape(
                new string[] { "\\  /", " \\/ ", " /\\ ", "/  \\" },
                ConsoleColor.Red) },
            { CellState.O, new Shape(
                new string[] { " -- ", "|  |", "|  |", " -- " },
                ConsoleColor.Green) },
            { CellState.Empty, new Shape(
                new string[] { "    ", "    ", "    ", "    " },
                ConsoleColor.Gray)
            }
        };
        private static readonly string horizontalBorder = "+" + new string('-', cellDimensions.Col) + "+";

        private static void InitConsoleWindow() {
            //Console.WindowWidth = 120;
            //Console.WindowHeight = 30;
            //Console.CursorVisible = false;
        }

        struct Shape {
            public string[] Rows;
            public ConsoleColor Color;

            public Shape(string[] rows, ConsoleColor color) {
                Rows = rows;
                Color = color;
            }
        }
    }
}