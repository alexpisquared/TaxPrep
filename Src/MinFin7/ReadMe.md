# Useful information 
## Useful information 
### Useful information 
#### Useful information 
Useful information  
Useful information  
Useful information  
Useful information  
Useful information  

Useful information  

Useful information  


dotnet new classlib -n Db.FinDemo7
cd .\Db.FinDemo7\
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet ef dbcontext scaffold "Server=.\SqlExpress;Database=FinDemo;Trusted_Connection=True;TrustServerCertificate=Yes;" Microsoft.EntityFrameworkCore.SqlServer -o Models --force
