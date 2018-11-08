@echo off
set corefx=E:\Programming\csharp7\corefx
set testhost=%corefx%\bin\testhost\netcoreapp-Windows_NT-Release-x64\shared\Microsoft.NETCore.App\9.9.9
set tools=%corefx%\Tools
%testhost%\corerun.exe %tools%\csc.dll /noconfig /nologo /r:%testhost%\System.Private.Corelib.dll /r:%testhost%\System.Runtime.dll /r:%testhost%\System.Runtime.Extensions.dll /r:%testhost%\System.Console.dll /r:%testhost%\System.IO.dll /r:%testhost%\System.IO.Filesystem.dll /r:%testhost%\System.ComponentModel.Primitives.dll /r:deps\BenchmarkDotNet.dll /r:%testhost%\System.Data.Common.dll /r:%testhost%\System.Data.dll /r:%testhost%\System.Linq.dll /r:%testhost%\System.Data.SqlClient.dll /langversion:latest /platform:x64 /optimize+ /debug:full /out:bin\Release\netcoreapp3.0\SqlBench.dll Program.cs
@echo on