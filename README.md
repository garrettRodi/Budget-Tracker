Budget Tracker

Budget Tracker is a comprehensive budgeting application designed to help users manage their finances by creating budgets, tracking expenses and incomes, setting saving goals, and generating financial reports. The final vision is to offer a fully functional application available as a web app, mobile app, and Windows (Microsoft Store) app, with robust reporting and multi‑currency support.
Table of Contents

    Project Overview and Goals
    Architecture Overview
        Layered Architecture Diagram
        Presentation Layer
        Application Layer
        Domain Layer
        Infrastructure Layer
        Testing Suite
    Current Usage
    Future Vision
    Installation and Setup
    Next Steps

Project Overview and Goals

The main objective of Budget Tracker is to provide users with a flexible and dynamic tool to:

    Create one or more budgets (referred to as BudgetContainers) that persist automatically.
    Define separate saving goals and income sources outside the main budget containers.
    Populate budgets with BudgetItems (e.g., food, rent, medical, entertainment) that include both planned (expected) and actual values.
    Allow users to input actual income and savings contributions over time.
    Track financial progress with notifications, detailed reports (e.g., monthly totals, yearly differences, spending habits), and visually appealing graphs.
    Support toggling between different currencies and calculating percentages of income.

The final goal is to expand this functionality across multiple platforms (web, phone, and Windows applications).
Architecture Overview

Budget Tracker is built using a layered architecture, ensuring separation of concerns and maintainability. The layers are organized as follows:
Layered Architecture Diagram

flowchart TD
    A[Presentation Layer]
    B[Application Layer]
    C[Domain Layer]
    D[Infrastructure Layer]
    E[Testing Suite]

    A --> B
    B --> C
    C --> D

    %% Additional connections for logging and external services
    D -->|Provides data access & external services| F[Database & APIs]

    E --- A
    E --- B
    E --- C
    E --- D

Figure: High-level architecture showing the flow from Presentation through Application to Domain and Infrastructure, with the Testing Suite integrated across layers.
Presentation Layer

Purpose:
This layer is responsible for user interaction. It provides a console-based user interface with menus and input helpers to capture user commands and display results.

Key Components:

    Main Program: Initializes dependency injection, applies EF Core migrations, and runs a loop to process user commands.
    UI Helpers (Menu & InputProcessor): Manage console output and handle input validation.
    User Commands: Methods to create, view, update, and delete expenses, budgets, and saving goals, as well as to generate various reports.

Technical Specifics:

    Uses .NET’s Generic Host to set up dependency injection.
    Registers EF Core DbContext with SQLite for data persistence.
    Implements a simple command loop with a switch statement to navigate different functionalities.

Application Layer

Purpose:
Acts as a bridge between the presentation and domain layers. This layer defines Data Transfer Objects (DTOs), command objects, mappers, and service interfaces for handling business operations.

Key Components:

    DTOs and Commands: Define structured objects such as BudgetDTO, ExpenseDTO, and commands like CreateExpenseCommand and UpdateBudgetCommand.
    Service Interfaces & Implementations:
        Services such as BudgetService, ExpenseService, IncomeService, SavingGoalsService, and ReportingService encapsulate business operations.
    Mappers: Static classes (e.g., BudgetMapper, ExpenseMapper) convert between DTOs/commands and domain entities.

Technical Specifics:

    Uses asynchronous methods (async/await) for operations.
    Integrates with UnitOfWork from the Infrastructure layer for data access.
    Validates business rules before processing data (e.g., using ExpenseValidator and BudgetValidator).

Domain Layer

Purpose:
Contains the core business logic and domain models. It defines entities, value objects, exceptions, repository interfaces, and validation logic.

Key Components:

    Entities:
        BudgetContainer & BudgetItem: Represent a budget and its associated categories.
        Expense, Income, SavingGoals, Category, and CategoryMapping: Represent the various financial aspects tracked by the system.
    Value Objects:
        Money: Encapsulates monetary values with currency.
        DateRange: Represents periods of time.
    Exceptions:
        Custom exceptions (e.g., InvalidBudgetException, InvalidExpenseException, NotFoundException) enforce business rules.
    Repository Interfaces:
        Generic repositories and specific ones (e.g., IBudgetRepository, IExpenseRepository) abstract data operations.
    Validators:
        Business rules are enforced via validators (e.g., BudgetValidator, ExpenseValidator).

