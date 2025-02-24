# Currency Exchange API

A .NET Core application for currency exchange with MySQL, Redis caching, and an external API.

## Setup Instructions

### Prerequisites
- .NET 7+
- MySQL Server
- Redis
- Git
- Visual Studio or VS Code

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/NinjaCarrots/CurrencyExchange.git
   cd CurrencyExchange.git

2. Configure the .env file or appsettings.json:
    {
      "ConnectionStrings": {
        "DefaultConnection": "server=localhost;database=CurrencyExchangeDB;user=root;password=yourpassword"
      },
      "Redis": {
        "Host": "localhost",
        "Port": 6379
      },
      "CurrencyExchangeApi": {
        "BaseUrl": "https://api.exchangeratesapi.io/latest",
        "ApiKey": "d3c00448907a365efc69186ec600e5e5"
      }
    }
   
3. Run database migrations:

    dotnet ef database update

4. Start the application:
    
    dotnet run
    Running Tests
   
5. Run the unit tests using:

    dotnet test

API Endpoints
GET /api/currency/rates - Fetch exchange rates
POST /api/currency/convert - Convert currency
