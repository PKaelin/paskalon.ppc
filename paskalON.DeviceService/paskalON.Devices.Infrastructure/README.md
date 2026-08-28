# Devices Infrastructure
This layer provides the necessary services and infrastructure for the application layer, such as databases, message queues, and web servers. 


## Database creation and schema updates
Is all done by using the tool ``dotnet ef``


### Tool installation
```
dotnet tool install --global dotnet-ef
```
*Remarks:*  
*Careful with other nuget package sources, could result in unauthorized 401 if no credential provider is installed.*  
*Check: C:\Users\[username]\AppData\Local\NuGet\CredentialProviders or temporarily disable Nuget.config ->  <packageSources> -> <add key="MyCompany" value="https://....*


### Create migrations classes
Open command line and change directory to the solution directory.
```
dotnet ef migrations add [MigrationName] --project ./paskalON.Devices.Infrastructure --startup-project ./paskalON.Devices.Service --output-dir "Storage/Migrations"
```
Creates a snapshot and the migration classes in ./paskalON.Devices.Infrastructure/Storage/Migrations
[MigrationName] naming: v_[Major]_[Minor]
Every database schema change is a minor version change!


### Update migration classes
When a new service database or a new table is added an functions/audit triggers have to be created by adding those in the created migration classes.
E.g. 
```
public partial class InitialCreate : Migration
{
   . . . 
   protected override void Up(MigrationBuilder migrationBuilder){ . . . CreateAudit(migrationBuilder); . . .}
   private void CreateAudit(MigrationBuilder migrationBuilder) { . . . migrationBuilder.Sql(@"xxxxxx");" . . .}
   . . . 
}
```


### Create update database
Open command line and change directory to the solution directory.
Create update to latest:
```
dotnet ef database update --project ./paskalON.Devices.Infrastructure --startup-project ./paskalON.Devices.Service --connection "Host=localhost:45001;Username=admin;Password=admin123;Database=operatingmodes"
```
Create update to specific:
```
dotnet ef database update [MigrationName] --project ./paskalON.Devices.Infrastructure --startup-project ./paskalON.Devices.Service --connection "Host=localhost:45001;Username=admin;Password=admin123;Database=operatingmodes"
```
To unapply all migrations:
```
dotnet ef database update 0 --project ./paskalON.Devices.Infrastructure --startup-project ./paskalON.Devices.Service --connection "Host=localhost:45001;Username=admin;Password=admin123;Database=operatingmodes"
```
