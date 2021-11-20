module Ubictionary.LanguageServer.Hover

open System.Threading.Tasks
open OmniSharp.Extensions.LanguageServer.Protocol
open OmniSharp.Extensions.LanguageServer.Protocol.Models
open OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities

let private markupContent content = 
    MarkedStringsOrMarkupContent(markupContent = MarkupContent(Kind = MarkupKind.Markdown, Value = content))

let private noHoverResult = Task.FromResult(Hover())

let private termMatches _ = true
let private termToString (t:Definitions.Term) = t.name

let handler (termFinder: Definitions.Finder) (p:HoverParams) (hc:HoverCapability) _ = 
    let terms = termFinder termMatches termToString
    match Seq.isEmpty terms with
    | true -> noHoverResult
    | _ -> Task.FromResult(Hover(Contents = new MarkedStringsOrMarkupContent(MarkupContent(Kind = MarkupKind.Markdown, Value = sprintf "### %s" (Seq.head terms)))))

let private registrationOptionsProvider (hc:HoverCapability) (cc:ClientCapabilities)  =
    HoverRegistrationOptions()

let registrationOptions = RegistrationOptionsDelegate<HoverRegistrationOptions, HoverCapability>(registrationOptionsProvider)