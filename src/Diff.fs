namespace bigdiff

module Diff =
    type Line =
        | Added of string
        | Removed of string
        | Unchanged of string
        | HunkInfo of string
        | MiscInfo of string

    let parseLine line =
        match line with
        | Util.Prefix "+" l -> Added line
        | Util.Prefix "-" l -> Removed line
        | Util.Prefix " " l -> Unchanged line
        | Util.Prefix "@@" l -> HunkInfo line
        | _ -> MiscInfo line

    type DiffItem =
        { title: string
          oldName: string
          newName: string
          lines: array<Line> }

    type Diff = array<DiffItem>

    let isContentLine (line: string) =
        not (line.StartsWith("---") || line.StartsWith("+++"))

    let removeGitDiffPrefix (s: string) =
        if s.StartsWith("a/") || s.StartsWith("b/") then
            s.Substring(2)
        else
            s

    let parseDiffItem (lines: seq<string>) =
        // EXAMPLE:
        // diff --git a/.CI/CreateDMG.sh b/.CI/CreateDMG.sh
        // index 8ad191e2..3eb2202c 100755
        // --- a/.CI/CreateDMG.sh
        // +++ b/.CI/CreateDMG.sh
        // @@ -1,7 +1,12 @@
        let header = Seq.filter (isContentLine >> not) lines

        // EXAMPLE:
        //  #!/bin/sh
        //
        // +if [ -d bin/chatterino.app ] && [ ! -d chatterino.app ]; then
        // +    mv bin/chatterino.app chatterino.app
        // +fi
        // +
        //  echo "Running MACDEPLOYQT"
        // -/usr/local/opt/qt/bin/macdeployqt chatterino.app
        // +$Qt5_DIR/bin/macdeployqt chatterino.app
        //  python3 -m venv venv
        let content =
            Seq.filter isContentLine lines
            |> Seq.map parseLine
            |> Array.ofSeq


        let newName =
            Seq.tryFind (Util.startsWith "+++") lines
            |> Option.map (fun s -> s.Substring(3).Trim() |> removeGitDiffPrefix)
            |> Option.defaultValue "<no new name>"

        let oldName =
            Seq.tryFind (Util.startsWith "---") lines
            |> Option.map (fun s -> s.Substring(3).Trim() |> removeGitDiffPrefix)
            |> Option.defaultValue "<no old name>"

        { title =
              if newName = oldName then
                newName
              else
                oldName + " -> " + newName
          newName = newName
          oldName = oldName
          lines = content }

    let parseDiff (lines: seq<string>) =
        Util.splitBy (fun (s: string) -> s.StartsWith("diff")) lines
        |> Seq.map (Seq.map Util.truncateString >> parseDiffItem)
        |> Array.ofSeq
