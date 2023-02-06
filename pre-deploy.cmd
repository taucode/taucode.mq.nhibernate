dotnet restore

dotnet build TauCode.Mq.NHibernate.sln -c Debug
dotnet build TauCode.Mq.NHibernate.sln -c Release

dotnet test TauCode.Mq.NHibernate.sln -c Debug
dotnet test TauCode.Mq.NHibernate.sln -c Release

nuget pack nuget\TauCode.Mq.NHibernate.nuspec