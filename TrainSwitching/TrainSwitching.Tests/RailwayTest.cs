using TrainSwitching.Logic;

namespace TrainSwitching.Tests;

public class RailwayTest
{
    [Fact]
    public void AcceptanceCriteria()
    {
        var input = """
            At track 4, add Locomotive from West
            At track 3, add Freight Wagon from West
            At track 3, remove 1 wagon from East
            At track 1, add Freight Wagon from West
            At track 3, add Locomotive from East
            At track 3, add Freight Wagon from West
            At track 3, add Freight Wagon from East
            At track 3, remove 2 wagons from West
            At track 3, train leaves to East
            At track 7, add Car Transport Wagon from West
            At track 10, add Car Transport Wagon from East
            At track 4, add Car Transport Wagon from West
            At track 4, remove 2 wagons from East
            At track 1, add Freight Wagon from West
            At track 5, add Car Transport Wagon from West
            At track 3, add Car Transport Wagon from West
            """;
        
        var station = new RailwayStation();
        var operations = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        station.ProcessOperations(operations);

        var checksum = station.GetChecksum();

        Assert.Equal(550, checksum);
    }
}