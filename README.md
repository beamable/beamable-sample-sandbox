# beamable-sample-sandbox
Public repository for sharing Beamable sample code for various purposes and in various states of completeness or usability.

# Prerequisites

Version: Beamable 1.19.20 and Unity 2022.3.22f1

# Expected Behavior

*Please describe the behavior you are expecting:*

 Create an instance of HarvestResponseDto and return it

# Current Behavior

*What is the current behavior?*

Got this error when tried to call a microservice API.
Error: "NullReferenceException: Object reference not set to an instance of an object."

# Failure Information (for bugs)

Please help provide information about the failure if this is a bug. If it is not a bug, please remove the rest of this template.

NullReferenceException

## Steps to Reproduce

Please provide detailed steps for reproducing the issue.

1. Implement the server code
2. Implement the client code
3. Run the project

## Context

Please provide any relevant information about your setup. This is important in case the issue is not reproducible except for under certain conditions.

* Device:
* Operating System:
* Browser and Version:

## Failure Logs

Please include any relevant log snippets or files here.

Exception Source: beamable.tooling.common
Stack Trace:
   at Beamable.Server.DefaultResponseSerializer.SerializeResponse(RequestContext ctx, Object result) in /Client/microservice/beamable.tooling.common/Microservice/ResponseSerializer.cs:line 101
   at Beamable.Server.ServiceMethodCollection.Handle(RequestContext ctx, String path, IParameterProvider parameterProvider) in /Client/microservice/beamable.tooling.common/Microservice/ServiceMethodCollection.cs:line 56
   at Beamable.Server.BeamableMicroService.HandleClientMessage(MicroserviceRequestContext ctx, Stopwatch sw) in /src/dbmicroservice/BeamableMicroService.cs:line 610
