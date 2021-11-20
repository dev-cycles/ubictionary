module Ubictionary.LanguageServer.Tests.HoverTests

open Expecto
open Swensen.Unquote
open OmniSharp.Extensions.LanguageServer.Protocol
open OmniSharp.Extensions.LanguageServer.Protocol.Models
open OmniSharp.Extensions.LanguageServer.Protocol.Document
open TestClient
open System.IO

[<Tests>]
let hoverTests =
    testList "Hover Tests" [
        testAsync "Given no ubictionary and no document sync, server response to hover request with empty result" {
            use! client = SimpleTestClient |> init

            let hoverParams = HoverParams()

            let! hover = client.TextDocument.RequestHover(hoverParams) |> Async.AwaitTask

            test <@ not hover.Contents.HasMarkupContent @>
        }

        testAsync "Given ubictionary and document sync, server response to hover request in default position" {
            let fileName = "one"
            let config = [
                    Workspace.optionsBuilder <| Path.Combine("fixtures", "completion_tests")
                    ConfigurationSection.ubictionaryPathOptionsBuilder $"{fileName}.yml"
                ]

            use! client = TestClient(config) |> init

            let textDocumentUri = $"file:///{System.Guid.NewGuid().ToString()}"

            client.TextDocument.DidOpenTextDocument(DidOpenTextDocumentParams(TextDocument = TextDocumentItem(
                LanguageId = "plaintext",
                Version = 0,
                Text = "secondTerm",
                Uri = textDocumentUri
            )))

            let hoverParams = HoverParams(
                TextDocument = textDocumentUri,
                Position = Position(0, 0)
            )

            let! hover = client.TextDocument.RequestHover(hoverParams) |> Async.AwaitTask

            test <@ hover.Contents.HasMarkupContent @>
            test <@ hover.Contents.MarkupContent.Kind = MarkupKind.Markdown @>
            test <@ hover.Contents.MarkupContent.Value.Contains("secondTerm") @>
        }

        testAsync "Given ubictionary and document sync, server response to hover request in defined position" {
            let fileName = "one"
            let config = [
                    Workspace.optionsBuilder <| Path.Combine("fixtures", "completion_tests")
                    ConfigurationSection.ubictionaryPathOptionsBuilder $"{fileName}.yml"
                ]

            use! client = TestClient(config) |> init

            let textDocumentUri = $"file:///{System.Guid.NewGuid().ToString()}"

            client.TextDocument.DidOpenTextDocument(DidOpenTextDocumentParams(TextDocument = TextDocumentItem(
                LanguageId = "plaintext",
                Version = 0,
                Text = "secondTerm thirdTerm",
                Uri = textDocumentUri
            )))

            let hoverParams = HoverParams(
                TextDocument = textDocumentUri,
                Position = Position(0, 12)
            )

            let! hover = client.TextDocument.RequestHover(hoverParams) |> Async.AwaitTask

            test <@ hover.Contents.HasMarkupContent @>
            test <@ hover.Contents.MarkupContent.Kind = MarkupKind.Markdown @>
            test <@ hover.Contents.MarkupContent.Value.Contains("thirdTerm") @>
        }
    ]