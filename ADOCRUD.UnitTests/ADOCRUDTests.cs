using ADOCRUD.UnitTests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ADOCRUD.UnitTests
{
    [TestClass]
    public class ADOCRUDTests
    {
        private int sampleItemId { get; set; }
        public ADOCRUDTests()
        {

        }

        [TestInitialize]
        public void Test_Intialize()
        {
            // Inserts a sample row to test
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["UnitTestDB"].ToString()))
            {
                conn.Open();

                StringBuilder nonQuery = new StringBuilder();
                nonQuery.Append("insert into Product (CategoryId, Name, Price, Description) ");
                nonQuery.Append("values (@categoryId, @name, @price, @description) ");
                nonQuery.Append("select @id = @@identity ");

                SqlCommand cmd = new SqlCommand(nonQuery.ToString(), conn);
                cmd.Parameters.Add("categoryId", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("name", SqlDbType.VarChar).Value = "Volleyball";
                cmd.Parameters.Add("price", SqlDbType.Decimal).Value = 10.99m;
                cmd.Parameters.Add("description", SqlDbType.VarChar).Value = "Soft ball that does not hurt the wrist";
                cmd.Parameters.Add("id", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                sampleItemId = Convert.ToInt32(cmd.Parameters["id"].Value);

                conn.Close();
            }
        }

        [TestCleanup]
        public void Test_Cleanup()
        {
            // Removes sample item from database 
            sampleItemId = 0;

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["UnitTestDB"].ToString()))
            {
                conn.Open();

                StringBuilder nonQuery = new StringBuilder();
                nonQuery.Append("delete from Product where Id = @itemId");

                SqlCommand cmd = new SqlCommand(nonQuery.ToString(), conn);
                cmd.Parameters.Add("itemId", SqlDbType.Int).Value = sampleItemId;
                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }

        /// <summary>
        /// Test insert was successful
        /// </summary>
        [TestMethod]
        public void TestInsert_Commit()
        {
            using (ADOCRUDContext context = new ADOCRUDContext(ConfigurationManager.ConnectionStrings["UnitTestDB"].ToString()))
            {
                Product p = new Product();
                p.CategoryId = 1;
                p.Name = "Basketball";
                p.Description = "NBA Size";
                p.Price = 59.99m;

                context.Insert<Product>(p);
                context.Commit();

                Product retreivedProduct = context.QueryItems<Product>("select * from dbo.Product where Id = @id", new { id = p.Id }).FirstOrDefault();

                Assert.IsTrue(retreivedProduct != null &&
                    retreivedProduct.Id > 0 &&
                    retreivedProduct.CategoryId == 1 &&
                    retreivedProduct.Name == "Basketball" &&
                    retreivedProduct.Description == "NBA Size" &&
                    retreivedProduct.Price == 59.99m, "An error occurred on insert");

            }
        }

        /// <summary>
        /// Test update was successful
        /// </summary>
        [TestMethod]
        public void TestUpdate_Commit()
        {
            using (ADOCRUDContext context = new ADOCRUDContext(ConfigurationManager.ConnectionStrings["UnitTestDB"].ToString()))
            {
                Product p = context.QueryItems<Product>("select * from Product where Id = @id", new { id = sampleItemId }).First();
                p.Name = "Waffle Maker";
                p.CategoryId = 3;
                p.Description = "Makes waffles";
                p.Price = 29.99m;

                context.Update<Product>(p);
                context.Commit();

                Product retreivedProduct = context.QueryItems<Product>("select * from dbo.Product where Id = @id", new { id = p.Id }).FirstOrDefault();

                Assert.IsTrue(retreivedProduct != null &&
                    retreivedProduct.Id == p.Id &&
                    retreivedProduct.CategoryId == 3 && 
                    p.Name == "Waffle Maker" && 
                    p.Description == "Makes waffles" &&
                    p.Price == 29.99m, "An error occurred on update");
            }
        }

        /// <summary>
        /// Test that changes were rolled back on failed insert
        /// </summary>
        [TestMethod]
        public void TestInsert_NoCommit()
        {
            Product p = new Product();
            Product retreivedProduct = null;

            using (ADOCRUDContext context = new ADOCRUDContext(ConfigurationManager.ConnectionStrings["UnitTestDB"].ToString()))
            {
                p.CategoryId = 1;
                p.Name = "Basketball";
                p.Description = "NBA Size";
                p.Price = 29.99m;

                context.Insert<Product>(p);
            }

            using (ADOCRUDContext context = new ADOCRUDContext(ConfigurationManager.ConnectionStrings["UnitTestDB"].ToString()))
            {
                retreivedProduct = context.QueryItems<Product>("select * from dbo.Product where Id = @id", new { id = p.Id }).FirstOrDefault();
            }

            Assert.IsTrue(retreivedProduct == null, "Insert should have failed but succeeded");
        }

        /// <summary>
        /// Test that changes were rolled back on failed update
        /// </summary>
        [TestMethod]
        public void TestUpdate_NoCommit()
        {
            Product p = null;
            Product retreivedProduct = null;

            using (ADOCRUDContext context = new ADOCRUDContext(ConfigurationManager.ConnectionStrings["UnitTestDB"].ToString()))
            {
                p = context.QueryItems<Product>("select * from Product where Id = @id", new { id = sampleItemId }).First();
                p.Name = "Waffle Maker";
                p.CategoryId = 3;
                p.Description = "Makes waffles";
                p.Price = 29.99m;

                context.Update<Product>(p);
            }

            using (ADOCRUDContext context = new ADOCRUDContext(ConfigurationManager.ConnectionStrings["UnitTestDB"].ToString()))
            {
                retreivedProduct = context.QueryItems<Product>("select * from dbo.Product where Id = @id", new { id = p.Id }).FirstOrDefault();
            }

            Assert.IsTrue(retreivedProduct != null &&
                retreivedProduct.Id == p.Id &&
                retreivedProduct.CategoryId != 3 &&
                retreivedProduct.Name != "Waffle Maker" &&
                retreivedProduct.Description != "Makes waffles" &&
                retreivedProduct.Price != 29.99m, "Update should have failed but succeeded");
        }

        /// <summary>
        /// Test multiple DB calls were saved correctly
        /// </summary>
        [TestMethod]
        public void TestMultipleDBCallsOnSingleTransaction_Commit()
        {
            Product p1 = null;
            Product p2 = null;

            Product retreivedProduct1 = null;
            Product retreivedProduct2 = null;

            using (ADOCRUDContext context = new ADOCRUDContext(ConfigurationManager.ConnectionStrings["UnitTestDB"].ToString()))
            {
                p1 = new Product();
                p1.CategoryId = 1;
                p1.Name = "Football";
                p1.Description = "NFL Size";
                p1.Price = 9.99m;

                p2 = new Product();
                p2.CategoryId = 2;
                p2.Name = "Shirt";
                p2.Description = "Made from cotton";
                p2.Price = 15.99m;

                context.Insert<Product>(p1);
                context.Insert<Product>(p2);
                context.Commit();
            }

            using (ADOCRUDContext context = new ADOCRUDContext(ConfigurationManager.ConnectionStrings["UnitTestDB"].ToString()))
            {
                retreivedProduct1 = context.QueryItems<Product>("select * from dbo.Product where Id = @id", new { id = p1.Id }).FirstOrDefault();
                retreivedProduct2 = context.QueryItems<Product>("select * from dbo.Product where Id = @id", new { id = p2.Id }).FirstOrDefault();
            }

            Assert.IsTrue(retreivedProduct1 != null &&
                retreivedProduct1.Id > 0 &&
                retreivedProduct1.Name == "Football" &&
                retreivedProduct1.Description == "NFL Size" &&
                retreivedProduct1.Price == 9.99m, "An error occurred on first insert");

            Assert.IsTrue(retreivedProduct2 != null &&
                retreivedProduct2.Id > 0 &&
                retreivedProduct2.Name == "Shirt" &&
                retreivedProduct2.Description == "Made from cotton" &&
                retreivedProduct2.Price == 15.99m, "An error occurred on 2nd insert");
        }

        /// <summary>
        /// Test multiple DB calls rolled back on failed saves
        /// </summary>
        [TestMethod]
        public void TestMultipleDBCallsOnSingleTransaction_NoCommit()
        {
            Product p1 = null;
            Product p2 = null;

            Product retreivedProduct1 = null;
            Product retreivedProduct2 = null;

            using (ADOCRUDContext context = new ADOCRUDContext(ConfigurationManager.ConnectionStrings["UnitTestDB"].ToString()))
            {
                p1 = new Product();
                p1.CategoryId = 1;
                p1.Name = "Football";
                p1.Description = "NFL Size";
                p1.Price = 9.99m;

                p2 = new Product();
                p2.CategoryId = 2;
                p2.Name = "Shirt";
                p2.Description = "Made from cotton";
                p2.Price = 15.99m;

                context.Insert<Product>(p1);
                context.Insert<Product>(p2);
            }

            using (ADOCRUDContext context = new ADOCRUDContext(ConfigurationManager.ConnectionStrings["UnitTestDB"].ToString()))
            {
                retreivedProduct1 = context.QueryItems<Product>("select * from dbo.Product where Id = @id", new { id = p1.Id }).FirstOrDefault();
                retreivedProduct2 = context.QueryItems<Product>("select * from dbo.Product where Id = @id", new { id = p2.Id }).FirstOrDefault();
            }

            Assert.IsTrue(retreivedProduct1 == null, "Insert should have failed, but succeeded");
            Assert.IsTrue(retreivedProduct2 == null, "Insert should have failed, but succeeded");
        }
    }
}
