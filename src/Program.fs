namespace bigdiff

open Elmish
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Input
open Avalonia.FuncUI
open Avalonia.FuncUI.Elmish
open Avalonia.FuncUI.Components.Hosts
open System

module Program =
    let rec readlines () =
        seq {
            let line = Console.ReadLine()

            if line <> null then
                yield line
                yield! readlines ()
        }

    type MainWindow() as this =
        inherit HostWindow()

        do
            base.Title <- "bigdiff"
            base.Width <- 1600.0
            base.Height <- 1000.0

            let lines = readlines () |> Array.ofSeq

            let diff = Diff.parseDiff lines

            //this.VisualRoot.VisualRoot.Renderer.DrawFps <- true
            //this.VisualRoot.VisualRoot.Renderer.DrawDirtyRects <- true

            Elmish.Program.mkSimple (fun () -> DiffViewer.init diff) DiffViewer.update DiffViewer.view
            |> Program.withHost this
            |> Program.run


    type App() =
        inherit Application()

        override this.Initialize() =
            this.Styles.Load "avares://Avalonia.Themes.Default/DefaultTheme.xaml"
            this.Styles.Load "avares://Avalonia.Themes.Default/Accents/BaseDark.xaml"

        override this.OnFrameworkInitializationCompleted() =
            match this.ApplicationLifetime with
            | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
                desktopLifetime.MainWindow <- MainWindow()
            | _ -> ()


    [<EntryPoint>]
    let main (args: string []) =
        if Array.tryHead args = Some "--version" then
            Console.WriteLine "bigdiff v1.1"
            0
        else
            AppBuilder
                .Configure<App>()
                .UsePlatformDetect()
                .UseSkia()
                .StartWithClassicDesktopLifetime(args)
