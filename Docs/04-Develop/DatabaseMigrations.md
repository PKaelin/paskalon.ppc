# Database Migrations
How the PostgreSQL databases of the microservices are created and updated with Entity Framework Core migrations (`dotnet ef`).


## Solutions With Migrations
The migration classes are stored in `Storage/Migrations` of the infrastructure project. The ports and database names come from [InfrastructureInternal/data.yaml](/InfrastructureInternal/data.yaml).

| Solution | Infrastructure project (`--project`) | Startup project (`--startup-project`) | Port | Database |
|----------|--------------------------------------|---------------------------------------|------|----------|
| paskalON.DeviceService | paskalON.Devices.Infrastructure | paskalON.Devices.Service | 45001 | devices |
| paskalON.PowerControls | paskalON.PowerControls.Infrastructure | paskalON.PowerControls.Service | 45011 | powercontrols |
| paskalON.OperatingModes | paskalON.OperatingModes.Infrastructure | paskalON.OperatingModes.Service | 45021 | operatingmodes |
| paskalON.Alarms | paskalON.Alarms.Infrastructure | paskalON.Alarms.Service | 45031 | alarms |

The commands below use `[Infrastructure]`, `[Service]`, `[Port]` and `[Database]` as placeholders for the values in the table. `[User]` and `[Password]` are the development credentials in [InfrastructureInternal/secrets](/InfrastructureInternal/secrets/README.md).


## Tool Installation
```
dotnet tool install --global dotnet-ef
```
*Remarks:*  
*Careful with other nuget package sources, could result in unauthorized 401 if no credential provider is installed.*  
*Check: C:\Users\[username]\AppData\Local\NuGet\CredentialProviders or temporarily disable Nuget.config ->  <packageSources> -> <add key="MyCompany" value="https://....*


## Create Migration Classes
Open command line and change directory to the solution directory.
```
dotnet ef migrations add [MigrationName] --project ./[Infrastructure] --startup-project ./[Service] --output-dir "Storage/Migrations"
```
Creates a snapshot and the migration classes in `./[Infrastructure]/Storage/Migrations`.

- [MigrationName] naming: `v_[Major]_[Minor]`
- Every database schema change is a minor version change!


## Update Migration Classes
When a new service database or a new table is added, functions/audit triggers have to be created by adding those in the created migration classes.
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


## Create Or Update The Database
Open command line and change directory to the solution directory.

Update to the latest migration:
```
dotnet ef database update --project ./[Infrastructure] --startup-project ./[Service] --connection "Host=localhost:[Port];Username=[User];Password=[Password];Database=[Database]"
```
Update to a specific migration:
```
dotnet ef database update [MigrationName] --project ./[Infrastructure] --startup-project ./[Service] --connection "Host=localhost:[Port];Username=[User];Password=[Password];Database=[Database]"
```
Unapply all migrations:
```
dotnet ef database update 0 --project ./[Infrastructure] --startup-project ./[Service] --connection "Host=localhost:[Port];Username=[User];Password=[Password];Database=[Database]"
```


## Repository Documentation
- [Development Overview](./Overview.md)
- [Repository README](/README.md)
