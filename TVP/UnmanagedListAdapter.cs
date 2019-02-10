namespace Wraith.Data.SqlClient
{
	using System;
	using System.Data;
	using Microsoft.SqlServer.Server;

    public class UnmanagedListAdapter<T> : TableValuedParameterAdapter<T>
        where T : unmanaged
    {
        public UnmanagedListAdapter(string udtTypeName,string udtTypeColumnName="value")
            : base(udtTypeName, new SqlMetaData[1] { SqlMetaData.InferFromValue(default(T), udtTypeColumnName) })
        { }

        public override void SetData(SqlDataRecord record, T value)
        {
            if (typeof(T) == typeof(int))
            {
                record.SetInt32(0, (int)(object)value);
            }
            else if (typeof(T) == typeof(long))
            {
                record.SetInt64(0, (long)(object)value);
            }
            else if (typeof(T) == typeof(short))
            {
                record.SetInt16(0, (short)(object)value);
            }
            else if (typeof(T) == typeof(byte))
            {
                record.SetByte(0, (byte)(object)value);
            }
            else if (typeof(T) == typeof(bool))
            {
                record.SetBoolean(0, (bool)(object)value);
            }
            else if (typeof(T) == typeof(DateTime))
            {
                record.SetDateTime(0, (DateTime)(object)value);
            }
            else if (typeof(T) == typeof(DateTimeOffset))
            {
                record.SetDateTimeOffset(0, (DateTimeOffset)(object)value);
            }
            else if (typeof(T) == typeof(decimal))
            {
                record.SetDecimal(0, (decimal)(object)value);
            }
            else if (typeof(T) == typeof(double))
            {
                record.SetDecimal(0, (decimal)(object)value);
            }
            else if (typeof(T) == typeof(float))
            {
                record.SetDecimal(0, (decimal)(object)value);
            }
            else if (typeof(T) == typeof(Guid))
            {
                record.SetGuid(0, (Guid)(object)value);
            }
            else
            {
                record.SetValue(0, value);
            }
        }
    }
}
