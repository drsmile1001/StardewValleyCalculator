using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewValleyCalculator
{
    public class Fish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public MoveType MoveType { get; set; }
        public int TimeRangeStart { get; set; }
        public int TimeRangeFinish { get; set; }
        public Season Season { get; set; }
        public Weather Weather { get; set; }

        
    }
    
    public enum MoveType
    {
        Dart,Floater,Mixed,Sinker,Smooth
    }

    [Flags]
    public enum Season
    {
        Spring = 0x01,
        Summer = 0x02,
        Fall = 0x04,
        Winter = 0x08
    }

    public class EnumIntialTool
    {
        public static Season GetSeason(IEnumerable<string> data)
        {
            Season result = data.Select(s => 
            (Season) Enum.Parse(typeof (Season), s.FirstLetterToUpper()))
            .Aggregate((current, season) => current | season);
            return result;
        }
    }

    [Flags]
    public enum Weather
    {
        Sunny = 0x01,
        Rainy = 0x02,
        Both = Sunny|Rainy
    }
    
}
