using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelBooking.Core.Entities;
using HotelBooking.Core.Services;
using HotelBooking.Core.Interfaces;
using Moq;
using Xunit;
using Reqnroll;
using Bogus;
using FluentAssertions;

namespace HotelBooking.Specs.StepDefinitions;

[Binding]
public class CreateBookingStepDefinitions
{
    private readonly Mock<IRepository<Booking>> bookingRepository;
    private readonly Mock<IRepository<Room>> roomRepository;
    private readonly IBookingManager bookingManager;
    private bool bookingCreationResult;
    private DateTime StartDate;
    private DateTime EndDate;
    private DateTime startTime;
    private DateTime endTime;
    private int testRoomId;
    private int testCustomerId;
    private readonly Faker<Room> roomFaker;
    
    
    public CreateBookingStepDefinitions()
    {
        bookingRepository = new Mock<IRepository<Booking>>();
        roomRepository = new Mock<IRepository<Room>>();
        bookingManager = new BookingManager(bookingRepository.Object, roomRepository.Object);

        roomFaker = new Faker<Room>()
            .RuleFor(r => r.Id, f => f.Random.Int(1, 1000))
            .RuleFor(r => r.Description, f => f.Commerce.ProductName());
        startTime = DateTime.MinValue;
        endTime = DateTime.MinValue;
    }
    
    [Given(@"the occupied period is from {startDate} to {endDate}")]
    public void GivenTheOccupiedPeriodIsFromTo(string start, string end)
    {
        startTime = Convert.ToDateTime(start.Trim('"'));
        endTime = Convert.ToDateTime(end.Trim('"'));
    }
    
    [Given(@"I have a booking with {startDate}, to {endDate}, in room number {roomID} by customer number {customerID}")]
    public void GivenIHaveABookingWithStartDateToEndDateInRoomNumberRoomIdByCustomerNumberCustomerId(string startDate, string endDate, int roomId, int customerId)
    {
        var start = Convert.ToDateTime(startDate.Trim('"'));
        var end = Convert.ToDateTime(endDate.Trim('"'));
        startDate = startDate;
        endDate = endDate;
        testRoomId = roomId;
        testCustomerId = customerId;
    }
    
    [When(@"I create the booking")]
    public async Task WhenICreateTheBooking()
    {
        var bookings = new List<Booking>();
        var rooms = roomFaker.Generate(2);
        
        var booking = new Booking
        {
            StartDate = startTime,
            EndDate = endTime,
            RoomId = testRoomId,
            CustomerId = testCustomerId
        };

        bookingCreationResult = await bookingManager.CreateBooking(booking);
    }
    
    [Then(@"the booking creation should fail due to overlapping dates")]
    public void ThenTheBookingCreationShouldFailDueToOverlappingDates()
    {
        bookingCreationResult.Should().Be(false);

    }
}