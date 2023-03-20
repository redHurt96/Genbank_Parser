using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace GenbankParser
{
    public partial class MainWindow : Window
    {
        private static readonly Dictionary<string, Action<Dictionary<string, string>, string>> _parseExpressions = new Dictionary<string, Action<Dictionary<string, string>, string>>()
        {
            [@"LOCUS\s+(\S+)"] = (data, value) => data["locus"] = value,
            [@"fwd_name: (\S+)[,|""]"] = (data, value) => data["fwdName"] = value,
            [@"fwd_seq:\s+(\S+)[,|""]"] = (data, value) => data["fwdSeq"] = value,
            [@"rev_name:\s+(\S+)[,|""]"] = (data, value) => data["revName"] = value,
            [@"rev_seq:\s+(\S+)[,|""]"] = (data, value) => data["revSeq"] = value,
            [@"(\S+) bp"] = (data, value) => data["Bp"] = value,
            //[@"AUTHORS\s+([a-z|A-Z| ]+)"] = (data, value) => data.Author = value,
            //[@"JOURNAL\s+Submitted\s+[(](\S+)[)]"] = (data, value) => data.SubmittedDate = value,
            //[@"specimen_voucher=""([\w| |<|>|:|-]+)"] = (data, value) => data.SpecimenVoucher = value,
            //[@"country=""([\w| ]+)"] = (data, value) => data.Country = value,
            //[@"country=""(?:[\w| ]+): ([\w+,\s]+)"] = (data, value) => data.Location = Regex.Replace(value, @"(\s{2,})", " "),
            //[@"lat_lon=""([\d]+.[\d]+ \w)"] = (data, value) => data.Latitude = value,
            //[@"lat_lon=""(?:[\d]+.[\d]+ \w) ([\d]+.[\d]+ \w)"] = (data, value) => data.Longitude = value,
            //[@"collection_date=""(\S+)"""] = (data, value) => data.CollectionDate = value,
            //[@"notabilis (L\d)"] = (data, value) => data.Lineage = value,
        };

        private List<Dictionary<string, string>> _values;

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
            WriteData();
        }

        private void LoadData()
        {
            string path = Path.Combine(Environment.CurrentDirectory, "db.gb");
            string rawData = File.ReadAllText(path);
            string[] splitData = rawData.Split(new string[] {"//"}, StringSplitOptions.RemoveEmptyEntries);

            _values = new List<Dictionary<string, string>>();

            foreach (string line in splitData)
            {
                if (string.IsNullOrWhiteSpace(line)) 
                    continue;

                Dictionary<string, string> value = new Dictionary<string, string>();

                foreach (var pair in _parseExpressions)
                {
                    string pattern = pair.Key;
                    Match match = Regex.Match(line, pattern);

                    if (match.Success)
                        pair.Value(value, match.Groups[1].Value);
                }

                ////correct voucher
                //if (string.IsNullOrWhiteSpace(value.SpecimenVoucher))
                //{
                //    Match match = Regex.Match(line, @"isolate=""(\S+)""");

                //    if (match.Success)
                //        value.SpecimenVoucher = match.Groups[1].Value;
                //}

                //if (string.IsNullOrWhiteSpace(value.SpecimenVoucher))
                //{
                //    Match match = Regex.Match(line, @"strain=""(\S+)""");

                //    if (match.Success)
                //        value.SpecimenVoucher = match.Groups[1].Value;
                //}

                if (value["Bp"].Length > 3)
                    continue;

                _values.Add(value);
            }
        }

        private void WriteData()
        {
            string path = Path.Combine("D:\\Downloads", "parsedData.csv");
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"Code, FwdName, FwdSeq, RevName, RevSeq");

            foreach (var data in _values)
                builder.AppendLine($"{data["locus"]}, {data.GetOfDefault("fwdName")}, {data.GetOfDefault("fwdSeq")}, {data.GetOfDefault("revName")}, {data.GetOfDefault("revSeq")}");
            
            File.WriteAllText(path, builder.ToString());
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) { }
    }
}
