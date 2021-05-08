namespace bigdiff

open Avalonia.Media

module DiffViewer =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.FuncUI.Types
    open Diff

    let internal fgAdded = Brush.Parse("#6EE777")
    let internal fgRemoved = Brush.Parse("#F88181")
    let internal fgUnchanged = Brush.Parse("#F3F4F6")
    let internal fgHunkInfo = Brush.Parse("#A78BFA")
    let internal fgMiscInfo = Brush.Parse("#999999")
    let internal bgSearch = Brush.Parse("#333333")
    let internal bgList = Brush.Parse("#444444")

    let internal fontFamily =
        FontFamily.Parse("Consolas, Courier New, Courier, Liberation Mono, monospace")

    let internal fontSize : double = 14.0

    type State =
        { count: int
          diff: Diff
          currentItem: DiffItem option
          searchTerm: string }

    let init (diff2: Diff) =
        { count = 0
          diff = diff2
          currentItem = None
          searchTerm = "" }

    type Msg =
        | SelectionIndexChanged of int
        | SelectItem of DiffItem
        | ChangeSearchTerm of string

    let filterItems (searchTerm: string) = 
        Seq.ofArray
        >> Seq.filter (fun (d: DiffItem) -> d.title.Contains(searchTerm))

    let update (msg: Msg) (state: State) : State =
        match msg with
        | SelectionIndexChanged index -> {
                state with currentItem =
                               state.diff
                               |> filterItems state.searchTerm
                               |> Seq.tryItem index
            }
        | SelectItem item -> { state with currentItem = Some item }
        | ChangeSearchTerm term -> { state with searchTerm = term }

    let viewItem (item: DiffItem option) : IView =
        let viewLine (line: Line) : IView =
            match line with
            | Added l ->
                upcast TextBlock.create [ TextBlock.text l
                                          TextBlock.fontFamily fontFamily
                                          TextBlock.fontSize fontSize
                                          TextBlock.foreground fgAdded ]
            | Removed l ->
                upcast TextBlock.create [ TextBlock.text l
                                          TextBlock.fontFamily fontFamily
                                          TextBlock.fontSize fontSize
                                          TextBlock.foreground fgRemoved ]
            | Unchanged l ->
                upcast TextBlock.create [ TextBlock.text l
                                          TextBlock.fontFamily fontFamily
                                          TextBlock.fontSize fontSize
                                          TextBlock.foreground fgUnchanged ]
            | HunkInfo l ->
                upcast TextBlock.create [ TextBlock.text l
                                          TextBlock.fontFamily fontFamily
                                          TextBlock.fontSize fontSize
                                          TextBlock.foreground fgHunkInfo ]
            | MiscInfo l ->
                upcast TextBlock.create [ TextBlock.text l
                                          TextBlock.fontFamily fontFamily
                                          TextBlock.fontSize fontSize
                                          TextBlock.foreground fgMiscInfo ]

        let viewLines (lines: Line array) : IView =
            upcast StackPanel.create [
                StackPanel.margin (new Avalonia.Thickness(16.0, 16.0, 0.0, 16.0))
                StackPanel.children (Array.map viewLine lines |> List.ofArray) ]

        item
        |> Option.map (fun i -> (upcast ScrollViewer.create [
                ScrollViewer.content (viewLines i.lines)
            ]: IView))
        |> Option.defaultWith (fun () -> upcast TextBlock.create [ TextBlock.text "select an item" ])


    let viewItemSelection (diff: Diff) (searchTerm: string) dispatch : IView =
        let viewButton item : IView =
            upcast ListBoxItem.create [
                ListBoxItem.padding 10.0
                ListBoxItem.fontSize fontSize
                ListBoxItem.content (
                    TextBlock.create [TextBlock.text item.title]
                )
            ]

        upcast
            DockPanel.create [
                DockPanel.background bgSearch
                DockPanel.width 400.0

                DockPanel.children [
                    TextBox.create [
                        TextBox.dock Dock.Top
                        TextBox.fontSize fontSize
                        TextBox.text searchTerm
                        TextBox.watermark "Search"
                        TextBox.margin 16.0
                        TextBox.onTextChanged (ChangeSearchTerm >> dispatch)
                    ]

                    ListBox.create [
                        ListBox.dock Dock.Left
                        ListBox.virtualizationMode ItemVirtualizationMode.None
                        ListBox.borderThickness 0.0
                        ListBox.onSelectedIndexChanged (SelectionIndexChanged >> dispatch)
                        ListBox.background bgList

                        ListBox.viewItems (
                            diff
                            |> filterItems searchTerm
                            |> Seq.map viewButton
                            |> List.ofSeq
                        )
                    ]
                ]
            ]


    let view (state: State) (dispatch) =
        // TextEditor.create [TextEditor.document state.document; TextEditor.background (Brush.Parse "#ff00ff")]

        DockPanel.create [
            DockPanel.children [
                viewItemSelection state.diff state.searchTerm dispatch
                viewItem state.currentItem ] ]
