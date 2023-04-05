using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;


namespace Example_Task_SW_Lasertechnik
{
    class Punkt
    /* Class representing a single Point in 2 dimensions.
     * Class supports empty contruction or value construction with both coordinates
    */
    {
        public int X;
        public int Y;
        public Punkt() { }
        public Punkt(int x, int y) { X = x; Y = y; }

        public void PrintPunkt()
            // not important for task
        {
            Console.Write($"({X},{Y})");
            Console.WriteLine();
        }

        public static bool Compare(Punkt firstPoint, Punkt secPoint)
        /* Comparer for Punkt class.
         * Compare checks equality between X and Y coordinates of two objects.
         * Return value is either true for equal and false for unqeual
        */
        {
            if (firstPoint.X == secPoint.X && firstPoint.Y == secPoint.Y) 
            {
                return true; 
            }
            else 
            { 
                return false; 
            }
        }

        public static int CountDuplicates(List<Punkt> allPoints)
        /* Simple counting routines for doublicates.
         * Count requires strict ascending or descending order!
         * Counter function will count multiple entries ONLY onces!
        */
        {
            int counts = 0;                 // Output
            bool curCheck;
            bool oldCheck = false;          // Secure bool for duplicates more then 2 elements
            for (int idx = 0; idx < allPoints.Count()-1; idx++)
            {
                curCheck = Punkt.Compare(allPoints[idx], allPoints[idx + 1]);
                if (curCheck && !oldCheck)
                {
                    counts++;
                }
                oldCheck = curCheck;
            }
            return counts;
        }
    }

    class Line 
        /* Class representing a line, by holding start, stop and points in between
         * Currently class is limited to slopes equal to infinity or zero
         * All other slope cases are covered by a warning and return of empty construction
        */
    {
        public Punkt start;                                 // represent beginning point for line
        public Punkt stop;                                  // represent end point for line
        public List<Punkt> intermed = new List<Punkt>();    // containing lines between beginning and end point
        public Line() { }
        public Line(int x1 = 0, int y1 = 0, int x2 = 0, int y2 = 0)
        {
            start = new Punkt(x1, y1);
            stop = new Punkt(x2, y2);
            int[] abscissa = { x1, x2 };    // Abszisse
            int[] ordinate = { y1, y2 };    // Ordinate

            // Secure loop for correct iteration at negative slopes, eg. (9, 8) -> (2, 8)
            Array.Sort(abscissa);
            Array.Sort(ordinate);
            if (x1 == x2)                   // Limit functionality to horizontal lines
            {
                for (int i = ordinate[0] + 1; i < ordinate[1]; i++)
                {
                    intermed.Add(new Punkt(x1, i));
                }
            }
            else if (y1 == y2)              // Limit functionality to vertical lines
            {
                for (int i = abscissa[0] + 1; i < abscissa[1]; i++)
                {
                    intermed.Add(new Punkt(i, y1));
                }
            } else                          // Warning treatment for slopes unqueal to infinity or zero
            {
                WarningException slopeWarning = new WarningException($"Line constructions only supported for vertical or horizontal lines!");
                Console.WriteLine(slopeWarning.ToString());
                return;
            }
        }

        public Line(int[] values)
            /* Construktor with line objects
             * Needs int array input exactly with 4 elements!
             * Case input array smaller then 4 elements, warning will be thrown and
             * objects is constructed empty
             * Case input array higher then 4 elements, warning be thrown. Only first
             * four elements will be used as coordinates
            */
        {
            if (values.Length < 4) 
            {
                WarningException emptyConstruction = new WarningException($"Line Constructor got lesser then 4 input (receivied {values.Length}). return Empty line object!");
                Console.WriteLine(emptyConstruction.ToString());
                return; 
            }else if(values.Length > 4)
            {
                WarningException emptyConstruction = new WarningException($"Line Constructor got more then points! Using first four and ignoring extra entries!");
                Console.WriteLine(emptyConstruction.ToString());
            }
            start = new Punkt(values[0], values[1]);
            stop = new Punkt(values[2], values[3]);
            int[] abscissa = { values[0], values[2] }; // Abszisse
            int[] ordinate = { values[1], values[3] }; // Ordinate
            Array.Sort(abscissa);
            Array.Sort(ordinate);
            if (values[0] == values[2])
            {
                for (int i = ordinate[0] + 1; i < ordinate[1]; i++)
                {
                    intermed.Add(new Punkt(values[0], i));
                }
            }
            else
            {
                for (int i = abscissa[0] + 1; i < abscissa[1]; i++)
                {
                    intermed.Add(new Punkt(i, values[1]));
                }
            }
        }

        public void PrintPunkt()
            // Not relevant for task
        {
            start.PrintPunkt();
            Console.Write("->");
            stop.PrintPunkt();
            Console.WriteLine();
        }

        public void PrintIntermed()
        // Not relevant for task
        {
            foreach (Punkt i in intermed)
            {
                i.PrintPunkt();
                Console.WriteLine();
            }
        }
    }

