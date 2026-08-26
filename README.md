# MoopelFrontend

## Runtime configuration

The hosted WebAssembly client loads its public deployment configuration from
`/app-config.json` before registering application services. The ASP.NET Core
host generates this response from its configuration, so one published artifact
or Docker image can be used in every environment.

Supply deployment values when the container starts:

```powershell
docker run --rm -p 8080:8080 `
  -e "Environment=Production" `
  -e "MoopelApiOptions__BaseUrl=https://api.example.com/" `
  moopelfrontend:latest
```

`Environment` must be `Test`, `Development`, or `Production`, and
`MoopelApiOptions__BaseUrl` must be an absolute URL. Values returned by
`/app-config.json` are visible to every browser and must never contain secrets.