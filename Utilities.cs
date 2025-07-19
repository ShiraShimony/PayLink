using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Project12
{
    internal class Utilities
    {
        /// <summary>
        /// Computes the SHA-256 hash of the given input string and returns the hash as a byte array.
        /// </summary>
        public static byte[] GetHash(string inputString)
        {
            // Use SHA256 hashing algorithm
            using (HashAlgorithm algorithm = SHA256.Create())
            {
                // Convert input string to UTF-8 bytes and compute hash
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
            }
        }

        /// <summary>
        /// Computes the SHA-256 hash of the given input string and returns the hash as a hexadecimal string.
        /// </summary>
        public static string GetHashString(string inputString)
        {
            // Initialize a string builder to store the hex characters
            StringBuilder sb = new StringBuilder();

            // Get the byte array of the hash
            foreach (byte b in GetHash(inputString))
            {
                // Convert each byte to two-digit hexadecimal and append
                sb.Append(b.ToString("X2"));
            }

            // Return the resulting hex string (uppercase)
            return sb.ToString();
        }

        /// <summary>
        /// Asynchronously retrieves the conversion rate from a given currency to Israeli Shekels (ILS) and returns the converted amount.
        /// </summary>
        /// <param name="currency">The currency code (e.g., "USD", "EUR").</param>
        /// <param name="amount">The amount to convert.</param>
        /// <returns>The equivalent amount in ILS.</returns>
        public async Task<double> GetCurrencyToShekelRateAsync(string currency, int amount)
        {
            // If the currency is already ILS, just return the amount
            if (currency == "ILS") return amount;

            // Ensure TLS 1.2 protocol for HTTPS request
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            using (HttpClient client = new HttpClient())
            {
                // Compose the URL for the currency conversion API- exchangerate.host
                string url = $"https://api.exchangerate.host/convert?access_key=7306466f48dc75d5c491d54e2ebf1807&from={currency}&to=ILS&amount={amount}";

                try
                {
                    // Send the HTTP GET request
                    HttpResponseMessage response = await client.GetAsync(url);

                    // Check if the response was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the JSON content of the response
                        string json = await response.Content.ReadAsStringAsync();

                        // Parse the JSON to extract the result field (converted value)
                        JObject obj = JObject.Parse(json);
                        return obj["result"]?.Value<double>() ?? 0;
                    }
                    else
                    {
                        // If not successful, throw an exception with status code
                        throw new Exception("HTTP Error: " + response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    // Log and rethrow exceptions to be handled by the caller
                    System.Diagnostics.Debug.WriteLine("Currency fetch failed: " + ex.Message);
                    throw;
                }
            }
        }
    }
}
