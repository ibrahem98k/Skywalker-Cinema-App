# Skywalker-Cinema-App

Skywalker Cinema is a fully-featured console-based cinema ticket booking system built with C#. 
It allows users to seamlessly browse movies, choose preferred showtimes, select ticket types, and pick their seats with a simple and intuitive console interface. 

Key features include:
- Multiple movies with multiple shows in different rooms and times.
- Interactive seat selection with clear indicators for selected, booked, regular, and deluxe seats.
- Support for various ticket types: Regular, Deluxe, AllDayRegular, and AllDayDeluxe, each with configurable pricing.
- Ability to apply discounts, either as a percentage or a fixed amount, at the time of booking.
- Unique ticket IDs generated for each booked seat.
- Booking cancellation by ticket ID, automatically updating the seat availability.
- Persistent data storage using JSON files (`movies.json` and `bookings.json`), allowing data to be saved and loaded between sessions.
- Default movies and shows initialized automatically if no data files exist, ensuring the system is ready to use out-of-the-box.
- Menu-driven, user-friendly console interface with input validation and helpful error messages.
- UTF-8 encoding for proper display of all characters in the console.

This project is perfect for learning how to manage complex data structures, work with collections like `List` and `Dictionary`, implement serialization with JSON, and build a fully interactive console application in C#.

It is ideal for students, hobbyists, and anyone interested in developing a real-world ticket booking system.

---

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
