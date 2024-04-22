using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Media;
using WMPLib;
using System.Diagnostics;

namespace SnakeGamePlatform
{
    public class GameEvents : IGameEvents
    {
        //Define game variables here! for example...
        //GameObject [] snake;
        TextLabel LblScore;
        TextLabel LblMaxScore;
        TextLabel LblMaxScorePosition;
        TextLabel LblPointsPosition;
        GameObject Food;
        GameObject[] Snake;
        GameObject[] Gvulot;
        Random AppleRND = new Random();
        int SnakeVelocity;
        int SnakeSize;
        bool IsPlaying;
        int CounterPoints;
        //This function is called by the game one time on initialization!
        //Here you should define game board resolution and size (x,y).
        //Here you should initialize all variables defined above and create all visual objects on screen.
        //You could also start game background music here.
        //use board Object to add game objects to the game board, play background music, set interval, etc...
        public void GameInit(Board board)
        {
            CounterPoints = 0;
            Snake = new GameObject[100];
            SnakeVelocity = 200;
            IsPlaying = true;

            //Setup board size and resolution!
            Board.resolutionFactor = 1;
            board.XSize = 600;
            board.YSize = 800;

            //Adding a text label to the game board.


            //Adding Game Object

            board.SetBackgroundImage(Properties.Resources.yaar1);


            Position foodPosition = new Position(400, 750);
            Food = new GameObject(foodPosition, 30, 30);
            Food.SetImage(Properties.Resources.dr);
            Food.direction = GameObject.Direction.RIGHT;
            board.AddGameObject(Food);

            Position snakePosition = new Position(300, 800);
            Snake[0] = new GameObject(snakePosition, 30, 30);
            Snake[0].SetImage(Properties.Resources.garg);
            Snake[0].direction = GameObject.Direction.LEFT;
            board.AddGameObject(Snake[0]);

            Position snake2Position = new Position(400, 800);
            Snake[1] = new GameObject(snakePosition, 30, 30);
            Snake[1].SetImage(Properties.Resources.garg);
            Snake[1].direction = GameObject.Direction.LEFT;
            board.AddGameObject(Snake[1]);

            SnakeSize = 2;

            gvulut(board);

            Position pointsPosition = new Position(100, 560);
            LblPointsPosition = new TextLabel("points:", pointsPosition);
            LblPointsPosition.SetFont("Ariel", 14);
            board.AddLabel(LblPointsPosition);

            Position labelPosition = new Position(100, 620);
            LblScore = new TextLabel("0", labelPosition);
            LblScore.SetFont("Ariel", 14);
            board.AddLabel(LblScore);

            

            //Play file in loop!
            board.PlayBackgroundMusic(@"\Images\gameSound.wav");
            //Play file once!
            board.PlayShortMusic(@"\Images\eat.wav");


            //Start game timer!
            board.StartTimer(SnakeVelocity);

        }


        //This function is called frequently based on the game board interval that was set when starting the timer!
        //Use this function to move game objects and check collisions
        public void GameClock(Board board)
        {



            //תזוזה של הנחש
            MoveSnake();




            //מיקום התפוח ומהירותו
            Moveappleandsnakevelocity(board);


            //בודק עם הנחש יוצא מגבולות המשחק או פוגע בעצמו
            Isoutofboards(board);



        }

        //This function is called by the game when the user press a key down on the keyboard.
        //Use this function to check the key that was pressed and change the direction of game objects acordingly.
        //Arrows ascii codes are given by ConsoleKey.LeftArrow and alike
        //Also use this function to handle game pause, showing user messages (like victory) and so on...
        public void KeyDown(Board board, char key)
        {
            if (key == (char)ConsoleKey.LeftArrow && Snake[0].direction != GameObject.Direction.RIGHT)
                Snake[0].direction = GameObject.Direction.LEFT;
            if (key == (char)ConsoleKey.RightArrow && Snake[0].direction != GameObject.Direction.LEFT)
                Snake[0].direction = GameObject.Direction.RIGHT;
            if (key == (char)ConsoleKey.DownArrow && Snake[0].direction != GameObject.Direction.UP)
                Snake[0].direction = GameObject.Direction.DOWN;
            if (key == (char)ConsoleKey.UpArrow && Snake[0].direction != GameObject.Direction.DOWN)
                Snake[0].direction = GameObject.Direction.UP;

            // אם המשתמש לוחץ על מקש הרווח כאשר הוא נפסל, המשחק מתחיל מחדש
            StartAgain(board, key);


        }






        //פעולות עזר

