
@ECHO OFF

REM Requirements :
REM  .NET Framework 3.5
REM  Sandcastle Help File Builder : http://shfb.codeplex.com/

"C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" /p:Configuration=Release /p:TargetFrameworkVersion=v3.5 /p:SignAssembly=true "SeleniumWrapper.csproj" 


pause
exit
