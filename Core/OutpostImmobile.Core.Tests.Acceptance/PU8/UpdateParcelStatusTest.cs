using System.Reflection;
using fit;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using OutpostImmobile.Api.Controllers;
using OutpostImmobile.Api.Request;
using OutpostImmobile.Core.Mediator;
using OutpostImmobile.Core.Parcels.Commands;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Core.Tests.Acceptance.PU8;

// Ten plik zawiera wszystko: SetUp, "use case", stan, fixture kolumnowe.
// Klasa testowa NUnit może zostać pusta (FitNesse uruchamia fixture’y), ale zostawiam ją jako „kotwicę” dla projektu.
[TestFixture]
public class UpdateParcelStatusTest
{
    [Test]
    public void Placeholder()
    {
        Assert.Pass("FitNesse fixtures are defined in this file. Run via FitNesse/FitSharp.");
    }
}



public sealed class ParcelStatusState
{
    public int? LastHttpStatusCode { get; set; }
    public string? LastSentParcelModels { get; set; }
    public int MediatorSendCalls { get; set; }
}

/// <summary>
/// “Realizator przypadku użycia” – ma metodę UpdateParcelStatus(...),
/// którą woła fixture FitNesse. W środku wywołujemy kontroler i zapisujemy stan.
/// </summary>
public sealed class UpdateParcelStatusUseCase
{
    private readonly ParcelStatusState _state;

    public UpdateParcelStatusUseCase(ParcelStatusState state)
    {
        _state = state;
    }

    public void UpdateParcelStatus(string requestsSpec)
    {
        _state.LastHttpStatusCode = null;
        _state.LastSentParcelModels = null;
        _state.MediatorSendCalls = 0;

        var requests = ParseRequests(requestsSpec);

        BulkUpdateParcelStatusCommand? capturedCommand = null;

        var mediator = new Mock<IMediator>();
        mediator
            .Setup(m => m.Send(It.IsAny<BulkUpdateParcelStatusCommand>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((cmd, _) =>
            {
                capturedCommand = (BulkUpdateParcelStatusCommand)cmd;
            })
            .Returns(Task.CompletedTask);

        // Wywołanie prywatnej metody kontrolera: ParcelController.UpdateParcelStatusAsync(...)
        var method = typeof(ParcelController)
            .GetMethod("UpdateParcelStatusAsync", BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException("UpdateParcelStatusAsync method not found.");

        var task = (Task)method.Invoke(null, new object[] { mediator.Object, requests })!;
        task.GetAwaiter().GetResult();

        var resultsWrapper = task.GetType().GetProperty("Result")!.GetValue(task);
        var resultObj = resultsWrapper!.GetType().GetProperty("Result")!.GetValue(resultsWrapper);

        _state.LastHttpStatusCode = resultObj is NoContent ? 204 : 400;

        mediator.Verify(
            m => m.Send(It.IsAny<BulkUpdateParcelStatusCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _state.MediatorSendCalls = 1;

        if (capturedCommand is null)
            throw new InvalidOperationException("Mediator command was not captured.");

        _state.LastSentParcelModels = string.Join(";",
            capturedCommand.ParcelModels.Select(pm => $"{pm.FriendlyId}={pm.Status}"));
    }

    private static List<UpdateParcelStatusRequest> ParseRequests(string spec)
    {
        // format: "P123=Delivered;P456=InTransit"
        var list = new List<UpdateParcelStatusRequest>();

        foreach (var part in spec.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var kv = part.Split('=', 2, StringSplitOptions.TrimEntries);
            if (kv.Length != 2)
                throw new ArgumentException($"Invalid segment '{part}'. Expected FriendlyId=Status.");

            list.Add(new UpdateParcelStatusRequest
            {
                FriendlyId = kv[0],
                ParcelStatus = Enum.Parse<ParcelStatus>(kv[1], ignoreCase: true)
            });
        }

        return list;
    }
}