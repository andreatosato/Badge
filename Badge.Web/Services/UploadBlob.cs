using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Badge.Web.Startup;

namespace Badge.Web.Services
{
    public interface IUploadBlob
    {
        Task<string> UploadFile(byte[] image, string nomeblob);
    }

    public class UploadBlob : IUploadBlob
    {
        private string _azureStorageOption;
        private CloudBlobClient _blobClient;

        public UploadBlob(string azureConnectionString)
        {
            _azureStorageOption = azureConnectionString;


            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount =CloudStorageAccount.Parse(_azureStorageOption);

            //Create the blob client.
            _blobClient = storageAccount.CreateCloudBlobClient();

        }

        public async Task<string> UploadFile(byte[] image, string nomeblob)
        {
            // Retrieve reference to a previously created container.
            CloudBlobContainer container = _blobClient.GetContainerReference("images");
            var permission = new BlobContainerPermissions() { PublicAccess = BlobContainerPublicAccessType.Blob };
            await container.SetPermissionsAsync(permission);
            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(nomeblob);
            await blockBlob.UploadFromByteArrayAsync(image, 0, image.Length);
            return blockBlob.Uri.AbsoluteUri;
        }
    }
}
