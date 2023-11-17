# TODO Example

Demo of both gRPC and the [JsonTranscoding](https://learn.microsoft.com/en-us/aspnet/core/grpc/json-transcoding?view=aspnetcore-7.0#http-protocol&WT.mc_id=DX-MVP-5004571) library that allows you to surface REST endpoints alongside of gRPC.

[YouTube video](https://www.youtube.com/watch?v=Rqz9XiSqH3E)
[Github Code](https://github.com/binarythistle/S06E04-Build-a-gRPC-service-in-NET-7)

Notes
- In TodoGrpc.csproj proto files are added manually
- In appsettings.json, Protocols must be changed from Http2 to Http1AndHttp2 to support both REST and gRPC
- 