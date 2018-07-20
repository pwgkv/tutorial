using AsciiEngine;
using Easy;
using System;
using System.Collections.Generic;

namespace AsciiEngine
{

    #region " Sprites "

    public class Sprite
    {

        double _OriginalX;
        double _OriginalY;
        double _X;
        double _Y;
        double _IncrementX;
        double _IncrementY;
        char _Ascii;
        double _Range;
        bool _Killed = false;

        public int X { get { return Numbers.Round(this._X); } }
        public int Y { get { return Numbers.Round(this._Y); } }

        public bool Alive
        {
            get { return Numbers.Distance(this._X, this._OriginalX, this._Y, this._OriginalY) < this._Range && !this._Killed; }
        }

        public void Hide()
        {
            Screen.TryWrite(Numbers.Round(this._X), Numbers.Round(this._Y), ' ');
        }

        public void Kill()
        {
            this._Killed = true;
        }
        public void Animate()
        {
            this.Hide();
            this._X += this._IncrementX;
            this._Y += this._IncrementY;
            Screen.TryWrite(Numbers.Round(this._X), Numbers.Round(this._Y), this._Ascii);
        }

        public Sprite(char c, double x, double y, double range) : this(c, x, y, -1, -1, range) { } // random direction increments

        public Sprite(char c, double x, double y, double incx, double incy, double range)
        {
            this._Ascii = c;
            this._OriginalX = x;
            this._OriginalY = y;
            this._X = x;
            this._Y = y;
            this._Range = range;

            if (incx == -1 && incy == -1)
            {
                // add a fraction to make sure it's not zero
                this._IncrementX = Easy.Numbers.Random.NextDouble() + .1;
                this._IncrementY = Easy.Numbers.Random.NextDouble() + .1;
                if (Numbers.Random.NextDouble() < .5) { this._IncrementX *= -1; }
                if (Numbers.Random.NextDouble() < .5) { this._IncrementY *= -1; }

            }
            else
            {
                this._IncrementX = incx;
                this._IncrementY = incy;

            }

        }

    }

    public class SpriteField
    {

        List<Sprite> _sprites = new List<Sprite>();

        public List<Sprite> Sprites
        {
            get { return _sprites; }
        }

        public void RemoveSprite(Sprite s)
        {
            this.Sprites.Find(x => s.Equals(x)).Kill();
        }

        public void Animate()
        {

            foreach (Sprite s in this._sprites.FindAll(x => !x.Alive))
            {
                s.Hide();
                this.Sprites.Remove(s);
            }

            foreach (Sprite s in this._sprites.FindAll(x => x.Alive)) { s.Animate(); }
        }

        public SpriteField() { }

    }

    #endregion

    #region " Screen "

    public class Screen
    {

        #region " Dimensions "

        public static int TopEdge { get { return 0; } }

        public static int BottomEdge { get { return Console.WindowHeight - 1; } }

        public static int LeftEdge { get { return 0; } }

        public static int RightEdge { get { return Console.WindowWidth - 1; } }

        public static int Width { get { return Console.WindowWidth; } }

        public static int Height { get { return Console.WindowHeight; } }

        public static void GetCenterXY(ref int x, ref int y)
        {
            x = Numbers.Round(Screen.Width / 2);
            y = Numbers.Round(Screen.Height / 2);
        }

        public static bool TrySetSize(int targetwidth, int targetheight)
        {
            return TrySetSize(targetwidth, targetheight, true);
        }

        public static bool TrySetSize(int targetwidth, int targetheight, bool adjustmanually)
        {

            try
            {
                Console.SetWindowSize(targetwidth, targetheight);
                Console.SetBufferSize(targetwidth, targetheight);
            }
            catch
            {

                while ((Console.WindowWidth != targetwidth || Console.WindowHeight != targetheight || !Console.KeyAvailable) && !Console.KeyAvailable && adjustmanually)
                {

                    Console.Clear();

                    if (Console.WindowWidth == targetwidth && Console.WindowHeight == targetheight)
                    {
                        Console.Write("Perfect size!\nPress ENTER to continue.");
                    }
                    else
                    {
                        Console.WriteLine("Resize windown to");
                        Console.WriteLine(targetwidth + " X " + targetheight);
                        Console.WriteLine("or keypress to quit.");
                        Console.WriteLine();

                        char leftarrow = '<'; // \x2190
                        char rightarrow = '>'; // \x2192
                        char uparrow = '^'; // \x2191
                        char downarrow = 'v'; // \x2193

                        Console.WriteLine(Console.WindowWidth + " X " + Console.WindowHeight);

                        Console.Write("resize: " + Console.WindowWidth + " ");
                        if (Console.WindowWidth > targetwidth) { Console.WriteLine(new String(leftarrow, Console.WindowWidth - targetwidth)); }
                        else if (Console.WindowWidth < targetwidth) { Console.WriteLine(new String(rightarrow, targetwidth - Console.WindowWidth)); }
                        else { Console.WriteLine("Perfect!"); }

                        Console.Write("resize: " + Console.WindowHeight + " ");
                        if (Console.WindowHeight > targetheight) { Console.Write(new String(uparrow, Console.WindowHeight - targetheight)); }
                        else if (Console.WindowHeight < targetheight) { Console.Write(new String(downarrow, targetheight - Console.WindowHeight)); }
                        else { Console.Write("Perfect!"); }
                    }

                    System.Threading.Thread.Sleep(200); // good enough so the CPU doesn't go crazy

                }

            }

            return (Console.WindowHeight == targetheight && Console.WindowWidth == targetwidth);

        }

        #endregion

        #region " Writing "

        public static bool TryWrite(int x, int y, string s)
        {
            bool success = false;
            try
            {
                if (y >= Screen.TopEdge && y <= Screen.BottomEdge)
                {
                    // the whole string should fit on the screen
                    if (x >= Screen.LeftEdge && x + s.Length < Screen.RightEdge)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(s);
                        success = true;
                    }
                    // some or all of the text is off the screen, so go character by character
                    else
                    {
                        char[] chars = s.ToCharArray();
                        for (int c = 0; c < chars.Length; c++) { Screen.TryWrite(x + c, y, chars[c]); }
                        success = (x >= Screen.LeftEdge && x + s.Length - 1 <= Screen.RightEdge && y >= Screen.TopEdge && y <= Screen.BottomEdge);
                    }
                }
            }
            catch { }
            return success;
        }

        public static bool TryWrite(int x, int y, char c)
        {
            // don't write anything past the screen edges
            // don't write anything in the lower right corner because that can cause scrolling
            if (x >= Screen.LeftEdge && x <= Screen.RightEdge && y >= Screen.TopEdge && y <= Screen.BottomEdge && !(x == Screen.RightEdge && y == Screen.BottomEdge))
            {
                Console.SetCursorPosition(x, y);
                Console.Write(c);
                return true;
            }
            else { return false; }
        }

        public static void Countdown(int start)
        {
            Keys.EatKeys();
            int x = 0;
            int y = 0;
            Screen.GetCenterXY(ref x, ref y);

            for (int n = start; n > 0; n--)
            {
                Screen.TryWrite(x, y, n + " ");
                if (Console.KeyAvailable) { n = 0; }
                else { System.Threading.Thread.Sleep(1000); }
            }
        }

        #endregion
    }

    #endregion 

}
