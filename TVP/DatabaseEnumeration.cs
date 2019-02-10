namespace Wraith.Data.SqlClient
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Threading;
	using System.Diagnostics;
	using System.Data.SqlClient;

	//[DebuggerStepThrough]
	//internal static class DatabaseEnumerable
	//{
	//	public static DatabaseEnumerable<T,TTable,TRow> Create<T,TTable,TRow>(TTable table,Converter<TRow,T> converter)
	//		where TTable : System.Data.DataTable
	//		where TRow : System.Data.DataRow
	//	{
	//		return new DatabaseEnumerable<T,TTable,TRow>(table,converter);
	//	}
	//}

	//[DebuggerStepThrough]
	//internal class DatabaseEnumerable<T,TTable,TRow> : IEnumerable<T>,IDisposable
	//	where TTable : System.Data.DataTable
	//	where TRow : System.Data.DataRow
	//{
	//	private TTable table;
	//	private Converter<TRow,T> converter;
	//	private bool disposed;

	//	public DatabaseEnumerable(TTable table,Converter<TRow,T> converter)
	//	{
	//		if (table==null)
	//		{
	//			throw new ArgumentNullException("table","table is null");
	//		}
	//		this.table=table;
	//		this.converter=converter;
	//	}

	//	#region IDisposable Members

	//	public void Dispose()
	//	{
	//		this.table=default(TTable);
	//		this.disposed=true;
	//	}

	//	#endregion

	//	#region IEnumerable<T> Members

	//	public IEnumerator<T> GetEnumerator()
	//	{
	//		if (this.disposed)
	//		{
	//			throw new ObjectDisposedException("DatabaseEnumerable");
	//		}
	//		return new DatabaseEnumerator<T,TTable,TRow>(this.table,converter);
	//	}

	//	#endregion

	//	#region IEnumerable Members

	//	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	//	{
	//		return this.GetEnumerator();
	//	}

	//	#endregion

	//	public List<T> ToList()
	//	{
	//		List<T> retval=null;
	//		if (this.table!=null && this.table.Rows!=null)
	//		{
	//			retval = new List<T>(this.table.Rows.Count);
	//			foreach (var item in this)
	//			{
	//				retval.Add(item);
	//			}
	//		}
	//		return retval;
	//	}


	//}

	//[DebuggerStepThrough]
	//internal class DatabaseEnumerator<T,TTable,TRow> : IEnumerator<T>
	//	where TTable : System.Data.DataTable
	//	where TRow : System.Data.DataRow
	//{
	//	private const int InvalidIndex =-1;
	//	private T current;
	//	private TTable table;
	//	private int currentIndex;
	//	private Converter<TRow,T> converter;
	//	private bool disposed;

	//	public DatabaseEnumerator(TTable table,Converter<TRow,T> converter)
	//	{
	//		//if (table==null)
	//		//{
	//		//	throw new ArgumentNullException("table","table is null");
	//		//}
	//		this.table=table;
	//		this.converter=converter;
	//		this.currentIndex = DatabaseEnumerator<T,TTable,TRow>.InvalidIndex;
	//	}

	//	#region IEnumerator<IEvent> Members

	//	public T Current
	//	{
	//		get
	//		{
	//			if (this.disposed)
	//			{
	//				throw new ObjectDisposedException("DatabaseEventEnumerator");
	//			}
	//			if (this.currentIndex==InvalidIndex)
	//			{
	//				throw new InvalidOperationException("");
	//			}
	//			//if (this.current==null)
	//			//{
	//				this.current = this.converter((TRow)this.table.Rows[this.currentIndex]);
	//			//}
	//			return this.current;
	//		}
	//	}

	//	#endregion

	//	#region IDisposable Members

	//	public void Dispose()
	//	{
	//		try
	//		{
	//			this.table.Dispose();
	//		}
	//		catch
	//		{
	//		}
	//		finally
	//		{
	//			this.table=null;
	//		}

	//		this.ClearCurrent();
	//		this.disposed=true;
	//	}

	//	#endregion

	//	#region IEnumerator Members

	//	object System.Collections.IEnumerator.Current
	//	{
	//		get { return this.Current; }
	//	}

	//	public bool MoveNext()
	//	{
	//		if (this.disposed)
	//		{
	//			throw new ObjectDisposedException("DatabaseEnumerator");
	//		}
	//		this.current=default(T);
	//		this.currentIndex+=1;
	//		return (this.table!=null && this.currentIndex<=(this.table.Rows.Count-1));
	//	}

	//	public void Reset()
	//	{
	//		if (this.disposed)
	//		{
	//			throw new ObjectDisposedException("DatabaseEnumerator");
	//		}
	//		this.currentIndex=InvalidIndex;
	//	}

	//	#endregion

	//	private void ClearCurrent()
	//	{
	//		this.current = default(T);
	//	}

	//}
}