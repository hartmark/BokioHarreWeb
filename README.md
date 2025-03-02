# BokioHarreWeb

Small MVC sample for getting data from Bokio API.
For now only showing the journal entries are supported.

## How to run

1. Clone repo
2. Go to folder BokioHarreWeb folder with terminal
3. Run these two commands to setup your secrets

```
dotnet user-secrets set "BokioToken" "the token you have created"
dotnet user-secrets set "CompanyId" "your comanyId guid"
```

4. Compile and run application
5. Go to http://localhost:5080

### OpenApi specification
https://docs.bokio.se/openapi/678f9a2f9fe26100446b55ad

### Documentation
https://docs.bokio.se/docs/getting-started-private