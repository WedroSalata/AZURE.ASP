using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Blobs.Models;
using System.Reflection.Metadata;

namespace LR1App.Pages
{
    public class CreateBlobModel : PageModel
    {
        public string? blobList { get; set; }
        public string? errors { get; set; }
        public BlobContainerClient? container { get; set; }
        public string sasKey = "SharedAccessSignature=sv=2023-01-03&ss=btqf&srt=sco&st=2024-03-09T11%3A40%3A25Z&se=2024-04-06T10%3A40%3A00Z&sp=rwdxlacu&sig=C8LCWBJFeacGp0yBlRPHFB7D7%2FwTZuMvixuM%2B61HVGY%3D;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;FileEndpoint=http://devstoreaccount1.file.core.windows.net;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";


        public void OnGet()
        {
            try
            {
                blobList = listing(Authenticate(sasKey, "111"));
            }
            catch (Exception e)
            {
                errors = $"Error while creating: {e}";
            }

        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                container ??= Authenticate(sasKey, "111");
                this.errors = await Create(container, "HelloWorld");
            }
            catch (Exception e)
            {
                errors = $"Error while creating: {e}";
            }

            return RedirectToPage("./../Index");
        }
        public BlobContainerClient Authenticate(string STRING_FROM_EXPLORER, string CONTAINER_NAME)
        {
            // Create a client that can authenticate with a connection string
            BlobServiceClient blobServiceClient = new(STRING_FROM_EXPLORER);

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(CONTAINER_NAME);

            containerClient.CreateIfNotExists();
            return containerClient;
        }

        public async Task<string> Create(BlobContainerClient containerClient, string fileName)
        {
            try
            {
                // Define the file content you want to upload
                string fileContent = "Hello, World112312312123!";
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(fileContent);

                // Convert the byte array to a stream
                using MemoryStream stream = new(byteArray);
                // Get a reference to the blob
                BlobClient blobClient = containerClient.GetBlobClient($"{fileName}.txt");
                // BlobClient blobClient = blobClient.Upload($"{fileName}.txt");

                // Upload the stream to the blob
                await blobClient.UploadAsync(stream, overwrite: true);

                return "Success";
            }
            catch (Exception e)
            {
                return $"Error while creating: {e}";
            }
        }
        public string listing(BlobContainerClient containerClient)
        {
            string list = "";

            foreach (var blobItem in containerClient.GetBlobs())
            {
                list += blobItem.Name + ", ";
            }
            return list;
        }
    }
}