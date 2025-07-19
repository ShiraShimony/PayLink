using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Project12
{
    public class Transfer
    {
        public enum RequestStatus { waiting = 0, approved = 1, rejected = 2 };
        private string date, source, destination;
        private double ammount;
        private bool isARequest;
        private RequestStatus status;
        public string Date { get => date; set => date = value; }
        public string Source { get => source; set => source = value; }
        public string Dest { get => destination; set => destination = value; }

        public double Amount { get => ammount; set => ammount = value; }
        public bool IsARequest { get => isARequest; set => isARequest = value; }
        public RequestStatus Status { get => status; set => status = value; }



        // קונסטרקטור ריק - נדרש עבור Firebase
        public Transfer() { }

        // קונסטרקטור מלא לשימוש עצמי
        public Transfer(string date, string source, string dest, double amount, bool isARequest, RequestStatus status)
        {
            Date = date;
            Source = source;
            Dest = dest;
            Amount = amount;
            IsARequest = isARequest;
            Status = status;
        }

        public override string ToString()
        {
            string title = "Transfer:", strStatus = "";
            switch (Status)
            {
                case RequestStatus.waiting:
                    strStatus = "waiting to be approved";
                    break;
                case RequestStatus.rejected:
                    strStatus = "Tranfer was rejected";
                    break;
                case RequestStatus.approved:
                    strStatus = "Successfully executed";
                    break;
            }
            if (IsARequest) title = "Request:";
            return $"{title}\nDate: {Date}\nDestination: {Dest}\nAmount: {Amount}\nStatus: {strStatus}";
        }
    }
}