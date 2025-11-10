using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using HotelBooking.Infrastructure;
using HotelBooking.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace HotelBooking.IntegrationTests;

public class CreateBookingTests : IClassFixture<CustomWebAppFactory<Program>>, IDisposable
{
    private readonly CustomWebAppFactory<Program> _factory;

    public CreateBookingTests(CustomWebAppFactory<Program> factory)
    {
        _factory = factory;
    }

    public void Dispose()
    {
        using var scope = _factory.Services.CreateScope();
        var services = scope.ServiceProvider;
        var db = services.GetRequiredService<HotelBookingContext>();
        db.Database.EnsureDeleted();
        GC.SuppressFinalize(this);
    }


    [Fact]
    public async Task BookingFails_WhenEndDateBeforeStartDate()
    {
        var booking = new
        {
            StartDate = DateTime.Today.AddDays(2),
            EndDate = DateTime.Today.AddDays(1),
            CustomerId = 1
        };


    }

    [Fact]
    public async Task BookingSucceeds_WhenValidDatesBeforeOccupied()
    {
        var booking = new
        {
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(2),
            CustomerId = 1
        };
    }
}