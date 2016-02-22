# ADOCRUD
Lightweight ORM thats built on top of ADO.NET and is partly an extension of Dapper. Handles automatic insert, update, and removal of objects without having to write any sql statements for Create, Update, and Remove functionality. The query part of this ORM is an extension of Dapper which means you still need to write sql statements to retrieve data, but that data will automatically be mapped to your C# objects. Most of this application was written primarily using reflection.<br /><br />
This ORM comes with an object class generator tool. This tool allows you to connect to a Sql Server database, grabs all the tables, and generates C# objects as .cs files and outputs them to the folder you specify.

```cs
public Product GetProductById(int productId)
{
  Product retreivedProduct = null;

  using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
  {
    retreivedProduct = context.QueryItems<Product>("select * from dbo.Product where Id = @id", new { id = productId }).FirstOrDefault();
  }
}

public void UpdateProduct(int productId)
{
  using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
  {
    Product p = this.GetProductById(productId);
    p.Name = "Basketball";
    context.Update<Product>(p);
    context.Commit();
  }
}
```

## ADOCRUDContext (ORM)

Model Example:<br />

```cs
using ADOCRUD.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ADOCRUD.UnitTests.Models
{
    [Table("Product", "dbo")]
    public class Product
    {
        [PrimaryKey]
        [Member]
        public int Id { get; set; }

        [Member]
        public int CategoryId { get; set; }

        [Member]
        public string Name { get; set; }

        [Member]
        public decimal Price { get; set; }

        [Member]
        public string Description { get; set; }

    }
}
```

For your objects to work with the ADOCRUD ORM, attributes are required. Table attribute specifies which database table the object maps to. The first argument in the table attribute is the table name. The 2nd argument specifies the schema the table belongs to. Member attribute specifies that the property corresponds to a column in the table. PrimaryKey attribute specifies that the property corresponds with the identity/primary key column of the table.

Insert Example: <br />

```cs
public void AddProduct(Product p)
{
  using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
  {
    context.Insert<Product>(p);
    context.Commit();
  }
}
```
Using statement opens up a connection to the database specified in the connection and also opens up a transaction scope. The insert does not finalize unless you call the commit method of the context. ADOCRUDContext closes the connection on Dispose() which means the connection here gets closed at the end of the using statement.

Update Example: <br />

```cs
public void UpdateProduct(Product p)
{
  using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
  {
      context.Update<Product>(p);
      context.Commit();
  }
}
```
Update behaves the same way as the insert. Opens up a transaction scope and connection in the beginning of the using statement. Completes the transaction on the commit method. Closes the connection on Dispose() which is the final bracket of the using statement.

```cs
public void Remove(Product p)
{
  using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
  {
      context.Remove<Product>(p);
      context.Commit();
  }
}
```
Remove behaves the same way as the previous 2 methods.

Query Example:

```cs
public Product GetProductById(int productId)
{
  Product retreivedProduct = null;

  using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
  {
    retreivedProduct = context.QueryItems<Product>("select * from dbo.Product where Id = @id", new { id = productId }).FirstOrDefault();
  }
}
```
QueryItems executes the select statement you pass in, grabs the results of the query and automatically maps it the C# object(s) and returns that/those object. The QueryItems function by default returns a list of objects, but you can limit it to a single object by using the "First()" or "FirstOrDefault()" method. Notice that to keep the query parameterized, you pass the parameters in the 2nd argument as a single object. To pass in extra parameters, you just add in comma separated values (i.e new { Id = productId, name = "Basketball", price = 11.99 })<br />

###<b>Limitations I've discovered:</b><br />
\#1. Nested connections not allowed - <i>automatic management of open and closing of connections via using statements (connection opens in constructor, closes on dispose) prevents you from opening a connection within a connection. If enough people request that they want to manage their own connections, I will take out the automatic management of opening and closing connections. For now I will keep it in as I think this is very useful. For those of you who don't understand what I am saying, the code sample below is what I mean of what won't work:<i>

##ADOCRUD Object Class Generator

The object class generator tool provides a graphical user interface that is pretty straight forward to use. This generator scans all the tables in the Sql Server database entered and generates C# objects/classes for each of those tables. <br />
Sql Server Datasource: Name of the server<br />
Database: Name of the database<br />
User Id: Sql server user id that has access to the database<br />
Password: Sql server password of the user<br />
C# Namespace: Namespace the C# class should be under<br />
Output Path: File path where all the C# classes should be produced<br />
Generate Objects: Clicking this button generates all the C# objects in the file path entered.
