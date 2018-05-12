using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureStorage.Controllers
{
    [Route("Storage")]
    public class StorageController : Controller
    {

        




        public IActionResult Index()
        {
            
           

            var getConn = new AppConnection();

            var accountName = getConn.GetAccount();
            var accountKey = getConn.GetKey();

            StorageCredentials credentials = new StorageCredentials(accountName, accountKey);
            var storageAccount = new CloudStorageAccount(credentials, true);



            //var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("customer");
            table.CreateIfNotExistsAsync();

            var customer = new CustomerEntity(Guid.NewGuid())
            {
                FirstName = "Tim",
                LastName = "Oleson",
                Email = "timothy.oleson@gmail.com",
                PhoneNumber = "443-520-4391"
            };

            var insertOperation = TableOperation.Insert(customer);
            table.ExecuteAsync(insertOperation);
            return View(customer);
        }
    }

    public class CustomerEntity : TableEntity
    {

        public CustomerEntity(Guid employeeId)
        {
            PartitionKey = "Customer";
            RowKey = employeeId.ToString();
        }

        public CustomerEntity()
        {


        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }


    public class AppConnection
    {
        public AppConnection()
        {
            
        }

        public static IConfigurationRoot Configuration;

        public string GetAccount()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            var AccountName = Configuration["AccountName:Value"];
           
            
            return AccountName;

        }

        public string GetKey()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            
           
            var Key = Configuration["Key:Value"];
            return Key;

        }






    }
    //public class CustomerContext : DbContext
    //{
    //    public DbSet<CustomerEntity> Customer { get; set; }

    //    public CustomerContext(DbContextOptions<CustomerContext> options)
    //        : base(options)
    //    { }



    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    {
    //        optionsBuilder.UseSqlServer("StorageConnectionString");
    //    }
    //}


}
    
