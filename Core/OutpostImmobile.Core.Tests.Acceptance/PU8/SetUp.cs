using fit;

namespace OutpostImmobile.Core.Tests.Acceptance.PU8;

public class SetUp : Fixture
{
    // statyczne instancje “realizatora przypadku użycia” + “stan”
    public static UpdateParcelStatusUseCase UpdateParcelStatus = null!;
    public static ParcelStatusState ParcelState = null!;

    public SetUp()
    {
        // tu składasz i łączysz “części systemu”
        ParcelState = new ParcelStatusState();
        UpdateParcelStatus = new UpdateParcelStatusUseCase(ParcelState);
    }
}