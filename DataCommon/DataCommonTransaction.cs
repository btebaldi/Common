using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace DataCommon
{
  public class DataCommonTransaction : List<DataCommonBaseTransaction>
    {
    private string mstrConnectString;

    public string ConnectString
    {
      get { return mstrConnectString; }
      set { mstrConnectString = value; }
    }

    public DataCommonTransaction(string ConnectString)
    {
      mstrConnectString = ConnectString;
    }

    public void Execute()
    {
      IDbTransaction trn = null;
      IDbConnection cnn = null;

      try
      {
        cnn = DataLayer.CreateConnection(mstrConnectString);
        cnn.Open();
        trn = cnn.BeginTransaction();

        foreach (DataCommonBaseTransaction dc in this)
        {
          // Prepare Command Object for Transaction
          dc.ConnectString = mstrConnectString;
          dc.PrepareForTransaction();
          // Set Transaction/Connection on Command Object
          dc.CommandObject.Connection = cnn;
          dc.CommandObject.Transaction = trn;

          switch (dc.TransType)
          {
            case DataCommonTransType.Insert:
              dc.Insert();
              break;

            case DataCommonTransType.Update:
              dc.Update();
              break;

            case DataCommonTransType.Delete:
              dc.Delete();
              break;

            default:
              break;
          }
        }

        trn.Commit();
      }
      catch (Exception ex)
      {
        if (trn != null)
        {
          trn.Rollback();
        }

        throw ex;
      }
      finally
      {
        if (cnn != null)
        {
          cnn.Close();
          cnn.Dispose();
        }

        if (trn != null)
        {
          trn.Dispose();
        }
      }
    }
    }
}
