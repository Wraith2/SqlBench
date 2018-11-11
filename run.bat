@echo off
set corefx=E:\Programming\csharp7\corefx
set testhost=%corefx%\bin\testhost\netcoreapp-Windows_NT-Release-x64\shared\Microsoft.NETCore.App\9.9.9
set tools=%corefx%\Tools
if not exist "%testhost%\BenchmarkDotNet.dll" (
	xcopy /E /Q /Y deps\*.* "%testhost%\"
)
pushd %testhost%
corerun.exe "E:\Programming\csharp7\dev\SqlBench\bin\Release\netcoreapp3.0\SqlBench.dll"
popd
@echo on