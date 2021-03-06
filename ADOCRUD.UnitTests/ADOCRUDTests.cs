﻿using ADOCRUD.UnitTests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
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
        private int productId { get; set; }
        private int secondProductId { get; set; }
        private int nullableId { get; set; }
        private int nonNullableId { get; set; }
        private int userId { get; set; }

        private int secondUserId { get; set; }

        private string connectionString { get; set; }

        private static Guid guid;
        public ADOCRUDTests()
        {
            connectionString = ConfigurationManager.ConnectionStrings["UnitTestDB"].ToString();
            productId = 0;
            nullableId = 0;
            nonNullableId = 0;
            userId = 0;
            guid = Guid.NewGuid();
            
        }

        [TestInitialize]
        public void Test_Intialize()
        {
            // Inserts a sample row to test
            using (SqlConnection conn = new SqlConnection(connectionString))
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

                productId = Convert.ToInt32(cmd.Parameters["id"].Value);

                conn.Close();
            }

            // Inserts a sample row to test
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                StringBuilder nonQuery = new StringBuilder();
                nonQuery.Append("insert into Product (CategoryId, Name, Price, Description) ");
                nonQuery.Append("values (@categoryId, @name, @price, @description) ");
                nonQuery.Append("select @id = @@identity ");

                SqlCommand cmd = new SqlCommand(nonQuery.ToString(), conn);
                cmd.Parameters.Add("categoryId", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("name", SqlDbType.VarChar).Value = "Baseball";
                cmd.Parameters.Add("price", SqlDbType.Decimal).Value = 5.99m;
                cmd.Parameters.Add("description", SqlDbType.VarChar).Value = "baseball to throw with your kids";
                cmd.Parameters.Add("id", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                secondProductId = Convert.ToInt32(cmd.Parameters["id"].Value);

                conn.Close();
            }

            // Inserts a sample row to test
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                StringBuilder nonQuery = new StringBuilder();
                nonQuery.Append("insert into [User] (FirstName, LastName) ");
                nonQuery.Append("values (@firstName, @lastName) ");
                nonQuery.Append("select @id = @@identity ");

                SqlCommand cmd = new SqlCommand(nonQuery.ToString(), conn);
                cmd.Parameters.Add("firstName", SqlDbType.VarChar).Value = "John";
                cmd.Parameters.Add("lastName", SqlDbType.VarChar).Value = "Williams";
                cmd.Parameters.Add("id", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                userId = Convert.ToInt32(cmd.Parameters["id"].Value);

                conn.Close();
            }

            // Inserts a sample row to test
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                StringBuilder nonQuery = new StringBuilder();
                nonQuery.Append("insert into [User] (FirstName, LastName) ");
                nonQuery.Append("values (@firstName, @lastName) ");
                nonQuery.Append("select @id = @@identity ");

                SqlCommand cmd = new SqlCommand(nonQuery.ToString(), conn);
                cmd.Parameters.Add("firstName", SqlDbType.VarChar).Value = "Mike";
                cmd.Parameters.Add("lastName", SqlDbType.VarChar).Value = "Johnson";
                cmd.Parameters.Add("id", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                secondUserId = Convert.ToInt32(cmd.Parameters["id"].Value);

                conn.Close();
            }
        }

        [TestCleanup]
        public void Test_Cleanup()
        {
            // Removes sample item from database 
            productId = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                StringBuilder nonQuery = new StringBuilder();
                nonQuery.Append("delete from Product where Id = @productId ");
                nonQuery.Append("delete from Product where Id = @secondProductId ");
                nonQuery.Append("delete from Nullable where Id = @nullableId ");
                nonQuery.Append("delete from NonNullable where Id = @nonNullableId ");
                nonQuery.Append("delete from UserProduct where UserId = @userId ");
                nonQuery.Append("delete from UserProduct where UserId = @secondUserId ");

                SqlCommand cmd = new SqlCommand(nonQuery.ToString(), conn);
                cmd.Parameters.Add("productId", SqlDbType.Int).Value = productId;
                cmd.Parameters.Add("secondProductId", SqlDbType.Int).Value = secondProductId;
                cmd.Parameters.Add("nullableId", SqlDbType.Int).Value = nullableId;
                cmd.Parameters.Add("nonNullableId", SqlDbType.Int).Value = nonNullableId;
                cmd.Parameters.Add("userId", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("secondUserId", SqlDbType.Int).Value = secondUserId;

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
            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
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
        /// Test that changes were rolled back on failed insert
        /// </summary>
        [TestMethod]
        public void TestInsert_NoCommit()
        {
            Product p = new Product();
            Product retreivedProduct = null;

            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
            {
                p.CategoryId = 1;
                p.Name = "Basketball";
                p.Description = "NBA Size";
                p.Price = 29.99m;

                context.Insert<Product>(p);
            }

            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
            {
                retreivedProduct = context.QueryItems<Product>("select * from dbo.Product where Id = @id", new { id = p.Id }).FirstOrDefault();
            }

            Assert.IsTrue(retreivedProduct == null, "Insert should have failed but succeeded");
        }

        /// <summary>
        /// Test insert of of nullable properties of all value types with non null values
        /// </summary>
        [TestMethod]
        public void TestInsert_NullablePropertiesWithValues()
        {
            Models.Nullable n = new Models.Nullable();
            n.Integer64Value = 64;
            n.ByteValue = 1;
            n.ByteArrayValue = new byte[4];
            n.ByteArrayValue[0] = 0;
            n.ByteArrayValue[1] = 1;
            n.ByteArrayValue[2] = 2;
            n.ByteArrayValue[3] = 3;
            n.BoolValue = true;
            n.CharValue = "a";
            n.DateTimeValue = Convert.ToDateTime("01/11/1984");
            n.DateTimeOffsetValue = DateTimeOffset.MaxValue;
            n.DecimalValue = 1.11m;
            n.FloatValue = 22;
            n.DoubleValue = 33;
            n.Integer32Value = 44;
            n.Single = 55;
            n.Integer16Value = 16;
            n.GuidValue = guid;
            n.StringValue = "Something";

            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
            {
                context.Insert(n);
                context.Commit();
            }

            Models.Nullable retrievedItem = null;

            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
            {
                retrievedItem = context.QueryItems<Models.Nullable>("select * from Nullable where id = @id", new { id = n.Id }).FirstOrDefault();
            }

            Assert.IsTrue(n != null &&
                n.Integer64Value == 64 &&
                n.ByteValue == 1 &&
                n.ByteArrayValue[0] == 0 &&
                n.ByteArrayValue[1] == 1 &&
                n.ByteArrayValue[2] == 2 &&
                n.ByteArrayValue[3] == 3 &&
                n.BoolValue == true &&
                n.CharValue == "a" &&
                n.DateTimeValue == Convert.ToDateTime("01/11/1984") &&
                n.DateTimeOffsetValue == DateTimeOffset.MaxValue &&
                n.DecimalValue == 1.11m &&
                n.FloatValue == 22 &&
                n.DoubleValue == 33 &&
                n.Integer32Value == 44 &&
                n.Single == 55 &&
                n.Integer16Value == 16 &&
                n.GuidValue == guid &&
                n.StringValue == "Something",
                "Insert of non-null values in nullable properties failed");
        }

        /// <summary>
        /// Test insert of nullable properties of all value types with null values
        /// </summary>
        [TestMethod]
        public void TestInsert_NullablePropertiesWithNullValues()
        {
            Models.Nullable n = new Models.Nullable();
            n.Integer64Value = null;
            n.ByteValue = null;
            n.ByteArrayValue = null;
            n.BoolValue = null;
            n.CharValue = null;
            n.DateTimeValue = null;
            n.DateTimeOffsetValue = null;
            n.DecimalValue = null;
            n.FloatValue = null;
            n.DoubleValue = null;
            n.Integer32Value = null;
            n.Single = null;
            n.Integer16Value = null;
            n.GuidValue = null;
            n.StringValue = null;

            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
            {
                context.Insert(n);
                context.Commit();
            }

            Models.Nullable retrievedItem = null;

            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
            {
                retrievedItem = context.QueryItems<Models.Nullable>("select * from Nullable where id = @id", new { id = n.Id }).FirstOrDefault();
            }

            Assert.IsTrue(n != null &&
                !n.Integer64Value.HasValue &&
                !n.ByteValue.HasValue &&
                n.ByteArrayValue == null &&
                !n.BoolValue.HasValue &&
                string.IsNullOrEmpty(n.CharValue) &&
                !n.DateTimeValue.HasValue &&
                !n.DateTimeOffsetValue.HasValue &&
                !n.DecimalValue.HasValue &&
                !n.FloatValue.HasValue &&
                !n.DoubleValue.HasValue &&
                !n.Integer32Value.HasValue &&
                !n.Single.HasValue &&
                !n.Integer16Value.HasValue &&
                !n.GuidValue.HasValue &&
                string.IsNullOrEmpty(n.StringValue),
            "Insert of null values in nullable properties failed");
        }

        /// <summary>
        /// Test Insert of non-nullable properties with values
        /// </summary>
        [TestMethod]
        public void TestInsert_NonNullablePropertiesWithValues()
        {
            NonNullable n = new NonNullable();
            n.Integer64Value = 64;
            n.ByteValue = 1;
            n.ByteArrayValue = new byte[4];
            n.ByteArrayValue[0] = 0;
            n.ByteArrayValue[1] = 1;
            n.ByteArrayValue[2] = 2;
            n.ByteArrayValue[3] = 3;
            n.BoolValue = true;
            n.CharValue = "a";
            n.DateTimeValue = Convert.ToDateTime("01/11/1984");
            n.DateTimeOffsetValue = DateTimeOffset.MaxValue;
            n.DecimalValue = 1.11m;
            n.FloatValue = 22;
            n.DoubleValue = 33;
            n.Integer32Value = 44;
            n.Single = 55;
            n.Integer16Value = 16;
            n.GuidValue = guid;
            n.StringValue = "Something";

            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
            {
                context.Insert(n);
                context.Commit();
            }

            NonNullable retrievedItem = null;

            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
            {
                retrievedItem = context.QueryItems<NonNullable>("select * from Nullable where id = @id", new { id = n.Id }).FirstOrDefault();
            }

            Assert.IsTrue(n != null &&
                n.Integer64Value == 64 &&
                n.ByteValue == 1 &&
                n.ByteArrayValue[0] == 0 &&
                n.ByteArrayValue[1] == 1 &&
                n.ByteArrayValue[2] == 2 &&
                n.ByteArrayValue[3] == 3 &&
                n.BoolValue == true &&
                n.CharValue == "a" &&
                n.DateTimeValue == Convert.ToDateTime("01/11/1984") &&
                n.DateTimeOffsetValue == DateTimeOffset.MaxValue &&
                n.DecimalValue == 1.11m &&
                n.FloatValue == 22 &&
                n.DoubleValue == 33 &&
                n.Integer32Value == 44 &&
                n.Single == 55 &&
                n.Integer16Value == 16 &&
                n.GuidValue == guid &&
                n.StringValue == "Something",
                "Insert of values in non nullable properties failed");
        }

        [TestMethod]
        public void TestInsertOfCompositeKeys_Commit()
        {
            UserProduct retrievedUserProduct = null;
            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
            {
                UserProduct ur = new UserProduct();
                ur.UserId = userId;
                ur.ProductId = productId;

                context.Insert<UserProduct>(ur);
                context.Commit();

                retrievedUserProduct = context.QueryItems<UserProduct>("select * from UserProduct where userId = @userId and productId = @productId", new { userId = userId, productId = productId }).FirstOrDefault();
            }

            Assert.IsTrue(retrievedUserProduct != null && retrievedUserProduct.UserId == userId && retrievedUserProduct.ProductId == productId, "An error occurred while inserting a composite key int crosswalk");
        }

        /// <summary>
        /// Test update was successful
        /// </summary>
        [TestMethod]
        public void TestUpdate_Commit()
        {
            using (ADOCRUDContext context = new ADOCRUDContext(ConfigurationManager.ConnectionStrings["UnitTestDB"].ToString()))
            {
                Product p = context.QueryItems<Product>("select * from Product where Id = @id", new { id = productId }).First();
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
        /// Test that changes were rolled back on failed update
        /// </summary>
        [TestMethod]
        public void TestUpdate_NoCommit()
        {
            Product p = null;
            Product retreivedProduct = null;

            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
            {
                p = context.QueryItems<Product>("select * from Product where Id = @id", new { id = productId }).First();
                p.Name = "Waffle Maker";
                p.CategoryId = 3;
                p.Description = "Makes waffles";
                p.Price = 29.99m;

                context.Update<Product>(p);

            }

            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
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
        /// Test removal of object
        /// </summary>
        [TestMethod]
        public void TestRemove_Commit()
        {
            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
            {
                Product p = context.QueryItems<Product>("select * from dbo.Product where Id = @id", new { id = productId }).FirstOrDefault();

                Assert.IsTrue(p != null && p.Id > 0);

                context.Remove(p);
                context.Commit();
            }

            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
            {
                Product p = context.QueryItems<Product>("select * from dbo.Product where Id = @id", new { id = productId }).FirstOrDefault();

                Assert.IsTrue(p == null);
            }
        }

        /// <summary>
        /// Test failed removal of object
        /// </summary>
        [TestMethod]
        public void TestRemove_NoCommit()
        {
            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
            {
                Product p = context.QueryItems<Product>("select * from dbo.Product where Id = @id", new { id = productId }).FirstOrDefault();

                Assert.IsTrue(p != null && p.Id > 0);

                context.Remove(p);
            }

            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
            {
                Product p = context.QueryItems<Product>("select * from dbo.Product where Id = @id", new { id = productId }).FirstOrDefault();

                Assert.IsTrue(p != null && p.Id > 0);
            }
        }

        [TestMethod]
        public void TestRemovalOfCompositeKeys_Commit()
        {
            UserProduct retrievedUP = null;

            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
            {
                UserProduct up = new UserProduct();
                up.UserId = userId;
                up.ProductId = productId;

                context.Insert<UserProduct>(up);
                context.Commit();
            }

            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
            {
                // Makes sure user product being retreived exists
                UserProduct up = context.QueryItems<UserProduct>("select * from dbo.UserProduct where UserId = @userId and ProductId = @productId", new { userId = userId, productId = productId }).FirstOrDefault();
                Assert.IsTrue(up != null && up.UserId == userId && up.ProductId == productId);

                context.Remove(up);
                context.Commit();

                retrievedUP = context.QueryItems<UserProduct>("select * from dbo.UserProduct where UserId = @userId and ProductId = @productId", new { userId = userId, productId = productId }).FirstOrDefault();
            }

            Assert.IsTrue(retrievedUP == null, "A problem occurred when trying to remove a composite key");
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

            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
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

            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
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

            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
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

            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
            {
                retreivedProduct1 = context.QueryItems<Product>("select * from dbo.Product where Id = @id", new { id = p1.Id }).FirstOrDefault();
                retreivedProduct2 = context.QueryItems<Product>("select * from dbo.Product where Id = @id", new { id = p2.Id }).FirstOrDefault();
            }

            Assert.IsTrue(retreivedProduct1 == null, "Insert should have failed, but succeeded");
            Assert.IsTrue(retreivedProduct2 == null, "Insert should have failed, but succeeded");
        }

        [TestMethod]
        public void TestQueryItem()
        {
            Product retreivedProduct = null;

            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
            {
                retreivedProduct = context.QueryItems<Product>("select * from dbo.Product where Id = @id", new { id = productId }).FirstOrDefault();
            }

            Assert.IsTrue(retreivedProduct != null &&
                retreivedProduct.Id == productId &&
                retreivedProduct.Name == "Volleyball" &&
                retreivedProduct.Price == 10.99m &&
                retreivedProduct.Description == "Soft ball that does not hurt the wrist", "Query Item is retreiving the object incorrectly");
        }

        [TestMethod]
        public void TestNestedConnections_Commit()
        {
            Product p = null;
            Product retreivedProduct = null;

            using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
            {
                using (ADOCRUDContext context2 = new ADOCRUDContext(connectionString))
                {
                    p = context2.QueryItems<Product>("select * from dbo.Product where Id = @id", new { id = productId }).FirstOrDefault();
                }

                p.Name = "Bowling ball";

                context.Update<Product>(p);
                context.Commit();

                retreivedProduct = context.QueryItems<Product>("select * from dbo.Product where Id = @id", new { id = productId }).FirstOrDefault();
            }

            Assert.IsTrue(retreivedProduct != null && retreivedProduct.Name == "Bowling ball", "An error occurred with nested connections");
        }
    }
}
