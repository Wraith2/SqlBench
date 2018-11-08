using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace BenchmarkSqlWrite
{
	class Program
	{
		static async Task Main(string[] args)
		{
			await Task.CompletedTask;

			BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);


			//Stopwatch timer = new Stopwatch();

			//var bench = new WriteBenchmarks();
			//bench.GlobalSetup();
			//timer.Start();
			//for (int index = 0; index<100; index++)
			//{
			//	//bench.IterationSetup();
			//	//bench.SyncFloat32();

			//	//bench.IterationSetup();
			//	//await bench.AsyncFloat32();

			//	//bench.ChangeType();

			//	bench.OpenClose();
			//}
			//timer.Stop();
			//bench.GlobalCleanup();

			//Console.WriteLine(timer.Elapsed);




		}
	}

	[MemoryDiagnoser]
	[MarkdownExporter]
	public class WriteBenchmarks
	{
		private SqlConnection syncConnection;
		private SqlConnection asyncConnection;
		private SqlCommand int32command;
		private SqlParameter int32parameter;
		private SqlCommand syncFloat32command;
		private SqlParameter syncFloat32parameter;
		private SqlCommand asyncFloat32command;
		private SqlParameter asyncFloat32parameter;

		private SqlCommand truncateCommandInt32;
		private SqlCommand truncateCommandFloat32;

		private object[] int32numbers;
		private object[] float32numbers;

		[GlobalSetup]
		public void GlobalSetup()
		{


			int32numbers = new object[1000];  //values in boxes so we don't box in the benchmark
			Random random = new Random();
			for (int index = 0; index<int32numbers.Length; index++)
			{
				int32numbers[index]=random.Next();
			}

			float32numbers = new object[1000]; //values in boxes so we don't box in the benchmark
			for (int index = 0; index<float32numbers.Length; index++)
			{
				float32numbers[index]=(float)random.NextDouble();
			}

			syncConnection = new SqlConnection("Data Source=(local);Initial Catalog=Northwind;Trusted_Connection=true;;Connect Timeout=1");
			syncConnection.Open();

			asyncConnection = new SqlConnection("Data Source=(local);Initial Catalog=Northwind;Trusted_Connection=true;;Connect Timeout=1");
			asyncConnection.OpenAsync();

			int32command = new SqlCommand("INSERT INTO Int32(value) VALUES (@value)", syncConnection);
			int32parameter = int32command.Parameters.Add("@value", System.Data.SqlDbType.Int, 4);
			//int32command.Prepare();



			//syncFloat32command = new SqlCommand("INSERT INTO Float32(value) VALUES (@value)", syncConnection);
			syncFloat32command = new SqlCommand("InsertFloat", syncConnection);
			syncFloat32command.CommandType = CommandType.StoredProcedure;

			syncFloat32parameter = syncFloat32command.Parameters.Add("@value", System.Data.SqlDbType.Float, 4);
			//syncFloat32command.Prepare();

			asyncFloat32command = new SqlCommand("INSERT INTO Float32(value) VALUES (@value)", syncConnection);
			asyncFloat32parameter = asyncFloat32command.Parameters.Add("@value", System.Data.SqlDbType.Float, 4);
			//asyncFloat32command.Prepare();

			truncateCommandInt32 = new SqlCommand("TRUNCATE TABLE Int32", syncConnection);
			truncateCommandInt32.Prepare();

			truncateCommandFloat32 = new SqlCommand("TRUNCATE TABLE Float32", syncConnection);
			truncateCommandFloat32.Prepare();
		}

		[IterationSetup]
		public void IterationSetup()
		{
			truncateCommandInt32.ExecuteNonQuery();
			truncateCommandFloat32.ExecuteNonQuery();
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
		public async Task AsyncFloat32()
		{
			for (int index = 0; index<float32numbers.Length; index++)
			{
				asyncFloat32parameter.Value=float32numbers[index];
				await asyncFloat32command.ExecuteNonQueryAsync();
			}
		}


		[Benchmark]
		public void OpenClose()
		{
			for (int index = 0; index<float32numbers.Length; index++)
			{
				using (var connection = new SqlConnection("Data Source=(local);Initial Catalog=Northwind;Trusted_Connection=true;Connect Timeout=1;Application Name=Test;"))
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

				command.CommandText="InsertInt";
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
			syncConnection.Close();
			asyncConnection.Close();
		}
	}
}
