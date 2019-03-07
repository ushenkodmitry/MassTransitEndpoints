#r "paket:
source https://api.nuget.org/v3/index.json
nuget FSharp.Core
nuget Fake.IO.FileSystem
nuget Fake.DotNet.Cli
nuget Fake.Core.Globbing
nuget Fake.Core.Target //"

open Fake.IO
open Fake.Core.Globbing.Operators
open Fake.Core
open Fake.DotNet
open Fake.IO.FileSystemOperators

let configuration           = Environment.environVarOrDefault "configuration"            "Debug"
let debugsymbols            = Environment.environVarOrDefault "debugsymbols"             "True"
let optimize                = Environment.environVarOrDefault "optimize"                 "False"
let targetframeworkversion  = Environment.environVarOrDefault "targetframeworkversion"   "netcoreapp2.2"


Target.create "Initialize" (fun _ ->

    Trace.trace <| sprintf "working dir:            %s" __SOURCE_DIRECTORY__
    Trace.trace <| sprintf "configuration:          %s" configuration
    Trace.trace <| sprintf "debugsymbols:           %s" debugsymbols
    Trace.trace <| sprintf "targetframeworkversion  %s" targetframeworkversion
    Trace.trace <| sprintf "optimize:               %s" optimize
    Trace.trace <| sprintf "solution:               %s" solution

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

open Fake.Core.TargetOperators

"Initialize"
    ==> "Restore"
    ==> "Build"

Target.runOrDefault "Build"