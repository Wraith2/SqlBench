namespace Wraith.Data.SqlClient
{
    using System;
    using System.Collections.Generic;
    using Microsoft.SqlServer.Server;

    //[DebuggerStepThrough]
    public sealed class SqlDataRecordEnumerator<T> : IEnumerator<SqlDataRecord>
    {
        private SqlDataRecord record;
        private ITableValuedParameterAdapter<T> adapter;
        private IEnumerator<T> enumerator;
        private bool moved;
        private bool fetched;

        public SqlDataRecordEnumerator(IEnumerator<T> enumerator, ITableValuedParameterAdapter<T> adapter)
        {
            this.adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            this.enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator)); ;
            record = new SqlDataRecord(adapter.MetaData);
        }

        public SqlDataRecord Current
        {
            get
            {
                CheckDisposed();
                if (!moved)
                {
                    throw new InvalidOperationException("cannot get Current without MoveNext");
                }
                if (!fetched)
                {
                    adapter.SetData(record, enumerator.Current);
                    fetched = true;
                }
                return record;
            }
        }

        public void Dispose()
        {
            record = null;
            adapter = null;
            if (enumerator != null)
            {
                enumerator.Dispose();
                enumerator = null;
            }
            moved = false;
            fetched = false;
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public bool MoveNext()
        {
            CheckDisposed();
            if (moved = enumerator.MoveNext())
            {
                fetched = false;
            }
            return moved;
        }

        public void Reset()
        {
            CheckDisposed();
            enumerator.Reset();
            moved = false;
            fetched = false;

        }

        private void CheckDisposed()
        {
            if (enumerator == null)
            {
                throw new ObjectDisposedException(nameof(SqlDataRecordEnumerator<T>));
            }
        }
    }

}