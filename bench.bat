@echo off
set corefx=E:\Programming\csharp7\corefx
set testhost=%corefx%\bin\testhost\netcoreapp-Windows_NT-Release-x64\shared\Microsoft.NETCore.App\9.9.9
set tools=%corefx%\Tools
if not exist "%testhost%\BenchmarkDotNet.dll" (
	xcopy /E /Q /Y deps\*.* "%testhost%\"
)
dotnet run -c Release -f netcoreapp3.0 -- -f *WriteBenchmarks* --coreRun "%testhost%\CoreRun.exe" --cli "E:\Programming\csharp7\sdk\dotnet.exe"
@echo on