using System;
using System.Collections.Generic;
using System.Linq;
using StardewValleyCalculator.Properties;

namespace StardewValleyCalculator
{
    public class EnergyAndHealth
    {
        public string Name { get; set; }

        public int Energy { get; set; }

        public int Health { get; set; }

        /// <summary>資料庫</summary>
        public static readonly IReadOnlyList<EnergyAndHealth> Database = InitialDatabase();


        private static IReadOnlyList<EnergyAndHealth> InitialDatabase()
        {
            List<string> dataLine = Resources.EnergyAndHealths.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return dataLine.Select(s => s.Split('\t')).Select(dataValues => new EnergyAndHealth
            {
                Name = dataValues[0],
                Energy = int.Parse(dataValues[1]),
                Health = int.Parse(dataValues[2])
            }).ToList();
        }
    }
}