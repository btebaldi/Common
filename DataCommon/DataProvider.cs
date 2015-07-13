using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace DataCommon
{
    public enum EnumProviders
    { SqlServer, OleDb, Odbc, Oracle }

    class DataProvider
    {

        #region "SQL Server Specific Methods"
        /* 
        // The following methods currently only handle SQL Server
        // However, these could be put into a Case statement to choose
        // which data provider to use based on the ConnectionString.
        // Or could even be created late bound.
        public static IDbConnection CreateConnection()
        {
            SqlConnection cnn = new SqlConnection();
            return cnn;
        }

        public static IDbCommand CreateCommand()
        {
            SqlCommand cmd = new SqlCommand();
            return cmd;
        }

        public static IDataParameter CreateParameter()
        {
            SqlParameter param = new SqlParameter();
            return param;
        }

        public static IDbDataAdapter CreateDataAdapter()
        {
            SqlDataAdapter da = new SqlDataAdapter();
            return da;
        }

        public static System.Data.SqlClient.SqlBulkCopy CreateBulkCopier()
        {
            throw new NotImplementedException("Metodo nao implementado.");
        }
        */
        #endregion

        #region "Generic Methods"
        // The following methods handles SQL Server, Oledb, Odbc


        public static IDbConnection CreateConnection(EnumProviders provider)
        {
            IDbConnection cnn;

            switch (provider)
            {
                case EnumProviders.SqlServer:
                    cnn = new System.Data.SqlClient.SqlConnection();
                    break;
                case EnumProviders.OleDb:
                    cnn = new System.Data.OleDb.OleDbConnection();
                    break;
                case EnumProviders.Odbc:
                    cnn = new System.Data.Odbc.OdbcConnection();
                    break;
                case EnumProviders.Oracle:
                    throw new NotImplementedException("Provider not implemented");
                    break;
                default:
                    cnn = new System.Data.SqlClient.SqlConnection();
                    break;
            }

            return cnn;
        }

        public static IDbCommand CreateCommand(EnumProviders provider)
        {
            IDbCommand cmd;

            switch (provider)
            {
                case EnumProviders.SqlServer:
                    cmd = new System.Data.SqlClient.SqlCommand();
                    break;
                case EnumProviders.OleDb:
                    cmd = new System.Data.OleDb.OleDbCommand();
                    break;
                case EnumProviders.Odbc:
                    cmd = new System.Data.Odbc.OdbcCommand();
                    break;
                case EnumProviders.Oracle:
                    throw new NotImplementedException("Provider not implemented");
                    break;
                default:
                    cmd = new System.Data.SqlClient.SqlCommand();
                    break;
            }

            return cmd;
        }

        public static IDataParameter CreateParameter(EnumProviders provider)
        {

            IDataParameter param;

            switch (provider)
            {
                case EnumProviders.SqlServer:
                    param = new System.Data.SqlClient.SqlParameter();
                    break;
                case EnumProviders.OleDb:
                    param = new System.Data.OleDb.OleDbParameter();
                    break;
                case EnumProviders.Odbc:
                    param = new System.Data.Odbc.OdbcParameter();
                    break;
                case EnumProviders.Oracle:
                    throw new NotImplementedException("Provider not implemented");
                    break;
                default:
                    param = new System.Data.SqlClient.SqlParameter();
                    break;
            }

            return param;
        }

        public static IDbDataAdapter CreateDataAdapter(EnumProviders provider)
        {
            IDbDataAdapter da;

            switch (provider)
            {
                case EnumProviders.SqlServer:
                    da = new System.Data.SqlClient.SqlDataAdapter();
                    break;
                case EnumProviders.OleDb:
                    da = new System.Data.OleDb.OleDbDataAdapter();
                    break;
                case EnumProviders.Odbc:
                    da = new System.Data.Odbc.OdbcDataAdapter();
                    break;
                case EnumProviders.Oracle:
                    throw new NotImplementedException("Provider not implemented");
                    break;
                default:
                    da = new System.Data.SqlClient.SqlDataAdapter();
                    break;
            }

            return da;
        }
        #endregion

    }
}
