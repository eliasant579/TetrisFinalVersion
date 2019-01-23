using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameSystemServices;

namespace Tetris_Game_Template
{
    public partial class GameScreen : UserControl
    {
        SolidBrush drawBrush = new SolidBrush(Color.White);

        int score = 0;
        int level = 1;
        int startPos = 0;
        int tempPos = 0;
        int gravityCounter = 0;
        int levelFallFreq = 6;
        Color shapeColor;
        bool tempCollision = false;

        //random generator used to get a new shape in NewShape
        Random shapeRandom = new Random();

        //character that defines the tetragram. Possible shapes are T, S, Z, I, L, J, O
        char shape;
        char nextShape;

        //point that to which the shape method referes to when defining the coordinates of the square in the tetragram
        Point shapeFondPoint;

        //I want to prove that I can include the little bump at the beginning! I'll fit the first counterClick block in the key down method, and I'll refresh

        //these booleans make the program run better, I get it :)
        Boolean leftArrowDown, downArrowDown, rightArrowDown, upArrowDown;

        //this stucture contains the squares' drawing point
        //there are extra rows an columns in order to deal with both te arrays at the same time
        Point[,] squareOrigin = new Point[12, 20];

        //Colors array. White is empty, black is outside border
        Color[,] squareColor = new Color[12, 20];

        //Shape array, contains for cells and is used to check collisions and move the current shape around. Defines tetragram's squares' coordinates
        Point[] pastShapeCoords = new Point[4];

        //I need this, sadly
        Point[] nextShapeCoords = new Point[4];

        public GameScreen()
        {
            InitializeComponent();
            InitializeGameValues();
        }

        public void InitializeGameValues()
        {
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    Point tempCoord = new Point(i, j);
                    if (i == 0 || i == 11 || j == 0 || j == 19)
                    {
                        squareColor[i, j] = Color.Black;
                    }
                    else if (pastShapeCoords.Contains(tempCoord) == true)
                    {
                        squareColor[i, j] = shapeColor;
                    }
                    else
                    {
                        squareColor[i, j] = Color.White;
                    }
                    squareOrigin[i, j] = new Point(100 + i * 21, 20 + j * 21);
                }
            }

