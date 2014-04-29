open System
open System.IO
open System.Text
open System.Web

let postsDir = "_posts"
let newPostExt = "md"
let newPageExt = "md"
let args = fsi.CommandLineArgs

if (args.Length < 2) then failwith "Task is missing"

let getUserInput (message : string) = 
    Console.WriteLine message
    Console.ReadLine()

let appendLine value (builder : StringBuilder) = builder.AppendLine value
let writeToFile path contents = File.WriteAllText(path, contents)
let toUrl (input : string) = input.ToLower().Replace(' ', '-')

let rec getYesOrNo message = 
    let input = getUserInput message
    match input.ToUpper() with
    | "Y" | "N" -> input
    | _ -> getYesOrNo message

let checkIfFileExists path = 
    if File.Exists path then 
        if getYesOrNo (sprintf "%s exists, override? [Y/N]" path) = "N" then failwith "Aborted by user"

/// Creates a new post, usage: fsi create.fsx post
let newPost title = 
    let path = sprintf "%s/%s-%s.%s" postsDir (DateTime.Now.ToString("yyyy-MM-dd")) (toUrl title) newPostExt
    checkIfFileExists path
    let category = getUserInput "Enter category or blank for none:"
    let tags = getUserInput "Enter tags, comma separated:"
    printfn "Creating new post: %s" path
    appendLine "---" (new StringBuilder())
    |> appendLine "layout: post"
    |> appendLine (sprintf "title: %s" (HttpUtility.HtmlEncode title))
    |> appendLine (sprintf "modified: %s" (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz")))
    |> appendLine (sprintf "category: %s" category)
    |> appendLine (sprintf "tags: [%s]" tags)
    |> appendLine "image:"
    |> appendLine "  feature: "
    |> appendLine "  credit: "
    |> appendLine "  creditlink: "
    |> appendLine "comments:"
    |> appendLine "share:"
    |> appendLine "---"
    |> string
    |> writeToFile path

/// Creates a new page, usage: fsi create.fsx page
let newPage title = 
    let path = sprintf "%s.%s" (toUrl title) newPostExt
    checkIfFileExists path
    let tags = getUserInput "Enter tags, comma separated:"
    printfn "Creating new page: %s" path
    appendLine "---" (new StringBuilder())
    |> appendLine "layout: page"
    |> appendLine (sprintf "permalink: /%s/" (toUrl title))
    |> appendLine (sprintf "title: %s" (HttpUtility.HtmlEncode title))
    |> appendLine (sprintf "modified: %s" (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz")))
    |> appendLine (sprintf "tags: [%s]" tags)
    |> appendLine "image:"
    |> appendLine "  feature: "
    |> appendLine "  credit: "
    |> appendLine "  creditlink: "
    |> appendLine "share:"
    |> appendLine "---"
    |> string
    |> writeToFile path

match args.[1] with
| "post" -> newPost (getUserInput "Please enter a post title:")
| "page" -> newPage (getUserInput "Please enter a page title:")
| x -> failwith (sprintf "Unknown task: %s" x)
