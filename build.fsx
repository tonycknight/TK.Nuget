#r "paket:
nuget Microsoft.Build 17.3.2
nuget Microsoft.Build.Framework 17.3.2
nuget Microsoft.Build.Tasks.Core 17.3.2
nuget Microsoft.Build.Utilities.Core 17.3.2
nuget Fake.IO.FileSystem
nuget Fake.DotNet.Cli
nuget Fake.DotNet.MSBuild
nuget Fake.BuildServer.GitHubActions
nuget Fake.Core.Target //"
#if !FAKE
  #load "./.fake/fakebuild.fsx/intellisense.fsx"
#endif

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.SystemHelper

let packageDir = "./package"
let publishDir = "./publish"
let mainSolution = "./Tk.Nuget.sln"

let runNumber = (match Fake.BuildServer.GitHubActions.Environment.CI false with
                    | true -> Fake.BuildServer.GitHubActions.Environment.RunNumber
                    | _ -> "0")
let commitSha = Fake.BuildServer.GitHubActions.Environment.Sha
let versionSuffix = match Fake.BuildServer.GitHubActions.Environment.Ref with
                    | null 
                    | "refs/heads/main" ->  ""
                    | _ ->                  "-preview"

let version =  sprintf "0.1.%s%s" runNumber versionSuffix
let infoVersion = match commitSha with
                    | null -> version
                    | sha -> sprintf "%s.%s" version sha                    

let assemblyInfoParams (buildParams)=
    [ ("Version", version); ("AssemblyInformationalVersion", infoVersion) ] |> List.append buildParams

let codeCoverageParams (buildParams)=
    [   ("CollectCoverage", "true"); 
        ("CoverletOutput", "./TestResults/coverage.info"); 
        ("CoverletOutputFormat", "cobertura");        
    ]  |> List.append buildParams

let packBuildParams (buildParams) =
    [ ("PackageVersion", version); ] |> List.append buildParams

let buildOptions = fun (opts: DotNet.BuildOptions) -> 
                                { opts with
                                    Configuration = DotNet.BuildConfiguration.Release;
                                    MSBuildParams = { opts.MSBuildParams with Properties = assemblyInfoParams opts.MSBuildParams.Properties; WarnAsError = Some [ "*" ]; }
                                    }
let restoreOptions = fun (opts: DotNet.RestoreOptions) -> opts

let packOptions = fun (opts: DotNet.PackOptions) -> 
                                { opts with 
                                    Configuration = DotNet.BuildConfiguration.Release; 
                                    NoBuild = false; 
                                    MSBuildParams = { opts.MSBuildParams with Properties = (packBuildParams opts.MSBuildParams.Properties |> assemblyInfoParams )};
                                    OutputPath = Some packageDir }

let publishProjects = !! "src/**/Jwt.Cli.csproj" |> List.ofSeq

Target.create "Clean" (fun _ ->
    !! "src/**/bin"
    ++ "src/**/obj"
    |> Shell.cleanDirs
    
    !! "test/**/bin"
    ++ "test/**/obj"
    ++ "test/**/TestResults"
    ++ packageDir
    ++ publishDir
    |> Shell.cleanDirs
        
    DotNet.exec id "clean" "" |> ignore
)

Target.create "Restore" (fun _ ->
    !! mainSolution
    |> Seq.iter (DotNet.restore restoreOptions)
)

Target.create "Build" (fun _ ->
    !! mainSolution
    |> Seq.iter (DotNet.build buildOptions)
)

Target.create "Pack" (fun _ -> publishProjects |> Seq.iter (DotNet.pack packOptions ) )


Target.create "SCA" (fun _ ->
    let args = "package --vulnerable --include-transitive"
    let result = DotNet.exec id "list" args
    if not result.OK then failwithf "dotnet sca failed!"      
)

Target.create "Check Style Rules" (fun _ ->
    let args = "--verify-no-changes"
    let result = DotNet.exec id "format" args
    if not result.OK then failwithf "Style rule checks failed!"      
)


Target.create "All" ignore

"Clean"
  ==> "Restore"
  ==> "Check Style Rules"
  ==> "Build"
  ==> "Pack"
  ==> "All"

"Restore"
  ==> "SCA"

"SCA"
  ==> "All"

Target.runOrDefault "All"
