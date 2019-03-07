#r "paket:
source https://api.nuget.org/v3/index.json
nuget FSharp.Core
nuget Fake.IO.FileSystem
nuget Fake.DotNet.Cli
nuget Fake.DotNet.NuGet
nuget Fake.BuildServer.Appveyor
nuget Fake.Core.Globbing
nuget Fake.Core.Target //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.IO
open Fake.Core.Globbing.Operators
open Fake.Core
open Fake.DotNet
open Fake.IO.FileSystemOperators
open Fake.DotNet.NuGet.NuGet
open Fake.BuildServer


let configuration           = Environment.environVarOrDefault "configuration"            "Debug"
let debugsymbols            = Environment.environVarOrDefault "debugsymbols"             "True"
let optimize                = Environment.environVarOrDefault "optimize"                 "False"
let targetframeworkversion  = Environment.environVarOrDefault "targetframeworkversion"   "netcoreapp2.2"
let version                 = AppVeyor.Environment.BuildVersion


Target.create "Initialize" (fun _ ->

    Trace.trace <| sprintf "working dir:            %s" __SOURCE_DIRECTORY__
    Trace.trace <| sprintf "configuration:          %s" configuration
    Trace.trace <| sprintf "debugsymbols:           %s" debugsymbols
    Trace.trace <| sprintf "targetframeworkversion  %s" targetframeworkversion
    Trace.trace <| sprintf "optimize:               %s" optimize

)


Target.create "Restore" (fun _ ->

    DotNet.restore id |> ignore

)

Target.create "Build" (fun _ ->

    let setMsBuildParams (defaults:MSBuild.CliArguments) =
        { defaults with
            Verbosity = Some(Quiet)
            Targets = ["Build"]
            Properties =
                [
                    "Optimize",         optimize
                    "DebugSymbols",     debugsymbols
                    "Configuration",    configuration
                ]
         }
    let setParams (defaults:DotNet.BuildOptions) =
        { defaults with
            MSBuildParams = setMsBuildParams defaults.MSBuildParams
        }

    DotNet.build setParams ("src" @@ "MassTransitServices.sln")

)

Target.create "CreateArtifacts" (fun _ ->

    let setNuGetParams (defaults: NuGetParams) =
        { defaults with
            Publish = false
            OutputPath = "artifacts"
            Version = version
        }

    NuGetPack setNuGetParams ("src" @@ "MassTransit.SmtpGateway" @@ "*.csproj")
    NuGetPack setNuGetParams ("src" @@ "MassTransit.SmtpGateway.Integration" @@ "*.csproj")
)

open Fake.Core.TargetOperators

"Initialize"
    ==> "Restore"
    ==> "Build"
    ==> "CreateArtifacts"

Target.runOrDefault "CreateArtifacts"