    class Program
    {
        static int[] ConvertArray(string[] strIn, int currentLine = 0)
        /* Help function converting string arrays in length 4 to int arrays
         * Handles corrupted entries via FormatException or uncomplete inputs
         * In case Exception triggered empty int array is returned!
        */
        {
            if (strIn.Length < 4) { return new int[0]; }
            int[] result = new int[4];
            for(int i = 0; i < 4; i++)
            {
                try
                {
                    result[i] = int.Parse(strIn[i]);
                }
                catch(System.FormatException)
                {
                    Console.WriteLine($"Line: {currentLine} is not convertable! Following entry is corrupted: '{strIn[i]}'. Skipping line!");
                    result = new int[0];
                    break;
                }
            }
            return result;
        }

        static List<Line> GetFile( string filePath)
        {
            /* Function parsing example Input
            */
            string currentEntry;                                    // in file line content
            string[] temp;                                          // line cleaned to interest
            int[] coords;                                           // x1 y1 x2 y2
            int loopCounter = 0;                                    // Line Counter
            List<Line> allLines = new List<Line>();                 // OUTPUT

            using (StreamReader sr = new StreamReader(filePath))
            {
                while (sr.Peek() >= 0)
                {
                    loopCounter++;
                    currentEntry = sr.ReadLine();
                    temp = currentEntry.Split(new Char[] {'-', ' ', '>', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    coords = ConvertArray(temp, loopCounter);
                    if (coords.Length < 4) { continue; }
                    if (coords[0] != coords[2] && coords[1] != coords[3]) { continue; }
                    allLines.Add(new Line(coords));
                }
            }
            Console.WriteLine($"Anzahl aller Koordinaten: {loopCounter}");
            return allLines;
        }

        static void Main(string[] args)
        {
            string filePath = @"C:\testFile.txt";       // Default Path
            while (!File.Exists(filePath))              // Handle when file not exists!
            {
                Console.WriteLine("Path to file is not existing! Enter a path! Type 'exit' to terminate!");
                filePath = Console.ReadLine();
                if (filePath == "exit") { return; }     // User termination
            }

            List<Line> allLines = GetFile(filePath);    // File coordinates as Lines objects

            // Unpacking all lines in one list
            List<Punkt> allPoints = new List<Punkt>();
            foreach(Line curLine in allLines)
            {
                allPoints.Add(curLine.start);
                allPoints.Add(curLine.stop);
                foreach(Punkt intermedPoint in curLine.intermed)
                {
                    allPoints.Add(intermedPoint);
                }
            }
            
            // Pre sorting before counting multiple entries
            var sortedList = allPoints.OrderByDescending(x => x.X).ThenBy(x => x.Y);
            allPoints = sortedList.ToList();

            int counts = Punkt.CountDuplicates(allPoints);  // Simple count of multiple entries

            // Present Result of file
            Console.WriteLine($"Anzahl der gültiger Linien: {allLines.Count}");
            Console.WriteLine($"Anzahl aller Punkte: {allPoints.Count}");
            Console.WriteLine($"Input hat {counts} als Resultat!");
            Console.WriteLine($"Press Enter to exit Console!");
            Console.ReadLine();
        }
    }
}