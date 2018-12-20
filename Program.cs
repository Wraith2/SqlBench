using System;
using System.Buffers;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CsProj;

namespace BenchmarkSqlWrite
{
	class Program
	{
		static async Task Main(string[] args)
		{
			await Task.CompletedTask;
			if (args != null && args.Length > 0)
			{
				BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
			}
			else
			{
				Stopwatch timer = new Stopwatch();

				var bench = new WriteBenchmarks();
				bench.GlobalSetup();
				timer.Start();
				for (int index = 0; index < 100; index++)
				{
					//bench.IterationSetup();
					//bench.SyncFloat32();

					bench.IterationSetup();
					bench.SyncGuid();

					//bench.IterationSetup();
					//await bench.AsyncFloat32();

					//bench.ChangeType();

					//bench.OpenClose();
				}
				timer.Stop();
				bench.GlobalCleanup();

				Console.WriteLine(timer.Elapsed);

			}

			Console.ReadLine();
		}
	}



	[MemoryDiagnoser]
	[MarkdownExporter]
	//[Config(typeof(Config))]
	[InProcess]
	public class WriteBenchmarks
	{
		public class Config : ManualConfig
		{
			public Config()
			{
				//Add(
				//	Job.Default.With(
				//		CsProjCoreToolchain.From(
				//			new BenchmarkDotNet.Toolchains.DotNetCli.NetCoreAppSettings(
				//				"netcoreapp3.0",
				//				"3.0.100-preview-009738",
				//				"CoreCLR3.0",
				//				customDotNetCliPath: @"E:\Programming\csharp7\sdk\dotnet.exe"
				//			)
				//		)
				//	)
				//);
				Add(Job.Default.With(CsProjCoreToolchain.NetCoreApp30));
			}
		}

		private string connectionString;

		private SqlConnection syncConnection;
		private SqlConnection asyncConnection;
		private SqlCommand int32command;
		private SqlParameter int32parameter;
		private SqlCommand syncFloat32command;
		private SqlParameter syncFloat32parameter;
		private SqlCommand syncGuidCommand;
		private SqlParameter syncGuidParameter;
		private SqlCommand asyncFloat32command;
		private SqlParameter asyncFloat32parameter;

		private SqlCommand truncateCommandInt32;
		private SqlCommand truncateCommandFloat32;
		private SqlCommand truncateCommandGuid;

		private object[] int32numbers;
		private object[] float32numbers;
		private object[] guids;

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
			connectionString = "Data Source=(local);Initial Catalog=Scratch;Integrated Security=false;Persist Security Info=True;User ID=user;Password=pass;Connect Timeout=1;MultipleActiveResultSets=True;";

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
			int32parameter = int32command.Parameters.Add("@value", System.Data.SqlDbType.Int, 4);
			//int32command.Prepare();

			//syncFloat32command = new SqlCommand("INSERT INTO Float32(value) VALUES (@value)", syncConnection);
			syncFloat32command = new SqlCommand("InsertFloat32", syncConnection);
			syncFloat32command.CommandType = CommandType.StoredProcedure;

			syncFloat32parameter = syncFloat32command.Parameters.Add("@value", System.Data.SqlDbType.Float, 4);
			//syncFloat32command.Prepare();


			syncGuidCommand = new SqlCommand("InsertGuid", syncConnection);
			syncGuidCommand.CommandType = CommandType.StoredProcedure;

			syncGuidParameter = syncGuidCommand.Parameters.Add("@value", System.Data.SqlDbType.UniqueIdentifier, 16);


			asyncFloat32command = new SqlCommand("INSERT INTO Float32(value) VALUES (@value)", syncConnection);
			asyncFloat32parameter = asyncFloat32command.Parameters.Add("@value", System.Data.SqlDbType.Float, 4);
			//asyncFloat32command.Prepare();

			truncateCommandInt32 = new SqlCommand("TRUNCATE TABLE Int32", syncConnection);
			truncateCommandInt32.Prepare();

			truncateCommandFloat32 = new SqlCommand("TRUNCATE TABLE Float32", syncConnection);
			truncateCommandFloat32.Prepare();

			truncateCommandGuid = new SqlCommand("TRUNCATE TABLE Guid", syncConnection);
			truncateCommandGuid.Prepare();
		}

		[IterationSetup]
		public void IterationSetup()
		{
			truncateCommandInt32.ExecuteNonQuery();
			truncateCommandFloat32.ExecuteNonQuery();
			truncateCommandGuid.ExecuteNonQuery();
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

		[Benchmark]
		public void SyncGuid()
		{
			for (int index = 0; index < guids.Length; index++)
			{
				syncGuidParameter.Value = guids[index];
				syncGuidCommand.ExecuteNonQuery();
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
			syncConnection.Close();
			asyncConnection.Close();
		}
	}
}
