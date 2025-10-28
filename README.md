# Skywalker-Cinema-App

A console-based cinema ticket booking system written in C#. This application allows users to browse movies, choose showtimes, select seats, pick ticket types, apply discounts, and manage bookings. All data is stored in JSON files for persistence.

-------------

## Features

- **Movie Management**
  - Preloaded list of popular movies.
  - Each movie has multiple shows in different rooms and times.
  
- **Show and Seat Selection**
  - Users can choose the day of the week and a specific showtime.
  - Seat layout displayed in console with clear indicators:
    - `[✓]` Selected seat  
    - `[X]` Already booked  
    - `[R]` Regular seat  
    - `[D]` Deluxe seat
  - Seats are validated to prevent double booking.
  
- **Ticket Types**
  - Regular
  - Deluxe
  - AllDayRegular
  - AllDayDeluxe
  - Ticket prices are configurable.

- **Booking and Cancellation**
  - Users can make a new booking and receive unique ticket IDs.
  - Bookings can be cancelled by entering the ticket ID.
  - Supports discounts (percentage or fixed amount) applied during booking.

- **Data Persistence**
  - Movies and bookings are saved to JSON files (`movies.json` and `bookings.json`) automatically.
  - System initializes default movies and shows if no JSON files are found.

- **Console Interface**
  - Menu-driven interface with clear options.
  - Input validation and error messages to guide users.
  - UTF-8 encoding for proper character display.

---

## Getting Started

1. **Clone the repository:**

```bash
git clone https://github.com/yourusername/Skywalker-Cinema-App.git
