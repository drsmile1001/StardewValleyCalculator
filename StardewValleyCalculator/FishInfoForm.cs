using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DebugTool;
using Newtonsoft.Json.Linq;
using StardewValleyCalculator.Properties;
using YamlDotNet.Serialization;

namespace StardewValleyCalculator
{
    public partial class FishInfoForm : Form
    {
        private bool _loaded = false;

        public FishInfoForm()
        {
            InitializeComponent();
        }

        public List<Location> LocationList { get; set; }

        public List<Fish> FishList { get; set; }

        private void FishInfoForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            var deserializer = new Deserializer();
            var fishYamlObject = deserializer.Deserialize(new StringReader(Resources.Fish));
            var fishJToken = JToken.FromObject(fishYamlObject);
            FishList = ((JObject) fishJToken["content"]).Properties().Select(pro => new
            {
                Id = int.Parse(pro.Name),
                Value = pro.Value.Value<string>().Split('/')
            }).Where(item => item.Value[1] != "trap")
                .Select(item => new Fish
                {
                    Id = item.Id,
                    Name = item.Value[0],
                    MoveType = (MoveType) Enum.Parse(typeof (MoveType), item.Value[2].FirstLetterToUpper()),
                    TimeRangeStart = int.Parse(item.Value[5].Split(' ')[0])/100,
                    TimeRangeFinish = int.Parse(item.Value[5].Split(' ')[1])/100,
                    Season = EnumIntialTool.GetSeason(item.Value[6].Split(' ')),
                    Weather = (Weather) Enum.Parse(typeof (Weather), item.Value[7].FirstLetterToUpper())
                }).ToList();
            var locationObject = deserializer.Deserialize(new StringReader(Resources.Locations));
            var locationJToken = JToken.FromObject(locationObject);
            LocationList = ((JObject) locationJToken["content"]).Properties().Select(pro => new
            {
                Location = pro.Name,
                Value = pro.Value.Value<string>().Split('/')
            }).Select(item => new Location
            {
                Name = item.Location,
                Range = new Dictionary<Season, List<IdAndRate>>
                {
                    {Season.Spring, IdAndRate.FromStrings(item.Value[0].Split(' '))},
                    {Season.Summer, IdAndRate.FromStrings(item.Value[1].Split(' '))},
                    {Season.Fall, IdAndRate.FromStrings(item.Value[2].Split(' '))},
                    {Season.Winter, IdAndRate.FromStrings(item.Value[3].Split(' '))},
                },
                Fish = new Dictionary<Season, List<IdAndState>>
                {
                    { Season.Spring, IdAndState.FromStrings(item.Value[4].Split(' '))},
                    { Season.Summer, IdAndState.FromStrings(item.Value[5].Split(' '))},
                    { Season.Fall, IdAndState.FromStrings(item.Value[6].Split(' '))},
                    { Season.Winter, IdAndState.FromStrings(item.Value[7].Split(' '))},
                },
                Unkonw = IdAndRate.FromStrings(item.Value[8].Split(' '))
            }).ToList();

            List<string> fishNameCbItem = new List<string> {"Any"};
            fishNameCbItem.AddRange(FishList.Select(fish => fish.Name));

            cbFishName.DataSource = fishNameCbItem;
            cbFishName.SelectedIndex = 0;

            cbSeason.DataSource = new List<string> {"Any","Spring","Summer","Fall","Winter"};
            cbSeason.SelectedIndex = 0;

            List<string> locationCbItem = new List<string> {"Any"};
            locationCbItem.AddRange(LocationList.Select(loc=>loc.Name));
            cbLocation.DataSource = locationCbItem;
            cbLocation.SelectedIndex = 0;

            cbWeather.DataSource = new List<string> {"Any","Both","Sunny","Rainy"};
            cbWeather.SelectedIndex = 0;
            _loaded = true;
            ShowData();
        }

        private void ShowData()
        {
            if (_loaded == false)
            {
                return;
            }
            var locQuery = from location in LocationList
                from seasonFishList in location.Fish
                where seasonFishList.Value != null
                from idAndState in seasonFishList.Value
                let fishFind = FishList.First(fish => fish.Id == idAndState.Id)
                select new ShowFishInfo
                {
                    Fish = fishFind,
                    LocName = location.Name,
                    Season = seasonFishList.Key,
                };

            string selectFishName = cbFishName.Text;
            if (selectFishName != "Any")
            {
                locQuery = locQuery.Where(item => item.Fish.Name == selectFishName);
            }
            string selectSeason = cbSeason.Text;
            if (selectSeason != "Any")
            {
                locQuery = locQuery.Where(item => item.Season == (Season) Enum.Parse(typeof (Season), selectSeason));
            }
            string selectLocation = cbLocation.Text;
            if (selectLocation != "Any")
            {
                locQuery = locQuery.Where(item => item.LocName == selectLocation);
            }

            string selectWeatherString = cbWeather.Text;
            if (selectWeatherString != "Any")
            {
                Weather selectWeather = (Weather) Enum.Parse(typeof (Weather), selectWeatherString);
                locQuery = locQuery.Where(item => item.Fish.Weather.HasFlag(selectWeather));
            }
            var locList = ShowFishInfo.ToDataTable(locQuery.ToList());

            SeasonLocationView.DataSource = locList;
            
            SeasonLocationView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            SeasonLocationView.AutoSize = false;
            SeasonLocationView.DoubleBuffered(true);

            for (int row = 0; row < SeasonLocationView.Rows.Count; row++)
            {
                Weather rowWeather = (Weather) Enum.Parse(typeof (Weather),
                    (string) SeasonLocationView.Rows[row].Cells[3].Value);
                Color backColor;
                switch (rowWeather)
                {
                    case Weather.Sunny:
                        backColor = Color.LightSalmon;
                        break;
                    case Weather.Rainy:
                        backColor = Color.LightBlue;
                        break;
                    case Weather.Both:
                        backColor = Color.LightGreen;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


                for (int column = 4; column < 24; column++)
                {
                    if ((string) SeasonLocationView.Rows[row].Cells[column].Value == "■")
                    {
                        SeasonLocationView.Rows[row].Cells[column].Style.BackColor = backColor;
                        SeasonLocationView.Rows[row].Cells[column].Value = null;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowData();
        }

        private void cbFishName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowData();
        }

        private void cbSeason_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowData();
        }

        private void cbLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowData();
        }

        private void cbWeather_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowData();
        }
    }

    public class ShowFishInfo
    {
        public Fish Fish { get; set; }
        public string LocName { get; set; }
        public Season Season { get; set; }

        public static DataTable ToDataTable(IEnumerable<ShowFishInfo> data)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FishName");
            dt.Columns.Add("LocName");
            dt.Columns.Add("Season");
            dt.Columns.Add("Weather");
            for (int time = 6; time < 26; time++)
            {
                dt.Columns.Add(time.ToString("D2"));
            }

            foreach (ShowFishInfo info in data)
            {
                List<string> row = new List<string>
                {
                    info.Fish.Name, info.LocName, info.Season.ToString(), info.Fish.Weather.ToString()
                };
                for (int time = 6; time < 26; time++)
                {
                    if (info.Fish.TimeRangeStart <= time && time < info.Fish.TimeRangeFinish)
                    {
                        row.Add("■");
                    }
                    else
                    {
                        row.Add("");
                    }
                }
                dt.Rows.Add(row.ToArray());
            }

            return dt;
        }
    }
}