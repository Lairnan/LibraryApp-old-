using System;
using System.Collections.Generic;
using DbConnect.Items;
using NUnit.Framework;

namespace NUnitTest;

public class Tests
{
    //[Test]
    /*public void Test1()
    {
        // arrange
        
        
        // act
        try
        {
            var add = Authors.Add("Test", "Test23", "42");
            Assert.AreEqual(add,1);
        }
        catch (Exception exception)
        {
            Assert.AreEqual(exception.Message,"Данная запись уже существует в базе данных");
        }
        try
        {
            var add = Authors.Add("Test", "Test1", "Test");
            Assert.AreEqual(add,1);
        }
        catch (Exception exception)
        {
            Assert.AreEqual(exception.Message,"Данная запись уже существует в базе данных");
        }
        try
        {
            var add = Authors.Add("Test", "test2", "Test");
            Assert.AreEqual(add,1);
        }
        catch (Exception exception)
        {
            Assert.AreEqual(exception.Message,"Данная запись уже существует в базе данных");
        }
        
        // assert
    }*/

    [Test]
    public void Test2()
    {
        try
        {
            var add = Types.Add("Test");
            Assert.AreEqual(add,1);
        }
        catch (Exception exception)
        {
            Assert.AreEqual(exception.Message,"Данная запись уже существует в базе данных");
        }
        try
        {
            var add = Categories.Add("Test");
            Assert.AreEqual(add,1);
        }
        catch (Exception exception)
        {
            Assert.AreEqual(exception.Message,"Данная запись уже существует в базе данных");
        }
        try
        {
            var add = Styles.Add("Test2");
            Assert.AreEqual(add,1);
        }
        catch (Exception exception)
        {
            Assert.AreEqual(exception.Message,"Данная запись уже существует в базе данных");
        }
        try
        {
            var add = Styles.Add("test");
            Assert.AreEqual(add,1);
        }
        catch (Exception exception)
        {
            Assert.AreEqual(exception.Message,"Данная запись уже существует в базе данных");
        }
    }
}