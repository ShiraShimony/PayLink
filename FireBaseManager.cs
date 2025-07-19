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

namespace Project12
{
    internal class FirebaseManager
    {
        private readonly FirebaseClient _firebaseClient;

        public FirebaseManager(string firebaseUrl)
        {
            _firebaseClient = new FirebaseClient(firebaseUrl);
        }

        public async Task SaveAccountAsync(Account account)
        {
            await _firebaseClient
                .Child("accounts")
                .Child(account.Id)
                .PutAsync(account);
        }

        public async Task<Account> GetAccountAsync(string title)
        {
            var account = await _firebaseClient.Child("accounts").Child(title).OnceSingleAsync<Account>();
            if (account == null)
                throw new AccountNotFoundException(title);
            return account;
        }

        public async Task<List<Account>> GetAccountsAsync()
        {
            var accounts = await _firebaseClient
                .Child("accounts")
                .OnceAsync<Account>();

            return accounts.Select(a => a.Object).ToList();
        }

        public async Task UpdateAccountAsync(string accountId, Account updatedAccount)
        {
            await _firebaseClient
                .Child("accounts")
                .Child(accountId)
                .PutAsync(updatedAccount);
        }

        public async Task DeleteAccountAsync(string accountId)
        {
            await _firebaseClient
                .Child("accounts")
                .Child(accountId)
                .DeleteAsync();
        }

        public async Task SendTransferAsync(string fromId, string toId, double amount, bool isARequest)
        {
            if (amount <= 0)
                throw new InvalidTransferAmountException(amount);

            var fromAcc = await GetAccountAsync(fromId);
            var toAcc = await GetAccountAsync(toId);

            if (fromAcc.Transfers == null)
                throw new MissingTransfersException(fromId);
            if (toAcc.Transfers == null)
                throw new MissingTransfersException(toId);

            string id = Guid.NewGuid().ToString();
            var newTransfer = new Transfer(DateTime.Now.ToString("dd/MM/yyyy"), fromId, toId, amount, isARequest, Transfer.RequestStatus.waiting);

            toAcc.Transfers[id] = newTransfer;
            fromAcc.Transfers[id] = newTransfer;

            toAcc.Transfers = toAcc.Transfers
                .OrderBy(kv => kv.Value.Status)
                .ThenBy(kv => DateTime.TryParse(kv.Value.Date, out var dt) ? dt : DateTime.MaxValue)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            fromAcc.Transfers = fromAcc.Transfers
                .OrderBy(kv => kv.Value.Status)
                .ThenBy(kv => DateTime.TryParse(kv.Value.Date, out var dt) ? dt : DateTime.MaxValue)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            await SaveAccountAsync(toAcc);
            await SaveAccountAsync(fromAcc);
        }

        public async Task<Transfer?> GetTransferAsync(string accountId, string transferId)
        {
            var transfer = await _firebaseClient
                .Child("accounts")
                .Child(accountId)
                .Child("Transfers")
                .Child(transferId)
                .OnceSingleAsync<Transfer>();

            if (transfer == null)
                throw new TransferNotFoundException(transferId, accountId);

            return transfer;
        }

        public async Task UpdateTransferAsync(string accountId, string transferId, Transfer updatedTransfer)
        {
            await _firebaseClient
                .Child("accounts")
                .Child(accountId)
                .Child("Transfers")
                .Child(transferId)
                .PutAsync(updatedTransfer);
        }

        public async Task ApproveTransfer(string transferId, string destId)
        {
            var transfer = await GetTransferAsync(destId, transferId);

            if (transfer.Status != Transfer.RequestStatus.waiting)
                throw new InvalidTransferStatusException(transferId, transfer.Status);

            Account dest = await GetAccountAsync(destId);
            Account source = await GetAccountAsync(transfer.Source);

            if (dest.Transfers == null)
                throw new MissingTransfersException(destId);
            if (source.Transfers == null)
                throw new MissingTransfersException(transfer.Source);

            int transferCoefficient = transfer.IsARequest ? -1 : 1;

            dest.addAmmount(transfer.Amount * transferCoefficient);
            source.addAmmount(transfer.Amount * transferCoefficient * -1);

            transfer.Status = Transfer.RequestStatus.approved;
            dest.Transfers[transferId] = transfer;
            source.Transfers[transferId] = transfer;

            await UpdateAccountAsync(dest.Id, dest);
            await UpdateAccountAsync(source.Id, source);
        }

        public async Task RejectTransferAsync(string transferId, string destId)
        {
            var transfer = await GetTransferAsync(destId, transferId);

            if (transfer.Status != Transfer.RequestStatus.waiting)
                throw new InvalidTransferStatusException(transferId, transfer.Status);

            Account dest = await GetAccountAsync(destId);
            Account source = await GetAccountAsync(transfer.Source);

            if (dest.Transfers == null)
                throw new MissingTransfersException(destId);
            if (source.Transfers == null)
                throw new MissingTransfersException(transfer.Source);

            transfer.Status = Transfer.RequestStatus.rejected;

            dest.Transfers[transferId] = transfer;
            source.Transfers[transferId] = transfer;

            await UpdateAccountAsync(dest.Id, dest);
            await UpdateAccountAsync(source.Id, source);
        }

        public async Task<bool> CheckPassword(string id, string password)
        {
            Account account = await GetAccountAsync(id);
            return (account.HashedPassword) == password;
        }

    }
}