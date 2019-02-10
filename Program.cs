using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CsProj;
using JetBrains.Profiler.Windows.Api;
using Microsoft.SqlServer.Server;
using Wraith.Data.SqlClient;

namespace BenchmarkSqlWrite
{
	class Program
	{
		static async Task Main(string[] args)
		{
			bool managed = false;
			Environment.SetEnvironmentVariable("System.Data.SqlClient.UseManagedSNIOnWindows",managed.ToString() );
            //Environment.SetEnvironmentVariable("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT", "1");
			await Task.CompletedTask;

            if (args != null && args.Length > 0)
            {
                BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
                Console.ReadLine();
            }
            else
            {
				Stopwatch timer = new Stopwatch();



				var bench = new WriteBenchmarks();
				bench.GlobalSetup();

			    if (MemoryProfiler.IsActive)
			    {
				    MemoryProfiler.EnableAllocations();
			    }

				timer.Start();

				if (MemoryProfiler.IsActive)
				{
					MemoryProfiler.Dump();
				}

				for (int index = 0; index < 100; index++)
				{
                    //bench.IterationSetup();

                    //bench.SyncFloat32();

                    //bench.ReadOrderDetails();

                    //bench.ReadStoredProc();

                    //bench.SyncGuid();

                    //await bench.AsyncFloat32();

                    //bench.ChangeType();

                    //bench.OpenClose();

                    bench.SyncWriteTableParam();
                }

				if (MemoryProfiler.IsActive)
				{
					MemoryProfiler.Dump();
				}

				timer.Stop();
				bench.GlobalCleanup();



				Console.WriteLine(timer.Elapsed);
				//if (!MemoryProfiler.IsActive)
				//{
				//	Console.ReadLine();
				//}
			}

			
		}
	}



	[MemoryDiagnoser]
	[MarkdownExporter]
    [InProcess]
	public class WriteBenchmarks
	{
		private string connectionString;

		private SqlConnection syncConnection;
		private SqlConnection asyncConnection;
		private SqlCommand int32command;
		private SqlParameter int32parameter;
		public SqlCommand syncFloat32command;
		public SqlParameter syncFloat32parameter;
		private SqlCommand syncGuidCommand;
		private SqlParameter syncGuidParameter;
		private SqlCommand asyncFloat32command;
		private SqlParameter asyncFloat32parameter;
        private SqlCommand syncWriteTableParamCommand;
        private SqlParameter syncWriteUdtParameter;

        private SqlCommand syncReadGuid;

		private SqlCommand truncateCommandInt32;
		private SqlCommand truncateCommandFloat32;
		private SqlCommand truncateCommandGuid;

		private object[] int32numbers;
		private object[] float32numbers;
		private object[] guids;
        private List<int> intList;

