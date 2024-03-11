using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Media;
using WMPLib;

namespace SnakeGamePlatform
{
    
    public class GameEvents:IGameEvents
    {
        //Define game variables here! for example...
        //GameObject [] snake;
        TextLabel lblScore;
        GameObject food;
        GameObject[] snake;
        GameObject[] gvulot; 
        Random appleRND = new Random();
        int snakeVelocity;
        int snakeSize;

        //This function is called by the game one time on initialization!
        //Here you should define game board resolution and size (x,y).
        //Here you should initialize all variables defined above and create all visual objects on screen.
        //You could also start game background music here.
        //use board Object to add game objects to the game board, play background music, set interval, etc...
        public void GameInit(Board board)
        {
            snake = new GameObject[100];
            snakeVelocity = 200;

            //Setup board size and resolution!
            Board.resolutionFactor = 1;
            board.XSize = 600;
            board.YSize = 800;

            //Adding a text label to the game board.
            Position labelPosition = new Position(100, 20);
            lblScore = new TextLabel("Kuku", labelPosition);
            lblScore.SetFont("Ariel", 14);
            board.AddLabel(lblScore);

            //Adding Game Object
            Position foodPosition = new Position(200, 100);
            food = new GameObject(foodPosition, 20, 20);
            food.SetImage(Properties.Resources.food);
            food.direction = GameObject.Direction.RIGHT;
            board.AddGameObject(food);

            Position snakePosition = new Position(400, 300);
            snake[0] = new GameObject(snakePosition, 20, 20);
            snake[0].SetBackgroundColor(Color.Green);
            snake[0].direction = GameObject.Direction.LEFT;
            board.AddGameObject(snake[0]);

            Position snake2Position = new Position(400, 300);
            snake[1] = new GameObject(snakePosition, 20, 20);
            snake[1].SetBackgroundColor(Color.Green);
            snake[1].direction = GameObject.Direction.LEFT;
            board.AddGameObject(snake[1]);

            snakeSize = 2;

            gvulot = new GameObject[4];

            Position boarddown = new Position(775, 0);
            gvulot[0] = new GameObject(boarddown, 800, 25);
            gvulot[0].SetBackgroundColor(Color.Black);
            board.AddGameObject(gvulot[0]);

            Position boardup = new Position(175, 0);
            gvulot[1] = new GameObject(boardup, 800, 25);
            gvulot[1].SetBackgroundColor(Color.Black);
            board.AddGameObject(gvulot[1]);

            Position boardright = new Position(200, 775);
            gvulot[2] = new GameObject(boardright, 25, 600);
            gvulot[2].SetBackgroundColor(Color.Black);
            board.AddGameObject(gvulot[2]);

            Position boardleft = new Position(200, 0);
            gvulot[3] = new GameObject(boardleft, 25, 600);
            gvulot[3].SetBackgroundColor(Color.Black);
            board.AddGameObject(gvulot[3]);



            //Play file in loop!
            board.PlayBackgroundMusic(@"\Images\gameSound.wav");
            //Play file once!
            board.PlayShortMusic(@"\Images\eat.wav");


            //Start game timer!
            board.StartTimer(snakeVelocity);
        }


        //This function is called frequently based on the game board interval that was set when starting the timer!
        //Use this function to move game objects and check collisions
        public void GameClock(Board board)
        {
            //תזוזה של הנחש
            MoveSnake();




            //מיקום התפוח ומהירותו
            Moveappleandsnakevelocity(board);



            Isoutofboards(board);





            
            

            

        }

        //This function is called by the game when the user press a key down on the keyboard.
        //Use this function to check the key that was pressed and change the direction of game objects acordingly.
        //Arrows ascii codes are given by ConsoleKey.LeftArrow and alike
        //Also use this function to handle game pause, showing user messages (like victory) and so on...
        public void KeyDown(Board board, char key)
        {
            if (key == (char)ConsoleKey.LeftArrow)
                snake[0].direction = GameObject.Direction.LEFT;
            if (key == (char)ConsoleKey.RightArrow)
                snake[0].direction = GameObject.Direction.RIGHT;
            if (key == (char)ConsoleKey.DownArrow)
                snake[0].direction = GameObject.Direction.DOWN;
            if (key == (char)ConsoleKey.UpArrow)
                snake[0].direction = GameObject.Direction.UP;

            if (key == ' ')
            {
                board.RemoveAll();
                GameInit(board);
            }
                
        
        }

        public void MoveSnake()
        {
            
            for (int i = snakeSize-1; i > 0; i--)
            {
                snake[i].SetPosition(snake[i - 1].GetPosition());
            }
            
            Position snakePosition = snake[0].GetPosition();
            if (snake[0].direction == GameObject.Direction.RIGHT)
                snakePosition.Y = snakePosition.Y + 20;
            else if (snake[0].direction == GameObject.Direction.LEFT)
                snakePosition.Y = snakePosition.Y - 20;
            else if (snake[0].direction == GameObject.Direction.UP)
            {
                snakePosition.X = snakePosition.X - 20;
            }
            else
            {
                snakePosition.X = snakePosition.X + 20;
            }
            snake[0].SetPosition(snakePosition);
        }

        public void SnakeGetBigger(Board board)
        {
                Position newSnakeObjectPosition = snake[snakeSize-1].GetPosition();
                MoveSnake();
                snake[snakeSize] = new GameObject(newSnakeObjectPosition, 20, 20);
                snake[snakeSize].SetBackgroundColor(Color.Green);
                board.AddGameObject(snake[snakeSize]);
                snakeSize++;
        }
        public void Isoutofboards(Board board)
        {
            if (snake[0].IntersectWith(gvulot[0]) || snake[0].IntersectWith(gvulot[1]) || snake[0].IntersectWith(gvulot[2]) || snake[0].IntersectWith(gvulot[3]))
            {
                board.StopTimer();
                //הצגת הודעה מתאימה
                Position labelPosition = new Position(400, 300);
                lblScore = new TextLabel("you lost, loser!! hit spacebar to play again", labelPosition);
                lblScore.SetFont("Ariel", 14);
                board.AddLabel(lblScore);

                snakeVelocity = 200;
                char key = (char)Console.Read();
                if (key == (char)ConsoleKey.Spacebar)
                {
                    board.StartTimer(snakeVelocity);
                }
            }

            for (int i = 1; i < snakeSize - 1; i++)
            {
                if (snake[0].IntersectWith(snake[i + 1]))
                {
                    board.StopTimer();
                    Position labelPosition = new Position(400, 300);
                    lblScore = new TextLabel("you lost, loser!! hit spacebar to play again", labelPosition);
                    lblScore.SetFont("Ariel", 14);
                    board.AddLabel(lblScore);
                }
            }
        }

        public void Moveappleandsnakevelocity(Board board)
        {
            if (food.IntersectWith(snake[0]))
            {

                int foodpositionX = appleRND.Next(200, 550);
                int foodpositionY = appleRND.Next(100, 750);
                Position foodposition = new Position(foodpositionX, foodpositionY);
                food.SetPosition(foodposition);

                snakeVelocity -= 2;
                board.StopTimer();
                board.StartTimer(snakeVelocity);
                SnakeGetBigger(board);
            }
        }

        
        

    }
}
