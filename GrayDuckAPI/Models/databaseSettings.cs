using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using GrayDuck.Services;

namespace GrayDuck.Models
{

    public class databaseSettings
    {
        
        private string ConnectionString = "";
        private string DatabaseName = "";

        public databaseSettings(IConfiguration _configuration)
        {
            try
            {
                //Read Config to get connectionString and databaseName values
                ConnectionString = _configuration.GetValue<string>("DatabaseSettings:ConnectionString");
                DatabaseName = _configuration.GetValue<string>("DatabaseSettings:DatabaseName");
            }
            catch (Exception ex)
            {
                ConnectionString = "";
                DatabaseName = "grayduck";
            }
        }

        public async Task <string> executeBulkQuery(string stringQuery)
        {
            string stringReturn = "";

            // Allows this to be a transaction
            stringQuery = "BEGIN TRANSACTION; " + stringQuery + " COMMIT TRANSACTION;";

            using (NpgsqlConnection objConn = new NpgsqlConnection(ConnectionString))
            {
                using (NpgsqlCommand objComm = new NpgsqlCommand(stringQuery, objConn))
                {

                    // Check Connection State before we open it up
                    if (objConn.State == ConnectionState.Closed)
                        await objConn.OpenAsync();

                    // Use Reader to Open and Execute Scalar Command (Query)
                    await objComm.ExecuteScalarAsync();
                }
            }

            return stringReturn;
        }

        public async Task <string> executeQuery(string stringQuery)
        {
            string stringReturn = "";

            using (NpgsqlConnection objConn = new NpgsqlConnection(ConnectionString))
            {
                using (NpgsqlCommand objComm = new NpgsqlCommand(stringQuery, objConn))
                {

                    // Check Connection State before we open it up
                    if (objConn.State == ConnectionState.Closed)
                        await objConn.OpenAsync();

                    // Use Reader to Open and Execute Scalar Command (Query)
                    stringReturn = System.Convert.ToString(await objComm.ExecuteScalarAsync());

                    if (stringQuery.Contains("INSERT") || stringQuery.Contains("insert"))
                    {
                        //objComm.CommandText = "Select @@IDENTITY";
                        //stringReturn = System.Convert.ToString(objComm.ExecuteScalar());
                    }
                }
            }

            return stringReturn;
        }

        public async Task <long> executeNonQuery(string stringQuery)
        {
            Int64 integerReturn = 0;

            using (NpgsqlConnection objConn = new NpgsqlConnection(ConnectionString))
            {
                using (NpgsqlCommand objComm = new NpgsqlCommand(stringQuery, objConn))
                {

                    // Check Connection State before we open it up
                    if (objConn.State == ConnectionState.Closed)
                        await objConn.OpenAsync();

                    // Execute Non query for use with INSERT, UPDATE, and DELETE queries, only returns the number of rows that was affected
                    integerReturn = await objComm.ExecuteNonQueryAsync();
                }
            }

            return integerReturn;
        }

        public async Task <string> executeQueryWithByte(string stringQuery, byte[] fileByte)
        {
            string stringReturn = "";

            using (NpgsqlConnection objConn = new NpgsqlConnection(ConnectionString))
            {
                using (NpgsqlCommand objComm = new NpgsqlCommand(stringQuery, objConn))
                {

                    // Add File Byte Paramater
                    objComm.Parameters.AddWithValue("@FileByte", fileByte);

                    // Check Connection State before we open it up
                    if (objConn.State == ConnectionState.Closed)
                        await objConn.OpenAsync();

                    // Use Reader to Open and Execute Scalar Command (Query)
                    stringReturn = System.Convert.ToString(await objComm.ExecuteScalarAsync());

                    if (stringQuery.Contains("INSERT") || stringQuery.Contains("insert"))
                    {
                        //objComm.CommandText = "Select @@IDENTITY";
                        //stringReturn = System.Convert.ToString(await objComm.ExecuteScalarAsync());
                    }
                }
            }

            return stringReturn;
        }

        public async Task <DataTable> executeReader(string stringQuery)
        {
            DataTable datatableReturn = new DataTable();

            using (NpgsqlConnection objConn = new NpgsqlConnection(ConnectionString))
            {
                using (NpgsqlCommand objComm = new NpgsqlCommand(stringQuery, objConn))
                {
                    // Set Reader Timeout to 2 min
                    objComm.CommandTimeout = 120;

                    // Check Connection State before we open it up
                    if (objConn.State == ConnectionState.Closed)
                        await objConn.OpenAsync();

                    // Use Reader to Open and Execute Reader Command (Query)
                    using (NpgsqlDataReader objReader = await objComm.ExecuteReaderAsync())
                    {
                        datatableReturn.Load(objReader, LoadOption.OverwriteChanges);
                    }
                }
            }

            return datatableReturn;

        }

        public async Task <string> executeScalarReturn(string stringQuery)
        {
            string stringReturn = "";
            object objectObject = new object();

            using (NpgsqlConnection objConn = new NpgsqlConnection(ConnectionString))
            {
                using (NpgsqlCommand objComm = new NpgsqlCommand(stringQuery, objConn))
                {

                    // Check Connection State before we open it up
                    if (objConn.State == ConnectionState.Closed)
                        await objConn.OpenAsync();

                    // Use Reader to Open and Execute Scalar Command (Query)
                    objectObject = await objComm.ExecuteScalarAsync();

                    // Convert NULL Error Issue
                    if (objectObject == null)
                        stringReturn = "";
                    else
                        stringReturn = objectObject.ToString();
                }
            }

            return stringReturn;
        }

        public string sqlCheck(string strInput)
        {
            string stringValue = "";
            try
            {
                if (String.IsNullOrEmpty(strInput)) {
                    //Nothing to do here
                } else {
                    stringValue = strInput.Replace("'", "''");
                }
                return stringValue;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }

}