        [GlobalSetup]
        public void GlobalSetup()
        {
            connectionString = "Data Source=(local);Initial Catalog=Northwind;Trusted_Connection=true;Connect Timeout=1";
            //SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            //builder.IntegratedSecurity = false;
            //builder.UserID = "user";
            //builder.Password = "pass";
            //builder.PersistSecurityInfo = true;
            //builder.MultipleActiveResultSets = true;
            //connectionString = builder.ToString();
            //Console.WriteLine(connectionString);
            connectionString = "Data Source=(local);Initial Catalog=Scratch;Integrated Security=false;Persist Security Info=True;User ID=user;Password=pass;Connect Timeout=1;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=True;";

            intList = new List<int>(1000);
            int32numbers = new object[1000];  //values in boxes so we don't box in the benchmark
            Random random = new Random();
            for (int index = 0; index < int32numbers.Length; index++)
            {
                int32numbers[index] = random.Next();
                intList.Add(random.Next());
            }

            float32numbers = new object[1000]; //values in boxes so we don't box in the benchmark
            for (int index = 0; index < float32numbers.Length; index++)
            {
                float32numbers[index] = (float)random.NextDouble();
            }

            guids = new object[1000];
            for (int index = 0; index < guids.Length; index++)
            {
                guids[index] = Guid.NewGuid();
            }

            syncConnection = new SqlConnection(connectionString);
            syncConnection.Open();

            asyncConnection = new SqlConnection(connectionString);
            asyncConnection.OpenAsync();

            int32command = new SqlCommand("INSERT INTO Int32(value) VALUES (@value)", syncConnection);
            int32parameter = int32command.Parameters.Add("@value", SqlDbType.Int, 4);
            //int32command.Prepare();

            //syncFloat32command = new SqlCommand("INSERT INTO Float32(value) VALUES (@value)", syncConnection);
            syncFloat32command = new SqlCommand("InsertFloat32", syncConnection);
            syncFloat32command.CommandType = CommandType.StoredProcedure;

            syncFloat32parameter = syncFloat32command.Parameters.Add("@value", SqlDbType.Real, 4);
            //syncFloat32command.Prepare();


            syncGuidCommand = new SqlCommand("InsertGuid", syncConnection);
            syncGuidCommand.CommandType = CommandType.StoredProcedure;

            syncGuidParameter = syncGuidCommand.Parameters.Add("@value", SqlDbType.UniqueIdentifier, 16);


            asyncFloat32command = new SqlCommand("INSERT INTO Float32(value) VALUES (@value)", syncConnection);
            asyncFloat32parameter = asyncFloat32command.Parameters.Add("@value", SqlDbType.Real, 4);
            //asyncFloat32command.Prepare();


            //syncWriteTableParamCommand = new SqlCommand("JoinInts", syncConnection)
            //{
            //    CommandType = CommandType.StoredProcedure
            //};
            //syncWriteUdtParameter = new SqlParameter("@ints", intList).SetupTableValuedParameter(new UnmanagedListAdapter<int>(udtTypeName: "dbo.Int32Table", udtTypeColumnName: "value"));
            //syncWriteTableParamCommand.Parameters.Add(syncWriteUdtParameter);


            syncWriteTableParamCommand = new SqlCommand("InsertJobResult", syncConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            syncWriteUdtParameter = new SqlParameter("@result", JobResultAdapter.Generator(500,1,100,DateTime.Now)).SetupTableValuedParameter(new JobResultAdapter());
            syncWriteTableParamCommand.Parameters.Add(syncWriteUdtParameter);

            truncateCommandInt32 = new SqlCommand("TRUNCATE TABLE Int32", syncConnection);
			truncateCommandInt32.Prepare();

			truncateCommandFloat32 = new SqlCommand("TRUNCATE TABLE Float32", syncConnection);
			truncateCommandFloat32.Prepare();

			//truncateCommandGuid = new SqlCommand("TRUNCATE TABLE Guid", syncConnection);
			//truncateCommandGuid.Prepare();

            syncReadGuid = new SqlCommand("SELECT value FROM Guid", syncConnection);

            //truncateCommandGuid.ExecuteNonQuery();
            //SyncGuid();

        }

		[IterationSetup]
		public void IterationSetup()
		{
			truncateCommandInt32.ExecuteNonQuery();
			truncateCommandFloat32.ExecuteNonQuery();
			//truncateCommandGuid.ExecuteNonQuery();
		}

		//[Benchmark]
		public void InsertInt32()
		{
			for (int index = 0; index<int32numbers.Length; index++)
			{
				int32parameter.Value=int32numbers[index];
				int32command.ExecuteNonQuery();
			}
		}

		//[Benchmark]
		public void SyncFloat32()
		{
			for (int index = 0; index<float32numbers.Length; index++)
			{
				syncFloat32parameter.Value=float32numbers[index];
				syncFloat32command.ExecuteNonQuery();
			}
		}

		//[Benchmark]
		public void SyncGuid()
		{
			for (int index = 0; index < guids.Length; index++)
			{
				syncGuidParameter.Value = guids[index];
				syncGuidCommand.ExecuteNonQuery();
			}
		}

        //[Benchmark]
        public int SyncReadGetField()
        {
            int emptyCount = 0;
            for (int index = 0; index < 1000; index++)
            {
                using (var reader = syncReadGuid.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Guid value = reader.GetFieldValue<Guid>(0);
                        if (value == Guid.Empty)
                        {
                            emptyCount += 1;
                        }
                    }
                }
            }
            return emptyCount;
        }

        //[Benchmark]
        public int SyncReadGetGuid()
        {
            int emptyCount = 0;
            for (int index = 0; index < 1000; index++)
            {
                using (var reader = syncReadGuid.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Guid value = reader.GetGuid(0);
                        if (value == Guid.Empty)
                        {
                            emptyCount += 1;
                        }
                    }
                }
            }
            return emptyCount;
        }

        //[Benchmark]
        public async Task AsyncFloat32()
		{
			for (int index = 0; index<float32numbers.Length; index++)
			{
				asyncFloat32parameter.Value=float32numbers[index];
				await asyncFloat32command.ExecuteNonQueryAsync();
			}
		}

