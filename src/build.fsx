#r "paket:
nuget Fake.Core.Target //"

open Fake.Core

// Default target
Target.create "Default" (fun _ ->
  Trace.trace "Hello World from FAKE"
)

// start build
Target.runOrDefault "Default"