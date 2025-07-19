using Project12;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class AccountNotFoundException : Exception
    {
        public AccountNotFoundException(string accountName)
            : base($"Account '{accountName}' was not found.") { }
    }

    public class MissingTransfersException : Exception
    {
        public MissingTransfersException(string accountName)
            : base($"Transfers dictionary is missing for account '{accountName}'.") { }
    }

    public class TransferNotFoundException : Exception
    {
        public TransferNotFoundException(string transferId, string accountName)
            : base($"Transfer '{transferId}' was not found in account '{accountName}'.") { }
    }

    public class InvalidTransferAmountException : Exception
    {
        public InvalidTransferAmountException(double amount)
            : base($"Invalid transfer amount: {amount}. Amount must be positive.") { }
    }

    public class InvalidTransferStatusException : Exception
    {
        public InvalidTransferStatusException(string transferId, Transfer.RequestStatus actualStatus)
            : base($"Transfer '{transferId}' is not in 'waiting' status (current: {actualStatus}).") { }
    }
