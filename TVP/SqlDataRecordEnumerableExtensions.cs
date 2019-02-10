namespace Wraith.Data.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    public static class TableValuedParameterExtensions
    {
        public static SqlParameter SetupTableValuedParameter<T>(this SqlParameter parameter, SqlDataRecordEnumerable<T> enumerable)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }
            if (enumerable is null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }
            parameter.SqlDbType = SqlDbType.Structured;
            parameter.Direction = ParameterDirection.Input;
            parameter.TypeName = enumerable.Adapter.UdtTypeName;
            object previousValue = parameter.Value;
            parameter.Value = enumerable;
            if (!(previousValue is null))
            {
                var value = previousValue as IEnumerable<T>;
                if (!(value is null))
                {
                    enumerable.Value = value;
                }
            }

            return parameter;
        }

        public static SqlParameter SetupTableValuedParameter<T>(this SqlParameter parameter, ITableValuedParameterAdapter<T> adapter, IEnumerable<T> value = null)
        {
            if (adapter is null)
            {
                throw new ArgumentNullException(nameof(adapter));
            }
            var enumerable = new SqlDataRecordEnumerable<T>(adapter);
            parameter = SetupTableValuedParameter(parameter, enumerable);
            if (!(value is null))
            {
                enumerable.Value = value;
            }
            return parameter;
        }

        public static SqlParameter SetTableValuedParameter<T>(this SqlParameter parameter,IEnumerable<T> value)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }
            var enumerable = parameter.Value as SqlDataRecordEnumerable<T>;
            if (enumerable is null)
            {
                throw new InvalidOperationException($"parameter value is not {nameof(SqlDataRecordEnumerable<T>)}");
            }
            enumerable.Value = value;
            return parameter;
        }

    }

}