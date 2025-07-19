using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Java.Util;

namespace Project12
{
    public class Account
    {
        private string id, name, hashedPassword;
        private double reminder;
        //private Dictionary<string, Transfer> transfers;

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string HashedPassword { get => hashedPassword; set => hashedPassword = value; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public double Remainder { get => reminder; set => reminder = value; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public Dictionary<string, Transfer> Transfers { get; set; } = new Dictionary<string, Transfer>();

        public Account(string id, string name, string hashed_password, double remainder = 5000)
        {
            Id = id;
            Name = name;
            HashedPassword = hashed_password;
            Remainder = remainder;
            Transfers = new Dictionary<string, Transfer>();
        }

        public Account() { }

        public override string ToString()
        {
            string result = $"Name: {Name}\nRemainder: {Remainder}\n";
            foreach (var kv in Transfers)
            {
                result += $"ID: {kv.Key}\n{kv.Value}\n";
            }
            return result;
        }

        public void addAmmount(double ammunnt)
        {
            Remainder += ammunnt;
        }

        public Dictionary<string, Transfer> GetWaitingTransfers()
        {
            Dictionary<string, Transfer> toReturn = new Dictionary<string, Transfer>();
            if (Transfers != null)
            {
                foreach (var pair in Transfers)
                {
                    if (pair.Value.Status == Transfer.RequestStatus.waiting)
                    {
                        toReturn.Add(pair.Key, pair.Value);
                    }
                }
            }

            return toReturn;

        }
    }

}