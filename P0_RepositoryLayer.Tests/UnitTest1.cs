using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using P0_RepositoryLayer.Models;
using System.Linq;
using P0_CLibrary.Models;

namespace P0_RepositoryLayer.Tests
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("Gabriel", "Schroeder")]
        [InlineData("None", "_")]
        [InlineData("Lorem", "Ipsum")]
        public void AddCustomerToDatabase(string strFirstName, string strLastName)
        {
            //Arrange
            DbContextOptions<StoreDbContext> options =  new DbContextOptionsBuilder<StoreDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

            Customer myGeneratedCustomer = new Customer();

            //Act
            using (StoreDbContext dbContext = new StoreDbContext( options ))
            {
                P0_RLayer myRepoLayer = new P0_RLayer( dbContext );

                myRepoLayer.CreateCustomer();
                myGeneratedCustomer = myRepoLayer.Customers.FirstOrDefault( x => x.ToString() == $"{strFirstName} {strLastName}" );
            }

            //Assert
            using (StoreDbContext dbContext = new StoreDbContext( options ))
            {
                P0_RLayer myRepoLayer = new P0_RLayer( dbContext );

                Customer DbCustomer = dbContext.Customers.FirstOrDefault(  x => x.ToString() == $"{strFirstName} {strLastName}");

                Assert.Equal(myGeneratedCustomer.CustomerID, DbCustomer.CustomerID);
            }

        }


    }
}
