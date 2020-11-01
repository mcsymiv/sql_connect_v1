using Google.Protobuf.WellKnownTypes;
using Microsoft.Azure.Mobile.Server;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TechTalk.SpecFlow;

namespace sql_connector_v1.Steps
{
    [Binding]
    public class ManagePersonsSteps
    {
        string connString = @"Data Source=DESKTOP-BNMTEH1\SQLEXPRESS;Initial Catalog=PersonsOrders;Integrated Security=True";
        string sqlExpression;
        SqlDataAdapter adapter;
        DataSet dataset;
        SqlConnection connection;
        SqlCommand command;
        int personID;

        /// <summary>
        /// Get Persons list with select * from Persons
        /// </summary>
        [Given(@"Connection with mcs DB is creted")]
        public void GivenConnectionWithMcsDBIsCreted()
        {
            using (connection = new SqlConnection(connString))
            {
                connection.Open();
                sqlExpression = String.Format("select * from Persons");
                command = new SqlCommand(sqlExpression, connection);
            }
        }
        
        [When(@"Send select command")]
        public void WhenSendSelectCommand()
        {
            using (connection = new SqlConnection(connString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sqlExpression, connection);
                dataset = new DataSet();
                adapter.Fill(dataset);
            }
        }

        [Then(@"Get a list of all Persons")]
        public void ThenGetAListOfAllPersons()
        {
            Assert.IsTrue(dataset.Tables[0].Rows[0] != null);
        }

        /// <summary>
        /// Add new Person to the DB with firstName, lastName, age, city info
        /// </summary>
        [Given(@"Person data is generated")]
        public void GivenPersonDataIsGeneratedTestTestovichTestograd()
        {
            sqlExpression = String.Format($"insert into Persons(FirstName, LastName, Age, City)" +
                    $"values('Harry','Potter',11,'London')");
        }

        [When(@"Send person data to Person table")]
        public void WhenSendPersonDataToPersonTable()
        {
            using (connection = new SqlConnection(connString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sqlExpression, connection);
                dataset = new DataSet();
                adapter.Fill(dataset);
            }
        }

        [Then(@"Person name is visible in table")]
        public void ThenPersonNameIsVisibleInTable()
        {
            dataset = GetDataSet($"select FirstName from Persons where FirstName = 'Harry'");
            string person = Convert.ToString(dataset.Tables[0].Rows[0].ItemArray[0]);
            Assert.IsTrue(person.Contains("Harry"));
        }

        /// <summary>
        /// Harry Potter buys a wand
        /// </summary>
        [Given(@"Person information id")]
        public void GivenPersonInformation()
        {
            personID = GetPersonID($"select ID from Persons where FirstName = 'Harry' and LastName = 'Potter'");
            sqlExpression = String.Format($"insert into Orders(SUM_order, ID)" +
            $"values(11,{personID})");
        }

        [When(@"Add order information")]
        public void WhenAddOrderInformation()
        {
            using (connection = new SqlConnection(connString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sqlExpression, connection);
            }
        }

        [Then(@"Person data is in Order table")]
        public void ThenPersonDataIsInOrderTable()
        {
            dataset = GetDataSet($"select ID from Orders where ID = {personID}");
            Assert.IsTrue(dataset.Tables[0].Rows[0] != null);
        }

        /// <summary>
        /// The boy who lived must die or Avada Kedavra request
        /// </summary>
        [When(@"Send delete command pointing to this person")]
        public void WhenSendDeleteCommandPointingToThisPerson()
        {
            sqlExpression = String.Format($"delete from Persons where LastName = 'Potter' and FirstName = 'Harry'");
            using (connection = new SqlConnection(connString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sqlExpression, connection);
                dataset = new DataSet();
                adapter.Fill(dataset);
            }
        }

        [Then(@"Person is removed from DB")]
        public void ThenPersonIsRemovedFromDB()
        {
            dataset = GetDataSet($"select FirstName from Persons where FirstName = 'Harry' and LastName = 'Potter'");
            int count = dataset.Tables[0].Rows.Count;
            Assert.IsTrue(count == 0);
        }
        public DataSet GetDataSet(string sqlRequest)
        {

            string sqlExpression = String.Format(sqlRequest);
            using (connection = new SqlConnection(connString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sqlExpression, connection);
                dataset = new DataSet();
                adapter.Fill(dataset);
            }
            return dataset;
        }
        public int GetPersonID(string sqlReuest)
        {
            DataSet dataset = new DataSet();
            dataset = GetDataSet(sqlReuest);
            return Convert.ToInt32(dataset.Tables[0].Rows[0].ItemArray[0]);
        }
    }
}
