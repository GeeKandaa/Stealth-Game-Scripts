using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Utils_Noise
{
    public readonly Dictionary<string, int> FloorNoise;
    public Utils_Noise()
    {
        // Dictionary containing default noise level of steps on specified floor types
        FloorNoise = new Dictionary<string, int>
        {
            { "Carpet", 3 },
            { "Stone", 8 },
            { "Wood", 10 },
            { "Metal", 15 }
        };
    }
}

