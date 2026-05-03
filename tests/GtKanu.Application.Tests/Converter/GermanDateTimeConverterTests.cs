namespace GtKanu.Application.Tests.Converter;

using GtKanu.Application.Converter;
using GtKanu.Application.Models;

public class GermanDateTimeConverterTests
{
    private readonly GermanDateTimeConverter _sut = new();

    [Fact]
    public void ToDateTime_ShouldReturnGermanFormat()
    {
        var date = new DateTimeOffset(2025, 6, 15, 16, 30, 0, TimeSpan.FromHours(2));
        var result = _sut.ToDateTime(date);
        result.Should().Be("15.06.2025 16:30");
    }

    [Fact]
    public void ToDateTimeShort_ShouldReturnShortGermanFormat()
    {
        var date = new DateTimeOffset(2025, 6, 15, 16, 30, 0, TimeSpan.FromHours(2));
        var result = _sut.ToDateTimeShort(date);
        result.Should().Be("15.06. 16:30");
    }

    [Fact]
    public void ToDate_WithDateTimeOffset_ShouldReturnDateOnly()
    {
        var date = new DateTimeOffset(2025, 6, 15, 16, 30, 0, TimeSpan.FromHours(2));
        var result = _sut.ToDate(date);
        result.Should().Be("15.06.2025");
    }

    [Fact]
    public void ToDate_WithDateOnly_ShouldReturnFormattedDate()
    {
        var date = new DateOnly(2025, 6, 15);
        var result = _sut.ToDate(date);
        result.Should().Be("15.06.2025");
    }

    [Fact]
    public void ToTime_ShouldReturnTimeOnly()
    {
        var date = new DateTimeOffset(2025, 6, 15, 16, 30, 0, TimeSpan.FromHours(2));
        var result = _sut.ToTime(date);
        result.Should().Be("16:30");
    }

    [Fact]
    public void ToUtc_WithDateOnly_ShouldConvertToUtc()
    {
        var date = new DateOnly(2025, 6, 15);
        var result = _sut.ToUtc(date);
        result.Offset.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void ToUtc_WithDateTime_ShouldConvertToUtc()
    {
        var date = new DateTime(2025, 6, 15, 16, 30, 0);
        var result = _sut.ToUtc(date);
        result.Offset.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void ToLocal_ShouldConvertToLocalTime()
    {
        var utc = new DateTimeOffset(2025, 6, 15, 12, 0, 0, TimeSpan.Zero);
        var result = _sut.ToLocal(utc);
        result.Hour.Should().Be(14);
    }

    [Fact]
    public void FromIsoDateTime_WithValidInput_ShouldParse()
    {
        var result = _sut.FromIsoDateTime("2025-06-15T14:30");
        result.Should().NotBeNull();
        result!.Value.Year.Should().Be(2025);
        result.Value.Month.Should().Be(6);
        result.Value.Day.Should().Be(15);
    }

    [Fact]
    public void FromIsoDateTime_WithInvalidInput_ShouldReturnNull()
    {
        var result = _sut.FromIsoDateTime("invalid");
        result.Should().BeNull();
    }

    [Fact]
    public void FromIsoDateTime_WithNullInput_ShouldReturnNull()
    {
        var result = _sut.FromIsoDateTime(null);
        result.Should().BeNull();
    }

    [Fact]
    public void ToIso_WithDateTimeOffset_ShouldReturnIsoFormat()
    {
        var date = new DateTimeOffset(2025, 6, 15, 14, 30, 0, TimeSpan.Zero);
        var result = _sut.ToIso(date);
        result.Should().Be("2025-06-15T14:30");
    }

    [Fact]
    public void ToIso_WithDateOnly_ShouldReturnIsoDate()
    {
        var date = new DateOnly(2025, 6, 15);
        var result = _sut.ToIso(date);
        result.Should().Be("2025-06-15");
    }

    [Fact]
    public void ToIso_WithTimeOnly_ShouldReturnIsoTime()
    {
        var time = new TimeOnly(14, 30);
        var result = _sut.ToIso(time);
        result.Should().Be("14:30");
    }

    [Fact]
    public void FromIsoTime_WithValidInput_ShouldParse()
    {
        var result = _sut.FromIsoTime("14:30");
        result.Should().NotBeNull();
        result!.Value.Hour.Should().Be(14);
        result.Value.Minute.Should().Be(30);
    }

    [Fact]
    public void FromIsoTime_WithInvalidInput_ShouldReturnNull()
    {
        var result = _sut.FromIsoTime("invalid");
        result.Should().BeNull();
    }

    [Fact]
    public void FromIsoDate_WithValidInput_ShouldParse()
    {
        var result = _sut.FromIsoDate("2025-06-15");
        result.Should().NotBeNull();
        result!.Value.Year.Should().Be(2025);
    }

    [Fact]
    public void FromIsoDate_WithInvalidInput_ShouldReturnNull()
    {
        var result = _sut.FromIsoDate("invalid");
        result.Should().BeNull();
    }

    [Fact]
    public void Format_SameDay_ShouldContainDateAndTimes()
    {
        var currentYear = DateTimeOffset.UtcNow.Year;
        var start = new DateTimeOffset(currentYear, 6, 15, 10, 0, 0, TimeSpan.FromHours(2));
        var end = new DateTimeOffset(currentYear, 6, 15, 12, 0, 0, TimeSpan.FromHours(2));
        var result = _sut.Format(start, end);
        result.Should().Contain("15.06.");
        result.Should().Contain("10:00");
        result.Should().Contain("12:00");
    }

    [Fact]
    public void Format_DifferentDays_ShouldContainBothDates()
    {
        var start = new DateTimeOffset(2025, 6, 15, 10, 0, 0, TimeSpan.FromHours(2));
        var end = new DateTimeOffset(2025, 6, 16, 12, 0, 0, TimeSpan.FromHours(2));
        var result = _sut.Format(start, end);
        result.Should().Contain("15.06.2025");
        result.Should().Contain("16.06.2025");
    }

    [Fact]
    public void Format_WithTimeSpanMoreThanOneDay_ShouldReturnDays()
    {
        var span = TimeSpan.FromDays(2.5);
        var expected = $"{span.TotalDays} Tage";
        var result = _sut.Format(span);
        result.Should().Be(expected);
    }

    [Fact]
    public void Format_WithTimeSpanLessThanOneDay_ShouldReturnHours()
    {
        var span = TimeSpan.FromHours(5.5);
        var expected = $"{span.TotalHours} Stunden";
        var result = _sut.Format(span);
        result.Should().Be(expected);
    }
}
