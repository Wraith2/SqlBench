namespace Wraith.Data.SqlClient
{
	using Microsoft.SqlServer.Server;

	public interface ITableValuedParameterAdapter<T>
	{
        SqlMetaData[] MetaData { get; }
        string UdtTypeName { get; }

        void SetData(SqlDataRecord record, T value);
	}
}
