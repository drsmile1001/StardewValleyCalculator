using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using StardewValleyCalculator.Properties;

namespace StardewValleyCalculator
{
    /// <summary>作物</summary>
    public class Crop
    {
        /// <summary>作物名</summary>
        public string Name { get; set; }

        /// <summary>生長季節</summary>
        public string Season { get; set; }

        /// <summary>購價</summary>
        public int BuyPrice { get; set; }

        /// <summary>賣方</summary>
        public string BuyStore { get; set; }

        /// <summary>售價</summary>
        public int SellPrice { get; set; }

        /// <summary>買方</summary>
        public string SellStore { get; set; }

        /// <summary>生長時間</summary>
        public int GrowthDays { get; set; }

        /// <summary>再生長時間</summary>
        public int? RepeatDays { get; set; }

        /// <summary>資料庫</summary>
        public static readonly IReadOnlyList<Crop> Database = InitialDatabase();

        
        private static IReadOnlyList<Crop> InitialDatabase()
        {
            string resourceCrops = Resources.Crops;
            List<string> dataLine = resourceCrops.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return dataLine.Select(s => s.Split('\t')).Select(dataValues => new Crop
            {
                Name = dataValues[0],
                Season = dataValues[1],
                BuyPrice = int.Parse(dataValues[2]),
                BuyStore = dataValues[3],
                SellPrice = int.Parse(dataValues[4]),
                SellStore = dataValues[5],
                GrowthDays = int.Parse(dataValues[6]),
                RepeatDays = dataValues[7] == "" ? (int?) null : int.Parse(dataValues[7])
            }).ToList();
        }
        
    }
}