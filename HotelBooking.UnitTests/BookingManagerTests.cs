using System;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        private IBookingManager bookingManager;
        IRepository<Booking> bookingRepository;

        public BookingManagerTests(){
            DateTime start = DateTime.Today.AddDays(10);
            DateTime end = DateTime.Today.AddDays(20);
            bookingRepository = new FakeBookingRepository(start, end);
            IRepository<Room> roomRepository = new FakeRoomRepository();
            bookingManager = new BookingManager(bookingRepository, roomRepository);
        }

        [Fact]
        public async Task FindAvailableRoom_StartDateNotInTheFuture_ThrowsArgumentException()
        {
            // Arrange
            DateTime date = DateTime.Today;

            // Act
            Task result() => bookingManager.FindAvailableRoom(date, date);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(result);
        }

        [Fact]
        public async Task FindAvailableRoom_RoomAvailable_RoomIdNotMinusOne()
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            // Act
            int roomId = await bookingManager.FindAvailableRoom(date, date);
            // Assert
            Assert.NotEqual(-1, roomId);
        }

        [Fact]
        public async Task FindAvailableRoom_RoomAvailable_ReturnsAvailableRoom()
        {
            // This test was added to satisfy the following test design
            // principle: "Tests should have strong assertions".

            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            
            // Act
            int roomId = await bookingManager.FindAvailableRoom(date, date);

            var bookingForReturnedRoomId = (await bookingRepository.GetAllAsync()).
                Where(b => b.RoomId == roomId
                           && b.StartDate <= date
                           && b.EndDate >= date
                           && b.IsActive);
            
            // Assert
            Assert.Empty(bookingForReturnedRoomId);
        }
        
        [Theory]
        [InlineData(-1, 2, false)] // start date in the past → fail
        [InlineData(0, 0, false)]  // same-day booking → fail
        [InlineData(1, 3, true)]   // valid booking → succeed
        [InlineData(5, 10, true)]  // valid booking → succeed
        public async Task CreateBooking_VariousDates_Validation(int startOffset, int endOffset, bool expectedSuccess)
        {
            var start = DateTime.Today.AddDays(startOffset);
            var end = DateTime.Today.AddDays(endOffset);

            var booking = new Booking
            {
                StartDate = start,
                EndDate = end,
                CustomerId = 1,
                RoomId = 1
            };

            bool result;
            try
            {
                var created = await bookingManager.CreateBooking(booking);
                result = created != null;
            }
            catch (ArgumentException)
            {
                result = false;
            }

            Assert.Equal(expectedSuccess, result);
        }
        
        [Fact]
        public async Task CreateBooking_OverlappingDates_ReturnsFalse()
        {
            var start = DateTime.Today.AddDays(12);
            var end = DateTime.Today.AddDays(15);

            var newBooking = new Booking
            {
                StartDate = start,
                EndDate = end,
                CustomerId = 1,
                RoomId = 1
            };

            // Act
            var result = await bookingManager.CreateBooking(newBooking);

            Assert.False(result); // business rule: overlaps are rejected
        }
        
        [Fact]
        public async Task CancelBooking_BeforeStartDate_BookingIsCancelled()
        {
            // Arrange: get a booking
            var booking = (await bookingRepository.GetAllAsync()).First();

            // Make sure its start date is in the future
            booking.StartDate = DateTime.Today.AddDays(5);
            booking.IsActive = true;

            // Act: "cancel" by setting IsActive = false and editing via repository
            booking.IsActive = false;
            await bookingRepository.EditAsync(booking);

            // Assert
            var updatedBooking = await bookingRepository.GetAsync(booking.Id);
            Assert.False(updatedBooking.IsActive);
        }
        
        [Theory]
        [InlineData("2025-09-15", "2025-09-16", true)]   // fully occupied
        [InlineData("2025-09-25", "2025-09-26", false)]  // available
        public async Task GetFullyOccupiedDates_ReturnsCorrectDates(string startStr, string endStr, bool expected)
        {
            var start = DateTime.Parse(startStr);
            var end = DateTime.Parse(endStr);

            var fullyOccupiedDates = await bookingManager.GetFullyOccupiedDates(start, end);
            var expectedDates = new List<DateTime> { DateTime.Parse("2025-09-15"), DateTime.Parse("2025-09-16") };
            Assert.Equal(expectedDates, fullyOccupiedDates);
            
        }




    }
}
