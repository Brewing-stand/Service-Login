# Login Service

## Table of Contents

1. [Description](#description)
2. [Purpose](#purpose)
3. [Getting Started](#getting-started)
   - [Using Docker](#using-docker)
   - [Running Locally](#running-locally)
     - [Prerequisites](#prerequisites)
     - [1. Clone the Repository](#1-clone-the-repository)
     - [2. Install Dependencies](#2-install-dependencies)
     - [3. Set Environment Variables](#3-set-environment-variables)
     - [4. Run the Server](#4-run-the-server)
       - [Production](#production)
       - [Development](#development)
     - [5. Access the Application](#5-access-the-application)
     - [6. Debugging (Development Only)](#6-debugging-development-only)
4. [Endpoints](#endpoints)
   - [1. `GET /api/auth/{code}`](#1-get-apiauthcode)
   - [2. `POST /api/auth/logout`](#2-post-apiauthlogout)
5. [Configuration](#configuration)


## Description
The Login Service is built using **C#** and **.NET 9**, serving as the authentication gateway for the Brewing Stand project. It integrates with GitHub to handle user authentication and issues secure cookies for use with other services.

## Purpose

The Login Service manages user authentication for the Brewing Stand platform. Its primary purposes include:
1. **User Authentication**: Verifying users by interacting with GitHub's OAuth system and issuing authentication cookies.
2. **Session Management**: Handling user sessions, including logging in and securely logging out users.
3. **Inter-Service Authentication**: Providing authentication cookies that allow users to seamlessly interact with other services in the Brewing Stand ecosystem.

## Getting started

### Running locally
Follow these steps to set up the Project Service for the Searchable DB Project.

### Prerequisites
- **.NET 8 SDK** installed on your machine
- **MongoDB** instance running (for accessing the database)

### 1. Clone the Repository
Start by cloning the project repository to your local machine:
```bash
git clone https://github.com/Nelissen-searchable-db/API-Project
cd project-service
```

#### 2. Install Dependencies
Navigate to the project directory and restore the required NuGet packages:
```bash
dotnet restore
```

#### 3. Set environment variables

Before using this service, set the `ConnectionString` environment variable in `appsettings.json` with a valid MongoDB/MongoDB compatible URL.

##### [For Development Only]

If you are on a development build, create `appsettings.Development.json` next to the existing app settings file and add your connection string as follows:

```json
{
    "ConnectionString": "YOUR_CONNECTIONSTRING"
}
```

where `YOUR_CONNECTIONSTRING` refers to the location of a testing/development database.

> **Important!** You may need to generate dummy data to get the desired results from your development instance. To do this, use the our provided dummy generator, which you can find [here](https://github.com/Nelissen-searchable-db/DummyDataGenerator).

#### 4. Run the server

##### Production
Navigate to the project directory and run the following command
```bash
dotnet run
```

##### Development
Like production, run the following command in the project directory
```bash
dotnet run --environment Development
```

#### 5. Access the Application
The API server should be available at port `5174`.

#### 6. Debugging (Development only)

If you are running the application in development mode, you can access the swagger UI at `http://localhost:5174/swagger/index.html`.

This UI will help you debug the API endpoints.

## Endpoints

### **1. `GET /api/auth/{code}`**
Retrieves the login code from GitHub and returns an authentication cookie for use with other services.

#### **Request**
- **Method**: `GET`
- **Path Parameter**:
  - `{code}`: The GitHub OAuth code provided after user authentication.
- **Headers**:
  - `Content-Type`: `application/json`
- **Body**: None

#### **Response**
- **Status Codes**:
  - `200 OK`: Authentication successful. A secure cookie is issued.
  - `400 Bad Request`: Invalid or missing GitHub OAuth code.
  - `401 Unauthorized`: Authentication failed.
  - `500 Internal Server Error`: An error occurred during the authentication process.
- **Headers**:
  - `Set-Cookie`: The issued cookie for authentication.
- **Body**:
  ```json
  {
    "User": {
      "id": "user-id",
      "username": "username",
      "avatar": "avatar-url"
    }
  }

### **2. `POST /api/auth/logout`**

Logs the user out by clearing their authentication cookie.

#### Request

- **Method**: POST  
- **Headers**:  
  - `Cookie`: The authentication cookie issued during login.  
- **Body**: None  

#### Response

- **Status Codes**:  
  - `200 OK`: Logout successful. Cookie is cleared.  
  - `401 Unauthorized`: User is not authenticated.  
- **Body**:  

```json
{
  "message": "Logged out successfully"
}
```

## Configuration

Example appsettings.json
```json
{
  "AppSettings": {
    "AllowedOrigins": ["http://localhost:5173", "https://localhost:5173"]
  },
  "ConnectionStrings": {
    "PostgresSQL_DB": "Host=brewingpostgress.postgres.database.azure.com;Port=5432;Username=BrewingPgresqlDBAdmin;Password=_kRE%peAb@p2eT_;Database=postgres;"
  },
  "GitSecrets": {
    "Client": "Ov23liAfQW8jdo7yBa0l",
    "Secret": "7069dfd241cf24cc7b56bb207e1e44e8c477ff2e"
  },
  "JwtSettings": {
    "SecretKey": "snCwqcVLkogiYDz4ZuSlYiRJ2uHEsiBdrsXRImVv0Pe5HVtP81"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```
