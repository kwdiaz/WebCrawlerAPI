
### Key Design Decisions Document

```markdown
# Key Design Decisions for Hacker News Web Crawler

## Overview

This document outlines the key design decisions made during the development of the Hacker News Web Crawler, focusing on architecture, technology choices, and implementation strategies.

## 1. Architecture

- **Layered Architecture**: The application follows a layered architecture pattern, separating concerns into models, services, controllers, and data access layers. This promotes clean code and maintainability.
  
## 2. Technology Choices

- **Framework**: ASP.NET Core was chosen for its performance, scalability, and support for modern web development practices. It also integrates seamlessly with Entity Framework for data access.
  
- **Database**: SQL Server was selected due to its robust feature set, including transaction support and security features. It also allows for easy integration with ASP.NET Core applications.

## 3. Data Storage

- **Tables Structure**:
  - **UsageData**: To track user interactions, allowing for future analytics and improvements based on user behavior.
  - **Entries**: This table captures the scraped data, enabling efficient querying and filtering.
  - **Users**: A dedicated table for user authentication ensures that user credentials are managed securely.

## 4. Filtering Logic

- **Word Count Filtering**: The decision to filter based on word count was influenced by user requirements. The implementation counts words while excluding symbols, ensuring accurate results. This logic is encapsulated in a service class, promoting reusability.

## 5. Authentication

- **Security Practices**: Passwords are hashed using HMACSHA512 to enhance security. This decision was made to protect user credentials against potential data breaches.
  
- **JWT for Authentication**: JSON Web Tokens (JWT) are used for user session management, providing a stateless and scalable approach to authentication.

## 6. Testing and Documentation

- **Swagger Integration**: Swagger UI is included to provide a clear and interactive way to test API endpoints. This decision improves the developer experience and facilitates testing.

## Conclusion

These design decisions collectively contribute to the overall functionality, security, and maintainability of the Hacker News Web Crawler. Continuous improvements and user feedback will drive future enhancements.
