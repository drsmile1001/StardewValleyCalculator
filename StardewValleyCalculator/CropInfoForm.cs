using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;
using System.Runtime.Remoting.Messaging;
using DebugTool;
using FolderSelect;
using Newtonsoft.Json.Linq;
using StardewValleyCalculator.Properties;
using YamlDotNet.Serialization;

namespace StardewValleyCalculator
{
    public partial class CropInfoForm : Form
    {
        private const int DaysInSeason = 28;
        private string _selectMode = "ProfitPerDay";
        private string _selectSeason = "All";

        private int _selectDateNow = 1;

        public CropInfoForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cbCalculateTarget.DataSource = new List<string> { "ProfitPerDay","EnergyAndHealth"};
            List<string> season = new List<string> {"All"};
            season.AddRange(Crop.Database.Select(crop => crop.Season).Distinct().ToList());
            cbSeason.DataSource = season;
            ShowTable();
        }

        private void ShowTable()
        {
            switch (_selectMode)
            {
                case "ProfitPerDay":
                    ShowProfitTable();
                    break;
                case "EnergyAndHealth":
                    ShowEnergyHealthTable();
                    break;
            }
            
            myDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void ShowEnergyHealthTable()
        {
            var crops = Crop.Database.ToList() as IEnumerable<Crop>;
            crops = _selectSeason == "All" ? crops : crops.Where(item => item.Season == _selectSeason);
            var result = crops.Join(EnergyAndHealth.Database, crop => crop.Name, eh => eh.Name, (crop, eh) => new
            {
                crop.Name,
                Repeat = crop.RepeatDays != null,
                crop.BuyPrice,
                eh.Energy,
                EnergyPerCost = (double) eh.Energy / crop.BuyPrice
            }).OrderByDescending(i=>i.EnergyPerCost).ToList();
                
            
            myDataGridView.DataSource = result;
        }

        private void ShowProfitTable()
        {
            var crops = Crop.Database.ToList() as IEnumerable<Crop>;
            crops = _selectSeason == "All" ? crops : crops.Where(item => item.Season == _selectSeason);
            int lastDates = DaysInSeason - _selectDateNow;

            var result = crops.Select(crop => new
            {
                crop,
                CanGrowth = crop.GrowthDays > lastDates,
            })
                .Select(item => new
                {
                    item.crop,
                    BuyTimes = item.CanGrowth
                        ? 0
                        : item.crop.RepeatDays == null ? lastDates/item.crop.GrowthDays : 1,
                    SellTimes = item.crop.GrowthDays > lastDates
                        ? 0
                        : item.crop.RepeatDays == null
                            ? lastDates/item.crop.GrowthDays
                            : (lastDates - item.crop.GrowthDays)/item.crop.RepeatDays.Value + 1,
                })
                .Select(item => new
                {
                    item.crop.Name,
                    item.crop.BuyPrice,
                    item.crop.BuyStore,
                    item.crop.SellPrice,
                    item.crop.SellStore,
                    item.crop.GrowthDays,
                    item.crop.RepeatDays,
                    item.BuyTimes,
                    OutCash = item.BuyTimes*item.crop.BuyPrice,
                    item.SellTimes,
                    InCash = item.SellTimes*item.crop.SellPrice,
                    Profit = item.SellTimes*item.crop.SellPrice - item.BuyTimes*item.crop.BuyPrice,
                    ProfitPerDay = item.crop.RepeatDays == null
                        ? (double) (item.SellTimes*item.crop.SellPrice - item.BuyTimes*item.crop.BuyPrice)/
                          (item.SellTimes*item.crop.GrowthDays)
                        : (double) (item.SellTimes*item.crop.SellPrice - item.BuyTimes*item.crop.BuyPrice)/
                          ((item.SellTimes - 1)*item.crop.RepeatDays.Value + item.crop.GrowthDays)
                }).OrderByDescending(item => item.ProfitPerDay).ToList();
            myDataGridView.DataSource = result;
        }

        private void cbSeason_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectSeason = cbSeason.Text;
            ShowTable();
        }

        private void nudDateNow_ValueChanged(object sender, EventArgs e)
        {
            _selectDateNow = (int) nudDateNow.Value;
            ShowTable();
        }

        private void cbCalculateTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectMode = cbCalculateTarget.Text;
            ShowTable();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FishInfoForm f= new FishInfoForm();
            f.Show();
        }
    }
}
