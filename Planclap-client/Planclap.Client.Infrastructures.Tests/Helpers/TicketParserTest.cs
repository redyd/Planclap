using System.Reflection;
using Planclap.Client.Domains.core;
using Planclap.Client.Infrastructures.Helpers;

namespace Planclap.Client.Infrastructures.Tests.Helpers;

/// <summary>
///     Réalisé à l'aide de l'IA
/// </summary>
[TestFixture]
public class TicketParserTest
{
    [SetUp]
    public void SetUp() => _parser = new TicketParser();

    private TicketParser _parser = new();

    [Test]
    public void Should_Return_Empty_List_When_Reservations_Is_Null_Or_Empty()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_parser.ParseTickets(null!), Is.Empty);
            Assert.That(_parser.ParseTickets(""), Is.Empty);
            Assert.That(_parser.ParseTickets("  "), Is.Empty);
        }
    }

    [Test]
    public void Should_Parse_Single_Ticket_Correctly_When_Valid_String_Given()
    {
        var result = _parser.ParseTickets("3-5=n");

        Assert.That(result, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].Seat.Row, Is.EqualTo(3));
            Assert.That(result[0].Seat.Column, Is.EqualTo(5));
            Assert.That(result[0].TicketType, Is.EqualTo(TicketType.Normal));
        }
    }

    [Test]
    public void Should_Parse_Multiple_Tickets_Correctly()
    {
        const string input = "1-1=c|2-2=s|3-3=n";
        var result = _parser.ParseTickets(input);

        Assert.That(result, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].TicketType, Is.EqualTo(TicketType.Child));
            Assert.That(result[1].TicketType, Is.EqualTo(TicketType.Senior));
            Assert.That(result[2].TicketType, Is.EqualTo(TicketType.Normal));
        }
    }

    [TestCase("invalidFormat")]
    [TestCase("3-5-7=n")]
    [TestCase("3=n-5")]
    public void Should_Throw_FormatException_When_Invalid_Reservation_Format(string invalid)
        => Assert.Throws<FormatException>(() => _parser.ParseTickets(invalid));

    [TestCase("=n")]
    [TestCase("-1-2=n")]
    [TestCase("1-2=")]
    public void Should_Throw_Exception_When_Invalid_Seat_Or_Type(string invalid)
        => Assert.Throws<FormatException>(() => _parser.ParseTickets(invalid));

    [TestCase("1-1=x", TicketType.Normal)]
    [TestCase("2-2=HDD", TicketType.Normal)]
    public void Should_Default_To_Normal_When_TicketType_Is_Unknown(string input, TicketType expected)
    {
        var result = _parser.ParseTickets(input);
        Assert.That(result[0].TicketType, Is.EqualTo(expected));
    }

    [TestCase("n", TicketType.Normal)]
    [TestCase("N", TicketType.Normal)]
    [TestCase("c", TicketType.Child)]
    [TestCase("C", TicketType.Child)]
    [TestCase("s", TicketType.Senior)]
    [TestCase("S", TicketType.Senior)]
    public void Should_Handle_Case_Insensitive_TicketType(string type, TicketType expected)
    {
        var input = $"1-1={type}";
        var result = _parser.ParseTickets(input);
        Assert.That(result[0].TicketType, Is.EqualTo(expected));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void ParseSeat_Should_Throw_ArgumentException_When_Value_Is_NullOrWhitespace(string? input)
    {
        var method = typeof(TicketParser)
            .GetMethod("ParseSeat", BindingFlags.NonPublic | BindingFlags.Static)!;

        var ex = Assert.Throws<TargetInvocationException>(() =>
            method.Invoke(null, [input])
        );

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ex!.InnerException, Is.TypeOf<ArgumentException>());
            Assert.That(ex.InnerException!.Message, Does.Contain("Seat value cannot be empty"));
        }
    }

    [TestCase("1", "Invalid seat format: 1. Expected format: 'row-column'")]
    [TestCase("1-2-3", "Invalid seat format: 1-2-3. Expected format: 'row-column'")]
    public void ParseSeat_Should_Throw_FormatException_When_InvalidFormat(string input, string expectedMessage)
    {
        var ex = Assert.Throws<TargetInvocationException>(() =>
            typeof(TicketParser)
                .GetMethod("ParseSeat", BindingFlags.NonPublic | BindingFlags.Static)!
                .Invoke(null, [input]))!;

        Assert.That(ex.InnerException, Is.TypeOf<FormatException>());
        Assert.That(ex.InnerException!.Message, Is.EqualTo(expectedMessage));
    }

    [TestCase("abc-1", "Invalid row number: abc")]
    [TestCase("1-xyz", "Invalid column number: xyz")]
    public void ParseSeat_Should_Throw_FormatException_When_RowOrColumn_InvalidNumber(string input, string expectedMessage)
    {
        var ex = Assert.Throws<TargetInvocationException>(() =>
            typeof(TicketParser)
                .GetMethod("ParseSeat", BindingFlags.NonPublic | BindingFlags.Static)!
                .Invoke(null, [input]))!;

        Assert.That(ex.InnerException, Is.TypeOf<FormatException>());
        Assert.That(ex.InnerException!.Message, Is.EqualTo(expectedMessage));
    }

    [Test]
    public void ParseSeat_Should_Return_Seat_When_ValidInput()
    {
        var method = typeof(TicketParser).GetMethod("ParseSeat", BindingFlags.NonPublic | BindingFlags.Static)!;
        var seat = (Seat)method.Invoke(null, ["5-10"])!;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(seat.Row, Is.EqualTo(5));
            Assert.That(seat.Column, Is.EqualTo(10));
        }
    }
}
