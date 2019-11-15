using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DatabaseConfigurationProvider
{
    internal sealed class DatabaseConfigurationProvider : ConfigurationProvider, IDisposable
    {
        private bool _disposed;
        private readonly IDbConnection _dbConnection;

        public DatabaseConfigurationProvider(string connectionString)
        {            
            _dbConnection = SqlClientFactory.Instance.CreateConnection();
            _dbConnection.ConnectionString = connectionString;
        }

        public override void Load()
        {
            _dbConnection.Open();
            IDataReader dbDataReader = null;
            try
            {
                var dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "SELECT [Key], [Value] FROM Configuration";
                dbDataReader = dbCommand.ExecuteReader();
                
                while (dbDataReader.Read())
                {
                    Data.Add((string)dbDataReader[0], (string)dbDataReader[1]);
                }
                
                dbDataReader.Close();
            }
            finally
            {
                if (dbDataReader != null)
                {
                    dbDataReader.Dispose();
                }
                
                _dbConnection.Close();
            }            
        }

        public override void Set(string key, string value)
        {
            _dbConnection.Open();
            IDbTransaction dbTransaction = null;
            try
            {
                var dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "INSERT INTO Configuration ([Key], [Value]) VALUES(@Key, @Value)";

                var keyParameter = dbCommand.CreateParameter();
                keyParameter.DbType = DbType.String;
                keyParameter.ParameterName = "@Key";
                keyParameter.Value = key;
                dbCommand.Parameters.Add(keyParameter);

                var valueParameter = dbCommand.CreateParameter();
                valueParameter.DbType = DbType.String;
                valueParameter.ParameterName = "@Value";
                valueParameter.Value = value;
                dbCommand.Parameters.Add(valueParameter);

                dbTransaction = _dbConnection.BeginTransaction();
                dbCommand.Transaction = dbTransaction;
                dbCommand.ExecuteNonQuery();
                dbTransaction.Commit();
            }
            catch(Exception)
            {
                dbTransaction.Rollback();
                throw;
            }
            finally
            {
                if (dbTransaction != null)
                {
                    dbTransaction.Dispose();
                }

                _dbConnection.Close();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _dbConnection.Dispose();
            }

            _disposed = true;
        }
    }
}