		//[Benchmark]
		public int ReadOrderDetails()
		{
			var builder = new SqlConnectionStringBuilder(this.connectionString) { InitialCatalog = "Northwind" };
			var connectionString = builder.ToString();
			int max = 0;
			for (int index = 0; index < 100; index++)
			{
				using (var connection = new SqlConnection(connectionString))
				using (var command = new SqlCommand("SELECT * FROM [Order Details]", connection))
				{
					connection.Open();
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							int value = reader.GetInt32(0);
							if (value > max)
							{
								max = value;
							}
						}
					}
				}
			}
			return max;
		}

        //[Benchmark]
        public int ReadStoredProc()
        {
            var builder = new SqlConnectionStringBuilder(this.connectionString) { InitialCatalog = "Northwind" };
            var connectionString = builder.ToString();
            int max = 0;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                for (int index = 0; index < 100; index++)
                {
                    using (var command = new SqlCommand("select * from Employees", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int value = reader.GetInt32(reader.GetOrdinal("EmployeeID"));
                                if (value > max)
                                {
                                    max = value;
                                }
                            }
                        }
                    }
                }
            }
            return max;
        }

        [Benchmark]
        public int SyncWriteTableParam()
        {
            int retval = 0;

            for (int index = 0; index < 100; index++)
            {
                syncWriteTableParamCommand.ExecuteNonQuery();
                //using (var reader = syncWriteTableParamCommand.ExecuteReader())
                //{
                //    if (reader.HasRows)
                //    {
                //        while (reader.Read())
                //        {
                //            int value = reader.GetInt32(0);
                //            if (value > retval)
                //            {
                //                retval = value;
                //            }
                //        }
                //    }
                //}
            }
            return retval;
        }

        //[Benchmark]
        public void OpenClose()
		{
			for (int index = 0; index<float32numbers.Length; index++)
			{
				using (var connection = new SqlConnection(connectionString))
				using (var command = new SqlCommand("SELECT count(*) FROM Northwind.dbo.Categories", connection))
				{
					connection.Open();
					command.ExecuteNonQuery();
				}
			}
		}

		public void ChangeType()
		{
			using (var command = new SqlCommand("", syncConnection))
			{
				command.Parameters.Add("value", SqlDbType.Float).Value=9.01f;

				command.CommandText="InsertInt32";
				command.CommandType=CommandType.StoredProcedure;

				command.ExecuteNonQuery();

				command.CommandText="INSERT INTO Float32(value) VALUES (@value)";
				command.CommandType=CommandType.Text;
				command.Prepare();
				command.ExecuteNonQuery();

				command.CommandText="INSERT INTO Float32(value) VALUES (@value)";
				command.CommandType=CommandType.Text;
				command.ExecuteNonQuery();

			}
		}

		[GlobalCleanup]
		public void GlobalCleanup()
		{
            //truncateCommandGuid.ExecuteNonQuery();

            syncConnection.Close();
			asyncConnection.Close();
		}
	}

    public class JobResult
    {
        public int LocationID { get; set; }
        public int JobID { get; set; }
        public DateTime Timestamp { get; set; }
        public int Counter { get; set; }
    }

    public class JobResultAdapter : TableValuedParameterAdapter<JobResult>
    {
        private static SqlMetaData[] s_metaData = CreateMetaData();
        private static Random random = new Random();

        public JobResultAdapter()
            :base("dbo.JobResult", s_metaData)
        {
        }

        public override void SetData(SqlDataRecord record, JobResult value)
        {
            record.SetInt32(0,value.LocationID);
            record.SetInt32(1, value.JobID);
            record.SetDateTime(2, value.Timestamp);
            record.SetInt32(3, value.Counter);
        }

        private static SqlMetaData[] CreateMetaData()
        {
            SqlMetaData[] retval = new SqlMetaData[4];
            retval[0] = new SqlMetaData("LocationID", SqlDbType.Int, false, true, SortOrder.Ascending, 0);
            retval[1] = new SqlMetaData("JobID", SqlDbType.Int, false, true, SortOrder.Ascending, 1);
            retval[2] = new SqlMetaData("Timestamp", SqlDbType.DateTime, false, false, SortOrder.Unspecified, -1);
            retval[3] = new SqlMetaData("Counter", SqlDbType.Int, false, false, SortOrder.Unspecified, -1);
            return retval;
        }

        public static IEnumerable<JobResult> Generator(int count, int location, int JobIDStart, DateTime timestampStart)
        {
            JobResult result = new JobResult
            {
                LocationID = location
            };
            for (int current = 0; current < count; current++)
            {
                result.JobID = JobIDStart + current;
                result.Timestamp = timestampStart.AddMinutes(current);
                result.Counter = random.Next();
                yield return result;
            }
            yield break;
        }
    }
}
