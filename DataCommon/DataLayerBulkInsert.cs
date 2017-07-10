using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCommon
{
    public class DataLayerBulkInsert
    {
        public static void SqlServerBulkInsert(string ConnectString, DataTable table)
        {
            // connect to SQL
            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ConnectString))
            {
                System.Data.SqlClient.SqlBulkCopy bulkCopy = new System.Data.SqlClient.SqlBulkCopy(connection,
                    System.Data.SqlClient.SqlBulkCopyOptions.TableLock |
                    //System.Data.SqlClient.SqlBulkCopyOptions.FireTriggers |
                    System.Data.SqlClient.SqlBulkCopyOptions.UseInternalTransaction,
                    null
                    );

                // set the destination table name
                bulkCopy.DestinationTableName = table.TableName;
                bulkCopy.BatchSize = 100;
                connection.Open();

                // write the data in the "dataTable"
                bulkCopy.WriteToServer(table);
                connection.Close();
            }
        }
    }
}
