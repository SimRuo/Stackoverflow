# Stack Overflow Benchmarker

Jag bygger det här projektet för att bli mer bekväm med både Dapper och Entity Framework. Målet är att mäta prestanda mellan dom båda frameworksen mot en riktig databas.
Allting kommer att köras mot en SQL server med StackOverflows databas från 2013 (https://www.brentozar.com/archive/2015/10/how-to-download-the-stack-overflow-database-via-bittorrent/)

### Tech Stack

- .NET Web API
- Entity Framework Core
- Dapper
- Microsoft SQL Server 2022 (Körs i en Docker)
- Stack Overflow 2013 Dataset

### Steg för steg

Jag känner att det var väldigt många steg för att komma igång med det här så jag dokumenterar själva setupen här, mest för min egen skull så att jag kommer ihåg vad jag gjorde

- Steg 1 \*

Laddar ner och spinner igång en docker instans med MSSQL

```
sudo docker run -d --name sqlserver \
 --restart unless-stopped \
 -e "ACCEPT_EULA=Y" \
 -e "SA_PASSWORD=YourStrongPassword123!" \
 -p 1433:1433 \
 -v mssql_data:/var/opt/mssql \
 mcr.microsoft.com/mssql/server:2022-latest
```

För att kolla vilka docker instanser som körs och för att starta docker instansen (som det är upplagt just nu startar den inte tillsammans med systemet)

```
sudo docker ps -a

sudo docker start sqlserver
```

- Steg 2 \*

Laddar ner databasen från länken högre upp. In i rätt sökväg och skicka över filerna till docker instansen.

```
sudo docker cp StackOverflow2013_1.mdf sqlserver:/var/opt/mssql/data/
sudo docker cp StackOverflow2013_2.ndf sqlserver:/var/opt/mssql/data/
sudo docker cp StackOverflow2013_3.ndf sqlserver:/var/opt/mssql/data/
sudo docker cp StackOverflow2013_4.ndf sqlserver:/var/opt/mssql/data/
sudo docker cp StackOverflow2013_log.ldf sqlserver:/var/opt/mssql/data/
```

Fixar lite rättigheter

```
sudo docker exec -u 0 -it sqlserver bash -c "\
 chmod 664 /var/opt/mssql/data/StackOverflow2013_* && \
 chown 10001:0 /var/opt/mssql/data/StackOverflow2013_*"
```

Skapar databasen från filerna som vi precis skickat över till docker instansen

```
sudo docker exec -it sqlserver /opt/mssql-tools18/bin/sqlcmd \
 -S localhost -U sa -P "YourStrongPassword123!" -No -Q "
CREATE DATABASE StackOverflow2013
ON
(FILENAME = '/var/opt/mssql/data/StackOverflow2013_1.mdf'),
(FILENAME = '/var/opt/mssql/data/StackOverflow2013_2.ndf'),
(FILENAME = '/var/opt/mssql/data/StackOverflow2013_3.ndf'),
(FILENAME = '/var/opt/mssql/data/StackOverflow2013_4.ndf'),
(FILENAME = '/var/opt/mssql/data/StackOverflow2013_log.ldf')
FOR ATTACH;"
```

- Steg 3 \*

Skapa .NET API

```
dotnet new webapi
```

- Steg 4 \*

Lägger till alla paket som vi kommer behöva

```
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Dapper
dotnet tool install --global dotnet-ef
```

- Steg 5 \*

Lägger till våran connection string så att vi kan kommunicera med databasen (appsettings.json)

```
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=StackOverflow2013;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;"
}

```

- Steg 6 \*

Generera mappar, klasser, filer baserat på våran databas:

```
dotnet ef dbcontext scaffold \
'Server=localhost;Database=StackOverflow2013;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;' \
Microsoft.EntityFrameworkCore.SqlServer \
--output-dir Data --context AppDbContext --no-onconfiguring --use-database-names

```

Det där skapar strukturen som vi vill ha

```
/Controllers
  BenchmarkController.cs      # Runs EF vs Dapper tests
/Repositories
  IPostRepository.cs          # Contract
  EfPostRepository.cs         # Uses AppDbContext (EF)
  DapperPostRepository.cs     # Uses SqlConnection (Dapper)
Data/
  AppDbContext.cs
  <entity>.cs
```