        //הפעולה מזיזה את הנחש
        public void MoveSnake()
        {

            for (int i = SnakeSize - 1; i > 0; i--)
            {
                Snake[i].SetPosition(Snake[i - 1].GetPosition());
            }

            Position snakePosition = Snake[0].GetPosition();
            if (Snake[0].direction == GameObject.Direction.RIGHT)
                snakePosition.Y = snakePosition.Y + 30;
            else if (Snake[0].direction == GameObject.Direction.LEFT)
                snakePosition.Y = snakePosition.Y - 30;
            else if (Snake[0].direction == GameObject.Direction.UP)
            {
                snakePosition.X = snakePosition.X - 30;
            }
            else
            {
                snakePosition.X = snakePosition.X + 30;
            }
            Snake[0].SetPosition(snakePosition);
        }

        //הפעולה בודקת עם הנחש אוכל תפוח ואם הוא אוכל היא מגדילה אותו
        public void SnakeGetBigger(Board board)
        {
            Position newSnakeObjectPosition = Snake[SnakeSize - 1].GetPosition();
            MoveSnake();
            Snake[SnakeSize] = new GameObject(newSnakeObjectPosition, 30, 30);
            Snake[SnakeSize].SetImage(Properties.Resources.garg);
            board.AddGameObject(Snake[SnakeSize]);
            SnakeSize++;

            CounterPoints += 20;
            LblScore.SetText(CounterPoints.ToString());

        }
        // הפעולה בודקת אם הנחש יצא מהגבולות
        public void Isoutofboards(Board board)
        {
            if (Snake[0].IntersectWith(Gvulot[0]) || Snake[0].IntersectWith(Gvulot[1]) || Snake[0].IntersectWith(Gvulot[2]) || Snake[0].IntersectWith(Gvulot[3]))
            {
                StopGame(board);
                if (CounterPoints > CounterMaxPoints)
                {
                    CounterMaxPoints = CounterPoints;
                    LblMaxScore.SetText(CounterMaxPoints.ToString());
                }
            }

            for (int i = 1; i < SnakeSize - 1; i++)
            {
                if (Snake[0].IntersectWith(Snake[i + 1]))
                {
                    StopGame(board);
                    if (CounterPoints > CounterMaxPoints)
                    {
                        CounterMaxPoints = CounterPoints;
                        LblMaxScore.SetText(CounterMaxPoints.ToString());
                    }
                }
            }
        }

        public void StopGame(Board board)
        {
            board.StopTimer();
            //הצגת הודעה מתאימה
            Position labelPosition = new Position(400, 800);
            LblScore = new TextLabel("you lost, loser!! hit spacebar to play again", labelPosition);
            LblScore.SetFont("Ariel", 14);
            board.AddLabel(LblScore);
            IsPlaying = false;

        }
        //הפעולה משנה את מיקון התפוח ואת המהירות של הנחש
        public void Moveappleandsnakevelocity(Board board)
        {
            if (Food.IntersectWith(Snake[0]))
            {

                int foodpositionX = AppleRND.Next(200, 650);
                int foodpositionY = AppleRND.Next(600, 1200);
                Position foodposition = new Position(foodpositionX, foodpositionY);
                Food.SetPosition(foodposition);
                if (SnakeVelocity > 0)
                {
                    SnakeVelocity -= 3;
                }
                board.StopTimer();
                board.StartTimer(SnakeVelocity);
                SnakeGetBigger(board);
            }
        }
        //הפעולה מתחילה את המשחק מחדש
        public void StartAgain(Board board, char key)
        {
            if (key == ' ' && !IsPlaying)
            {
                board.RemoveAll();
                GameInit(board);
            }
        }
        //הפעולה יוצרת את גבולות המשחק
        public void gvulut(Board board)
        {
            Gvulot = new GameObject[4];

            Position boarddown = new Position(775, 500);
            Gvulot[0] = new GameObject(boarddown, 800, 25);
            Gvulot[0].SetBackgroundColor(Color.Black);
            board.AddGameObject(Gvulot[0]);

            Position boardup = new Position(175, 500);
            Gvulot[1] = new GameObject(boardup, 800, 25);
            Gvulot[1].SetBackgroundColor(Color.Black);
            board.AddGameObject(Gvulot[1]);

            Position boardright = new Position(200, 1275);
            Gvulot[2] = new GameObject(boardright, 25, 600);
            Gvulot[2].SetBackgroundColor(Color.Black);
            board.AddGameObject(Gvulot[2]);

            Position boardleft = new Position(200, 500);
            Gvulot[3] = new GameObject(boardleft, 25, 600);
            Gvulot[3].SetBackgroundColor(Color.Black);
            board.AddGameObject(Gvulot[3]);




        }
    }
}




