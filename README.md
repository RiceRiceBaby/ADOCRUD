# ADOCRUD
Lightweight ORM thats built on top of ADO.NET and is partly an extension of Dapper. Handles automatic insert, update, and removal of objects without having to write any sql statements for Create, Update, and Remove functionality. The query part of this ORM is an extension of Dapper which means you still need to write sql statements to retrieve data, but that data will automatically be mapped to your C# objects. Most of this application was written primarily using reflection.<br /><br />
This ORM comes with an object class generator tool. This tool allows you to connect to a Sql Server database, grabs all the tables, and generates C# objects as .cs files and outputs them to the folder you specify.

## ADOCRUDContext (ORM)

Model Example:<br />

```cs
using ADOCRUD.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ADOCRUDExamples.Models
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

For your objects to work with the ADOCRUD ORM, attributes are required. Table attribute specifies which database table the object maps to. The first argument in the table attribute is the table name. The 2nd argument specifies the schema the table belongs to. Member attribute specifies that the property corresponds to a column in the table. Properties that do not have this member attribute will be ignored and excluded from any database manipulation (insert, update, remove). PrimaryKey attribute specifies that the property corresponds with the identity/primary key column of the table.

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
Using statement opens up a connection to the database specified in the connection and also opens up a transaction scope. The insert does not finalize unless you call the commit method of the context. ADOCRUDContext closes the connection and disposes the transaction on Dispose() which means the connection here gets closed at the end of the using statement.

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
Update behaves the same way as the insert. Opens up a transaction scope and connection in the beginning of the using statement. Completes the transaction on the commit method. Closes the connection and disposes the transaction on Dispose() which is the final bracket of the using statement.

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
  Product p = null;

  using (ADOCRUDContext context = new ADOCRUDContext(connectionString))
  {
    p = context.QueryItems<Product>("select * from dbo.Product where Id = @id", new { id = productId }).FirstOrDefault();
  }
  
  return p;
}
```
QueryItems executes the select statement you pass in, grabs the results of the query and automatically maps it the C# object(s) and returns that/those object. The QueryItems function by default returns a list of objects, but you can limit it to a single object by using the "First()" or "FirstOrDefault()" method. Notice that to keep the query parameterized, you pass the parameters in the 2nd argument as a single object. To pass in extra parameters, you just add in comma separated values (i.e new { Id = productId, name = "Basketball", price = 11.99 })<br />

###<b>Limitations:</b><br />
\#1. Does not support nested transactions, but does support nested connections. In other words, you can create a using context within another using context, but if both contexts call their respective commmit method and the outer context fails and the inner context succeeds, the changes in that inner context will still be applied to the database while the outer commit will roll back whatever it tried to do.<br />

\#2. Spelling and letter casing between C# properties and their corresponding sql columns the properties maps to must be exactly the same.<br />
<i>Example that works: SQL Column: ProductId, C# Property: public int ProductId { get; set; }<br />
Example that does not work: SQL Column: product_id, C# Property: public int ProductId { get; set; }</i>

##ADOCRUD Object Class Generator

The object class generator tool provides a graphical user interface that is pretty straight forward to use. This generator scans all the tables in the Sql Server database entered and generates C# objects/classes for each of those tables. <br />
<b>Sql Server Datasource:</b> <i>Name of the server</i><br />
<b>Database:</b> <i>Name of the database</i><br />
<b>User Id:</b> <i>Sql server user id that has access to the database</i><br />
<b>Password:</b> <i>Sql server password of the user</i><br />
<b>C# Namespace:</b> <i>Namespace the C# class should be under</i><br />
<b>Output Path:</b> <i>File path where all the C# classes should be generated</i><br />
<b>Generate Objects:</b> <i>Clicking this button generates all the C# objects in the file path entered.</i>

##License
License is under Apache. This means that this software is free for personal or commercial use.

## Issues
If you find an issue with this software, please click <a href="https://github.com/RiceRiceBaby/ADOCRUD/issues">here</a> to submit a ticket.

##Author
Name: Daniel Li<br />
My Twitter Handle: https://twitter.com/DanielDavidLi<br />
LinkedIn Profile: https://www.linkedin.com/in/danieldli?trk=hp-identity-name<br />
