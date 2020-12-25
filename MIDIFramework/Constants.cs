using System;
using System.Collections.Generic;
using System.Text;

namespace MIDIFramework
{
    public class Constants
    {
        public readonly static Dictionary<string, int> _notesDict = new Dictionary<string, int>() {
            { "C3", 36 },
            { "D3", 38 },
            { "E3", 40 },
            { "F3", 41 },
            { "G3", 43 },
            { "A3", 45 },
            { "B3", 47 },

            { "C4", 48 },
            { "D4", 50 },
            { "E4", 52 },
            { "F4", 53 },
            { "G4", 55 },
            { "A4", 57 },
            { "B4", 59 },

            { "C5", 60 },
            { "D5", 62 },
            { "E5", 64 },
            { "F5", 65 },
            { "G5", 67 },
            { "A5", 69 },
            { "B5", 71 },

            { "C6", 72 },
            { "D6", 74 },
            { "E6", 76 },
            { "F6", 77 },
            { "G6", 79 },
            { "A6", 81 },
            { "B6", 83 }
        };
    }
}
