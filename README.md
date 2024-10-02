# WebCrawlerAPI
## Hacker News Web Crawler

## Overview

This project is a web crawler built using ASP.NET Core that extracts data from [Hacker News](https://news.ycombinator.com/). The crawler retrieves the first 30 entries, including their rank, title, points, and number of comments. Additionally, the project provides functionality to filter entries based on the word count in the title.

## Features

- Scrape the first 30 entries from Hacker News.
- Filter entries with:
  - More than five words in the title, ordered by the number of comments.
  - Less than or equal to five words in the title, ordered by points.
- Store usage data, including request timestamps and applied filters.
- User authentication for registering and logging in.

## Database Schema

The project uses SQL Server with the following tables:

- **UsageData**: Tracks user interactions with filters applied.
- **Entries**: Stores the scraped entries with relevant details.
- **Users**: Manages user accounts for authentication.

### Table Definitions

- **UsageData**
  - `Id`: Primary Key
  - `RequestTimestamp`: Timestamp of the request
  - `AppliedFilter`: The filter applied by the user
  - `UserIdentifier`: Identifier for the user making the request

- **Entries**
  - `Id`: Primary Key
  - `Rank`: Rank of the entry
  - `Title`: Title of the entry
  - `Points`: Points awarded to the entry
  - `Comments`: Number of comments on the entry
  - `WordCount`: Word count of the title

- **Users**
  - `Id`: Primary Key
  - `Username`: Unique username
  - `PasswordHash`: Hashed password for security
  - `Email`: Unique email address
  - `CreatedAt`: Account creation timestamp

## Getting Started

## Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) (NET 8)
- SQL Server
- [Visual Studio](https://visualstudio.microsoft.com/)
- [Postman](https://www.postman.com/) (optional for testing APIs)

### Installation

1. Clone the repository:
   ```bash
   cd C:\ruta\de\tu\carpeta
   git clone https://github.com/kwdiaz/WebCrawlerAPI.git
   
2. In the folder you clone the repository, open WebCrawlerAPI.sln 

### Database Setup

1. Create a new database named `HackerNewsDb` in SQL Server.
2. Execute the following SQL script to create the necessary tables:
    ```sql
    CREATE TABLE UsageData (
        Id INT PRIMARY KEY IDENTITY(1,1),
        RequestTimestamp DATETIME,
        AppliedFilter NVARCHAR(50),
        UserIdentifier NVARCHAR(100)
    );
    
    CREATE TABLE Entries (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Rank INT,
        Title NVARCHAR(255),
        Points INT,
        Comments INT,
        WordCount INT
    );
    
    CREATE TABLE Users (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Username NVARCHAR(50) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(512) NOT NULL,
        Email NVARCHAR(256) NOT NULL UNIQUE,
        CreatedAt DATETIME DEFAULT GETDATE()
    );

### Configuration

1. **Change the Connection String**:
   - Open the `appsettings.json` file in the project.
   - Locate the `ConnectionStrings` section and update the `HackerNewsDatabase` string with your SQL Server credentials:

     ```json
     "ConnectionStrings": {
         "HackerNewsDatabase": "Server=localhost; Database=HackerNewsDb; Integrated Security=True; Trusted_Connection=True; TrustServerCertificate=True;"
     }
   
   - Or in case you have SSMS with credentials
  
      ```json
         "ConnectionStrings": {
             "HackerNewsDatabase": "Server=server_name;Database=HackerNewsDb;User Id=your_id;Password=your_pass;TrustServerCertificate=True;MultipleActiveResultSets=true"
         }

### Testing the Project

1. **Run the Project:**
   - Launch the project by clicking the green button in Visual Studio. This button has an arrow, that allows you to select your preferred web browser to run the application.

2. **Access Swagger UI:**
   - Once the project is running, Swagger UI will automatically open in your browser. This interface provides access to the available APIs.

3. **Register a New User:**
   - Navigate to the `/Auth/Register` endpoint and fill in the required fields using the following schema:
     ```json
     {
       "username": "string",
       "password": "string",
       "email": "string"
     }
     ```
   - Note: Currently, there is no input validation, so ensure you provide valid data.

4. **Log In:**
   - After registration, proceed to the `/Auth/Login` endpoint with the following schema:
     ```json
     {
       "username": "string",
       "password": "string"
     }
     ```
   - Upon successful execution, you will receive a JSON Web Token (JWT).

5. **Authenticate API Requests:**
   - To access other protected APIs, click on the lock icon in Swagger UI.
   - In the authorization dialog, enter your token prefixed by the word `Bearer`, formatted like this:
     ```
     Bearer [token]
     ```
   - With the token successfully added, you will now have access to the other available APIs.

