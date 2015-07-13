using System;
using System.Text;
using System.Data;

namespace DataCommon
{
    /// <summary>
    /// The DataLayer class that handles all the interaction with a database.
    /// Has the ability to return a DataReader from the DataLayer class.
    /// Has a data modification methods to the DataLayer class.
    /// </summary>
    public class DataLayer
    {
        #region "Data Retrieval Methods"
        /// <summary>
        /// Return a DataSet given a SQL string and a Connection String.
        /// </summary>
        /// <param name="SQL">SQL string</param>
        /// <param name="ConnectString">Connection String</param>
        /// <returns>Results of the SQL string</returns>
        public static DataSet GetDataSet(string SQL, string ConnectString)
        {
            DataSet ds = new DataSet();
            IDbDataAdapter da;

            da = CreateDataAdapter(SQL, ConnectString);

            da.Fill(ds);

            return ds;
        }

        public static DataSet GetDataSet(IDbCommand cmd, string ConnectString)
        {
            DataSet ds = new DataSet();
            IDbDataAdapter da;

            da = CreateDataAdapter(cmd, ConnectString);

            da.Fill(ds);

            return ds;
        }


        /// <summary>
        /// Return a DataTable given a SQL string and a Connection String.
        /// </summary>
        /// <param name="SQL">SQL string</param>
        /// <param name="ConnectString">Connection String</param>
        /// <returns>Results of the SQL string</returns>
        public static DataTable GetDataTable(string SQL, string ConnectString)
        {
            DataSet ds = new DataSet();
            DataTable dt = null;

            ds = GetDataSet(SQL, ConnectString);

            if (ds.Tables.Count > 0)
            { dt = ds.Tables[0]; }

            return dt;
        }

        /// <summary>
        /// Return a DataReader given a SQL string and a Connection String.
        /// </summary>
        /// <param name="SQL">SQL string</param>
        /// <param name="ConnectString">Connection String</param>
        /// <returns>DataReader of the SQL String</returns>
        public static IDataReader GetDataReader(string SQL, string ConnectString)
        {
            IDataReader dr;
            IDbCommand cmd = CreateCommand(SQL, ConnectString);

            dr = GetDataReader(cmd, ConnectString);

            return dr;
        }


        public static IDataReader GetDataReader(IDbCommand cmd, string ConnectString)
        {
            IDataReader dr;
            try
            {
                // Open the Connection Object
                if (cmd.Connection.State != ConnectionState.Open)
                { cmd.Connection.Open(); }

                // Create the DataReader
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception e)
            {
                // If there is an exception, close the connection
                if (cmd.Connection.State != ConnectionState.Closed)
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                }

                // Dispose of the Command
                cmd.Dispose();

                // Rethrow the exception
                throw e;
            }

            return dr;
        }



        /// <summary>
        /// Returns an Object data type given a SQL string and a Connection String.
        /// The SQL string must use one of the appropriate database scalar functions such as Min(), Max(), Sum(), etc.
        /// </summary>
        /// <param name="SQL">SQL string</param>
        /// <param name="ConnectString">Connection String</param>
        public static object ExecuteScalar(string SQL, string ConnectString)
        {
            IDbCommand cmd = null;
            object value = null;

            try
            {
                // Create Command with Connection Object
                cmd = CreateCommand(SQL, ConnectString, true);

                // Execute SQL
                value = cmd.ExecuteScalar();
            }
            catch
            {
                throw;
            }

            finally
            {
                // Close the connection
                if (cmd.Connection.State == ConnectionState.Open)
                { cmd.Connection.Close(); }

                // Dispose of the Objects
                cmd.Connection.Dispose();
                cmd.Dispose();
            }

            return value;
        }
        #endregion

