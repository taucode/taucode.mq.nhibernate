dotnet restore

dotnet build --configuration Debug
dotnet build --configuration Release

dotnet test -c Debug .\test\TauCode.Mq.NHibernate.Tests\TauCode.Mq.NHibernate.Tests.csproj
dotnet test -c Release .\test\TauCode.Mq.NHibernate.Tests\TauCode.Mq.NHibernate.Tests.csproj

nuget pack nuget\TauCode.Mq.NHibernate.nuspec
