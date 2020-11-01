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
            Assert.IsFalse(dataset.Tables[0].Rows[0] != null);
        }

        /// <summary>
        /// Add new Person to the DB with firstName, lastName, age, city info
        /// </summary>
        [Given(@"Person data is generated (.*) (.*) (.*) (.*)")]
        public void GivenPersonDataIsGeneratedTestTestovichTestograd(string firstName, string lastName, int age, string city)
        {
            sqlExpression = String.Format($"insert into Persons(firstName, lastName, age, city)" +
                    $"values('{firstName}','{lastName}',{age},'{city}')");
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
        [Then(@"Get person name")]
        public void ThenGetPersonTest()
        {
            sqlExpression = String.Format($"select firstName from Persons where firstName = 'firstName'");
            using (connection = new SqlConnection(connString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sqlExpression, connection);
                dataset = new DataSet();
                adapter.Fill(dataset);
            }
            var person = dataset.Tables[0].Rows[0].ItemArray.ToString();
            Assert.IsTrue(person.Contains("firstName"));
        }
    }
}