        #region "Data Modification Methods"
        /// <summary>
        /// This method will perform the actual ExecuteNonQuery on a command object.
        /// The other ExecuteSQL methods are simply overrides of this one.
        /// The DisposeOfCommand parameter is a boolean value to let this method know whether or not to dispose of the command and connection object after the ExecuteNonQuery method has been performed.
        /// </summary>
        /// <param name="cmd">Command object</param>
        /// <param name="DisposeOfCommand">A boolean value to dispose or not of the command and connection object</param>
        /// <returns>return the number of rows affected</returns>
        public static int ExecuteSQL(IDbCommand cmd, bool DisposeOfCommand)
        {
            int intRows = 0;
            bool boolOpen = false;

            try
            {
                // Open the Connection
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                else
                {
                    boolOpen = !DisposeOfCommand; //WTF???
                }

                // Execute SQL 
                intRows = cmd.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (!boolOpen)
                {
                    // Close the connection
                    if (cmd.Connection.State == ConnectionState.Open)
                    {
                        cmd.Connection.Close();
                    }
                    // Dispose of the Objects
                    cmd.Connection.Dispose();
                }
                if (DisposeOfCommand)
                { cmd.Dispose(); }
            }

            return intRows;
        }

        /// <summary>
        /// This overloaded method calls the first ExecuteSQL() method and passes in a "true" value to the DisposeOfCommand parameter.
        /// </summary>
        /// <param name="cmd">Command object</param>
        /// <returns>return the number of rows affected</returns>
        public static int ExecuteSQL(IDbCommand cmd)
        {
            return ExecuteSQL(cmd, true);
        }

        /// <summary>
        /// This overloaded method calls the first ExecuteSQL() method after creating a Command and Connection object 
        /// and passing in a "true" value to the DisposeOfCommand parameter.
        /// </summary>
        /// <param name="SQL">SQL String</param>
        /// <param name="ConnectString">Connection String</param>
        /// <returns>return the number of rows affected</returns>
        public static int ExecuteSQL(string SQL, string ConnectString)
        {

            IDbCommand cmd = null;
            int intRows;

            cmd = CreateCommand(SQL, ConnectString);

            //Execute SQL
            intRows = ExecuteSQL(cmd, true);

            return intRows;
        }
        #endregion

        #region "Create Connection/Command/Parameter/DataAdapter Objects"
        /// <summary>
        /// This method will create an ADO.NET Connection object.
        /// This method is called by all the Data Retrieval methods and the Data Modification methods.
        /// </summary>
        /// <param name="ConnectString">Connection String</param>
        /// <returns>ADO.NET Connection</returns>
        public static IDbConnection CreateConnection(string ConnectString)
        {
            IDbConnection conn;

            conn = DataProvider.CreateConnection(GetProvider(ConnectString));
            conn.ConnectionString = ConnectString;

            return conn;
        }

        #region Create Command

        /// <summary>
        /// This method will create an ADO.NET Command object.
        /// This method is called by all the Data Retrieval methods and the Data Modification methods.
        /// </summary>
        /// <param name="SQL">SQL String</param>
        /// <returns>ADO.NET Command</returns>
        //public static IDbCommand CreateCommand(string SQL)
        //{
        //    IDbCommand cmd;
        //    cmd = DataProvider.CreateCommand(EnumProviders.SqlServer);
        //    cmd.CommandText = SQL;
        //    return cmd;
        //}

        /// <summary>
        /// This method calls the CreateCommand(SQL) to create the command object.
        /// Then it calls the CreateConnection() method to create a connection and place the new connection into the Connection property of the newly created command object.
        /// </summary>
        /// <param name="SQL">SQL String</param>
        /// <param name="ConnectString">Connection String</param>
        /// <returns>ADO.NET Command (w/ Closed connection)</returns>
        public static IDbCommand CreateCommand(string SQL, string ConnectString)
        {
            return CreateCommand(SQL, ConnectString, false);
        }

        /// <summary>
        /// This method calls the CreateCommand(SQL) to create the command object.
        /// Then it calls the CreateConnection() method to create a connection and place the new connection into the Connection property of the newly created command object.
        /// </summary>
        /// <param name="SQL">SQL String</param>
        /// <param name="ConnectString">Connection String</param>
        /// <param name="OpenConnection">bolean value to open (or not) the connection</param>
        /// <returns>ADO.NET Command (w/ Connection)</returns>
        public static IDbCommand CreateCommand(string SQL, string ConnectString, bool OpenConnection)
        {
            IDbCommand cmd;

            cmd = DataProvider.CreateCommand(GetProvider(ConnectString));
            cmd.CommandText = SQL;

            cmd.Connection = CreateConnection(ConnectString);
            if (OpenConnection)
            {
                cmd.Connection.Open();
            }

            return cmd;
        }
        #endregion

