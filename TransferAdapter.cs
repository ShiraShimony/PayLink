using Android.App;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project12
{
    public class TransferAdapter : BaseAdapter<KeyValuePair<string, Transfer>>
    {
        private readonly Activity context;
        private readonly List<KeyValuePair<string, Transfer>> transfers;
        private readonly string accountId;
        private readonly FirebaseManager firebaseManager;
        private Action onTransfersChanged;
        private bool onlyView;



        public TransferAdapter(Activity context, Dictionary<string, Transfer> transfers, string accountId, string firebaseManager,bool onlyView, Action onTransfersChange = null)
        {
            this.context = context;
            this.transfers = SortTransfersByDateDescending(transfers);
            this.accountId = accountId;
            this.firebaseManager = new FirebaseManager(firebaseManager);
            this.onTransfersChanged = onTransfersChange; 
            this.onlyView = onlyView;
        }


        /// <summary>
        /// Sorts a dictionary of transfers by date in descending order.
        /// Assumes each transfer's date is stored as a string and tries to parse it.
        /// Transfers with invalid dates are treated as having DateTime.MinValue.
        /// 
        /// Uses: DateTime.TryParse - a .NET method to safely parse date strings.
        /// </summary>
        /// <param name="transfers">Dictionary of transfer ID and Transfer objects</param>
        /// <returns>Sorted list of key-value pairs</returns>
        private List<KeyValuePair<string, Transfer>> SortTransfersByDateDescending(Dictionary<string, Transfer> transfers)
        {
            List<KeyValuePair<string, Transfer>> pairs = new List<KeyValuePair<string, Transfer>>(transfers);

            pairs.Sort(CompareTransfersByDateDescending);

            return pairs;
        }

        /// <summary>
        /// Comparison method to sort transfers from newest to oldest.
        /// </summary>
        private int CompareTransfersByDateDescending(KeyValuePair<string, Transfer> a, KeyValuePair<string, Transfer> b)
        {
            DateTime dateA = ParseDateOrMin(a.Value.Date);
            DateTime dateB = ParseDateOrMin(b.Value.Date);

            // For descending order, we reverse the comparison
            return dateB.CompareTo(dateA);
        }

        /// <summary>
        /// Attempts to parse a date string into a DateTime.
        /// Returns DateTime.MinValue if parsing fails.
        /// 
        /// Uses: DateTime.TryParse - from System namespace (not Android-specific).
        /// </summary>
        private DateTime ParseDateOrMin(string dateStr)
        {
            DateTime dt;
            try
            {
                dt = DateTime.Parse(dateStr);
            }
            catch (FormatException)
            {
                dt = DateTime.MinValue;
            }
            return dt;

        }

        public override int Count => transfers.Count;

        public override KeyValuePair<string, Transfer> this[int position] => transfers[position];

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView ?? context.LayoutInflater.Inflate(Resource.Layout.transfer_list_item, parent, false);

            TextView mainText = view.FindViewById<TextView>(Resource.Id.transferMainText);
            TextView subText = view.FindViewById<TextView>(Resource.Id.transferSubText);
            Button approveButton = view.FindViewById<Button>(Resource.Id.approveButton);
            Button rejectButton = view.FindViewById<Button>(Resource.Id.rejectButton);

            var pair = transfers[position];
            string transferId = pair.Key;
            Transfer transfer = pair.Value;

            string transferType = "Transfer";
            if (transfer.IsARequest) transferType = "Request";

            mainText.Text = $"{transfer.Source} → {transfer.Dest} | {transfer.Date}";
            subText.Text = $"{transferType} | Amount: {transfer.Amount}₪ | Status: {transfer.Status}";

            if (!onlyView && transfer.Dest == accountId && transfer.Status == Transfer.RequestStatus.waiting)
            {
                approveButton.Visibility = ViewStates.Visible;
                rejectButton.Visibility = ViewStates.Visible;

                approveButton.Click += async (s, e) =>
                {
                    approveButton.Enabled = false;
                    rejectButton.Enabled = false;

                    try
                    {
                        await firebaseManager.ApproveTransfer(transferId, accountId);
                        Account account = await firebaseManager.GetAccountAsync(accountId);
                        Toast.MakeText(context, "Transfer approved", ToastLength.Short).Show();
                        transfer.Status = Transfer.RequestStatus.approved;

                        // Notify any subscribers that the list of transfers has changed.
                        // This safely invokes the delegate only if there are listeners attached.
                        onTransfersChanged?.Invoke();

                        NotifyDataSetChanged();
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(context, "Error: " + ex.Message, ToastLength.Long).Show();
                    }
                };

                rejectButton.Click += async (s, e) =>
                {
                    approveButton.Enabled = false;
                    rejectButton.Enabled = false;

                    try
                    {
                        await firebaseManager.RejectTransferAsync(transferId, accountId);
                        Toast.MakeText(context, "Transfer rejected", ToastLength.Short).Show();
                        transfer.Status = Transfer.RequestStatus.rejected;
                        NotifyDataSetChanged();
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(context, "Error: " + ex.Message, ToastLength.Long).Show();
                    }
                };
            }
            else
            {
                approveButton.Visibility = ViewStates.Gone;
                rejectButton.Visibility = ViewStates.Gone;
            }

            return view;
        }

        public void SetOnTransfersChangedCallback(Action callback)
        {
            onTransfersChanged = callback;
        }

    }
}
