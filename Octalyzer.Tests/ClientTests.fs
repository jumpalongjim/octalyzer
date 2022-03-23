namespace Octalyzer.Tests

open Xunit
//open Amazon.Lambda.TestUtilities
//open Amazon.Lambda.APIGatewayEvents

open Octalyzer.Types
open Octopus.Client
open System.Text.Json
open Swensen.Unquote
open System


module ClientTests =

    let consumptionResponse = """{
    "count": 48,
    "next": null,
    "previous": null,
    "results": [
        {
            "consumption": 0.063,
            "interval_start": "2018-05-19T00:30:00Z",
            "interval_end": "2018-05-19T01:00:00Z"
        },
        {
            "consumption": 0.071,
            "interval_start": "2018-05-19T00:00:00Z",
            "interval_end": "2018-05-19T00:30:00Z"
        },
        {
            "consumption": 0.073,
            "interval_start": "2018-05-18T23:30:00Z",
            "interval_end": "2018-05-18T00:00:00Z"
        }
    ]
    }"""

    [<Fact>]
    let ``Consumption response can be deserialized``() =
        let consumption = JsonSerializer.Deserialize<CollectionResponse<ConsumptionDto>>(consumptionResponse)
        test <@ consumption.count = 48 @>
        test <@ consumption.next = null @>
        test <@ Seq.length consumption.results = 3 @>
        let aConsumption = Seq.head consumption.results
        test <@ aConsumption.Amount = 0.063m @>
        test <@ aConsumption.Start.Date = DateTime.Parse("2018-05-19") @>

    [<Fact>]
    let ``Query clause formats period_from``() =
        let option = PeriodFrom(DateTimeOffset.Parse("2022-01-01"))
        let clause = BuildQueryStringClause option
        test <@ clause = "period_from=2022-01-01T00:00:00Z" @>

    [<Fact>]
    let ``Query clause formats period_to correctly``() =
        let option = PeriodTo(DateTimeOffset.Parse("2022-01-01"))
        let clause = BuildQueryStringClause option
        test <@ clause = "period_to=2022-01-01T00:00:00Z" @>

    [<Fact>]
    let ``Query clause formats group correctly``() =
        let option = ConsumptionGroup(Hour)
        let clause = BuildQueryStringClause option
        test <@ clause = "group_by=hour" @>

    [<Fact>]
    let ``Query clause formats order correctly``() =
        let option = ConsumptionOrder(Forward)
        let clause = BuildQueryStringClause option
        test <@ clause = "order_by=period" @>

    [<Fact>]
    let ``Query clause formats size correctly``() =
        let option = PageSize(14)
        let clause = BuildQueryStringClause option
        test <@ clause = "page_size=14" @>

    [<Fact>]
    let ``Electricity meter URI is correct``() =
        let electricityMeter = Mpan "meterpan"
        test<@ MeterSegment electricityMeter = "electricity-meter-points/meterpan" @>

    [<Fact>]
    let ``Gas meter URI is correct``() =
        let gasMeter = Mprn "meterpan"
        test<@ MeterSegment gasMeter = "gas-meter-points/meterpan" @>

    [<Fact>]
    let ``Full URI is created``() =
        let meter = { Mpxn = Mprn "meterpan"; Serial = "meterserial" }
        let options = [ PageSize(14); ConsumptionGroup(Hour) ]
        test<@ BuildUri options meter = "https://api.octopus.energy/v1/gas-meter-points/meterpan/meters/meterserial/consumption/?page_size=14&group_by=hour" @>

