namespace Wraith.Data.SqlClient
{
    using System;
    using Microsoft.SqlServer.Server;

    //[DebuggerStepThrough]
    public abstract class TableValuedParameterAdapter<T> : ITableValuedParameterAdapter<T>
	{
        protected TableValuedParameterAdapter(string sqlTypeName,SqlMetaData[] metaData)
		{
			if (metaData==null || metaData.Length==0)
			{
				throw new ArgumentNullException("metaData","metaData is null or empty");
			}
			if (string.IsNullOrEmpty(sqlTypeName))
			{
				throw new ArgumentNullException("sqlTypeName","sqlStypeName is null or empty");
			}
            this.MetaData=metaData;
			this.UdtTypeName=sqlTypeName;
		}

        public SqlMetaData[] MetaData { get; }

        public string UdtTypeName { get; }

        public abstract void SetData(SqlDataRecord record, T value);

    }
}