        #region Create Parameter
        /// <summary>
        /// This method creates a new parameter object with the given name.
        /// </summary>
        /// <param name="ParameterName">Parameter name</param>
        /// <returns>Data Parameter</returns>
        public static IDataParameter CreateParameter(string ParameterName, EnumProviders Provider)
        {
            IDataParameter param;

            param = DataProvider.CreateParameter(Provider);
            param.ParameterName = ParameterName;

            return param;
        }

        /// <summary>
        /// This method creates a new parameter object with the given name.
        /// It also fills in the DataType property with the passed in data type.
        /// </summary>
        /// <param name="ParameterName">Parameter name</param>
        /// <param name="DataType">Parameter data type</param>
        /// <returns>Data Parameter</returns>
        public static IDataParameter CreateParameter(string ParameterName, DbType DataType, EnumProviders Provider)
        {
            IDataParameter param;

            param = CreateParameter(ParameterName, Provider);
            //param = DataProvider.CreateParameter();
            //param.ParameterName = ParameterName;
            param.DbType = DataType;


            return param;
        }

        /// <summary>
        /// This method creates a new parameter object with the given name.
        /// It fills in the DataType property with the passed in data type.
        /// It also fills in the value with the passed in Value.
        /// </summary>
        /// <param name="ParameterName">Parameter name</param>
        /// <param name="DataType">Parameter data type</param>
        /// <param name="Value">Parameter Value</param>
        /// <param name="Provider">The Provider type that the parameter will be returned</param>
        /// <returns>Data Parameter</returns>
        public static IDataParameter CreateParameter(string ParameterName, DbType DataType, object Value, EnumProviders Provider)
        {
            IDataParameter param;

            param = CreateParameter(ParameterName, DataType, Provider);
            //param = DataProvider.CreateParameter();
            //param.ParameterName = ParameterName;
            //param.DbType = DataType;
            param.Value = Value;

            return param;
        }

        public static IDataParameter CreateParameter(string ParameterName, DbType DataType, object Value, string ConnectString)
        { return CreateParameter(ParameterName, DataType, Value, GetProvider(ConnectString)); }
        #endregion

        /// <summary>
        /// This method creates a DataAdapter object from the given SQL string and Connection String.
        /// </summary>
        /// <param name="SQL">SQL String</param>
        /// <param name="ConnectString">Connection String</param>
        /// <returns>Data Adapter</returns>
        public static IDbDataAdapter CreateDataAdapter(string SQL, string ConnectString)
        {
            IDbCommand cmd;

            cmd = CreateCommand(SQL, ConnectString, false);

            return CreateDataAdapter(cmd, ConnectString);
        }

        public static IDbDataAdapter CreateDataAdapter(IDbCommand cmd, string ConnectString)
        {
            IDbDataAdapter da;

            da = DataProvider.CreateDataAdapter(GetProvider(ConnectString));

            da.SelectCommand = cmd;

            return da;
        }
        #endregion

        /// <summary>
        /// This method determines a provider from the given Connection String.
        /// The default value is the SqlServer provider.
        /// </summary>
        /// <param name="ConnectString">Connection Stirng</param>
        /// <returns>Provider</returns>
        public static EnumProviders GetProvider(string ConnectString)
        {
            EnumProviders provider;

            System.Data.Common.DbConnectionStringBuilder builder = new System.Data.Common.DbConnectionStringBuilder();
            builder.ConnectionString = ConnectString;

            // Initial value
            provider = EnumProviders.SqlServer;

            string connectionStringProviderValue = "";
            if (builder.ContainsKey("Provider"))
            { connectionStringProviderValue = builder["Provider"].ToString(); }
            else
            { connectionStringProviderValue = ""; }

            switch (connectionStringProviderValue)
            {
                case "Microsoft.ACE.OLEDB.12.0":
                    provider = EnumProviders.OleDb;
                    break;
                case "Microsoft.Jet.OLEDB.4.0;":
                    provider = EnumProviders.OleDb;
                    break;
                case "SQLNCLI10":
                    provider = EnumProviders.SqlServer;
                    break;
                default:
                    provider = EnumProviders.SqlServer;
                    break;
            }

            return provider;
        }

    }
}
