using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app 
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app 
// is first launched.
using Newtonsoft.Json.Linq;

namespace Prototype1.Data
{
    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem
    {
        public SampleDataItem(String uniqueId, String title, String type)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Subtitle = type;
        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public string Content { get; private set; }


        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup
    {
        public SampleDataGroup(String uniqueId, String type, String title, String subtitle, string imagePath)
        {
            this.UniqueId = uniqueId;
            this.Type = type;
            this.Title = title;
            this.Subtitle = subtitle;
            this.ImagePath = imagePath;
            this.Items = new ObservableCollection<SampleDataItem>();
        }

        public SampleDataGroup(String uniqueId, String title, String subtitle)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Subtitle = subtitle;
            this.ImagePath =
                "Assets/avatar1.jpg";
            this.Items = new ObservableCollection<SampleDataItem>();
        }

        public string Type { get; private set; }
        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public ObservableCollection<SampleDataItem> Items { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// SampleDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _groups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> Groups
        {
            get { return this._groups; }
        }

        public static async Task<IEnumerable<SampleDataGroup>> GetGroupsAsync()
        {
            //await _sampleDataSource.GetSampleDataAsync();
            await _sampleDataSource.GetSampleDataAsync2();

            return _sampleDataSource.Groups;
        }

        public static async Task<SampleDataGroup> GetGroupAsync(string uniqueId)
        {
            await _sampleDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Groups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<SampleDataItem> GetItemAsync(string uniqueId)
        {
            await _sampleDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Groups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        private async Task GetSampleDataAsync()
        {
            /*
            if (this._groups.Count != 0)
                return;

            Uri dataUri = new Uri("ms-appx:///DataModel/SampleData.json");

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);
            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["Groups"].GetArray();

            foreach (JsonValue groupValue in jsonArray)
            {
                JsonObject groupObject = groupValue.GetObject();
                SampleDataGroup group = new SampleDataGroup(groupObject["UniqueId"].GetString(),
                                                            groupObject["Title"].GetString(),
                                                            groupObject["Subtitle"].GetString());


                foreach (JsonValue itemValue in groupObject["Items"].GetArray())
                {
                    JsonObject itemObject = itemValue.GetObject();
                    group.Items.Add(new SampleDataItem(itemObject["UniqueId"].GetString(),
                                                       itemObject["Title"].GetString(),
                                                       itemObject["Subtitle"].GetString());
                }
            
                await GetSampleDataAsync2();
                this.Groups.Add(group);
            }
             */

            await GetSampleDataAsync2();

        }

        private async Task GetSampleDataAsync2()
        {
            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri("https://api.groupme.com/v3/");
                client.BaseAddress = new Uri("https://v2.groupme.com/");
                client.DefaultRequestHeaders.Accept.Clear();

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // TODO: add your token here
                HttpResponseMessage response = await client.GetAsync("inbox?token=XXXX");

                if (response.IsSuccessStatusCode)
                {
                    string respString = await response.Content.ReadAsStringAsync();

                    JObject jsonResp = JObject.Parse(respString);

                    JsonObject jsonObject = JsonObject.Parse(respString);

                    foreach (JObject groupObject in jsonResp["response"]["inbox"])
                    {
                        string imagePath;
                        try
                        {
                            imagePath = (string) groupObject["avatar_url"];
                            if (string.IsNullOrEmpty(imagePath))
                                imagePath = "Assets/avatar1.jpg";
                        }
                        catch (Exception)
                        {
                            imagePath = "Assets/avatar1.jpg";
                        }

                        var name = (string) groupObject["label"];
                        var group = new SampleDataGroup(
                            (string)groupObject["meta"]["group_id"],
                            (string)groupObject["type"],
                            name,
                            "",
                            imagePath);

                        Groups.Add(group);

                    }
                }
            }
           
        }
    }
}