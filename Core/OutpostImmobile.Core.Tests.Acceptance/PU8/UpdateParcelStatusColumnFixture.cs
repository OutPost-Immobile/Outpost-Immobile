using fit;

namespace OutpostImmobile.Core.Tests.Acceptance.PU8;

public class UpdateParcelStatusColumnFixture : ColumnFixture
{
    // dane wejściowe pobierane ze strony FitNesse
    public string Requests;

    // wykonanie przypadku użycia
    public void Execute()
    {
        SetUp.UpdateParcelStatus.UpdateParcelStatus(Requests);
    }

    // sprawdzenie stanu “encji” (tu: efekt wywołania kontrolera i wysłanej komendy)
    public int StatusCode()
        => SetUp.ParcelState.LastHttpStatusCode ?? -1;

    public string SentParcelModels()
        => SetUp.ParcelState.LastSentParcelModels ?? "<null>";

    public int MediatorSendCalls()
        => SetUp.ParcelState.MediatorSendCalls;
}