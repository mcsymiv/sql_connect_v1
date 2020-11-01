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
        string connString = @"Data Source=DESKTOP-BNMTEH1\SQLEXPRESS;Initial Catalog=mcs;Integrated Security=True";
        string sqlExpression;
        SqlDataAdapter adapter;
        DataSet dataset;
        SqlConnection connection;
        SqlCommand command;

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
            sqlExpression = String.Format($"insert into Persons(firstName, lastName, age, city)" +
                    $"values('Harry','Potter',12,'London')");
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
            sqlExpression = String.Format($"select firstName from Persons where firstName = 'Harry'");
            using (connection = new SqlConnection(connString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sqlExpression, connection);
                dataset = new DataSet();
                adapter.Fill(dataset);
            }
            string person = Convert.ToString(dataset.Tables[0].Rows[0].ItemArray[0]);
            Assert.IsTrue(person.Contains("Harry"));
        }

        [When(@"Send delete command indicating (.*) (.*)")]
        public void WhenSendDeleteCommandIndicating(string lastName, string firstName)
        {
            sqlExpression = String.Format($"delete from Persons where lastName = '{lastName}' and firstName = '{firstName}'");
            using (connection = new SqlConnection(connString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sqlExpression, connection);
            }
        }

        [Then(@"Person is removed from DB")]
        public void ThenPersonIsRemovedFromDB()
        {
            sqlExpression = String.Format($"select firstName from Persons where firstName = 'Harry' and lastName = 'Potter'");
            using (connection = new SqlConnection(connString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sqlExpression, connection);
                dataset = new DataSet();
                adapter.Fill(dataset);
            }
            int count = dataset.Tables[0].Rows.Count;
            Assert.IsTrue(count == 0);
        }


    }
}
