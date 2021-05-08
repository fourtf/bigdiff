namespace bigdiff

module Util =
    let splitBy f input =
        let i = ref 0

        input
        |> Seq.map
            (fun x ->
                if f x then incr i
                !i, x)
        |> Seq.groupBy fst
        |> Seq.map (fun (_, b) -> Seq.map snd b)

    let startsWith (prefix: string) (s: string) = s.StartsWith(prefix)

    let (|Prefix|_|) (p: string) (s: string) =
        if s.StartsWith(p) then
            Some(s.Substring(p.Length))
        else
            None


    let truncateString (s: string) =
        if s.Length > 200 then
            s.Remove(200) + "..."
        else
            s
