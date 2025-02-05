# Appointment Booking System

---
### Description

This project is for the backend of an appointment booking system that allows customers to schedule appointments with sales managers to discuss
one or more products. Target of this MVP is to have a website that displays available appointment slots that a customer can choose
from.
Here in this project, the focus is to implement the backend for the system with an endpoint that returns the available appointment slots 
for a customer.

**Followings are rules that need to be considered when checking for available appointment slots for a customer:**<br/>
* Each slot corresponds to a one-hour appointment
* Slots can have overlapping time ranges. For example, it is possible to have the following three slots:
  * 10:30 - 11:30
  * 11:00 - 12:00
  * 11:30 - 12:30
* A sales manager CANNOT be booked for two overlapping slots at the same time. For example, if a sales manger has a slot booked at 10:30 - 11:30, then the 11:00 - 12:00 cannot be booked anymore.
* Customers are matched to sales managers based on specific criteria. A slot CANNOT be booked by a customer if the sales manager does not match any of these three criteria:
  * Language. German, English need to be supported as of now. 
  * Product(s) to discuss. SolarPanels, Heatpumps are available products as of now.
  * Internal customer rating. Customer could have Gold, Silver or Bronze rating.
* Customers can book one appointment to discuss multiple products.

---
### Requirements

Design and implement a REST endpoint that:
* Listens for POST requests on this route: `http://localhost:3000/calendar/query`
* Connects to the provided **Postgres database** instance 
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

NOTES: Appointment booking is out of scope for now. The current focus is on returning available slots based on sales managers' availability and matching criteria.

---
### Database Schema

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
The backend is built using C# and .NET 8 with Entity Framework Core (EF Core) as the ORM. The architecture follows the Clean Architecture pattern, which separates concerns into four layers:

- **API Layer** (`Api`) – Exposes endpoints via CalendarController
- **Application Layer** (`Application`) – Contains business logic and services
- **Domain Layer** (`Domain`) – Defines core entities
- **Infrastructure Layer** (`Infrastructure`) – Handles database interactions via AppointmentBookingRepository

**NOTE**: Security features, such as token authorization, have not been implemented yet since this task focuses only on slot fetching. A security mechanism (e.g., JWT) can be introduced when the slot booking feature is added.

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
### How It Works:

* The API receives a **POST** request with customer and appointment details.
* **CalendarService** processes the request:
  * Fetches available slots from the repository.
  * Filters slots based on language, product, and customer rating.
  * Ensures slots do not overlap with already booked ones.
* The API returns the filtered list of available slots.

---
### **Database & Optimization**
- The application uses **PostgreSQL** as the database as mentioned in requirement.
- **EF Core with LINQ queries** is used for querying sales managers and available slots.
- To optimize performance:
  - Added a view containing **Available Slots**.
  - **Indexes** are added on relevant columns.
  - **GIN indexes** are used for array-based filtering.
- Details of added indexes are as follows:

|Table	| Index	| Columns | Type| Purpose|
|-------|-------|---------|-----|--------|
| sales_managers | 	idx_sales_managers_arrays | languages, customer_ratings, products | 	GIN	 | Searching across multiple array fields. |
|slots|	idx_slots_sales_manager_id| sales_manager_id | B-tree | 	Enhances performance for joins between slots and sales_managers using sales_manager_id. |
|slots|	idx_slots_start_date_booked| start_date, booked | B-tree | 	Speeds up queries that filter available slots on a specific start_date. |
|slots|	idx_slots_end_date_booked| end_date, booked | B-tree | 	Speeds up queries that filter available slots on a specific end_date. |

**View: vw_available_slots**

| Column Name      | From Table       | Comment                                                  |
|------------------|------------------|----------------------------------------------------------|
| id               | slots            | ID of the slot.                                          |
| start_date       | slots            | Start date and time of the slot                          |
| end_date         | slots            | End date and time of the slot                            |
| sales_manager_id | slots            | ID of the sales manager the slot belongs to              |
| languages        | sales_managers   | List of languages spoken by sales manager                |
| products         | sales_managers   | List of products the sales manager can work with         |
| customer_ratings | sales_managers   | List of customer ratings the sales manager can work with |

---
### **Testing**
- The project includes **unit tests** using **NUnit** and **Moq**.
- I have added the provided tests from `test-app` into `CalendarControllerTests.cs` for easy execution within the project. Additionally, I have included some extra tests as well.

---
### **Validation**
- **FluentValidation** is used for request validation, ensuring only valid data is processed.
- Validation of supported Language, Products and Customer Rating are done with regex matching and patterns are provided from appsettings, this increase flexibility for adding more supported Language or Product easily.

---
### **Exception Handling**
- A global exception handler has been implemented to manage unexpected errors. I chose to use a custom exception handler class instead of the built-in one to provide greater flexibility in handling exceptions.

---
### **Logging**
- The ILogger interface provided by the framework is utilized to log information and errors during execution. In the future, a more advanced logging library(like Serilog) could be integrated to enhance monitoring and diagnostics.

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
    * Once the docker container is up and running, ensure the database can be connected using any DB query tool.
The default connection string is `postgres://postgres:mypassword123!@localhost:5432/coding-challenge`.
    * If connecting to database in some other location is required, please update the value of the key `CodingChallengeDb` in section `ConnectionStrings` in `appsettings.json` file inside `AppointmentBooking.Api` with the details of your database connection.

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
   NOTE: Additional data could be loaded to the running instance to do extensive testing with large amount of data.

* **To run the tests within the backend application can be run as follows.**
  * Navigate to `AppointmentBooking.Services.Tests` inside the project folder(`AppointmentBooking/AppointmentBooking.Services.Tests`) and run the following commands.
    ```shell
     dotnet test
    ```
  * Navigate to `AppointmentBooking.Api.Tests` inside the project folder(`AppointmentBooking/AppointmentBooking.Api.Tests`) and run the following commands.
    ```shell
     dotnet test
    ```
       
