namespace Wraith.Data.SqlClient
{
    using System;
    using Microsoft.SqlServer.Server;

    public sealed class DelegateSqlDataRecordAdapter<T> : TableValuedParameterAdapter<T>
    {
        private Action<SqlDataRecord, T> setter;

        public DelegateSqlDataRecordAdapter(string sqlTypeName, SqlMetaData[] metaData, Action<SqlDataRecord, T> setter)
            : base(sqlTypeName, metaData)
        {
            this.setter = setter ?? throw new ArgumentNullException(nameof(setter));
        }

        public override void SetData(SqlDataRecord record, T value)
        {
            setter(record, value);
        }
    }
}
