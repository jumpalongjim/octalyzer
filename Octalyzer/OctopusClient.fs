module Octopus.Client
open Octalyzer.Types

open System
open System.Text.Json
open System.Net.Http
open System.Net.Http.Headers

[<Literal>]
let BaseUrl = "https://api.octopus.energy/"
let dateFormatString = "yyyy-MM-ddTHH:mm:ssZ"

let BuildQueryStringClause option =
    match option with
    | PeriodFrom pf -> $"period_from={pf.ToString(dateFormatString)}"
    | PeriodTo pt -> $"period_to={pt.ToString(dateFormatString)}"
    | ConsumptionGroup group -> $"group_by={group.ToString().ToLowerInvariant()}"
    | ConsumptionOrder order -> $"""order_by={match order with |Forward -> "period" | Reverse -> "-period"}"""
    | PageSize size -> $"page_size={size}"

let BuildQueryString options =
    String.Join("&", Seq.map BuildQueryStringClause options)

let MeterSegment meter =
    match meter with
    | Mpan m -> $"electricity-meter-points/{m}"
    | Mprn m -> $"gas-meter-points/{m}"

let BuildUri options meter =
    $"{BaseUrl}v1/{MeterSegment meter.Mpxn}/meters/{meter.Serial}/consumption/?{BuildQueryString options}"

let ReadConsumption (apiKey:string) meter scope =
    async {
        let uri = BuildUri scope meter
        let client = new HttpClient()
        let credentials = Convert.ToBase64String(Text.ASCIIEncoding.UTF8.GetBytes(apiKey + ":"))
        client.DefaultRequestHeaders.Authorization <- new AuthenticationHeaderValue("Basic", credentials)
        try
            let! response = client.GetAsync(uri) |> Async.AwaitTask
            let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            match response.IsSuccessStatusCode with
            | true -> return Ok (JsonSerializer.Deserialize<CollectionResponse<ConsumptionDto>>(content))
            | _ -> return Error $"Request: {uri}\r\nResponse: {content}"
        with
        | :? HttpRequestException as error -> return Error error.Message
        | error -> return Error error.InnerException.Message
    }

let ReadMeterConsumption (apiKey:string) meter =
    ReadConsumption apiKey meter
