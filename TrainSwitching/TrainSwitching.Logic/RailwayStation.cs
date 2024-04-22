using System.ComponentModel;

namespace TrainSwitching.Logic;

public class RailwayStation
{
    public Track[] Tracks { get; } = new Track[10];

    public RailwayStation()
    {
        // Tracks: The station has 10 tracks numbered from 1 to 10.
        // Tracks 1-8 can be accessed from both the East and West sides.
        // Tracks 9-10 are terminal tracks and can only be accessed from the West.
        for (int i = 1; i <= 8; i++)
        {
            Tracks[i - 1] = new Track(i, ReachableFrom.Both);
        }

        for (int i = 9; i <= 10; i++)
        {
            Tracks[i - 1] = new Track(i, ReachableFrom.West);
        }
    }

    public void ProcessOperation(string operation)
    {
        /*
        The operation is a string that describes an operation that needs to be performed at the station.
        The operation string is formatted as follows:

        At track 4, add Locomotive from West
        At track 3, add Freight Wagon from West
        At track 3, remove 1 wagon from East
        At track 3, remove 2 wagons from West
        At track 3, train leaves to East
        At track 5, add Car Transport Wagon from West

        This method processes ONE operation at a time.
        */

        var parts = operation.Split(' ');
        var trackNumber = int.Parse(parts[2][..^1]);
        var track = Tracks[trackNumber - 1];

        switch (parts[3])
        {
            case "add":
                {
                    var wagonType = parts[4] switch
                    {
                        "Passenger" => WagonType.Passenger,
                        "Locomotive" => WagonType.Locomotive,
                        "Freight" => WagonType.Freight,
                        "Car" => WagonType.CarTransport,
                        _ => throw new InvalidEnumArgumentException("Invalid wagon type.")
                    };
                    var direction = Enum.Parse<Direction>(parts[^1]);
                    track.Add(wagonType, direction);
                    break;
                }
            case "remove":
                {
                    var numberOfWagons = int.Parse(parts[4]);
                    var direction = Enum.Parse<Direction>(parts[^1]);
                    track.Remove(numberOfWagons, direction);
                    break;
                }
            case "train":
                {
                    var direction = Enum.Parse<Direction>(parts[^1]);
                    track.TrainLeavesTrack(direction);
                    break;
                }
            default:
                throw new InvalidOperationException("Invalid operation.");
        }
    }

    public int GetChecksum()
    {
        /*
        Add the values of all wagons on a track.
        Multiply the sum by the track number (tracks are one-based).
        */

        var checksum = 0;
        for (int i = 0; i < Tracks.Length; i++)
        {
            checksum += Tracks[i].GetChecksum() * (i + 1);
        }

        return checksum;
    }

    public void ProcessOperations(string[] operations)
    {
        foreach (var operation in operations)
        {
            try
            {
                ProcessOperation(operation);
            }
            catch
            {
            }
        }
    }
}

[Flags]
public enum ReachableFrom
{
    East = 0x1,
    West = 0x2,
    Both = East | West
}

public enum Direction
{
    East,
    West
}

public enum WagonType
{
    Passenger,
    Locomotive,
    Freight,
    CarTransport,
}

public class Track(int trackNumber, ReachableFrom reachableFrom)
{
    public int TrackNumber { get; } = trackNumber;

    public ReachableFrom ReachableFrom { get; } = reachableFrom;

    // If a wagon is added from the east, it should be added to the beginning of the list.
    // If a wagon is added from the west, it should be added to the end of the list.
    public List<WagonType> Wagons { get; } = [];

    private void CheckDirectionAvailability(Direction direction)
    {
        if (direction == Direction.East && !ReachableFrom.HasFlag(ReachableFrom.East))
        {
            throw new InvalidOperationException("Track is not reachable from the East.");
        }

        if (direction == Direction.West && !ReachableFrom.HasFlag(ReachableFrom.West))
        {
            throw new InvalidOperationException("Track is not reachable from the West.");
        }
    }
    /// <summary>
    /// Add a wagon to the track.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when direction if not available for the track.
    /// </exception>
    public void Add(WagonType wagonType, Direction whereToAdd)
    {
        CheckDirectionAvailability(whereToAdd);

        if (whereToAdd == Direction.East)
        {
            Wagons.Insert(0, wagonType);
        }
        else
        {
            Wagons.Add(wagonType);
        }
    }

    /// <summary>
    /// Remove a number of wagons from the track.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when direction if not available for the track.
    /// Also thrown if not enough wagons are available to remove.
    /// </exception>
    public void Remove(int numberOfWagons, Direction whereToRemove)
    {
        CheckDirectionAvailability(whereToRemove);
        if (Wagons.Count < numberOfWagons)
        {
            throw new InvalidOperationException("Not enough wagons to remove.");
        }

        if (whereToRemove == Direction.East)
        {
            Wagons.RemoveRange(0, numberOfWagons);
        }
        else
        {
            Wagons.RemoveRange(Wagons.Count - numberOfWagons, numberOfWagons);
        }
    }

    /// <summary>
    /// Move the train from the track.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when direction if not available for the track.
    /// Also thrown if no locomtive is available to move the train.
    /// </exception>
    public void TrainLeavesTrack(Direction direction)
    {
        CheckDirectionAvailability(direction);

        if (!Wagons.Contains(WagonType.Locomotive))
        {
            throw new InvalidOperationException("No locomotive to move the train.");
        }

        Wagons.Clear();
    }

    public int GetChecksum()
    {
        var points = 0;
        foreach (var wagon in Wagons)
        {
            points += wagon switch
            {
                WagonType.Passenger => 1,
                WagonType.Locomotive => 10,
                WagonType.Freight => 20,
                WagonType.CarTransport => 30,
                _ => throw new InvalidEnumArgumentException("Invalid wagon type.")
            };
        }

        return points;
    }
}
