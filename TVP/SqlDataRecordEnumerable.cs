namespace Wraith.Data.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.SqlServer.Server;

    [DebuggerStepThrough]
    public class SqlDataRecordEnumerable<T> : IEnumerable<SqlDataRecord>
	{
        public SqlDataRecordEnumerable(ITableValuedParameterAdapter<T> adapter, IEnumerable<T> value = null)
        {
            this.Adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            this.Value = value;
        }

        public ITableValuedParameterAdapter<T> Adapter { get; }

        public IEnumerable<T> Value { get; set; }

        public IEnumerator<SqlDataRecord> GetEnumerator() => new SqlDataRecordEnumerator<T>(this.Value.GetEnumerator(), this.Adapter);

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

}