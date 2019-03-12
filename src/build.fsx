#r "paket:
source https://api.nuget.org/v3/index.json
nuget FSharp.Core                           4.5.0.0
nuget Fake.IO.FileSystem
nuget Fake.DotNet.Cli
nuget Fake.DotNet.NuGet
nuget Fake.BuildServer.Appveyor
nuget Fake.Core.Globbing
nuget Fake.Core.Target //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.IO
open Fake.Core
open Fake.DotNet
open Fake.IO.FileSystemOperators
open Fake.DotNet.NuGet.NuGet
open Fake.BuildServer
open Fake.IO.Globbing.Operators;


let configuration           = AppVeyor.Environment.Configuration
let version                 = AppVeyor.Environment.BuildVersion
let debugsymbols            = Environment.environVarOrDefault "debugsymbols"             "False"
let optimize                = Environment.environVarOrDefault "optimize"                 "True"
let targetframeworkversion  = Environment.environVarOrDefault "targetframeworkversion"   "netstandard2.0"


let shouldPublish = AppVeyor.Environment.RepoBranch = "master"

let nugetworking = "nugetworking"
let artifacts = "artifacts"


Target.create "Initialize" (fun _ ->

    Trace.trace <| sprintf "source dir:             %s" __SOURCE_DIRECTORY__
    Trace.trace <| sprintf "configuration:          %s" configuration
    Trace.trace <| sprintf "debugsymbols:           %s" debugsymbols
    Trace.trace <| sprintf "targetframeworkversion  %s" targetframeworkversion
    Trace.trace <| sprintf "optimize:               %s" optimize
    Trace.trace <| sprintf "artifacts dir:          %s" artifacts

    Directory.ensure artifacts

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

Target.create "CreateSmtpGatewayArtifacts" (fun _ ->

    let workingdir = (nugetworking @@ "smtpgateway")

    Directory.ensure workingdir

    !! ("src" @@ "MassTransit.SmtpGateway" @@ "bin" @@ configuration @@ targetframeworkversion @@ "MassTransit.SmtpGateway.dll")
    ++ ("src" @@ "MassTransit.SmtpGateway" @@ "bin" @@ configuration @@ targetframeworkversion @@ "MassTransit.SmtpGateway.xml")
        |> Shell.copy workingdir
    

    let setNuGetParams (defaults: NuGetParams) =
        { defaults with
            Project = "MassTransit.SmtpGateway"
            Publish = false
            Description = "Smtp service for MassTransit"
            Authors = ["Ushenko Dmitry"]
            OutputPath = artifacts
            Version = version
            WorkingDir = workingdir
            Tags = "MassTransit Smtp"
            Properties = 
                [
                    "Configuration", configuration
                ]
            DependenciesByFramework = 
                [
                    { 
                        FrameworkVersion = "netstandard2.0"
                        Dependencies = 
                        [
                            "MassTransit", "5.3.2"
                            "MailKit", "2.1.3"
                        ]
                    }
                ]
        }

    NuGetPack setNuGetParams "package.nuspec"
)

Target.create "CreateSmtpGatewayIntegrationArtifacts" (fun _ ->

    let workingdir = (nugetworking @@ "smtpgatewayintegration")

    Directory.ensure workingdir

    !! ("src" @@ "MassTransit.SmtpGateway" @@ "bin" @@ configuration @@ targetframeworkversion @@ "MassTransit.SmtpGateway.Integration.dll")
    ++ ("src" @@ "MassTransit.SmtpGateway" @@ "bin" @@ configuration @@ targetframeworkversion @@ "MassTransit.SmtpGateway.Integration.xml")
        |> Shell.copy workingdir

    let setNuGetParams (defaults: NuGetParams) =
        { defaults with
            Project = "MassTransit.SmtpGateway.Integration"
            Publish = false
            Description = "Integration package for SmtpGateway"
            Authors = ["Ushenko Dmitry"]
            OutputPath = artifacts
            Version = version
            WorkingDir = workingdir
            Tags = "MassTransit Smtp"
            Properties = 
                [
                    "Configuration", configuration
                ]
            DependenciesByFramework = 
                [
                    { 
                        FrameworkVersion = "netstandard2.0"
                        Dependencies = 
                        [
                            "MassTransit", "5.3.2"
                        ]
                    }
                ]
        }

    NuGetPack setNuGetParams "package.nuspec"
)

Target.create "Publish" (fun _ ->

    Trace.log "Publishing..."

)

Target.createFinal "Finalize" (fun _ ->

    Directory.delete nugetworking

)

open Fake.Core.TargetOperators

"Initialize"
    ==> "Restore"
    ==> "Build"
    ==> "CreateSmtpGatewayArtifacts"
    ==> "CreateSmtpGatewayIntegrationArtifacts"
    =?> ("Publish",                         shouldPublish)                                   
    ==> "Finalize"

Target.runOrDefault "Finalize"