# How to Store User Secrets

In dev environment, use User Secrets to store connection strings.

Create user secret for default connection string:
> dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=tcp:abc.com; Initial Catalog=abc; Persist Security Info=False; User ID=xyz; Password=abc; MultipleActiveResultSets=False; TrustServerCertificate=False; Connection Timeout=30;"

The user secrets are stored in the following file:
> C:\Users\\[username]\AppData\Roaming\Microsoft\UserSecrets\\[Guid]\secrets.json

In Staging and Production, add the connection strings with variables in DevOps.