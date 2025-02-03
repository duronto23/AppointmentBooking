# Appointment Booking System

### Description

---
This project is for the backend of an appointment booking system that allows customers to schedule appointments with sales managers to discuss
one or more products. Target of this MVP is to have a website that displays available appointment slots that a customer can choose
from.
Here in this project, the main focus is to implement the backend for the system with an endpoint that returns the available appointment slots 
for a customer.

**Followings are rules that need to be considered when checking for available appointment slots for a customer:**<br/>
* Each slot corresponds to a one-hour appointment
* Slots can have overlapping time ranges. For example, it is possible to have the following three slots:
  * 10:30 - 11:30
  * 11:00 - 12:00
  * 11:30 - 12:30
* A sales manager CANNOT be booked for two overlapping slots at the same time. For example, if a sales manger has a slot booked at 10:30 - 11:30, then the 11:00 - 12:00 cannot be booked anymore.
* Customers are matched to sales managers based on specific criteria. A slot CANNOT be booked by a customer if the sales manager does not match any of these three criteria:
  * Language. Currently we have 2 possible languages: German, English 
  * Product(s) to discuss. Currently we have 2 possible products: SolarPanels, Heatpumps 
  * Internal customer rating. Currently we have 3 possible ratings: Gold, Silver, Bronze.
* Customers can book one appointment to discuss multiple products.

### Requirements

---
Design and implement a REST endpoint that:
* Listens for POST requests on this route: `http://localhost:3000/calendar/query`
* Connects to the provided Postgres database instance 
* Receives a request body in this format:
```json
{
  "date": "2024-05-03",
  "products": ["SolarPanels", "Heatpumps"],
  "language": "German",
  "rating": "Gold"
}
```

* Returns a response with an array of available slots that can be booked by the customer in this format
```json
[
  {
    "available_count": 1,
    "start_date": "2024-05-03T10:30:00.00Z"
  },
  {
    "available_count": 2,
    "start_date": "2024-05-03T12:00:00.00Z"
  }
]
```

NOTES: Book appointments is out of scope at this moment. Current focus is to return available slots.

### Database Schema

---
The provided database has the following schema:

**Table: sales_managers**

| Column Name     | Column Type         | Comment                                                  |
|-----------------|---------------------|----------------------------------------------------------|
| id (PK)         | serial              | ID of the sales manager                                  |
| name            | varchar(250)        | Full name of sales manager                               |
| languages       | array(varchar(100)) | List of languages spoken by sales manager                |
| products        | array(varchar(100)) | List of products the sales manager can work with         |
| customer_ratings| array(varchar(100)) | List of customer ratings the sales manager can work with |

**Table: slots**

| Column Name           | Column Type | Comment                                                   |
|-----------------------|-------------|-----------------------------------------------------------|
| id (PK)               | serial      | ID of the slot                                            |
| start_date            | timestampz  | Start date and time of the slot                           |
| end_date              | timestampz  | End date and time of the slot                             |
| booked                | bool        | Value indicating whether the slot has already been booked |
| sales_manager_id (FK) | integer     | ID of the sales manager the slot belongs to               |

# Backend Implementation Details

---
### **Technical Details**
This project is built using **C# and .NET 8**, following the **Clean Architecture** pattern to maintain separation of concerns and ensure scalability. Used **Entity Framework Core (EF Core)** for database interaction, and the application is structured into four key layers:

- **API Layer** (`Api`): Contains controllers and exposes endpoints.
- **Application Layer** (`Application`): Contains business logic and services.
- **Domain Layer** (`Domain`): Defines core entities.
- **Infrastructure Layer** (`Infrastructure`): Handles database interactions using **EF Core**.

---
### **Implementation Details**
The project implements one main feature: **querying available time slots for appointments** based on sales managers’ availability and customer preferences.

#### **Controllers**
**`CalendarController`** – The only controller in the project, responsible for handling requests related to time slot availability.<br />
**The endpoint that implemented for querying available slots:**
- **`POST /calendar/query`** – This endpoint receives a request containing customer preferences (language, product, rating, and date) and returns available time slots based on the stored data about availability of sales managers.

#### **Services**
**`CalendarService`** – Implements the core logic for determining available slots.
  - Fetches data from the repository that filters out booked or overlapping slots and applies customer preferences to match sales managers based on language, products, and customer rating.

#### **Repositories**
**`AppointmentBookingRepository`** – Handles database queries using **Entity Framework Core** to retrieve slot availability efficiently.

---
### **Database & Optimization**
- The application uses **PostgreSQL** as the database.
- **EF Core with LINQ queries** is used for querying sales managers and available slots.
- To optimize performance:
  - **Indexes** are added on relevant columns (e.g., `start_date`, `booked`, `sales_manager_id`).
  - **GIN indexes** are used for array-based filtering (`languages`, `customer_ratings`, `products`).

---
### **Testing**
- The project includes **unit tests** using **NUnit** and **Moq**.
- Tests that provided withing the `test-app` are added with some others tests in `CalendarControllerTests.cs`.

---

### **Validation**
- **FluentValidation** is used for request validation, ensuring only valid data is processed.
---
### **Limitations**

1. **Security**: Authentication and authorization have not been implemented in this project yet.
2. **Appointment Booking**: The current implementation focuses solely on retrieving available slots efficiently. A booking mechanism is not yet in place. In the future, additional logic will be required to reserve slots, ensuring consistency between displayed availability and actual bookings.

---
## **How to run**
1. **Cloning the repo**: Clone the repo with the following command.
    ```shell
      git clone https://github.com/duronto23/AppointmentBooking.git
    ```
2. **Running the database**: Navigate to the `database` folder inside the project directory (`/AppointmentBooking/database`) with terminal and run the following commands(**Docker must be installed before running the command**).
    ```shell
      docker build -t enpal-coding-challenge-db .
      docker run --name enpal-coding-challenge-db -p 5432:5432 -d enpal-coding-challenge-db
    ```
    Once the docker container is up and running, ensure the database can be connected using any DB query tool.
The default connection string is `postgres://postgres:mypassword123!@localhost:5432/coding-challenge`

3. **Running the backend**: Navigate to the `AppointmentBooking.Api` folder inside the project directory (`/AppointmentBooking/AppointmentBooking.Api`) and run the following commands.
    ```shell
      dotnet build
      dotnet run
    ```
   Backend application should now running and the endpoint for getting the slots is running on `http://localhost:3000/calendar/query`.
4. **Testing**: Navigate to the `test-app` folder inside the project directory (`AppointmentBooking/test-app`) and run the following commands(**Node must be installed before running the command**).
    ```shell
      npm install
      npm run test
    ```
   * **To run the tests within the backend application can be run as follows.**
     * Navigate to `AppointmentBooking.Services.Tests` inside the project folder(`AppointmentBooking/AppointmentBooking.Services.Tests`) and run the following commands.
       ```shell
        dotnet test
       ```
     * Navigate to `AppointmentBooking.Api.Tests` inside the project folder(`AppointmentBooking/AppointmentBooking.Api.Tests`) and run the following commands.
       ```shell
        dotnet test
       ```
       
