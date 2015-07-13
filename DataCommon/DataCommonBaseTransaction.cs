using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace DataCommon
{
    public enum DataCommonTransType
    {
        NA,
        Insert,
        Update,
        Delete
    }

    [Serializable]
    public abstract class DataCommonBaseTransaction
    {
        private string mstrSQL;
        private string mstrConnectString;
        private IDbCommand mcmd;
        private DataCommonTransType mTransType;

        public IDbCommand CommandObject
        {
            get { return mcmd; }
            set { mcmd = value; }
        }

        public DataCommonTransType TransType
        {
            get { return mTransType; }
            set { mTransType = value; }
        }

        public string SQL
        {
            get { return mstrSQL; }
            set { mstrSQL = value; }
        }

        public string ConnectString
        {
            get { return mstrConnectString; }
            set { mstrConnectString = value; }
        }

        public virtual void PrepareForTransaction()
        {
            mcmd = DataLayer.CreateCommand(mstrSQL, mstrConnectString);
        }

    public abstract void Validate();
        public abstract int Insert();
        public abstract int Update();
        public abstract int Delete();
    }
}