Technical Specifics:

    Enforces rules like budget dates not being in the past and ensuring non-negative expenses.
    Contains enumerations (e.g., BudgetFrequency) to standardize budget recurrence.

Infrastructure Layer

Purpose:
Handles all data access, external service calls, and logging. It implements repository patterns and integrates with EF Core for persistence.

Key Components:

    EF Core DbContext (BudgetTrackerDbContext):
        Maps entities to database tables and seeds initial data (e.g., default categories and mappings).
    Repository Implementations:
        Concrete implementations of repository interfaces for budgets, expenses, income, saving goals, and category mappings.
        GenericRepository<T>: Provides a common implementation for CRUD operations.
    UnitOfWork:
        Manages transaction scopes across multiple repositories.
    External Services:
        CurrencyConversionService: Connects to an external API to convert currencies.
    Logging:
        Implements logging through a file logger (FileLogger) and a database logger (DatabaseLogger).

Technical Specifics:

    Uses SQLite as the default database provider.
    Contains EF Core migrations and a model snapshot for database versioning.
    Configures entity relationships (e.g., one-to-many between BudgetContainer and BudgetItem).

Testing Suite

Purpose:
Ensures the reliability and correctness of the application through unit and integration tests.

Key Components:

    Integration Tests:
        Test repositories and UnitOfWork using an in-memory database.
        Validate CRUD operations on budgets, expenses, saving goals, and external services (e.g., currency conversion).
    Unit Tests:
        Test individual components such as service methods, mappers, and validators.
        Use fake implementations (e.g., FakeUnitOfWork, FakeBudgetRepository) to simulate data access.
    Test Structure:
        Organized into IntegrationTests, UnitTests, and MockTests to cover different aspects of the system.

Technical Specifics:

    Uses xUnit as the testing framework.
    Employs dependency injection and in-memory databases to simulate production scenarios.

Current Usage

At present, Budget Tracker is implemented as a console application. When you run the application:

    Initialization:
        The program initializes dependency injection and registers all services.
        EF Core applies any pending migrations to the SQLite database.

    Main Menu:
        A console-based main menu allows users to choose operations such as creating or updating expenses, budgets, saving goals, or generating various reports.

    Business Operations:
        Commands entered through the console are processed by the application services.
        Data is validated against business rules (using domain validators) and then persisted via repositories.

    Logging & Reporting:
        Any errors are logged using a file logger.
        Reports are generated by aggregating data from expenses, budgets, and incomes.

Future Vision

The final version of Budget Tracker will expand on the current codebase by:

    Cross-Platform Availability:
        Providing a user-friendly interface on web, mobile, and Windows platforms.
    Enhanced User Interface:
        Moving beyond the console-based UI to rich, interactive dashboards.
        Incorporating visual graphs and charts to display financial progress.
    Advanced Reporting:
        Offering detailed monthly, yearly, and variance reports comparing planned vs. actual figures.
        Allowing users to toggle currencies and view percentages of income allocations.
    Robust Data Management:
        Ensuring automatic persistence of multiple budgets, saving goals, and income sources.
        Supporting future integrations with financial APIs and external data sources.
    Notification & Alerts:
        Implementing notifications for users regarding financial milestones and potential issues.
    Modular & Scalable Architecture:
        Building on the layered architecture to allow for easy maintenance and future enhancements.

Installation and Setup

    Clone the Repository:

git clone https://github.com/yourusername/BudgetTracker.git
cd BudgetTracker

Restore Dependencies:

Use the .NET CLI to restore packages:

dotnet restore

Apply Migrations and Update Database:

Ensure that the SQLite database is created and updated:

dotnet ef database update --project "04 - Infrastructure"

Run the Application:

Start the console application:

dotnet run --project "01 - Presentation Layer"

Run Tests:

To run the integrated and unit tests:

    dotnet test

Next Steps

    User Interface Improvements: Transition from a console-based UI to graphical interfaces for web and mobile.
    Feature Enhancements: Implement features such as notifications, currency toggling, and dynamic report generation.
    Deployment: Package the application for multiple platforms (web server, mobile app store, Microsoft Store).
    Continuous Testing & Integration: Expand test coverage and incorporate continuous integration pipelines.
