using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewValleyCalculator
{
    public class Location
    {
        public string Name { get; set; }
        public Dictionary<Season,List<IdAndRate>> Range { get; set; }
        public Dictionary<Season,List<IdAndState>> Fish { get; set; }
        public List<IdAndRate> Unkonw { get; set; }
    }

    public class IdAndRate
    {
        public int Id { get; set; }
        public double Rate { get; set; }

        public static List<IdAndRate> FromStrings(IList<string> data)
        {
            int dataCount = data.Count;
            if (dataCount == 1)
            {
                if (data[0] != "-1")
                {
                    throw new ArgumentOutOfRangeException();
                }
                return null;
            }
            if (dataCount%2 != 0) throw new ArgumentOutOfRangeException();
            List<IdAndRate> result = new List<IdAndRate>();
            for (int index = 0; index + 1 < data.Count; index += 2)
            {
                int id = int.Parse(data[index]);
                double rate = double.Parse(data[index + 1]);
                result.Add(new IdAndRate {Id = id, Rate = rate});
            }
            return result;
        }
    }

    public class IdAndState
    {
        public int Id { get; set; }
        public int State { get; set; }

        public static List<IdAndState> FromStrings(IList<string> data)
        {
            int dataCount = data.Count;
            if (dataCount == 1)
            {
                if (data[0] != "-1")
                {
                    throw new ArgumentOutOfRangeException();
                }
                return null;
            }
            if (dataCount%2 != 0) throw new ArgumentOutOfRangeException();
            List<IdAndState> result = new List<IdAndState>();
            for (int index = 0; index + 1 < data.Count; index += 2)
            {
                int id = int.Parse(data[index]);
                int state = int.Parse(data[index + 1]);
                result.Add(new IdAndState {Id = id, State = state});
            }
            return result;
        }
    }
}
