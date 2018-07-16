using System;

public class TUI {

#region " Properties "

    public static int LeftEdge {
        get {return 0;}
    }

    public static int RightEdge {
        get {return Console.WindowWidth;}
    }        

    public static int BottomEdge {
        get {return Console.WindowHeight-1;}
    }

    public static bool PastEdge(int X, int Offset, string Ascii) {
        return X+Offset < TUI.LeftEdge || X+Offset > TUI.RightEdge-Ascii.Length;
    }

    public static bool PastEdge(int X, string Ascii) {
        return PastEdge(X,0,Ascii);
    }

#endregion

#region " Screen Manipulation "


    public static void ConsoleSetup() {
        Console.Clear();
        Console.CursorVisible = false;
        Console.WriteLine("Set your console size, then press any key to begin...");
        Console.ReadKey(); 
        Console.Clear();  
    }

#endregion
    public static string StringRepeat(int n, char c) {
        return new String(c, n);
    }

}