            shape = NewShape('p');
            nextShape = NewShape(shape);
            ShapeImplement(startPos);
            gameTimer.Enabled = true;
        }

        private void GameScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // opens a pause screen is escape is pressed. Depending on what is pressed
            // on pause screen the program will either continue or exit to main menu
            if (e.KeyCode == Keys.Escape)
            {
                if (gameTimer.Enabled == true)
                {
                    gameTimer.Enabled = false;
                    rightArrowDown = leftArrowDown = upArrowDown = downArrowDown = false;

                    DialogResult result = PauseForm.Show();

                    if (result == DialogResult.Cancel)
                    {
                        gameTimer.Enabled = true;
                    }
                    else if (result == DialogResult.Abort)
                    {
                        MainForm.ChangeScreen(this, "MenuScreen");
                    }
                }
                else
                {
                    MainForm.ChangeScreen(this, "MenuScreen");
                }
            }

            //TODO - basic player 1 key down bools set below. Add remainging key down
            // required for player 1 or player 2 here.

            //player 1 button presses
            switch (e.KeyCode)
            {
                case Keys.B:
                    leftArrowDown = true;
                    break;
                case Keys.Space:
                    downArrowDown = true;
                    break;
                case Keys.M:
                    rightArrowDown = true;
                    break;
                case Keys.N:
                    upArrowDown = true;
                    break;
            }
        }

        private void GameScreen_KeyUp(object sender, KeyEventArgs e)
        {
            //TODO - basic player 1 key up bools set below. Add remainging key up
            // required for player 1 or player 2 here.

            //player 1 button releases
            switch (e.KeyCode)
            {
                case Keys.B:
                    leftArrowDown = false;
                    break;
                case Keys.Space:
                    downArrowDown = false;
                    break;
                case Keys.M:
                    rightArrowDown = false;
                    break;
                case Keys.N:
                    upArrowDown = false;
                    break;
            }
        }

        /// <summary>
        /// This is the Game Engine and repeats on each interval of the timer. For example
        /// if the interval is set to 16 then it will run each 16ms or approx. 50 times
        /// per second
        /// </summary>
        private void gameTimer_Tick(object sender, EventArgs e)
        {

            //if up arrow is pressed AND piece isn't at the boudaries' sides AND shape different to I, you are allowed to change position
            //if shape IS I it MUSTN'T be at x=8. Otherwise don't change position
            if (upArrowDown == true && shapeFondPoint.Y != 1 && shapeFondPoint.Y != 18 && shapeFondPoint.X != 1 && shapeFondPoint.X != 10 && (shape != 'I' || shapeFondPoint.X != 9 && shapeFondPoint.Y != 17) && tempCollision == false)
            {
                tempPos = (startPos + 1) % 4;
                leftArrowDown = false;
                rightArrowDown = false;
                downArrowDown = false;
            }
            else
            {
                upArrowDown = false;
            }

            gravityCounter++;

            if (gravityCounter == levelFallFreq)
            {
                shapeFondPoint.Y++;
                gravityCounter = 0;
            }
            else if (true)
            {
                if (leftArrowDown == true)
                {
                    shapeFondPoint.X--;
                }
                if (rightArrowDown == true)
                {
                    shapeFondPoint.X++;
                }
                if (downArrowDown == true)
                {
                    shapeFondPoint.Y++;
                }
            }

            Refresh();
        }

        private void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            //this block of functions rappresents the major problem. It's kinda messy
            {
                ShapeImplement(tempPos);
                CollisionCheck();
                ShapeImplement(startPos);
                DeleteRows();
            }

            //set the color to the cells belonging to the defined new shape
            for (int j = 0; j < 4; j++)
            {
                squareColor[nextShapeCoords[j].X, nextShapeCoords[j].Y] = shapeColor;
                pastShapeCoords[j] = nextShapeCoords[j];
            }

            drawBrush.Color = Color.Black;
            e.Graphics.FillRectangle(drawBrush, squareOrigin[0, 0].X, squareOrigin[0, 0].Y, 251, 419);

            //drawing process happens here
            for (int i = 1; i < 11; i++)
            {
                for (int j = 1; j < 19; j++)
                {
                    drawBrush.Color = squareColor[i, j];        //brush's color is set to the color of the cell you are drawing
                    e.Graphics.FillRectangle(drawBrush, squareOrigin[i, j].X, squareOrigin[i, j].Y, 20, 20);
                }
            }

            scoreLabel.Text = "Score: " + score;
            levelLabel.Text = "Level: " + level;
            nextShapeLabel.Text = "Next shape \n" + nextShape;

            if(gameTimer.Enabled == false)
            {
                drawBrush.Color = Color.White;
                Brush gameOverBrush = new SolidBrush(Color.Black);
                Font gameOverFont = new Font("Arial", 16, FontStyle.Bold);
                Font escapeFont = new Font("Arial", 10, FontStyle.Bold);

                e.Graphics.FillRectangle(drawBrush, 121, 186, 209, 90);

                e.Graphics.DrawString("GAME OVER", gameOverFont, gameOverBrush, 138, 200);
                e.Graphics.DrawString("Press Escape to return \n     to the main menu", escapeFont, gameOverBrush, 130, 230);
            }
        }

        public void CollisionCheck()
        {
            bool collisionValue = false;

            //last shape is erased
            for (int j = 0; j < 4; j++)
            {
                squareColor[pastShapeCoords[j].X, pastShapeCoords[j].Y] = Color.White;
            }

            //position of the shape and collision check
            for (int i = 0; i < 4; i++)
            {
                Point tempCoord = nextShapeCoords[i];

                if (tempCoord.X < 1 || tempCoord.X > 10)
                {
                    shapeFondPoint = pastShapeCoords[0];
                }

                else if (squareColor[tempCoord.X, tempCoord.Y] != Color.White)
                {
                    if (shapeFondPoint.Y == 2)
                    {
                        //game over
                        gameTimer.Enabled = false;                      
                    }

                    if (tempCoord.Y > pastShapeCoords[i].Y)
                    {
                        collisionValue = true;
                    }
                    else
                    {
                        shapeFondPoint = pastShapeCoords[0];
                    }
                }
            }

            if (collisionValue == false)
            {
                startPos = tempPos;
            }
            else
            {
                tempPos = startPos;

                if (upArrowDown == true && shape == 'I')
                {
                    tempCollision = true;
                }
                else
                {
                    tempCollision = false;
                    for (int j = 0; j < 4; j++)
                    {
                        squareColor[pastShapeCoords[j].X, pastShapeCoords[j].Y] = shapeColor;
                    }
                    shape = nextShape;
                    nextShape = NewShape(shape);
                    collisionValue = false;
                }
            }
        }

        public void DeleteRows()
        {
            bool rowCompleted;

            for (int i = 1; i < 19; i++)
            {
                rowCompleted = true;

                for (int j = 1; j < 11; j++)
                {
                    if (squareColor[j, i] == Color.White)
                    {
                        rowCompleted = false;
                    }
                }

                if (rowCompleted == true)
                {
                    for (int j = 1; j < 11; j++)
                    {
                        squareColor[j, i] = Color.White;
                    }

                    for (int k = i; k > 0; k--)
                    {
                        for (int j = 1; j < 11; j++)
                        {
                            if (k != 1)
                            {
                                squareColor[j, k] = squareColor[j, k - 1];
                            }
                            else
                            {
                                squareColor[j, k] = Color.White;
                            }
                        }
                    }
                    score++;
                    if (score % 2 == 0)
                    {
                        level++;
                        if (level <= 3)
                        {
                            levelFallFreq--;
                        }
                    }
                }
            }
        }

        public char NewShape(char lastShape)
        {
            shapeFondPoint = new Point(5, 1);
            startPos = 0;
            tempPos = 0;
            gravityCounter = 0;

            //replicating the original algorithm, according to Chad Birch's post https://gaming.stackexchange.com/questions/13057/tetris-difficulty#13129
            int shapeNumber = shapeRandom.Next(0, 8);
            switch (shapeNumber)
            {
                case 1:
                    if (lastShape == 'T')
                    {
                        goto default;
                    }
                    return 'T';
                case 2:
                    if (lastShape == 'S')
                    {
                        goto default;
                    }
                    return 'S';
                case 3:
                    if (lastShape == 'Z')
                    {
                        goto default;
                    }
                    return 'Z';
                case 4:
                    if (lastShape == 'I')
                    {
                        goto default;
                    }
                    return 'I';
                case 5:
                    if (lastShape == 'L')
                    {
                        goto default;
                    }
                    return 'L';
                case 6:
                    if (lastShape == 'J')
                    {
                        goto default;
                    }
                    return 'J';
                case 7:
                    if (lastShape == 'O')
                    {
                        goto default;
                    }
                    return 'O';
                default:
                    shapeNumber = shapeRandom.Next(1, 8);
                    switch (shapeNumber)
                    {
                        case 1:
                            return 'T';
                        case 2:
                            return 'S';
                        case 3:
                            return 'Z';
                        case 4:
                            return 'I';
                        case 5:
                            return 'L';
                        case 6:
                            return 'J';
                        default:
                            return 'O';
                    }
            }
        }

        public void ShapeImplement(int position)
        {
            switch (shape)
            {
                case 'T':
                    switch (position)
                    {
                        case 0:
                            nextShapeCoords[0] = new Point(shapeFondPoint.X, shapeFondPoint.Y);
                            nextShapeCoords[1] = new Point(shapeFondPoint.X + 1, shapeFondPoint.Y);
                            nextShapeCoords[2] = new Point(shapeFondPoint.X - 1, shapeFondPoint.Y);
                            nextShapeCoords[3] = new Point(shapeFondPoint.X, shapeFondPoint.Y + 1);
                            break;
                        case 1:
                            nextShapeCoords[0] = new Point(shapeFondPoint.X, shapeFondPoint.Y);
                            nextShapeCoords[1] = new Point(shapeFondPoint.X, shapeFondPoint.Y - 1);
                            nextShapeCoords[2] = new Point(shapeFondPoint.X - 1, shapeFondPoint.Y);
                            nextShapeCoords[3] = new Point(shapeFondPoint.X, shapeFondPoint.Y + 1);
                            break;
                        case 2:
                            nextShapeCoords[0] = new Point(shapeFondPoint.X, shapeFondPoint.Y);
                            nextShapeCoords[1] = new Point(shapeFondPoint.X + 1, shapeFondPoint.Y);
                            nextShapeCoords[2] = new Point(shapeFondPoint.X - 1, shapeFondPoint.Y);
                            nextShapeCoords[3] = new Point(shapeFondPoint.X, shapeFondPoint.Y - 1);
                            break;
                        case 3:
                            nextShapeCoords[0] = new Point(shapeFondPoint.X, shapeFondPoint.Y);
                            nextShapeCoords[1] = new Point(shapeFondPoint.X, shapeFondPoint.Y - 1);
                            nextShapeCoords[2] = new Point(shapeFondPoint.X + 1, shapeFondPoint.Y);
                            nextShapeCoords[3] = new Point(shapeFondPoint.X, shapeFondPoint.Y + 1);
                            break;
                    }
                    shapeColor = Color.FromArgb(255, 255, 215, 0);
                    break;
                case 'S':
                    switch (position)
                    {
                        case 0:
                            nextShapeCoords[0] = new Point(shapeFondPoint.X, shapeFondPoint.Y);
                            nextShapeCoords[1] = new Point(shapeFondPoint.X + 1, shapeFondPoint.Y);
                            nextShapeCoords[2] = new Point(shapeFondPoint.X - 1, shapeFondPoint.Y + 1);
                            nextShapeCoords[3] = new Point(shapeFondPoint.X, shapeFondPoint.Y + 1);
                            break;
                        case 1:
                            nextShapeCoords[0] = new Point(shapeFondPoint.X, shapeFondPoint.Y);
                            nextShapeCoords[1] = new Point(shapeFondPoint.X, shapeFondPoint.Y - 1);
                            nextShapeCoords[2] = new Point(shapeFondPoint.X + 1, shapeFondPoint.Y);
                            nextShapeCoords[3] = new Point(shapeFondPoint.X + 1, shapeFondPoint.Y + 1);
                            break;
                        case 2:
                            goto case 0;
                        case 3:
                            goto case 1;
                    }
                    shapeColor = Color.LightSkyBlue;
                    break;
                case 'Z':
                    switch (position)
                    {
                        case 0:
                            nextShapeCoords[0] = new Point(shapeFondPoint.X, shapeFondPoint.Y);
                            nextShapeCoords[1] = new Point(shapeFondPoint.X - 1, shapeFondPoint.Y);
                            nextShapeCoords[2] = new Point(shapeFondPoint.X + 1, shapeFondPoint.Y + 1);
                            nextShapeCoords[3] = new Point(shapeFondPoint.X, shapeFondPoint.Y + 1);
                            break;
                        case 1:
                            nextShapeCoords[0] = new Point(shapeFondPoint.X, shapeFondPoint.Y);
                            nextShapeCoords[1] = new Point(shapeFondPoint.X, shapeFondPoint.Y + 1);
                            nextShapeCoords[2] = new Point(shapeFondPoint.X + 1, shapeFondPoint.Y);
                            nextShapeCoords[3] = new Point(shapeFondPoint.X + 1, shapeFondPoint.Y - 1);
                            break;
                        case 2:
                            goto case 0;
                        case 3:
                            goto case 1;
                    }
                    shapeColor = Color.Green;
                    break;
                case 'I':
                    switch (position)
                    {
                        case 0:
                            nextShapeCoords[0] = new Point(shapeFondPoint.X, shapeFondPoint.Y);
                            nextShapeCoords[1] = new Point(shapeFondPoint.X - 1, shapeFondPoint.Y);
                            nextShapeCoords[2] = new Point(shapeFondPoint.X + 1, shapeFondPoint.Y);
                            nextShapeCoords[3] = new Point(shapeFondPoint.X + 2, shapeFondPoint.Y);
                            break;
                        case 1:
                            nextShapeCoords[0] = new Point(shapeFondPoint.X, shapeFondPoint.Y);
                            nextShapeCoords[1] = new Point(shapeFondPoint.X, shapeFondPoint.Y - 1);
                            nextShapeCoords[2] = new Point(shapeFondPoint.X, shapeFondPoint.Y + 1);
                            nextShapeCoords[3] = new Point(shapeFondPoint.X, shapeFondPoint.Y + 2);
                            break;
                        case 2:
                            goto case 0;
                        case 3:
                            goto case 1;
                    }
                    shapeColor = Color.FromArgb(255, 255, 140, 0);
                    break;
                case 'L':
                    switch (position)
                    {
                        case 0:
                            nextShapeCoords[0] = new Point(shapeFondPoint.X, shapeFondPoint.Y);
                            nextShapeCoords[1] = new Point(shapeFondPoint.X + 1, shapeFondPoint.Y);
                            nextShapeCoords[2] = new Point(shapeFondPoint.X - 1, shapeFondPoint.Y);
                            nextShapeCoords[3] = new Point(shapeFondPoint.X - 1, shapeFondPoint.Y + 1);
                            break;
                        case 1:
                            nextShapeCoords[0] = new Point(shapeFondPoint.X, shapeFondPoint.Y);
                            nextShapeCoords[1] = new Point(shapeFondPoint.X - 1, shapeFondPoint.Y - 1);
                            nextShapeCoords[2] = new Point(shapeFondPoint.X, shapeFondPoint.Y - 1);
                            nextShapeCoords[3] = new Point(shapeFondPoint.X, shapeFondPoint.Y + 1);
                            break;
                        case 2:
                            nextShapeCoords[0] = new Point(shapeFondPoint.X, shapeFondPoint.Y);
                            nextShapeCoords[1] = new Point(shapeFondPoint.X + 1, shapeFondPoint.Y);
                            nextShapeCoords[2] = new Point(shapeFondPoint.X - 1, shapeFondPoint.Y);
                            nextShapeCoords[3] = new Point(shapeFondPoint.X + 1, shapeFondPoint.Y - 1);
                            break;
                        case 3:
                            nextShapeCoords[0] = new Point(shapeFondPoint.X, shapeFondPoint.Y);
                            nextShapeCoords[1] = new Point(shapeFondPoint.X, shapeFondPoint.Y - 1);
                            nextShapeCoords[2] = new Point(shapeFondPoint.X, shapeFondPoint.Y + 1);
                            nextShapeCoords[3] = new Point(shapeFondPoint.X + 1, shapeFondPoint.Y + 1);
                            break;
                    }
                    shapeColor = Color.FromArgb(255, 0, 0, 205);
                    break;
                case 'J':
                    switch (position)
                    {
                        case 0:
                            nextShapeCoords[0] = new Point(shapeFondPoint.X, shapeFondPoint.Y);
                            nextShapeCoords[1] = new Point(shapeFondPoint.X + 1, shapeFondPoint.Y);
                            nextShapeCoords[2] = new Point(shapeFondPoint.X - 1, shapeFondPoint.Y);
                            nextShapeCoords[3] = new Point(shapeFondPoint.X + 1, shapeFondPoint.Y + 1);
                            break;
                        case 1:
                            nextShapeCoords[0] = new Point(shapeFondPoint.X, shapeFondPoint.Y);
                            nextShapeCoords[1] = new Point(shapeFondPoint.X - 1, shapeFondPoint.Y + 1);
                            nextShapeCoords[2] = new Point(shapeFondPoint.X, shapeFondPoint.Y - 1);
                            nextShapeCoords[3] = new Point(shapeFondPoint.X, shapeFondPoint.Y + 1);
                            break;
                        case 2:
                            nextShapeCoords[0] = new Point(shapeFondPoint.X, shapeFondPoint.Y);
                            nextShapeCoords[1] = new Point(shapeFondPoint.X + 1, shapeFondPoint.Y);
                            nextShapeCoords[2] = new Point(shapeFondPoint.X - 1, shapeFondPoint.Y);
                            nextShapeCoords[3] = new Point(shapeFondPoint.X - 1, shapeFondPoint.Y - 1);
                            break;
                        case 3:
                            nextShapeCoords[0] = new Point(shapeFondPoint.X, shapeFondPoint.Y);
                            nextShapeCoords[1] = new Point(shapeFondPoint.X, shapeFondPoint.Y - 1);
                            nextShapeCoords[2] = new Point(shapeFondPoint.X, shapeFondPoint.Y + 1);
                            nextShapeCoords[3] = new Point(shapeFondPoint.X + 1, shapeFondPoint.Y - 1);
                            break;
                    }
                    shapeColor = Color.Purple;
                    break;
                case 'O':
                    switch (position)
                    {
                        case 0:
                            nextShapeCoords[0] = new Point(shapeFondPoint.X, shapeFondPoint.Y);
                            nextShapeCoords[1] = new Point(shapeFondPoint.X + 1, shapeFondPoint.Y);
                            nextShapeCoords[2] = new Point(shapeFondPoint.X, shapeFondPoint.Y + 1);
                            nextShapeCoords[3] = new Point(shapeFondPoint.X + 1, shapeFondPoint.Y + 1);
                            break;
                        case 1:
                            goto case 0;
                        case 2:
                            goto case 0;
                        case 3:
                            goto case 0;
                    }
                    shapeColor = Color.Red;
                    break;
            }
        }
    }

}
