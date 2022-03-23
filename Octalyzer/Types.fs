module Octalyzer.Types

open System
open System.Text.Json.Serialization

type Interval = {
    Start: DateTimeOffset
    End: DateTimeOffset
}

type ConsumptionDto = {
    [<JsonPropertyName("consumption")>]
    Amount: decimal
    [<JsonPropertyName("interval_start")>]
    Start: DateTimeOffset
    [<JsonPropertyName("interval_end")>]
    End: DateTimeOffset
}

type CollectionResponse<'T> = {
    count: int;
    next: string;
    previous: string
    results: ConsumptionDto seq
}

type MPXN = | Mpan of string | Mprn of string

type Meter = {
    Mpxn: MPXN;
    Serial: string
}

type ConsumptionOrder = Forward | Reverse
type ConsumptionGroup = HalfHour | Hour | Day | Week | Month | Quarter

type QueryOption =
    | PeriodTo of DateTimeOffset
    | PeriodFrom of DateTimeOffset
    | PageSize of int
    | ConsumptionOrder of ConsumptionOrder
    | ConsumptionGroup of ConsumptionGroup

//type ConsumptionRequestScope = {
//    PeriodFrom: DateTimeOffset option
//    PeriodTo: DateTimeOffset option
//    PageSize: int option
//    OrderBy: ConsumptionOrder option
//    GroupBy: ConsumptionGroup option
//}
