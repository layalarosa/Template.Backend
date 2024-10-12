# Template Backend

Project in .Net 9 Web API Version="9.0.0-preview.7.24406.2

Overview
This project is a web application to manage reservations and events at a conference center. It allows administrators to configure and manage rooms, while users can search for availability, reserve spaces, and receive confirmations. The user interface is built with Angular, while the backend is driven by ASP.NET Core and the database uses SQL Server along with Entity Framework Core.

Technologies Used

SQL Server: Stores data about users, reservations, rooms, and conference center configurations.

Entity Framework Core: Acts as the ORM that allows you to interact with SQL Server from C# code. It facilitates the creation of queries and the mapping of objects to relational data.

C#: Language used to build the backend with ASP.NET Core, where business logic and RESTful APIs are handled.

ASP.NET Core: Framework to build the backend, which exposes RESTful APIs for the frontend (Angular) to consume data and perform CRUD operations.

Angular: Frontend framework for building an interactive and dynamic user interface. Angular connects to the backend via HTTP to perform operations and display data reactively.

Key Features

Room Management:

Administrators can add, modify and delete rooms.
Configuration of capacity, availability and features of each room.

Reservation System:

Users can search for available rooms by date, capacity and equipment.
Room reservations with automatic confirmation and email notifications.
Users can view and manage their reservations.
Administrative Panel:

Administrators can view a general reservation calendar, analyze usage statistics and manage cancellations.
Reservation reports can be exported in formats such as Excel or PDF.

User-Friendly Interface (Angular):

Advanced search system to find rooms and filtering options.
Dynamic forms and real-time validations.
Responsive and fluid user experience for mobile and desktop devices.
Project Architecture

Backend (ASP.NET Core, Entity Framework, C#):

RESTful API with controllers that expose endpoints for reservations, room management, and administration.
Entity classes such as Room, Reservation, User, etc., defined in C# and mapped to SQL tables using Entity Framework.
Business validations and authorization logic in the backend.

Frontend (Angular):

Angular components for each functionality (reservations, room management, administration panel).
Services that communicate with the REST API to interact with the backend and manage the application state.
Routes configured for navigation between the different views.

Interaction between Backend and Frontend:

Angular communicates with the backend via HTTP, using services and observables to obtain data and update it in the interface.
ASP.NET Core is responsible for serving the API and handling Angular requests, and also exposes the data in a secure and efficient manner.

Project Benefits
Separation of Responsibilities: Using Angular and ASP.NET Core allows for the separation of business logic and presentation logic, facilitating maintenance and scalability.
User Experience: Angular provides a fast and interactive interface, improving the user experience.
Scalability and Performance: ASP.NET Core and SQL Server allow for handling a high volume of bookings and queries with solid performance.

10/12/2024 Update on the way v.1.0

The code will be improved and we will be working with the web api in .Net Core.
